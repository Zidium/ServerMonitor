using System;
using System.Threading;
using System.Threading.Tasks;
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
            TaskComponent.Log.Debug($"Starting task, interval: {Interval}");

            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken = _cancellationTokenSource.Token;

            DoStart();

            // Запускаем задачу сразу, но через таймер, чтобы был отдельный поток
            _timer = new Timer(OnTimerCallback, null, 0, Timeout.Infinite);
        }

        protected virtual void DoStart() { }

        public void Stop()
        {
            TaskComponent.Log.Debug("Stopping task");
            _cancellationTokenSource.Cancel();
            _timer.Dispose();
            _timer = null;
            WaitForStop().Wait();
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
                TaskComponent.Log.Error(exception);
            }

            // Отправим сигнал о выполнении задачи, с запасом времени в один интервал
            TaskUnittest.SendResult(UnitTestResult.Success, Interval + Interval);

            // Независимо от наличия ошибки, назначаем следующее выполнение
            _timer.Dispose();
            _timer = new Timer(OnTimerCallback, null, Interval, TimeSpan.FromMilliseconds(-1));

            _inProgress = false;
        }

        protected IComponentControl TaskComponent
        {
            get
            {
                if (_taskComponent == null)
                {
                    _taskComponent = ZidiumHelper.MonitorComponent.GetOrCreateChildComponentControl("Task", Name);
                }

                return _taskComponent;
            }
        }

        private IComponentControl _taskComponent;

        protected IUnitTestControl TaskUnittest
        {
            get
            {
                if (_taskUnittest == null)
                {
                    _taskUnittest = TaskComponent.GetOrCreateUnitTestControl("Main");
                }

                return _taskUnittest;
            }
        }

        private IUnitTestControl _taskUnittest;
    }
}
