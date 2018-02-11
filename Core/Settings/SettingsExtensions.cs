using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ZidiumServerMonitor
{
    public static class SettingsExtensions
    {
        public static IServiceCollection AddSettings(this IServiceCollection services, string filename)
        {
            var builder = new ConfigurationBuilder().AddJsonFile(filename, true);
            var config = builder.Build();
            var settings = config.Get<Settings>() ?? new Settings();
            services.AddSingleton(settings);
            return services;
        }
    }
}
