using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Zidium;

namespace ZidiumServerMonitor
{
    public class Application
    {
        public void Start()
        {
            var services = new ServiceCollection();
            services.AddSettings(@"settings.json");
            services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace));
            DependencyInjection.Services = services.BuildServiceProvider();

            var loggerFactory = DependencyInjection.Services.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddConsole(LogLevel.Trace);
            loggerFactory.AddZidiumErrors(null, "Application");
            loggerFactory.AddZidiumLog(null, "Application");

            var settings = DependencyInjection.Services.GetRequiredService<Settings>();

            var logger = loggerFactory.CreateLogger("Application");
            logger.LogDebug($"Server component Id : {settings.ServerId}");

            try
            {
                _tasks = new BaseTask[]
                {
                    new FreeMemoryTask(),
                    new DiskSpaceTask()
                };

                foreach (var task in _tasks)
                {
                    task.Start();
                }
            }
            catch (Exception exception)
            {
                logger.LogCritical(exception, exception.Message);
            }
        }

        private BaseTask[] _tasks;

        public void Stop()
        {
            foreach (var task in _tasks)
            {
                task.Stop();
            }
        }
    }
}
