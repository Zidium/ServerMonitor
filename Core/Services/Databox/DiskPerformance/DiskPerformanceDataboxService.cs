using System.Linq;

namespace ZidiumServerMonitor
{
    internal class DiskPerformanceDataboxService : BaseDataboxService<DiskPerformanceDatabox>
    {
        protected override DiskPerformanceDatabox Copy(DiskPerformanceDatabox value)
        {
            return new DiskPerformanceDatabox()
            {
                Disks = value.Disks.ToDictionary(t => t.Key, t => new DiskPerformanceDatabox.Disk()
                {
                    QueueLengthSum = t.Value.QueueLengthSum,
                    PercentTimeSum = t.Value.PercentTimeSum,
                    Count = t.Value.Count
                })
            };
        }

        public void Set(string name, double queueLength, double percentTime)
        {
            Update(data =>
            {
                if (!data.Disks.TryGetValue(name, out var disk))
                {
                    disk = new DiskPerformanceDatabox.Disk();
                    data.Disks.Add(name, disk);
                }

                disk.QueueLengthSum += queueLength;
                disk.PercentTimeSum += percentTime;
                disk.Count++;
            });
        }
    }
}
