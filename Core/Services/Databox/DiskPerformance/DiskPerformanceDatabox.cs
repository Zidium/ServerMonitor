using System;

namespace ZidiumServerMonitor
{
    public class DiskPerformanceDatabox
    {
        public double QueueLengthSum;

        public double PercentTimeSum;

        public int Count;

        public double AverageQueueLength
        {
            get
            {
                return QueueLengthSum / Math.Max(Count, 1);
            }
        }

        public double AveragePercentTime
        {
            get
            {
                return PercentTimeSum / Math.Max(Count, 1);
            }
        }
    }
}
