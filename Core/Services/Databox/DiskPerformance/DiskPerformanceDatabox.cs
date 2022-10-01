using System.Collections.Generic;

namespace ZidiumServerMonitor
{
    internal class DiskPerformanceDatabox
    {
        public Dictionary<string, Disk> Disks = new Dictionary<string, Disk>();

        internal class Disk
        {
            public double QueueLengthSum;

            public double PercentTimeSum;

            public int Count;

            public double AverageQueueLength
            {
                get
                {
                    return QueueLengthSum / Count;
                }
            }

            public double AveragePercentTime
            {
                get
                {
                    return PercentTimeSum / Count;
                }
            }
        }
    }
}
