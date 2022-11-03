using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZidiumServerMonitor
{
    public class CpuTask : BaseTask
    {
        public CpuTask(
            ILoggerFactory loggerFactory,
            IZidiumComponentsProvider zidiumComponentsProvider,
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
            var cpuUsage = databox.UsageCount > 0 ? Math.Round(databox.AverageUsagePercent, 2) : (double?)null;
            Logger.LogInformation($"CPU usage: {cpuUsage}%");
            ZidiumComponentsProvider.GetServerComponent().SendMetric("CPU usage, %", cpuUsage, _options.ActualInterval);
            return Task.CompletedTask;
        }
    }
}
