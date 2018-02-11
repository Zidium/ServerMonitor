using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zidium;
using Zidium.Api;

namespace ZidiumServerMonitor
{
    /// <summary>
    /// Базовый класс периодической задачи
    /// </summary>
    public abstract class BaseTask
    {
        /// <summary>
        /// Период выполнения
        /// </summary>
        public abstract TimeSpan Interval { get; }

        /// <summary>
        /// Таймаут выполнения
        /// </summary>
        public abstract TimeSpan Actual { get; }

        /// <summary>
        /// Название задачи
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Основное действие
        /// </summary>
        public abstract void DoWork();

        private CancellationTokenSource _cancellationTokenSource;

        protected CancellationToken CancellationToken;

        public void Start()
        {
            TaskComponent = ZidiumHelper.MonitorComponent.GetOrCreateChildComponentControl("Task", Name);
            TaskUnittest = TaskComponent.GetOrCreateUnitTestControl("Main");

            var loggerFactory = DependencyInjection.Services.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddZidiumErrors(TaskComponent.Info.Id, Name);
            loggerFactory.AddZidiumLog(TaskComponent.Info.Id, Name);
            Logger = loggerFactory.CreateLogger(Name);

            Logger.LogDebug($"Starting task, interval: {Interval}, timeout: {Actual}");

            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken = _cancellationTokenSource.Token;

            DoStart();

            // Запускаем задачу сразу, но через таймер, чтобы был отдельный поток
            _timer = new Timer(OnTimerCallback, null, 0, Timeout.Infinite);
        }

        protected virtual void DoStart() { }

        public void Stop()
        {
            Logger.LogDebug("Stopping task");
            _cancellationTokenSource.Cancel();
            _timer.Dispose();
            _timer = null;
            WaitForStop().Wait();
            Logger.LogDebug("Task stopped");
        }

        private async Task WaitForStop()
        {
            var timeoutDateTime = DateTime.Now + _waitForStopTimeout;
            while (DateTime.Now < timeoutDateTime && _inProgress)
            {
                await Task.Delay(1000);
            }
        }

        private static TimeSpan _waitForStopTimeout = TimeSpan.FromSeconds(10);

        private Timer _timer;

        private bool _inProgress;

        private void OnTimerCallback(object state)
        {
            _inProgress = true;

            try
            {
                CancellationToken.ThrowIfCancellationRequested();
                DoWork();

                // Отправим сигнал о выполнении задачи, с запасом времени в таймаут
                TaskUnittest.SendResult(UnitTestResult.Success, Actual);
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (OperationCanceledException)
            {
                // Задача остановлена, выходим
                _inProgress = false;
                return;
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, exception.Message);
            }

            // Независимо от наличия ошибки, назначаем следующее выполнение
            _timer.Dispose();
            _timer = new Timer(OnTimerCallback, null, Interval, TimeSpan.FromMilliseconds(-1));

            _inProgress = false;
        }

        protected IComponentControl TaskComponent { get; private set; }

        protected IUnitTestControl TaskUnittest { get; private set; }

        protected ILogger Logger { get; private set; }
    }
}
