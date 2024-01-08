using Appeon.ComponentsApp.SmsMessaging.Common;
using Twilio;

namespace Appeon.ComponentsApp.SmsMessaging.Twilio;

public class TwilioSmsMessagingServiceBuilder : ISmsMessagingServiceBuilder<TwilioSmsMessagingService>
{
    private readonly string _sid;
    private readonly string _authToken;

    public TwilioSmsMessagingServiceBuilder(string sid, string authToken)
    {
        _sid = sid;
        _authToken = authToken;
    }

    public TwilioSmsMessagingService Build()
    {
        TwilioClient.Init(_sid, _authToken);

        return new TwilioSmsMessagingService();
    }
}
