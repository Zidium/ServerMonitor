using Microsoft.Extensions.DependencyInjection;

namespace ZidiumServerMonitor
{
    public class Worker
    {
        public void Start()
        {
            var services = new ServiceCollection();
            services.AddSettings(@"settings.json");
            DependencyInjection.Services = services.BuildServiceProvider();

            var settings = DependencyInjection.Services.GetRequiredService<Settings>();
            ZidiumHelper.MonitorComponent.Log.Debug($"Server component Id : {settings.ServerId}");

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
