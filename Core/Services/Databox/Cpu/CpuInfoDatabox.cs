namespace ZidiumServerMonitor
{
    internal class CpuInfoDatabox
    {
        public int UsagePercentSum;

        public int UsageCount;

        public double AverageUsagePercent
        {
            get
            {
                return (double)UsagePercentSum / UsageCount;
            }
        }
    }
}
