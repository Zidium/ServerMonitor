using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZidiumServerMonitor
{
    internal class DiskPerformanceCollector : BaseCollector
    {
        public DiskPerformanceCollector(
            ILoggerFactory loggerFactory,
            IOptions<DiskPerformanceTaskOptions> options,
            DiskPerformanceService diskPerformanceService,
            DiskPerformanceDataboxService diskPerformanceDataboxService
            ) : base(loggerFactory, options.Value.Enabled)
        {
            _options = options.Value;
            _diskPerformanceService = diskPerformanceService;
            _diskPerformanceDataboxService = diskPerformanceDataboxService;
        }

        private readonly DiskPerformanceTaskOptions _options;

        private readonly DiskPerformanceService _diskPerformanceService;

        private readonly DiskPerformanceDataboxService _diskPerformanceDataboxService;

        protected override string Name => "DiskPerformanceCollector";

        protected override TimeSpan Interval => TimeSpan.FromSeconds(1);

        protected override Task DoWork(CancellationToken cancellationToken)
        {
            Logger.LogTrace("Getting disks performance...");
            var disksPerformance = _diskPerformanceService.GetForAllDisks();
            cancellationToken.ThrowIfCancellationRequested();

            ExceptionDispatchInfo firstException = null;
            foreach (var disk in _options.Disks)
            {
                try
                {
                    var diskPerformance = disksPerformance.FirstOrDefault(t => string.Equals(disk, t.Name, StringComparison.OrdinalIgnoreCase));

                    if (diskPerformance == null)
                        throw new Exception($"Performance data for disk '{disk}' not found");

                    var percentDiskTime = Math.Min(diskPerformance.PercentDiskTime, 100);

                    Logger.LogTrace($"Disk '{diskPerformance.Name}', AvgDiskQueueLength: {diskPerformance.AvgDiskQueueLength}, PercentDiskTime: {percentDiskTime}");
                    _diskPerformanceDataboxService.Set(diskPerformance.Name, diskPerformance.AvgDiskQueueLength, percentDiskTime);
                }
                catch (Exception exception)
                {
                    if (firstException == null)
                        firstException = ExceptionDispatchInfo.Capture(exception);
                }
            }

            if (firstException != null)
                firstException.Throw();

            return Task.CompletedTask;
        }
    }
}
