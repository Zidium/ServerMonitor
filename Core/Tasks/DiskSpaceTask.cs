﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ZidiumServerMonitor
{
    public class DiskSpaceTask : BaseTask
    {
        public DiskSpaceTask()
        {
            _settings = DependencyInjection.Services.GetRequiredService<Settings>().Disk;
        }

        private Settings.FreeDiskSpaceTaskSettings _settings;

        public override TimeSpan Interval { get { return _settings.Interval; } }

        public override TimeSpan Actual { get { return _settings.Timeout; } }

        public override string Name { get { return "DiskSpaceTask"; } }

        public override void DoWork()
        {
            foreach (var disk in _settings.Disks)
            {
                var freeSpace = FreeSpaceHelper.GetDriveFreeSpace(disk);

                if (freeSpace.HasValue)
                {
                    var freeSpaceGb = (double)freeSpace.Value / 1024 / 1024 / 1024;
                    var freeSpaceGbRounded = Math.Round(freeSpaceGb, 2);
                    ZidiumHelper.ServerComponent.SendMetric("Free space on disk " + disk + ", Gb", freeSpaceGbRounded, Actual);
                    Logger.LogInformation($"Free space on disk {disk}: {freeSpaceGbRounded} Gb");
                }
            }
        }

        protected override void DoStart()
        {
            Logger.LogDebug($"Disks: {string.Join(", ", _settings.Disks)}");
        }
    }
}
