namespace ZidiumServerMonitor
{
    internal class DiskSpaceTaskOptions : BaseTaskOptions
    {
        public string[] Disks { get; set; } = new string[0];
    }
}
