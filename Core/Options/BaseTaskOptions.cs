using System;

namespace ZidiumServerMonitor
{
    public class BaseTaskOptions
    {
        public string Schedule { get; set; } = "0 * * * * *";

        public TimeSpan ActualInterval { get; set; } = TimeSpan.FromMinutes(2);

        public bool Enabled { get; set; } = true;
    }
}
