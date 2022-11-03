using System.Text.Json.Serialization;

namespace ZidiumServerMonitor
{
    internal class IostatJson
    {
        [JsonPropertyName("sysstat")]
        public SysStat Sysstat { get; set; }

        internal class SysStat
        {
            [JsonPropertyName("hosts")]
            public Host[] Hosts { get; set; }
        }

        internal class Host
        {
            [JsonPropertyName("statistics")]
            public Statistics[] Statistics { get; set; }
        }

        internal class Statistics
        {
            [JsonPropertyName("disk")]
            public Disk[] Disk { get; set; }
        }

        internal class Disk
        {
            [JsonPropertyName("disk_device")]
            public string DiskDevice { get; set; }

            [JsonPropertyName("aqu-sz")]
            public double? AquSz { get; set; }

            [JsonPropertyName("avgqu-sz")]
            public double? AvgquSz { get; set; }

            [JsonPropertyName("util")]
            public double Util { get; set; }
        }
    }
}
