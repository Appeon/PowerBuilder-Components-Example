namespace Appeon.ComponentsApp.SmsMessaging.Common
{
    public interface ISmsMessagingService
    {
        void RegisterMessageCallback(Action<string, string> callback);

        bool SendSms(
            string fromPhoneNumber,
            string toPhoneNumber,
            string message,
            bool schedule,
            DateTime scheduleFor,
            out Message? sentMsg,
            out string? error,
            int attempts = 1);

        bool GetAvailableNumbers(out string[]? numbers, out string? error);
    }
}