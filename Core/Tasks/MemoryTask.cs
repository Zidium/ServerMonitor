using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZidiumServerMonitor
{
    internal class MemoryTask : BaseTask
    {
        public MemoryTask(
            ILoggerFactory loggerFactory,
            ZidiumComponentsProvider zidiumComponentsProvider,
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

            if (!availablePhysicalMemory.HasValue)
            {
                Logger.LogInformation("Memory info not ready yet");
                return Task.CompletedTask;
            }

            var freeMemoryGb = (double)availablePhysicalMemory / 1024 / 1024 / 1024;
            var freeMemoryGbRounded = Math.Round(freeMemoryGb, 2);
            var actual = GetNextDelay().Delay * 2;
            Logger.LogInformation($"Free memory: {freeMemoryGbRounded} Gb");
            ZidiumComponentsProvider.GetServerComponent().SendMetric("Free memory, Gb", freeMemoryGbRounded, actual);            
            return Task.CompletedTask;
        }
    }
}
