using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ZidiumServerMonitor
{
    internal class FreeSpaceService
    {
        public long? GetDriveFreeSpace(string drive)
        {
            DriveInfo di;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                di = new DriveInfo(drive);
            }
            else
            {
                var drives = DriveInfo.GetDrives();
                Console.WriteLine(string.Join(", ", drives.Select(t => t.Name)));
                di = drives.FirstOrDefault(t => t.Name.StartsWith(drive, StringComparison.OrdinalIgnoreCase));
            }

            if (di == null)
                return null;

            return di.AvailableFreeSpace;
        }
    }
}
