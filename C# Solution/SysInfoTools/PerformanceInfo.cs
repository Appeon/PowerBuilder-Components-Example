namespace Appeon.ComponentsApp.SystemInfoTools
{
    public class PerformanceInfo
    {
        public double CpuUtilization { get; set; }
        public double MemoryUtilizationPercent { get; set; }
        public double MemoryUtilization { get; set; }

        public double[] ToDoubleArray()
        {
            return new double[] { CpuUtilization, MemoryUtilizationPercent, MemoryUtilization };
        }
    }
}
