using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Zidium;

namespace ZidiumServerMonitor
{
    internal abstract class BaseCollector : BackgroundService
    {
        protected BaseCollector(
            ILoggerFactory loggerFactory,
            bool enabled
            )
        {
            _loggerFactory = loggerFactory;
            _enabled = enabled;
        }

        private readonly ILoggerFactory _loggerFactory;

        protected abstract string Name { get; }

        protected abstract TimeSpan Interval { get; }

        protected ILogger Logger;

        private bool _enabled;

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Logger = _loggerFactory.CreateLogger(Name);
            _loggerFactory.AddZidiumErrors(null, Name);
            _loggerFactory.AddZidiumLog(null, Name);

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
                await DoWork(cancellationToken);
                await Task.Delay(Interval, cancellationToken);
            }
        }

        protected abstract Task DoWork(CancellationToken cancellationToken);
    }
}
