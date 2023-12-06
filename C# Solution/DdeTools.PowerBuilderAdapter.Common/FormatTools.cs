using System.Text;

namespace Appeon.ComponentsApp.DdeTools.PowerBuilderAdapter.Common
{
    public static class FormatTools
    {
        public static string GetString(byte[] data, int format)
        {
            return (format switch
            {
                Formats.CF_TEXT => Encoding.ASCII,
                Formats.CF_UNICODETEXT => Encoding.Unicode,
                _ => throw new ArgumentException("Invalid format", nameof(format)),
            }).GetString(data);
        }

        public static byte[] GetBytes(string str, int format)
        {
            return (format switch
            {
                Formats.CF_TEXT => Encoding.ASCII,
                Formats.CF_UNICODETEXT => Encoding.Unicode,
                _ => throw new ArgumentException("Invalid format", nameof(format)),
            }).GetBytes(str);
        }
    }
}
