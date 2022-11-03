using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace ZidiumServerMonitor
{
    public class DiskPerformanceDataboxServiceFactory
    {
        public DiskPerformanceDataboxServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private readonly IServiceProvider _serviceProvider;

        private readonly ConcurrentDictionary<string, DiskPerformanceDataboxService> _disks = new();

        public DiskPerformanceDataboxService GetDataboxService(string diskName)
        {
            return _disks.GetOrAdd(diskName, _ =>
            {
                return _serviceProvider.GetRequiredService<DiskPerformanceDataboxService>();
            });
        }
    }
}
