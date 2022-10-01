using System.Collections.Generic;

namespace ZidiumServerMonitor
{
    internal interface IDiskPerformanceOsSpecific
    {
        public List<DiskPerformance> GetForAllDisks();
    }
}
