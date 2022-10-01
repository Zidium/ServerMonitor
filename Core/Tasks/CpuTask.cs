using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZidiumServerMonitor
{
    internal class CpuTask : BaseTask
    {
        public CpuTask(
            ILoggerFactory loggerFactory,
            ZidiumComponentsProvider zidiumComponentsProvider,
            IOptions<CpuTaskOptions> options,
            CpuInfoDataboxService cpuInfoDataboxService
            ) : base(loggerFactory, zidiumComponentsProvider, options.Value)
        {
            _options = options.Value;
            _cpuInfoDataboxService = cpuInfoDataboxService;
        }

        private readonly CpuTaskOptions _options;

        private readonly CpuInfoDataboxService _cpuInfoDataboxService;

        public override string Name => "CpuTask";

        protected override Task DoWork(CancellationToken cancellationToken)
        {
            var databox = _cpuInfoDataboxService.GetAndReset();

            if (databox.UsageCount == 0)
            {
                Logger.LogInformation("Cpu info not ready yet");
                return Task.CompletedTask;
            }

            var cpuUsage = Math.Round(databox.AverageUsagePercent, 2);
            var actual = GetNextDelay().Delay * 2;
            Logger.LogInformation($"CPU usage: {cpuUsage}%");
            ZidiumComponentsProvider.GetServerComponent().SendMetric("CPU usage, %", cpuUsage, actual);
            return Task.CompletedTask;
        }
    }
}
