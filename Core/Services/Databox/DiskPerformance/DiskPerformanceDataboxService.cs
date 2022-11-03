namespace ZidiumServerMonitor
{
    public class DiskPerformanceDataboxService : BaseDataboxService<DiskPerformanceDatabox>
    {
        protected override DiskPerformanceDatabox Copy(DiskPerformanceDatabox value)
        {
            return new DiskPerformanceDatabox()
            {
                QueueLengthSum = value.QueueLengthSum,
                PercentTimeSum = value.PercentTimeSum,
                Count = value.Count
            };
        }

        public void Set(double queueLength, double percentTime)
        {
            Update(data =>
            {
                data.QueueLengthSum += queueLength;
                data.PercentTimeSum += percentTime;
                data.Count++;
            });
        }
    }
}
