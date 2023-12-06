namespace Appeon.ComponentsApp.DdeTools.PowerBuilderAdapter.Common
{
    public static class Formats
    {
        public const int CF_TEXT = 1;
        public const int CF_UNICODETEXT = 13;

        public static bool CanAcceptFormat(int format)
        {
            return format == CF_TEXT || format == CF_UNICODETEXT;
        }
    }
}
