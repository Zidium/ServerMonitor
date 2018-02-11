using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NickStrupat;

namespace ZidiumServerMonitor
{
    public class FreeMemoryTask : BaseTask
    {
        public FreeMemoryTask()
        {
            _settings = DependencyInjection.Services.GetRequiredService<Settings>().Memory;
            _computerInfo = new ComputerInfo();
        }

        private Settings.FreeMemoryTaskSettings _settings;

        private ComputerInfo _computerInfo;

        public override TimeSpan Interval { get { return _settings.Interval; } }

        public override TimeSpan Actual { get { return _settings.Timeout; } }

        public override string Name { get { return "FreeMemoryTask"; } }

        public override void DoWork()
        {
            var freeMemory = (long)_computerInfo.AvailablePhysicalMemory;
            var freeMemoryGb = (double)freeMemory / 1024 / 1024 / 1024;
            var freeMemoryGbRounded = Math.Round(freeMemoryGb, 2);
            ZidiumHelper.ServerComponent.SendMetric("Free memory, Gb", freeMemoryGbRounded);
            Logger.LogInformation($"Free memory: {freeMemoryGbRounded} Gb");
        }
    }
}
