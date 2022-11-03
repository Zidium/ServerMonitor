using System.Collections.Generic;
using System.Management;

namespace ZidiumServerMonitor
{
    internal class DiskPerformanceServiceWindows : IDiskPerformanceOsSpecific
    {
        private readonly string _managementScope = "root\\cimv2";
        private readonly EnumerationOptions _enumerationOptions = new EnumerationOptions()
        {
            ReturnImmediately = true,
            Rewindable = false,
            Timeout = ManagementOptions.InfiniteTimeout
        };

        public List<DiskPerformance> GetForAllDisks()
        {
            var result = new List<DiskPerformance>();

            var queryString = "SELECT AvgDiskQueueLength, PercentDiskTime, Name FROM win32_perfformatteddata_perfdisk_physicaldisk";
            using ManagementObjectSearcher mos = new ManagementObjectSearcher(_managementScope, queryString, _enumerationOptions);

            foreach (ManagementObject mo in mos.Get())
            {
                var item = new DiskPerformance()
                {
                    Name = GetPropertyString(mo["Name"]),
                    AvgDiskQueueLength = GetPropertyValue<ulong>(mo["AvgDiskQueueLength"]),
                    PercentDiskTime = GetPropertyValue<ulong>(mo["PercentDiskTime"])
                };

                result.Add(item);
            }

            return result;
        }

        private T GetPropertyValue<T>(object obj) where T : struct
        {
            return (obj == null) ? default(T) : (T)obj;
        }

        private string GetPropertyString(object obj)
        {
            return (obj is string str) ? str : string.Empty;
        }
    }
}
