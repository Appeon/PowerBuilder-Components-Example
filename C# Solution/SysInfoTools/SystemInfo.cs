namespace Appeon.ComponentsApp.SystemInfoTools
{
    public class SystemInfo
    {
        public string OsCaption { get; set; }
        public string OsArchitecture { get; set; }
        public string OsVersion { get; set; }
        public string CpuName { get; set; }

        public string[] ToStringArray()
        {
            return new string[] {
                OsCaption,
                OsArchitecture,
                OsVersion,
                CpuName
            };
        }
    }
}
