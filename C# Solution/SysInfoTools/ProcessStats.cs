namespace Appeon.ComponentsApp.SystemInfoTools
{
    public class ProcessStats
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public double AllocatedMemory { get; set; }
        public double CpuUsage { get; set; }

        public override string ToString()
        {
            return $"{ProcessId},{ProcessName},{CpuUsage},{AllocatedMemory}";
        }

        public string[] ToArray()
        {
            return new string[]
            {
                ProcessId.ToString(),
                ProcessName,
                AllocatedMemory.ToString(),
                CpuUsage.ToString()
            };
        }
    }
}
