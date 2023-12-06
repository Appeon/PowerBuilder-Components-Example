using FluentFTP;

namespace Appeon.ComponentsApp.FtpClientWrapper
{
    public class FtpClientFactory
    {
        public static FtpClientWrapper? CreateFtpClient(string address,
            int port,
            string? username,
            string? password,
            int authenticationMode,
            out string? error)
        {
            error = null;

            try
            {
                var client = new AsyncFtpClient(address,
                port: port,
                user: username,
                pass: password,
                config: new FtpConfig
                {
                    EncryptionMode = authenticationMode switch
                    {
                        >= 0 and <= 3 => (FtpEncryptionMode)authenticationMode,
                        _ => throw new ArgumentException("Invalid authentication mode", nameof(authenticationMode)),
                    },
                    ValidateAnyCertificate = true,
                    DataConnectionEncryption = true,
                    BulkListing = true,
                    LogUserName = true,
                    LogHost = true,

                });

                if (string.IsNullOrEmpty(address))
                {
                    throw new Exception("Server address cannot be null");
                }
                var task = client.AutoConnect();
                if (!task.Wait(TimeSpan.FromSeconds(10)))
                { // timed out
                    throw new TimeoutException("Operation timed out");
                }

                return new FtpClientWrapper(client);
            }
            catch (Exception e)
            {
                error = e.Message;
                return null;
            }


        }
    }
}
