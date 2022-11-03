using System;
using System.IO;

namespace ZidiumServerMonitor
{
    public class FreeSpaceService
    {
        public long GetDriveFreeSpace(string drive)
        {
            DriveInfo di;

            /*
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
            */
            di = new DriveInfo(drive);

            if (di == null)
                throw new Exception($"Disk {drive} not found");

            return di.AvailableFreeSpace;
        }
    }
}
