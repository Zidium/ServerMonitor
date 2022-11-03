namespace ZidiumServerMonitor
{
    public class DiskPerformanceTaskOptions : BaseTaskOptions
    {
        public string[] Disks { get; set; } = new string[0];
    }
}
