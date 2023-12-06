using Appeon.ComponentsApp.DdeTools.PowerBuilderAdapter.DdeServer;

namespace Appeon.ComponentsApp.DdeTools.PowerBuilderAdapter
{
    public class Hotlink
    {
        public NDde.Client.DdeClient Client { get; set; }
        public HotlinkParams Params { get; set; }
        public string? CallbackHandle { get; set; }

        public Hotlink(NDde.Client.DdeClient client, HotlinkParams @params)
        {
            Client = client;
            Params = @params;
        }
    }

}
