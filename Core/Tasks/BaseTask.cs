using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Zidium;
using Zidium.Api;
using Zidium.Api.Dto;

namespace ZidiumServerMonitor
{
    public abstract class BaseTask : BackgroundService
    {
        protected BaseTask(
            ILoggerFactory loggerFactory,
            IZidiumComponentsProvider zidiumComponentsProvider,
            BaseTaskOptions options)
        {
            _loggerFactory = loggerFactory;
            ZidiumComponentsProvider = zidiumComponentsProvider;
            _options = options;
        }

        private readonly ILoggerFactory _loggerFactory;

        protected readonly IZidiumComponentsProvider ZidiumComponentsProvider;

        private readonly BaseTaskOptions _options;

        protected IComponentControl TaskComponent { get; private set; }

        protected IUnitTestControl TaskUnittest { get; private set; }

        protected ILogger Logger;

        private CronExpression _cronExpression;

        /// <summary>
        /// Название задачи
        /// </summary>
        public abstract string Name { get; }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Logger = _loggerFactory.CreateLogger(Name);

            if (!_options.Enabled)
            {
                Logger.LogInformation("Task disabled");
                return;
            }

            TaskComponent = ZidiumComponentsProvider.GetMonitorComponent().GetOrCreateChildComponentControl("Task", Name);
            TaskUnittest = TaskComponent.GetOrCreateUnitTestControl("Main");
            ZidiumComponentsProvider.ConnectLoggerToZidium(TaskComponent.Info?.Id, Name);

            Logger.LogInformation($"Schedule: {_options.Schedule}");
            _cronExpression = CronExpression.Parse(_options.Schedule, CronFormat.IncludeSeconds);
            Logger.LogInformation($"Actual interval: {_options.ActualInterval:c}");
            Logger.LogTrace($"{Name} started");
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (!_options.Enabled)
                return;

            Logger.LogTrace($"{Name} stopped");
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var preDelay = GetNextDelay();
            Logger.LogTrace($"Waiting for {preDelay.OccurenceTime.ToString("o")}");

            await Task.Delay(preDelay.Delay, cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                Logger.LogTrace("Running task...");
                try
                {
                    await DoWork(cancellationToken);
                    Logger.LogTrace("Task completed");
                    TaskUnittest.SendResult(UnitTestResult.Success, _options.ActualInterval);
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, exception.Message);
                    TaskUnittest.SendResult(UnitTestResult.Alarm, _options.ActualInterval, exception.Message);
                }

                var delay = GetNextDelay();
                Logger.LogTrace($"Waiting for {delay.OccurenceTime.ToString("o")}");

                await Task.Delay(delay.Delay, cancellationToken);
            }
        }

        protected DelayInfo GetNextDelay()
        {
            var currentUtcTime = DateTimeOffset.Now;
            var occurenceTime = _cronExpression.GetNextOccurrence(currentUtcTime, TimeZoneInfo.Local).GetValueOrDefault();
            var delay = occurenceTime - currentUtcTime;

            return new DelayInfo()
            {
                OccurenceTime = occurenceTime,
                Delay = delay
            };
        }

        protected abstract Task DoWork(CancellationToken cancellationToken);

    }
}
