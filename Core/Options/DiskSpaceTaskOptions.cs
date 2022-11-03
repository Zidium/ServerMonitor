namespace ZidiumServerMonitor
{
    public class DiskSpaceTaskOptions : BaseTaskOptions
    {
        public string[] Disks { get; set; } = new string[0];
    }
}
