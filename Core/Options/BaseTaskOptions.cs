namespace ZidiumServerMonitor
{
    internal class BaseTaskOptions
    {
        public string Schedule { get; set; } = "0 * * * * *";

        public bool Enabled { get; set; } = true;
    }
}
