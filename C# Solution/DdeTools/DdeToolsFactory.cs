using Appeon.ComponentsApp.DdeTools.DdeClient;
using Appeon.ComponentsApp.DdeTools.DdeServer;

namespace Appeon.ComponentsApp.DdeTools
{
    public class DdeToolsFactory
    {
        public static DdeServerWrapper? CreateServer(string service, out string? error)
        {
            error = null;

            try
            {
                var server = new DdeServerWrapper(service);
                return server;
            }
            catch (Exception e)
            {
                error = e.Message;
                return null;
            }
        }

        public static DdeClientWrapper? CreateClient(string service, string topic, out string? error) { 
            error = null;

            try
            {
                var client = new DdeClientWrapper(service, topic);

                return client;
            }
            catch (Exception e)
            {
                error = e.Message;
                return null;
            }
        }


    }
}
