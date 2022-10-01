using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ZidiumServerMonitor
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Program");
            Application.OnStart(logger, host.Services);
            host.Run();
            Application.OnShutdown(logger, host.Services);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    Application.ConfigureServices(hostContext, services);
                })
                .ConfigureLogging((hostContext, builder) =>
                {
                    Application.ConfigureLogging(hostContext, builder);
                });
    }
}
