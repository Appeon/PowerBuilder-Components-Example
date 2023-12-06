using System.Security.Principal;

namespace Appeon.ComponentsApp.ProcessTools.Windows;

public class ProcessTools
{
    public static bool RunningAsAdmin() =>
       new WindowsPrincipal(WindowsIdentity.GetCurrent())
           .IsInRole(WindowsBuiltInRole.Administrator);
}
