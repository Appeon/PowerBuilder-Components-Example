using Appeon.ComponentsApp.DdeTools.PowerBuilderAdapter.Common;
using NDde.Client;

namespace Appeon.ComponentsApp.DdeTools.PowerBuilderAdapter.DdeClient
{
    public class DdeClientPbAdapter
    {
        public NDde.Client.DdeClient Client { get; }
        public int Timeout { get; set; } = 60000;

        public DdeClientPbAdapter(NDde.Client.DdeClient client)
        {
            Client = client;
        }

        public short? ExecRemote(string? command, out string? error)
        {
            error = null;

            try
            {
                Client.Execute(command);
            }
            catch (Exception e)
            {
                error = e.Message;
                return -2;
            }

            return 1;
        }

        public short? GetRemote(string? location, out string? target, bool ansi, out string? error)
        {
            target = null;
            error = null;
            var format = ansi ? Formats.CF_TEXT : Formats.CF_UNICODETEXT;
            try
            {
                var data = Client.Request(location, format, 10000);
                
                target = FormatTools.GetString(data, format);

                return 1;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -2;
            }
        }

        public short? SendRemote(string location, string data, bool ansi, out string? error)
        {
            error = null;

            var format = ansi ? Formats.CF_TEXT: Formats.CF_UNICODETEXT;

            try
            {
                Client.Poke(location, FormatTools.GetBytes(data, format), format, 1000);
            }
            catch (Exception e)
            {
                error = e.Message;
                return -2;
            }

            return 1;
        }
    }
}
