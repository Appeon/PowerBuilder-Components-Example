using System.Text.Json;

namespace Appeon.ComponentsApp.DdeTools.Common.Messages
{
    public static class MessageSerializer
    {
        public static string SerializeMessage(Message message)
        {
            return JsonSerializer.Serialize(message);
        }

        public static Message? DeserializeMessage(string jsonString)
        {
            return JsonSerializer.Deserialize<Message>(jsonString);
        }
    }
}
