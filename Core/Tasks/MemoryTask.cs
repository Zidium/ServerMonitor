using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZidiumServerMonitor
{
    public class MemoryTask : BaseTask
    {
        public MemoryTask(
            ILoggerFactory loggerFactory,
            IZidiumComponentsProvider zidiumComponentsProvider,
            IOptions<MemoryTaskOptions> options,
            MemoryInfoDataboxService memoryInfoDataboxService
            ) : base(loggerFactory, zidiumComponentsProvider, options.Value)
        {
            _options = options.Value;
            _memoryInfoDataboxService = memoryInfoDataboxService;
        }

        private readonly MemoryTaskOptions _options;

        private readonly MemoryInfoDataboxService _memoryInfoDataboxService;

        public override string Name => "MemoryTask";

        protected override Task DoWork(CancellationToken cancellationToken)
        {
            var availablePhysicalMemory = _memoryInfoDataboxService.GetAndReset().MinAvailablePhysical;
            var freeMemoryGb = availablePhysicalMemory.HasValue ? (double)availablePhysicalMemory / 1024 / 1024 / 1024 : (double?)null;
            var freeMemoryGbRounded = freeMemoryGb.HasValue ? Math.Round(freeMemoryGb.Value, 2) : (double?)null;
            Logger.LogInformation($"Free memory: {freeMemoryGbRounded} Gb");
            ZidiumComponentsProvider.GetServerComponent().SendMetric("Free memory, Gb", freeMemoryGbRounded, _options.ActualInterval);
            return Task.CompletedTask;
        }
    }
}
