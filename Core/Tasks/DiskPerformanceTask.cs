using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZidiumServerMonitor
{
    internal class DiskPerformanceTask : BaseTask
    {
        public DiskPerformanceTask(
            ILoggerFactory loggerFactory,
            ZidiumComponentsProvider zidiumComponentsProvider,
            IOptions<DiskPerformanceTaskOptions> options,
            DiskPerformanceDataboxService diskPerformanceDataboxService
            ) : base(loggerFactory, zidiumComponentsProvider, options.Value)
        {
            _diskPerformanceDataboxService = diskPerformanceDataboxService;
        }

        private readonly DiskPerformanceDataboxService _diskPerformanceDataboxService;

        public override string Name => "DiskPerformanceTask";

        protected override Task DoWork(CancellationToken cancellationToken)
        {
            var databox = _diskPerformanceDataboxService.GetAndReset();

            if (databox.Disks.Count == 0)
            {
                Logger.LogInformation("Disk performance info not ready yet");
                return Task.CompletedTask;
            }

            var actual = GetNextDelay().Delay * 2;

            foreach (var disk in databox.Disks)
            {
                var name = disk.Key;
                var averageQueueLength = Math.Round(disk.Value.AverageQueueLength, 2);
                var averagePercentTime = Math.Round(disk.Value.AveragePercentTime, 2);

                Logger.LogInformation($"Disk '{name}', AverageQueueLength: {averageQueueLength}, AveragePercentTime: {averagePercentTime}");
                ZidiumComponentsProvider.GetServerComponent().SendMetric($"Disk {name}, Average queue length", averageQueueLength, actual);
                ZidiumComponentsProvider.GetServerComponent().SendMetric($"Disk {name}, Average percent time, %", averagePercentTime, actual);
            }

            return Task.CompletedTask;
        }
    }
}
