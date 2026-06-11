using System.Runtime.InteropServices;

namespace Appeon.Windows.PBInterop.Win32
{
    internal static class Gdi32
    {
        [DllImport("gdi32.dll")]
        internal static extern uint GetBkColor(IntPtr hdc);

        [DllImport("gdi32.dll")]
        internal static extern uint GetTextColor(IntPtr hdc);
    }
}
