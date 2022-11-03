using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ZidiumServerMonitor
{
    public abstract class BaseCollector : BackgroundService
    {
        protected BaseCollector(
            ILoggerFactory loggerFactory,
            IZidiumComponentsProvider zidiumComponentsProvider, 
            bool enabled
            )
        {
            _loggerFactory = loggerFactory;
            _zidiumComponentsProvider = zidiumComponentsProvider;
            _enabled = enabled;
        }

        private readonly ILoggerFactory _loggerFactory;
        private readonly IZidiumComponentsProvider _zidiumComponentsProvider;

        protected abstract string Name { get; }

        protected abstract TimeSpan Interval { get; }

        protected ILogger Logger;

        private bool _enabled;

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Logger = _loggerFactory.CreateLogger(Name);
            _zidiumComponentsProvider.ConnectLoggerToZidium(null, Name);

            if (!_enabled)
            {
                Logger.LogInformation($"{Name} disabled");
                return;
            }

            Logger.LogInformation($"Interval: {Interval}");

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await DoWork(cancellationToken);
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, exception.Message);
                }

                await Task.Delay(Interval, cancellationToken);
            }
        }

        protected abstract Task DoWork(CancellationToken cancellationToken);
    }
}
