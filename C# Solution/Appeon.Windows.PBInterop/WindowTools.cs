using Appeon.Windows.PBInterop.Win32;

namespace Appeon.Windows.PBInterop;

public class WindowTools
{
    public static uint GetBackgroundColor(uint hWnd)
    {
        var dc = User32.GetDCEx(new IntPtr(hWnd), IntPtr.Zero, DeviceContextValues.Window);

        return Gdi32.GetBkColor(dc);
    }

    public static uint GetTextColor(uint hWnd)
    {
        var dc = User32.GetDCEx(new IntPtr(hWnd), IntPtr.Zero, DeviceContextValues.Window);
        return Gdi32.GetTextColor(dc);
    }

    public static void GetWindowScreenRect(uint hWnd, out int top, out int bottom, out int right, out int left)
    {
        string wrapper = "";
        User32.GetWindowRect(new System.Runtime.InteropServices.HandleRef(wrapper, new IntPtr(hWnd)), out User32.RECT rect);

        top = rect.Top;
        bottom = rect.Bottom;
        right = rect.Right;
        left = rect.Left;
    }

    public static void GetWindowRect(uint hWnd, out int top, out int bottom, out int right, out int left)
    {
        string wrapper = "";
        User32.GetWindowRect(new System.Runtime.InteropServices.HandleRef(wrapper, new IntPtr(hWnd)), out User32.RECT rect);
        var window = User32.GetActiveWindow();
        User32.GetWindowRect(new System.Runtime.InteropServices.HandleRef(wrapper, window), out User32.RECT windowRect);
        var height = (User32.GetSystemMetrics(SystemMetric.SM_CYFRAME)
            + User32.GetSystemMetrics(SystemMetric.SM_CYCAPTION)
            + User32.GetSystemMetrics(SystemMetric.SM_CXPADDEDBORDER));

        top = rect.Top - windowRect.Top - height;
        bottom = rect.Bottom - windowRect.Top - height;
        right = rect.Right - windowRect.Left;
        left = rect.Left - windowRect.Left;
    }

}
