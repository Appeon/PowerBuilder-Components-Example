using NDde.Client;
using PowerBuilderEventInvoker.DotNet;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("Appeon.ComponentsApp.DdeTools")]
namespace Appeon.ComponentsApp.DdeTools.DdeClient
{
    public class DdeClientWrapper : IDisposable
    {
        private bool disposed = false;

        public string Service { get; set; }
        public string Topic { get; set; }
        public int DefaultTimeout { get; set; } = 60000;
        private readonly NDde.Client.DdeClient client;

        private readonly IList<(string, string)> logCallbacks;
        private readonly IList<(string, string)> disconnectedCallbacks;


        public DdeClientWrapper(string service, string topic)
        {
            Service = service;
            Topic = topic;

            logCallbacks = new List<(string, string)>();
            disconnectedCallbacks = new List<(string, string)>();

            client = new NDde.Client.DdeClient(service, topic);
            client.Advise += Client_Advise;
            client.Disconnected += Client_Disconnected;
        }

        public void AddLogCallback(string objectName, string callback)
        {
            logCallbacks.Add((objectName, callback));
        }

        public void AddDisconnectedCallback(string objectName, string callback)
        {
            disconnectedCallbacks.Add((objectName, callback));
        }


        private void Log(string message)
        {
            foreach (var (objectName, callback) in logCallbacks)
            {
                EventInvoker.InvokeEvent(objectName, callback, message);
            }
        }

        public int Connect(out string? error)
        {
            error = null;

            try
            {
                client.Connect();
                Log("Client connected");
                return 1;
            }
            catch (Exception e)
            {
                Log("Client could not connect");
                error = e.Message;
                return -1;
            }
        }

        public int Disconnect(out string? error)
        {
            error = null;

            try
            {
                client.Disconnect();
                Log("Client disconnected");
                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
        }

        public int StartAdvise(string topic, out string? error)
        {
            error = null;

            try
            {
                client.StartAdvise(topic, 1, true, DefaultTimeout);
                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
        }

        public int StopAdvise(string topic, out string? error) { 
            error = null;

            try
            {
                client.StopAdvise(topic, DefaultTimeout);
                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
        }

        public int Execute(string command, out string? error)
        {
            error = null;

            try
            {
                client.Execute(command, DefaultTimeout);
                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
        }

        public int Poke(string item, string data, out string? error)
        {
            error = null;

            try
            {
                client.Poke(item, Encoding.UTF8.GetBytes(data), 1, DefaultTimeout);
                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
        }

        public int Request(string item, out string? response, out string? error)
        {
            error = null;
            response = null;

            try
            {
                response = client.Request(item, DefaultTimeout);
                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }


        }


        private void Client_Disconnected(object? sender, DdeDisconnectedEventArgs e)
        {
            Log("Disconnected");

            foreach (var (objectName, callback) in disconnectedCallbacks)
            {
                EventInvoker.InvokeEvent(objectName, callback);
            }
        }

        private void Client_Advise(object? sender, DdeAdviseEventArgs e)
        {
            Log($"Advised: [{e.Text ?? "Null"}]");
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                }

                client.Dispose();
                logCallbacks.Clear();
                disconnectedCallbacks.Clear();
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DdeClientWrapper() { Dispose(false); }
    }
}

