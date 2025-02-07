using System;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZidiumServerMonitor
{
    public class DiskPerformanceCollector : BaseCollector
    {
        public DiskPerformanceCollector(
            ILoggerFactory loggerFactory,
            IZidiumComponentsProvider zidiumComponentsProvider,
            IOptions<DiskPerformanceTaskOptions> options,
            DiskPerformanceService diskPerformanceService,
            DiskPerformanceDataboxServiceFactory diskPerformanceDataboxServiceFactory
            ) : base(loggerFactory, zidiumComponentsProvider, options.Value.Enabled)
        {
            _options = options.Value;
            _diskPerformanceService = diskPerformanceService;
            _diskPerformanceDataboxServiceFactory = diskPerformanceDataboxServiceFactory;
        }

        private readonly DiskPerformanceTaskOptions _options;

        private readonly DiskPerformanceService _diskPerformanceService;

        private readonly DiskPerformanceDataboxServiceFactory _diskPerformanceDataboxServiceFactory;

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
                var diskName = disk;

                try
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        diskName = CheckLinuxDiskById(diskName);

                    var diskPerformance = disksPerformance.FirstOrDefault(t => string.Equals(diskName, t.Name, StringComparison.OrdinalIgnoreCase));

                    if (diskPerformance == null)
                        throw new Exception($"Performance data for disk '{disk}' not found");

                    diskPerformance.PercentDiskTime = Math.Min(diskPerformance.PercentDiskTime, 100);

                    Logger.LogTrace($"Disk '{disk}', AvgDiskQueueLength: {diskPerformance.AvgDiskQueueLength}, PercentDiskTime: {diskPerformance.PercentDiskTime}");
                    _diskPerformanceDataboxServiceFactory.GetDataboxService(disk).Set(diskPerformance.AvgDiskQueueLength, diskPerformance.PercentDiskTime);
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

        private static string CheckLinuxDiskById(string disk)
        {
            string idPath = $"/dev/disk/by-id/{disk}";

            if (File.Exists(idPath))
            {
                var linkTarget = File.ResolveLinkTarget(idPath, true);

                if (linkTarget != null)
                    disk = linkTarget.Name;
            }

            return disk;
        }
    }
}
