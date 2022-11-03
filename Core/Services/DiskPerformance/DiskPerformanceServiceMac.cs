using System.Collections.Generic;

namespace ZidiumServerMonitor
{
    internal class DiskPerformanceServiceMac : IDiskPerformanceOsSpecific
    {
        public List<DiskPerformance> GetForAllDisks()
        {
            return new List<DiskPerformance>();
        }
    }
}
