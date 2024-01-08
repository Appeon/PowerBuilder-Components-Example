namespace Appeon.ComponentsApp.SmsMessaging.Common;

public class Message
{
    public string Body { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public DateTime Sent { get; set; }

    public Message(string body, string from, string to, DateTime sent)
    {
        Body = body;
        From = from;
        To = to;
        Sent = sent;
    }
}
