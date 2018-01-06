using System;

namespace ZidiumServerMonitor
{
    public class Settings
    {
        public Guid ServerId { get; set; }

        public FreeMemoryTaskSettings Memory { get; set; } = new FreeMemoryTaskSettings();

        public FreeDiskSpaceTaskSettings Disk { get; set; } = new FreeDiskSpaceTaskSettings();

        public class FreeMemoryTaskSettings
        {
            public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(10);
        }

        public class FreeDiskSpaceTaskSettings
        {
            public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(10);

            public string[] Disks { get; set; } = new string[0];
        }
    }
}
