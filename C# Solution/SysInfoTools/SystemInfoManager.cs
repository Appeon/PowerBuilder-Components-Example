namespace Appeon.ComponentsApp.SystemInfoTools
{
    public class SystemInfoManager
    {
        public static void GetSystemInfo(out string[] systemInfo)
        {
            systemInfo = SystemInfoTools.GetSystemInfo().ToStringArray();
        }
    }
}
