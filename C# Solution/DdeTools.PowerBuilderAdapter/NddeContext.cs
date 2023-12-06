using Appeon.ComponentsApp.DdeTools.Common.Messages;
using Appeon.ComponentsApp.DdeTools.PowerBuilderAdapter.Common;
using Appeon.ComponentsApp.DdeTools.PowerBuilderAdapter.DdeClient;
using Appeon.ComponentsApp.DdeTools.PowerBuilderAdapter.DdeServer;
using NDde.Client;
using PowerBuilderEventInvoker.DotNet;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Appeon.ComponentsApp.DdeTools.PowerBuilderAdapter
{
    public class NddeContext
    {
        public IDictionary<int, DdeServerPbAdapter> ServerAdapters { get; set; }
        public IDictionary<int, ICollection<(int, DdeClientPbAdapter)>> ClientAdapters { get; set; }
        public IDictionary<int, ICollection<Hotlink>> Hotlinks { get; set; }

        private string? CallbackObject;

        public NddeContext()
        {
            ServerAdapters = new Dictionary<int, DdeServerPbAdapter>();
            ClientAdapters = new Dictionary<int, ICollection<(int, DdeClientPbAdapter)>>();
            Hotlinks = new Dictionary<int, ICollection<Hotlink>>();
        }

        public bool GetServer(int handle, out DdeServerPbAdapter? adapter)
        {
            return ServerAdapters.TryGetValue(handle, out adapter);
        }

        public void EventTestThreaded()
        {

            new Thread(new ThreadStart(() =>
            {
                InvokeEvent("test");
            })).Start();
        }

        public void EventTest()
        {
            InvokeEvent("test");
        }

        //[MethodImpl(MethodImplOptions.NoInlining)]
        public int? RegisterServer(int handle,
            string callbackObject,
            string? serviceName,
            string? topic,
            string? item,
            out string? error,
            out DdeServerPbAdapter? serverInstance)
        {
            serverInstance = null;
            error = null;

            if (serviceName is null || topic is null)
                return null;

            if (ServerAdapters.ContainsKey(handle))
            {
                error = "Window already registered as server";
                return -1;
            }
            if (ClientAdapters.ContainsKey(handle))
            {
                error = "Window already registered as client";
                return -1;
            }
            foreach (var value in ServerAdapters.Values)
            {
                if (value.Service == serviceName && value.Topic == topic)
                {
                    error = "Service/topic already registered";
                    return -1;
                }
            }

            CallbackObject = callbackObject;
            var server = new DdeServerPbAdapter(handle, CallbackObject, serviceName)
            {
                Topic = topic
            };
            var items = (item ?? "").Split(',');
            foreach (var itemToken in items)
            {
                if (!string.IsNullOrEmpty(itemToken))
                {
                    server.Items.Add(itemToken);
                }
            }
            try
            {
                server.Register();
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }

            server.AdviseCallback += (service, msg) =>
            {
                InvokeEvent("RemoteHotLinkStartEx", MessageSerializer.SerializeMessage(msg));
            };

            server.ExecuteCallback += (service, msg) =>
            {
                //InvokeEvent("test");
                InvokeEvent("RemoteExecEx", MessageSerializer.SerializeMessage(msg));
            };

            server.PokeCallback += (service, msg) =>
            {
                InvokeEvent("RemoteSendEx", MessageSerializer.SerializeMessage(msg));
            };

            server.RequestCallback += (service, message) =>
            {
                InvokeEvent("RemoteRequestEx", MessageSerializer.SerializeMessage(message));
            };

            server.UnadviseCallback += (service, msg) =>
            {
                InvokeEvent("RemoteHotLinkStopEx", MessageSerializer.SerializeMessage(msg));
            };

            ServerAdapters[handle] = server;
            serverInstance = server;
            return 1;
        }


        public int? ExecuteSingleCommand(string? command, string? appname, string? topic, out string? error)
        {
            error = null;
            if (command is null || appname is null || topic is null)
                return null;
            using var client = new NDde.Client.DdeClient(appname, topic);
            try
            {
                client.Connect();
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
            try
            {
                client.Execute(command);
            }
            catch (Exception e)
            {
                error = e.Message;
                return -2;
            }

            return 1;
        }

        public int? ExecuteCommandOnHandle(
            int windowHandle,
            int? channelHandle,
            string? command,
            out string? error)
        {
            error = null;

            if (channelHandle is null || command is null) return null;

            var client = GetClient(windowHandle, channelHandle.Value);

            if (client is null)
            {
                error = "Channel not found";
                return -1;
            }

            return client.ExecRemote(command, out error);
        }

        public int? SendSingleDatum(string? location,
            string? value,
            string? appname,
            string? topicname,
            string? bansi,
            out string? error)
        {
            error = null;
            if (location is null ||
                value is null ||
                appname is null ||
                topicname is null ||
                bansi is null)
                return null;

            using var client = new NDde.Client.DdeClient(appname, topicname);

            try
            {
                client.Connect();
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }

            int format = (bansi.ToLower() switch
            {
                "" or null or "false" => Formats.CF_UNICODETEXT,
                _ => Formats.CF_TEXT,
            });

            byte[] data = FormatTools.GetBytes(value, format);

            try
            {
                client.Poke(location, data, format, 10000);
            }
            catch (Exception e)
            {
                error = e.Message;
                return -2;
            }

            return 1;
        }

        public short? SendDataThroughChannel(int windowHandle,
            int? channelHandle,
            string? location,
            string? data,
            bool? ansi,
            out string? error
            )
        {
            error = null;
            if (channelHandle is null ||
                location is null ||
                data is null ||
                ansi is null)
                return null;

            var client = GetClient(windowHandle, channelHandle.Value);

            if (client is null)
            {
                return -1;
            }

            return client.SendRemote(location, data, ansi.Value, out error);
        }

        public int? RequestSingleData(string? location,
            string? appname,
            string? topicname,
            bool? bansi,
            out string? target,
            out string? error)
        {
            error = null;
            target = null;

            if (location is null ||
                appname is null ||
                topicname is null ||
                bansi is null)
            {
                return null;
            }

            using var client = new NDde.Client.DdeClient(appname, topicname);

            try
            {
                client.Connect();
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }

            var format = bansi.Value ? Formats.CF_TEXT : Formats.CF_UNICODETEXT;

            try
            {
                target = FormatTools.GetString(client.Request(location, format, 10000), format);
            }
            catch (Exception e)
            {
                error = e.Message;
                return -2;
            }

            return 1;
        }

        public int? RequestDataThroughChannel(int windowHandle, int? channelHandle,
            string? location,
            out string? data,
            bool? bansi,
            out string? error)
        {
            data = null;
            error = null;

            if (channelHandle is null ||
                location is null ||
                bansi is null)
                return null;

            var client = GetClient(windowHandle, channelHandle.Value);

            if (client is null)
            {
                error = "Channel not found";
                return -1;
            }

            return client.GetRemote(location, out data, bansi.Value, out error);
        }

        private void InvokeEvent(string name, string? arguments = null)
        {
            if (CallbackObject is not null)
                EventInvoker.InvokeEvent(CallbackObject, name, arguments);
        }

        public int UnregisterServer(int handle,
            string? appName,
            string? topic,
            out string? error)
        {
            error = null;
            var exists = ServerAdapters.TryGetValue(handle, out DdeServerPbAdapter? adapter);

            if (!exists)
            {
                return -1;
            }

            if (adapter == null)
                throw new InvalidOperationException("Adapter is null");

            if (adapter.Topic != topic || adapter.Service != appName)
                return -2;

            try
            {
                adapter.Unregister();
            }
            catch (Exception e)
            {
                error = e.Message;
                return -3;
            }

            ServerAdapters.Remove(handle);
            return 1;
        }

        public int? StartHotlink(int? handle,
            string callbackHandle,
            string? location,
            string? appname,
            string? topic,
            bool? bansi,
            out string? error)
        {
            error = null;
            if (handle is null ||
                appname is null ||
                topic is null ||
                bansi is null ||
                location is null)
                return null;

            if (Hotlinks.TryGetValue(handle.Value, out var hotlinks)
                && hotlinks.Select(hl => hl.Params)
                .Contains(new HotlinkParams(appname, topic, location)))
            {
                error = "Hotlink already registered";
                return -1;
            }

            var client = new NDde.Client.DdeClient(appname, topic);
            if (!Hotlinks.ContainsKey(handle.Value))
            {
                Hotlinks[handle.Value] = new HashSet<Hotlink>();
            }

            try
            {
                client.Connect();
            }
            catch (Exception e)
            {
                error = e.Message;
                client.Dispose();
                return -1;
            }

            int format = (bansi.Value ? Formats.CF_TEXT : Formats.CF_UNICODETEXT);

            try
            {
                client.StartAdvise(location, format, true, 10000);
                client.Advise += Client_AdviseHandler;
            }
            catch (Exception e)
            {
                error = e.Message;
                client.Dispose();
                return -2;
            }

            Hotlinks[handle.Value]
                .Add(new Hotlink(client,
                                new HotlinkParams(
                                    appname,
                                    topic,
                                    location))
                { CallbackHandle = callbackHandle });
            return 1;
        }

        private void Client_AdviseHandler(object? sender, DdeAdviseEventArgs e)
        {
            if (sender is null)
            {
                return;
            }
            var hotlink = (from hotlinkSet in Hotlinks.Values
                           from eachHotlink in hotlinkSet
                           where eachHotlink.Client == (NDde.Client.DdeClient)sender
                           select eachHotlink).FirstOrDefault();


            if (hotlink?.CallbackHandle is not null)
                EventInvoker.InvokeEvent(hotlink.CallbackHandle, "HotlinkAlarmEx",
                    MessageSerializer.SerializeMessage(new Message
                    {
                        Item = e.Item,
                        MessageContent = FormatTools.GetString(e.Data, e.Format),
                        Service = hotlink.Params.ApplicationName,
                        Topic = hotlink.Params.Topic,
                        MessageOrigin = hotlink.Params.ApplicationName
                    }));

        }

        public void WindowClosing(int handle)
        {

            try
            {
                ServerAdapters[handle].Unregister();
                ServerAdapters[handle].Disconnect();
                ServerAdapters[handle].Dispose();

                foreach (var hotlink in Hotlinks[handle])
                {

                    hotlink.Client.Disconnect();
                    hotlink.Client.Dispose();
                }

                ServerAdapters.Remove(handle);
                Hotlinks.Remove(handle);
            }
            catch { }
        }

        public int? StopHotlink(int? handle,
            string? location,
            string? appname,
            string? topic,
            out string? error)
        {
            error = null;
            if (handle is null ||
                appname is null ||
                topic is null ||
                location is null)
                return null;

            if (!(Hotlinks.TryGetValue(handle.Value, out var hotlinks)
                && hotlinks.Where(hl => hl.Params == new HotlinkParams(appname, topic, location))
                   .FirstOrDefault() is var hotlink
                && hotlink is not null))
            {
                error = "Hotlink not registered";
                return -1;
            }

            try
            {
                hotlink.Client.StopAdvise(location, 10000);
            }
            catch (Exception e)
            {
                error = e.Message;

                return -2;
            }

            hotlink.Client.Disconnect();
            hotlink.Client.Dispose();
            Hotlinks[handle.Value].Remove(hotlink);

            return 1;
        }





        public int Disconnect(int handle)
        {
            if (ServerAdapters.ContainsKey(handle))
            {
                ServerAdapters[handle].Unregister();
                ServerAdapters.Remove(handle);
                return 1;
            }

            return -1;

        }

        public void DisconnectAll()
        {
            foreach (var server in ServerAdapters.Values)
            {
                try
                {
                    server.Unregister();
                }
                catch
                {
                }
            }

            ServerAdapters.Clear();
        }

        public int? CreateClient(int handle, string? appname, string? topic, out string? error)
        {
            error = null;
            if (appname is null || topic is null)
                return null;

            var newClient = new NDde.Client.DdeClient(appname, topic);

            try
            {
                newClient.Connect();
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }

            if (!ClientAdapters.ContainsKey(handle))
            {
                ClientAdapters[handle] = new HashSet<(int, DdeClientPbAdapter)>();
            }

            var newAdapter = new DdeClientPbAdapter(newClient);
            var hashCode = newAdapter.GetHashCode();
            ClientAdapters[handle].Add((hashCode, newAdapter));

            return hashCode;
        }

        public int? CloseClient(int windowHandle, int? clientHandle)
        {
            if (clientHandle is null)
                return -9;


            var client = (
                ClientAdapters[windowHandle]
                .Where(((int handle, DdeClientPbAdapter client) c) => c.handle == clientHandle)
                .Select(((int handle, DdeClientPbAdapter client) c) => c.client)
                ).FirstOrDefault();

            if (client is null)
            {
                return -1;
            }

            client.Client.Disconnect();
            client.Client.Dispose();
            ClientAdapters.Remove(clientHandle.Value);

            return 1;
        }

        public DdeClientPbAdapter? GetClient(int windowHandle, int channelHandle)
        {
            if (ClientAdapters.TryGetValue(windowHandle, out var adapters)
                    && adapters.Where(item => item.Item1 == channelHandle)
                        .Select(item => item.Item2)
                        .FirstOrDefault() is var client
                    && client is not null)
            {
                return client;
            }

            return null;
        }

        public void RaiseEvent(string eventName)
        {
            if (CallbackObject is not null)
                EventInvoker.InvokeEvent(CallbackObject, eventName);
        }

        public static Message? DeserializeMessage(string jsonString) =>
                MessageSerializer.DeserializeMessage(jsonString);
    }
}
