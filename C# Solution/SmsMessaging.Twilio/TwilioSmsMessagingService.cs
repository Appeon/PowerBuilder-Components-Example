using Appeon.ComponentsApp.SmsMessaging.Common;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Appeon.ComponentsApp.SmsMessaging.Twilio;

public class TwilioSmsMessagingService : ISmsMessagingService
{

    public bool GetAvailableNumbers(out string[]? numbers, out string? error)
    {
        error = null;
        numbers = null;

        List<string> numbersList = new();
        try
        {
            var incomingPhoneNumbers = IncomingPhoneNumberResource.Read();

            foreach (var record in incomingPhoneNumbers)
            {
                numbersList.Add(record.PhoneNumber.ToString());
            }

            numbers = numbersList.ToArray();
        }
        catch (Exception e)
        {
            error = e.Message;
            return false;
        }
        return true;
    }

    public void RegisterMessageCallback(Action<string, string> callback)
    {
        throw new NotImplementedException();
    }

    public bool SendSms(string fromPhoneNumber,
        string toPhoneNumber,
        string message,
        bool schedule,
        DateTime scheduleFor,
        out Message? sentMsg,
        out string? error,
        int attempts = 1
        )
    {
        error = null;
        sentMsg = null;

        if (schedule && scheduleFor < DateTime.Now)
        {
            error = "Cannot send message in the past";
            return false;
        }

        try
        {
            var options = new CreateMessageOptions(
               new PhoneNumber(toPhoneNumber))
            {
                From = new PhoneNumber(fromPhoneNumber),
                Body = message,
                Attempt = attempts,
            };


            if (schedule)
            {
                options.SendAt = scheduleFor;
            }

            var twilioMsg = MessageResource.Create(options);

            sentMsg = new Message(twilioMsg.Body,
                twilioMsg.From.ToString(),
                twilioMsg.To,
                twilioMsg.DateCreated!.Value);
        }
        catch (Exception e)
        {
            error = e.Message;
            return false;
        }

        return true;
    }
}
