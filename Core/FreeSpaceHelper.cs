using System;
using System.IO;
using System.Linq;

namespace ZidiumServerMonitor
{
    public static class FreeSpaceHelper
    {
        public static long? GetDriveFreeSpace(string drive)
        {
            var drives = DriveInfo.GetDrives();
            var driveInfo = drives.FirstOrDefault(t => t.Name.StartsWith(drive, StringComparison.OrdinalIgnoreCase));

            if (driveInfo == null)
                return null;

            return driveInfo.AvailableFreeSpace;
        }
    }
}
