using System;
using System.IO;
using System.Linq;

namespace ZidiumServerMonitor
{
    public static class FreeSpaceHelper
    {
        public static long? GetDriveFreeSpace(string drive)
        {
            DriveInfo di;

            try
            {
                di = new DriveInfo(drive);
            }
            catch
            {
                var drives = DriveInfo.GetDrives();
                di = drives.FirstOrDefault(t => t.Name.StartsWith(drive, StringComparison.OrdinalIgnoreCase));

                if (di == null)
                    return null;
            }

            return di.AvailableFreeSpace;
        }
    }
}
