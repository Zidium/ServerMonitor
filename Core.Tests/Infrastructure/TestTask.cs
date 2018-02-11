using System;

namespace ZidiumServerMonitor.Tests
{
    internal class TestTask : BaseTask
    {
        public override TimeSpan Interval
        {
            get { return TimeSpan.FromSeconds(10); }
        }

        public override TimeSpan Actual
        {
            get { return TimeSpan.FromSeconds(60); }
        }

        public override string Name { get { return "TestTask"; } }

        public int ExecutionCount;

        public override void DoWork()
        {
            ExecutionCount++;
        }
    }
}
