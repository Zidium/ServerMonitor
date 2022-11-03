using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZidiumServerMonitor
{
    public class DiskPerformanceTask : BaseTask
    {
        public DiskPerformanceTask(
            ILoggerFactory loggerFactory,
            IZidiumComponentsProvider zidiumComponentsProvider,
            IOptions<DiskPerformanceTaskOptions> options,
            DiskPerformanceDataboxServiceFactory diskPerformanceDataboxServiceFactory
            ) : base(loggerFactory, zidiumComponentsProvider, options.Value)
        {
            _options = options.Value;
            _diskPerformanceDataboxServiceFactory = diskPerformanceDataboxServiceFactory;
        }

        private readonly DiskPerformanceTaskOptions _options;

        private readonly DiskPerformanceDataboxServiceFactory _diskPerformanceDataboxServiceFactory;

        public override string Name => "DiskPerformanceTask";

        protected override Task DoWork(CancellationToken cancellationToken)
        {
            foreach (var disk in _options.Disks)
            {
                var databox = _diskPerformanceDataboxServiceFactory.GetDataboxService(disk).GetAndReset();

                var averageQueueLength = databox.Count > 0 ? Math.Round(databox.AverageQueueLength, 2) : (double?)null;
                var averagePercentTime = databox.Count > 0 ? Math.Round(databox.AveragePercentTime, 2) : (double?)null;

                Logger.LogInformation($"Disk '{disk}', AverageQueueLength: {averageQueueLength}, AveragePercentTime: {averagePercentTime}");
                ZidiumComponentsProvider.GetServerComponent().SendMetric($"Disk {disk}, Average queue length", averageQueueLength, _options.ActualInterval);
                ZidiumComponentsProvider.GetServerComponent().SendMetric($"Disk {disk}, Average percent time, %", averagePercentTime, _options.ActualInterval);
            }

            return Task.CompletedTask;
        }
    }
}
