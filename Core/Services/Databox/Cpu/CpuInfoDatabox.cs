using System;

namespace ZidiumServerMonitor
{
    public class CpuInfoDatabox
    {
        public int UsagePercentSum;

        public int UsageCount;

        public double AverageUsagePercent
        {
            get
            {
                return (double)UsagePercentSum / Math.Max(UsageCount, 1);
            }
        }
    }
}
