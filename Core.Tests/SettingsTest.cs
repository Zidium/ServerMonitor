using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ZidiumServerMonitor.Tests
{
    public class SettingsTest
    {
        [Fact]
        public void NoSettingsFileTest()
        {
            var services = new ServiceCollection();
            services.AddSettings("NonExistsFile.json");
            DependencyInjection.Services = services.BuildServiceProvider();

            var settings = DependencyInjection.Services.GetRequiredService<Settings>();
            Assert.NotNull(settings);

            Assert.NotNull(settings.Memory);
            Assert.Equal(TimeSpan.FromMinutes(10), settings.Memory.Interval);
            Assert.Equal(TimeSpan.FromMinutes(20), settings.Memory.Timeout);

            Assert.NotNull(settings.Disk);
            Assert.Equal(TimeSpan.FromMinutes(10), settings.Disk.Interval);
            Assert.Equal(TimeSpan.FromMinutes(20), settings.Disk.Timeout);
            Assert.NotNull(settings.Disk.Disks);
            Assert.Empty(settings.Disk.Disks);
        }

        [Fact]
        public void NormalFileTest()
        {
            var services = new ServiceCollection();
            services.AddSettings(@"TestFiles\settings.json");
            DependencyInjection.Services = services.BuildServiceProvider();

            var settings = DependencyInjection.Services.GetRequiredService<Settings>();
            Assert.NotNull(settings);

            Assert.Equal(new Guid("1CA421FE-59CE-4DF2-8F83-58705965DED0"), settings.ServerId);

            Assert.NotNull(settings.Memory);
            Assert.Equal(TimeSpan.FromMinutes(5), settings.Memory.Interval);
            Assert.Equal(TimeSpan.FromMinutes(6), settings.Memory.Timeout);

            Assert.NotNull(settings.Disk);
            Assert.Equal(TimeSpan.FromMinutes(1), settings.Disk.Interval);
            Assert.Equal(TimeSpan.FromMinutes(2), settings.Disk.Timeout);
            Assert.NotNull(settings.Disk.Disks);
            Assert.Equal(2, settings.Disk.Disks.Length);
            Assert.Equal("C", settings.Disk.Disks[0]);
            Assert.Equal("D", settings.Disk.Disks[1]);
        }
    }
}
