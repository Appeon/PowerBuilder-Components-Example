namespace Appeon.ComponentsApp.SmsMessaging.Common;

public interface ISmsMessagingServiceBuilder<T>
    where T : ISmsMessagingService
{
    T Build();
}
