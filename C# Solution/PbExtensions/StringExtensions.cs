using System;
using System.Text.RegularExpressions;

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

        public static bool Match(string source, string pattern, out string error)
        {
            error = null;

            try
            {
                return Regex.Match(source, pattern).Success;
            }
            catch (Exception e)
            {
                error = e.ToString();
            }

            return false;
        }
    }
}
