using System.Text;

namespace Appeon.ComponentsApp.SystemInfoTools
{
    public class Main
    {

        public static string[] GetSystemInfo()
        {
            return SystemInfoTools.GetSystemInfo().ToStringArray();
        }

        public static double[] GetPerformanceInfo()
        {
            return SystemInfoTools.GetPerformanceInfo().ToDoubleArray();
        }

        public static string GetProcessTable()
        {
            var sb = new StringBuilder();
            var processes = SystemInfoTools.GetProcesses();

            for (int i = 0; i < processes.Count; i++)
                sb.AppendLine(processes[i].ToString());

            return sb.ToString();
        }
    }
}
