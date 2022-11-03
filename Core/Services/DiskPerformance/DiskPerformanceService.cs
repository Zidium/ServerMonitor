using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ZidiumServerMonitor
{
    public class DiskPerformanceService
    {
        public DiskPerformanceService()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                _internalService = new DiskPerformanceServiceWindows();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                _internalService = new DiskPerformanceServiceLinux();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                _internalService = new DiskPerformanceServiceMac();
        }

        private readonly IDiskPerformanceOsSpecific _internalService;

        public List<DiskPerformance> GetForAllDisks()
        {
            if (_internalService == null)
                throw new Exception("Unsupported OS");

            return _internalService.GetForAllDisks();
        }

    }
}
