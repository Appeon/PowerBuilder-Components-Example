using System;

namespace Appeon.CSharpPbExtensions
{
    public class StringExtensions
    {
        public static string Replace(string source, string pattern, string replace)
        {
            return source.Replace(pattern, replace);
        }

        public static void Split(string source, string separator, out string[] stringArray)
        {
            stringArray = source.Split(separator);
        }
    }
}
