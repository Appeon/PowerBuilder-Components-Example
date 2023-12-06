
using PowerBuilderEventInvoker.DotNet;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Appeon.ComponentsApp.DdeTools")]
namespace Appeon.ComponentsApp.DdeTools.DdeServer
{
    public class DdeServerWrapper
    {
        private readonly IList<(string, string)> LogCallbacks;

        private readonly DdeServerAdapter serverAdapter;

        internal DdeServerWrapper(string service)
        {
            LogCallbacks = new List<(string, string)>();

            serverAdapter = new DdeServerAdapter(service);
            serverAdapter.Log += Log;
        }

        private string? _adviseContents;

        public string? AdviseContents
        {
            get { return _adviseContents; }
            set
            {
                _adviseContents = value;
                serverAdapter.AdviseContents = value;
            }
        }

        public void AcceptRequests(bool accept) => serverAdapter.AcceptRequests = accept;

        public void AcceptCommands(bool accept) => serverAdapter.AcceptCommands = accept;


        public void AddLogCallback(string objectName, string callback)
        {
            LogCallbacks.Add((objectName, callback));
        }

        public int Advise(string topic, string item, out string? error)
        {
            error = null;
            try
            {
                serverAdapter.Advise(topic, item);
                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
        }

        public int Register(out string? error)
        {
            error = null;
            try
            {
                serverAdapter.Register();
            }
            catch (Exception e)
            {
                Log("Server failed to register");
                error = e.Message;
                return -1;
            }
            return 1;
        }

        public int Unregister(out string? error)
        {
            error = null;

            try
            {
                serverAdapter.Unregister();
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }

            return 1;
        }

        public int Disconnect(out string? error)
        {
            error = null;

            try
            {
                serverAdapter.Disconnect();
                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
        }

        public int Pause(out string? error)
        {
            error = null;

            try
            {
                serverAdapter.Pause();
                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return 1;
            }
        }

        public int Resume(out string? error)
        {
            error = null;
            try
            {
                serverAdapter.Resume();
                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }
        }

        private void Log(string message)
        {
            foreach (var (obj, callback) in LogCallbacks)
            {
                EventInvoker.InvokeEvent(obj, callback, message);
            }
        }
    }
}
