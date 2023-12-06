using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;

namespace Appeon.ComponentsApp.SystemInfoTools
{
    public static class SystemInfoTools
    {
        public static SystemInfo GetSystemInfo()
        {
            var info = new SystemInfo();

            //Create an object of ManagementObjectSearcher class and pass query as parameter.
            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            foreach (ManagementObject managementObject in mos.Get().Cast<ManagementObject>())
            {
                if (managementObject["Caption"] != null)
                {
                    info.OsCaption = managementObject["Caption"].ToString();
                }
                if (managementObject["OSArchitecture"] != null)
                {
                    info.OsArchitecture = managementObject["OSArchitecture"].ToString();

                }
                if (managementObject["Version"] != null)
                {
                    info.OsVersion = managementObject["Version"].ToString();
                }
            }

            RegistryKey processor_name = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree);   //This registry entry contains entry for processor info.

            if (processor_name != null)
            {
                if (processor_name.GetValue("ProcessorNameString") != null)
                {
                    info.CpuName = processor_name.GetValue("ProcessorNameString").ToString();   //Display processor info.
                }
            }

            return info;
        }

        public static PerformanceInfo GetPerformanceInfo()
        {
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            var memCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use", "");
            var memBytesCounter = new PerformanceCounter("Memory", "Committed Bytes", "");
            cpuCounter.NextValue();
            Thread.Sleep(50);
            return new PerformanceInfo
            {
                CpuUtilization = double.Parse(string.Format("{0:0.0000}", cpuCounter.NextValue())),
                MemoryUtilizationPercent = double.Parse(string.Format("{0:0.0000}", memCounter.NextValue())),
                MemoryUtilization = double.Parse(string.Format("{0:0.00}", memBytesCounter.NextValue() / 1024.0 / 1024.0)),
            };
        }

        public static IList<ProcessStats> GetProcesses()
        {
            var processes = Process.GetProcesses();
            var procCpuTimeMap = new Dictionary<int, Tuple<double, double>>();

            foreach (var process in processes)
            {
                if (process.Id == 0)
                    continue;
                try
                {

                    procCpuTimeMap.Add(process.Id,
                                       new Tuple<double, double>(DateTime.Now.Ticks,
                                            process.TotalProcessorTime.Ticks)
                    );
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine($"Process {process.Id} [{process.ProcessName}] has exited.");
                }
            }

            Thread.Sleep(50);

            var newProcessesList = new List<ProcessStats>(processes.Length);
            long newMemory;
            foreach (var p in processes)
            {
                if (!procCpuTimeMap.ContainsKey(p.Id)) // Process was not added to the map
                    continue;
                p.Refresh();

                var timeDiff = DateTime.Now.Ticks - procCpuTimeMap[p.Id].Item1;
                double cpuTimeDiff;

                try
                {
                    // Will throw if application was terminated while we waited
                    // Cannot use Process.HasExited because it throws Access Denied in some cases
                    cpuTimeDiff = p.TotalProcessorTime.Ticks - procCpuTimeMap[p.Id].Item2;
                    newMemory = p.PrivateMemorySize64;
                }
                catch
                {
                    Console.Error.WriteLine("Error reading process info the 2nd time");
                    continue;
                }

                newProcessesList.Add(new ProcessStats()
                {
                    ProcessId = p.Id,
                    ProcessName = p.ProcessName,
                    AllocatedMemory = newMemory >> 20, /* AllocatedMemory = newMemory / 1024 / 1024 */
                    CpuUsage = double.Parse(string.Format("{0:0.00}", cpuTimeDiff
                            / (timeDiff * 2)
                            * 100.0))

                });
            }

            return newProcessesList;

        }
    }
}
