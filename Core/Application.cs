using System;
using Hardware.Info;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Zidium;
using Zidium.Api;
using Zidium.Api.XmlConfig;

namespace ZidiumServerMonitor
{
    public static class Application
    {
        public static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            var zidiumConfig = hostContext.Configuration.GetSection("Zidium").Get<Config>();
            Client.Instance = new Client(zidiumConfig);
            services.AddSingleton(Client.Instance);
            services.AddSingleton<ZidiumComponentsProvider>();

            services.Configure<DiskSpaceTaskOptions>(hostContext.Configuration.GetSection("Tasks:DiskSpaceTask"));
            services.Configure<MemoryTaskOptions>(hostContext.Configuration.GetSection("Tasks:MemoryTask"));
            services.Configure<CpuTaskOptions>(hostContext.Configuration.GetSection("Tasks:CpuTask"));
            services.Configure<DiskPerformanceTaskOptions>(hostContext.Configuration.GetSection("Tasks:DiskPerformanceTask"));
            services.Configure<ServerOptions>(hostContext.Configuration.GetSection("Server"));
            services.AddHostedService<DiskSpaceTask>();
            services.AddHostedService<MemoryInfoCollector>();
            services.AddHostedService<MemoryTask>();
            services.AddHostedService<CpuInfoCollector>();
            services.AddHostedService<CpuTask>();
            services.AddHostedService<DiskPerformanceCollector>();
            services.AddHostedService<DiskPerformanceTask>();
            services.AddSingleton<FreeSpaceService>();
            services.AddSingleton<HardwareInfo>();
            services.AddSingleton<MemoryInfoDataboxService>();
            services.AddSingleton<CpuInfoDataboxService>();
            services.AddSingleton<DiskPerformanceService>();
            services.AddSingleton<DiskPerformanceDataboxService>();
        }

        public static void ConfigureLogging(HostBuilderContext hostContext, ILoggingBuilder builder)
        {
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            builder.AddNLog();
            builder.AddZidiumLog(null, "Program");
            builder.AddZidiumErrors(null, "Program");
            LogManager.Configuration = new NLogLoggingConfiguration(hostContext.Configuration.GetSection("NLog"));
        }

        public static void OnStart(Microsoft.Extensions.Logging.ILogger logger, IServiceProvider services)
        {
            logger.LogDebug("Starting...");
            logger.LogDebug($"Environment: {services.GetRequiredService<IHostEnvironment>().EnvironmentName}");
            logger.LogInformation("Checking connection with Zidium...");
            Client.Instance.ApiService.GetEcho(Guid.NewGuid().ToString()).Check();
            logger.LogInformation("Connection OK");
            logger.LogDebug("Started");
        }

        public static void OnShutdown(Microsoft.Extensions.Logging.ILogger logger, IServiceProvider services)
        {
            logger.LogDebug("Finished");
            Client.Instance.Flush();
            LogManager.Shutdown();
        }
    }
}
