using Appeon.ComponentsApp.DdeTools.Common.Messages;
using Appeon.ComponentsApp.DdeTools.PowerBuilderAdapter.Common;
using NDde.Server;
using System.Runtime.CompilerServices;
using System.Text;

namespace Appeon.ComponentsApp.DdeTools.PowerBuilderAdapter.DdeServer
{
    public class DdeServerPbAdapter : NDde.Server.DdeServer
    {


        public event Action<string>? Log;
        public event Action<string>? Error;

        public event Action<string, Message>? ExecuteCallback;
        public event Action<string, Message>? AdviseCallback;
        public event Action<string, Message>? UnadviseCallback;
        public event Action<string, Message>? RequestCallback;
        public event Action<string, Message>? PokeCallback;

        private bool? acceptNextData;
        private string? responseData;

        public ICollection<string> Items { get; set; }
        public string? Topic { get; set; }

        public int Handle { get; }
        public string CallbackObject { get; }
        public string? ResponseData
        {
            get => responseData;
            set => responseData = value;
        }

        public DdeServerPbAdapter(int handle, string callbackObj, string service) : base(service)
        {
            Items = new HashSet<string>();
            Handle = handle;
            CallbackObject = callbackObj;
        }

        public int SetAcceptData(bool accept)
        {
            if (acceptNextData is null)
                return -1;

            acceptNextData = accept;
            return 1;
        }

        public override void Register()
        {
            base.Register();
            Log?.Invoke("Server registered");
        }

        public override void Unregister()
        {
            base.Unregister();
            Log?.Invoke("Server unregistered");
        }

        protected override void OnAfterConnect(DdeConversation conversation)
        {
            base.OnAfterConnect(conversation);
            Log?.Invoke("OnAfterConnect:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString());
        }

        protected override bool OnBeforeConnect(string topic)
        {
            Log?.Invoke("OnBeforeConnect:".PadRight(16)
                    + " Service='" + base.Service + "'"
                    + " Topic='" + topic + "'");
            return Topic is null || topic == Topic;
        }

        protected override void OnDisconnect(DdeConversation conversation)
        {
            base.OnDisconnect(conversation);

            Log?.Invoke("OnDisconnect:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString());
        }

        //[MethodImpl(MethodImplOptions.NoInlining)]
        protected override ExecuteResult OnExecute(DdeConversation conversation, string command)
        {
            Log?.Invoke("OnExecute:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString()
                    + " Command='" + command + "'");
            acceptNextData = false;
            ExecuteCallback?.Invoke(conversation.Service, new Message
            {
                MessageContent = command,
                MessageOrigin = conversation.Service,
                Service = conversation.Service,
                Topic = conversation.Topic,
            });
            //GlobalResetEvent.WaitOne(WaitTimeout);
            if (!(Topic is null || conversation.Topic == Topic))
            {
                Log?.Invoke("Unsupported topic. Ignoring");
                return ExecuteResult.NotProcessed;
            }
            var returnValue = acceptNextData ?? false ? ExecuteResult.Processed : ExecuteResult.NotProcessed;
            acceptNextData = null;
            return returnValue;
        }

        protected override PokeResult OnPoke(DdeConversation conversation, string item, byte[] data, int format)
        {
            var stringData = FormatTools.GetString(data, format);
            Log?.Invoke("OnPoke:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString()
                    + " Item='" + item + "'"
                    + " Data=" + stringData
                    + " Format=" + format.ToString());
            acceptNextData = false;
            PokeCallback?.Invoke(conversation.Service, new Message
            {
                MessageContent = stringData,
                MessageOrigin = conversation.Service,
                Service = conversation.Service,
                Topic = conversation.Topic,
                Item = item,
            });
            //GlobalResetEvent.WaitOne();
            if (!(Topic is null || conversation.Topic == Topic))
            {
                Log?.Invoke("Unsupported topic. Ignoring");
                return PokeResult.NotProcessed;
            }

            var returnValue = acceptNextData ?? false ? PokeResult.Processed : PokeResult.NotProcessed;
            acceptNextData = null;
            return returnValue;

        }

        protected override RequestResult OnRequest(DdeConversation conversation, string item, int format)
        {
            Log?.Invoke("OnRequest:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString()
                    + " Item='" + item + "'"
                    + " Format=" + format.ToString());
            if (!(Topic is null || conversation.Topic == Topic))
            {
                Log?.Invoke("Unsupported topic. Ignoring");
                return RequestResult.NotProcessed;
            }
            if (Formats.CanAcceptFormat(format))
            {
                acceptNextData = false;
                RequestCallback?.Invoke(conversation.Service, new Message()
                {
                    Item = item,
                    Service = conversation.Service,
                    Topic = conversation.Topic,
                });
            }
            else
                return RequestResult.NotProcessed;
            if (acceptNextData ?? false)
            {
                try
                {
                    return new RequestResult(FormatTools.GetBytes(ResponseData ?? string.Empty, format));
                }
                catch
                {
                    return RequestResult.NotProcessed;
                }
            }
            return RequestResult.NotProcessed;
        }

        protected override bool OnStartAdvise(DdeConversation conversation, string item, int format)
        {
            Log?.Invoke("OnStartAdvise:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString()
                    + " Item='" + item + "'"
                    + " Format=" + format.ToString());

            if (!(Topic is null || conversation.Topic == Topic))
            {
                Log?.Invoke("Unsupported topic. Ignoring");
                return false;
            }

            if (!(Items.Count == 0 || Items.Contains(item)))
            {
                Log?.Invoke("Unsupported item. Ignoring");
                return false;
            }
            acceptNextData = true;

            AdviseCallback?.Invoke(conversation.Service, new Message
            {
                Item = item,
                Service = conversation.Service,
                Topic = conversation.Topic,
            });

            var willAccept = acceptNextData ?? true;
            acceptNextData = null;
            return Formats.CanAcceptFormat(format) && willAccept;
        }

        protected override byte[] OnAdvise(string topic, string item, int format)
        {
            Log?.Invoke("OnAdvise:".PadRight(16)
                    + " Service='" + Service + "'"
                    + " Topic='" + topic + "'"
                    + " Item='" + item + "'"
                    + " Format=" + format.ToString());

            return FormatTools.GetBytes(ResponseData ?? string.Empty, format);

        }

        protected override void OnStopAdvise(DdeConversation conversation, string item)
        {
            Log?.Invoke("OnStopAdvise:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString()
                    + " Item='" + item + "'");
            UnadviseCallback?.Invoke(conversation.Service, new Message
            {
                Topic = conversation.Topic,
                Item = item,    
                Service = conversation.Service,
            });
        }


    }
}
