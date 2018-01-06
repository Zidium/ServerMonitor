using System;
using Microsoft.Extensions.DependencyInjection;

namespace ZidiumServerMonitor
{
    /// <summary>
    /// Глобальный контейнер для DI
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceProvider Services { get; set; }

        public static T CreateInstance<T>()
        {
            return (T) ActivatorUtilities.CreateInstance(Services, typeof(T));
        }
    }
}
