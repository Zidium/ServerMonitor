using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hardware.Info;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZidiumServerMonitor
{
    internal class CpuInfoCollector : BaseCollector
    {
        public CpuInfoCollector(
            ILoggerFactory loggerFactory,
            HardwareInfo hardwareInfo,
            CpuInfoDataboxService cpuInfoDataboxService,
            IOptions<CpuTaskOptions> options
            ) : base(loggerFactory, options.Value.Enabled)
        {
            _hardwareInfo = hardwareInfo;
            _cpuInfoDataboxService = cpuInfoDataboxService;
            _options = options.Value;
        }

        private readonly HardwareInfo _hardwareInfo;

        private readonly CpuInfoDataboxService _cpuInfoDataboxService;

        private readonly CpuTaskOptions _options;

        protected override string Name => "CpuInfoCollector";

        protected override TimeSpan Interval => TimeSpan.FromSeconds(1);

        protected override Task DoWork(CancellationToken cancellationToken)
        {
            Logger.LogTrace("Refreshing cpu info...");
            _hardwareInfo.RefreshCPUList();
            cancellationToken.ThrowIfCancellationRequested();

            var usagePercent = (int)_hardwareInfo.CpuList.Average(t => (double)t.PercentProcessorTime);
            Logger.LogTrace($"CPU usage: {usagePercent}%");
            _cpuInfoDataboxService.Set(usagePercent);

            return Task.CompletedTask;
        }
    }
}
