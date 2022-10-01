using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ZidiumServerMonitor
{
    internal class DiskSpaceTask : BaseTask
    {
        public DiskSpaceTask(
            ILoggerFactory loggerFactory,
            ZidiumComponentsProvider zidiumComponentsProvider,
            IOptions<DiskSpaceTaskOptions> options,
            FreeSpaceService freeSpaceService) : base(loggerFactory, zidiumComponentsProvider, options.Value)
        {
            _options = options.Value;
            _freeSpaceService = freeSpaceService;
        }

        private readonly DiskSpaceTaskOptions _options;
        private readonly FreeSpaceService _freeSpaceService;

        public override string Name => "DiskSpaceTask";

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);

            if (!_options.Enabled)
                return;

            Logger.LogInformation($"Disks: {string.Join(", ", _options.Disks)}");
        }

        protected override Task DoWork(CancellationToken cancellationToken)
        {
            ExceptionDispatchInfo firstException = null;
            foreach (var disk in _options.Disks)
            {
                try
                {
                    var freeSpace = _freeSpaceService.GetDriveFreeSpace(disk);

                    if (freeSpace.HasValue)
                    {
                        var freeSpaceGb = (double)freeSpace.Value / 1024 / 1024 / 1024;
                        var freeSpaceGbRounded = Math.Round(freeSpaceGb, 2);
                        var actual = GetNextDelay().Delay * 2;
                        Logger.LogInformation($"Free space on disk {disk}: {freeSpaceGbRounded} Gb");
                        ZidiumComponentsProvider.GetServerComponent().SendMetric("Free space on disk " + disk + ", Gb", freeSpaceGbRounded, actual);                        
                    }
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
