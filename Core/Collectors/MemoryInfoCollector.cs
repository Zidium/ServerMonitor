using System;
using System.Threading;
using System.Threading.Tasks;
using Hardware.Info;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZidiumServerMonitor
{
    internal class MemoryInfoCollector : BaseCollector
    {
        public MemoryInfoCollector(
            ILoggerFactory loggerFactory,
            HardwareInfo hardwareInfo,
            MemoryInfoDataboxService memoryInfoDataboxService,
            IOptions<MemoryTaskOptions> options
            ) : base(loggerFactory, options.Value.Enabled)
        {
            _hardwareInfo = hardwareInfo;
            _memoryInfoDataboxService = memoryInfoDataboxService;
            _options = options.Value;
        }

        private readonly HardwareInfo _hardwareInfo;

        private readonly MemoryInfoDataboxService _memoryInfoDataboxService;

        private readonly MemoryTaskOptions _options;

        protected override string Name => "MemoryInfoCollector";

        protected override TimeSpan Interval => TimeSpan.FromSeconds(10);

        protected override Task DoWork(CancellationToken cancellationToken)
        {
            Logger.LogTrace("Refreshing memory info...");
            _hardwareInfo.RefreshMemoryStatus();
            cancellationToken.ThrowIfCancellationRequested();

            var availablePhysicalMemory = _hardwareInfo.MemoryStatus.AvailablePhysical;
            Logger.LogTrace($"Available physical memory: {availablePhysicalMemory}");
            _memoryInfoDataboxService.Set(availablePhysicalMemory);

            return Task.CompletedTask;
        }
    }
}
