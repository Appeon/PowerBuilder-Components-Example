using Appeon.ComponentsApp.DdeTools.Common.Messages;
using NDde.Server;
using System.Text;

namespace Appeon.ComponentsApp.DdeTools.DdeServer
{
    public class DdeServerAdapter : NDde.Server.DdeServer
    {
        public event Action<string>? Log;
        public event Action<string>? Error;

        public event Action<string, Message>? ExecuteCallback;
        public event Action<string>? AdviseCallback;
        public event Action<string>? UnadviseCallback;
        public event Action<string>? RequestCallback;
        public event Action<string, Message>? PokeCallback;

        public ICollection<string> Items { get; set; }
        public string? Topic { get; set; }
        public bool AcceptRequests { get; set; }
        public bool AcceptCommands { get; set; }

        public string? AdviseContents { get; set; }

        public DdeServerAdapter(string service) : base(service)
        {
            Items = new HashSet<string>();
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

        protected override ExecuteResult OnExecute(DdeConversation conversation, string command)
        {
            Log?.Invoke("OnExecute:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString()
                    + " Command='" + command + "'" + $"[{(AcceptCommands ? "Accepted" : "Rejected")}]");
            if (!(Topic is null || conversation.Topic == Topic))
            {
                Log?.Invoke("Unsupported topic. Ignoring");
                return ExecuteResult.NotProcessed;
            }
            ExecuteCallback?.Invoke(conversation.Service, new Message
            {
                MessageContent = $"{command} [{(AcceptCommands ? "Accept" : "Reject")}]",
                MessageOrigin = conversation.Service,
                Service = conversation.Service,
                Topic = conversation.Topic,
            });
            return AcceptCommands ? ExecuteResult.Processed : ExecuteResult.NotProcessed;
        }

        protected override PokeResult OnPoke(DdeConversation conversation, string item, byte[] data, int format)
        {
            var stringData = Encoding.UTF8.GetString(data);
            Log?.Invoke("OnPoke:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString()
                    + " Item='" + item + "'"
                    + " Data=" + stringData
                    + " Format=" + format.ToString());
            if (!(Topic is null || conversation.Topic == Topic))
            {
                Log?.Invoke("Unsupported topic. Ignoring");
                return PokeResult.NotProcessed;
            }
            ExecuteCallback?.Invoke(conversation.Service, new Message
            {
                MessageContent = stringData,
                MessageOrigin = conversation.Service,
                Service = conversation.Service,
                Topic = conversation.Topic,
                Item = item,
            });
            return PokeResult.Processed;
        }

        protected override RequestResult OnRequest(DdeConversation conversation, string item, int format)
        {
            Log?.Invoke("OnRequest:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString()
                    + " Item='" + item + "'"
                    + " Format=" + format.ToString());

            if (!AcceptRequests)
            {
                return RequestResult.NotProcessed;
            }

            if (!(Topic is null || conversation.Topic == Topic))
            {
                Log?.Invoke("Unsupported topic. Ignoring");
                return RequestResult.NotProcessed;
            }
            if (format == 1)
            {
                RequestCallback?.Invoke(conversation.Service);
                return new RequestResult(Encoding.ASCII.GetBytes($"Time={DateTime.Now}\0"));
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

            AdviseCallback?.Invoke(conversation.Service);
            return format == 1;
        }

        protected override byte[] OnAdvise(string topic, string item, int format)
        {
            Log?.Invoke("OnAdvise:".PadRight(16)
                    + " Service='" + this.Service + "'"
                    + " Topic='" + topic + "'"
                    + " Item='" + item + "'"
                    + " Format=" + format.ToString());

            if (format == 1)
            {
                return AdviseContents is null ? Array.Empty<byte>() : Encoding.ASCII.GetBytes(AdviseContents);
            }
            return base.OnAdvise(topic, item, format);
        }

        protected override void OnStopAdvise(DdeConversation conversation, string item)
        {
            Log?.Invoke("OnStopAdvise:".PadRight(16)
                    + " Service='" + conversation.Service + "'"
                    + " Topic='" + conversation.Topic + "'"
                    + " Handle=" + conversation.Handle.ToString()
                    + " Item='" + item + "'");
            UnadviseCallback?.Invoke(conversation.Service);
        }
    }
}
