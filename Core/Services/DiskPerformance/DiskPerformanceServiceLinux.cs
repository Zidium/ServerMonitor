using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace ZidiumServerMonitor
{
    internal class DiskPerformanceServiceLinux : IDiskPerformanceOsSpecific
    {
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        public List<DiskPerformance> GetForAllDisks()
        {
            var iostatResult = ReadProcessOutput("iostat", "-x -y -o JSON 1 1");
            var iostatJson = JsonSerializer.Deserialize<IostatJson>(iostatResult, _jsonOptions);

            var result = new List<DiskPerformance>();

            if (iostatJson.Sysstat.Hosts.Length == 0 || iostatJson.Sysstat.Hosts[0].Statistics.Length == 0)
                return result;

            foreach (var diskStats in iostatJson.Sysstat.Hosts[0].Statistics[0].Disk)
            {
                var item = new DiskPerformance()
                {
                    Name = diskStats.DiskDevice,
                    AvgDiskQueueLength = diskStats.AquSz ?? diskStats.AvgquSz ?? 0,
                    PercentDiskTime = diskStats.Util
                };

                result.Add(item);
            }

            return result;
        }

        private string ReadProcessOutput(string cmd, string args)
        {
            var processStartInfo = new ProcessStartInfo(cmd, args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            using var process = Process.Start(processStartInfo);
            using StreamReader streamReader = process.StandardOutput;
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception($"Command \"{cmd} {args}\" returned a non-zero exit code ({process.ExitCode}).");

            return streamReader.ReadToEnd().Trim();
        }
    }
}
