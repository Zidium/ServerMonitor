namespace ZidiumServerMonitor
{
    public class CpuInfoDataboxService : BaseDataboxService<CpuInfoDatabox>
    {
        protected override CpuInfoDatabox Copy(CpuInfoDatabox value)
        {
            return new CpuInfoDatabox()
            {
                UsagePercentSum = value.UsagePercentSum,
                UsageCount = value.UsageCount
            };
        }

        public void Set(int usagePercent)
        {
            Update(data =>
            {
                data.UsagePercentSum += usagePercent;
                data.UsageCount++;
            });
        }
    }
}
