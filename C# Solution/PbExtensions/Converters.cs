using System;

namespace Appeon.CSharpPbExtensions
{
    public class Converters
    {
        public static byte[] Base64ToByteArray(string base64)
        {
            return Convert.FromBase64String(base64);
        }

        public static string ByteArrayToBase64(byte[] array)
        {
            return Convert.ToBase64String(array);
        }
    }
}
