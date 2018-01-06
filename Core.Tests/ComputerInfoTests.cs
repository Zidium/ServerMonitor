using NickStrupat;
using Xunit;

namespace ZidiumServerMonitor.Tests
{
    public class ComputerInfoTests
    {
        [Fact]
        public void GetAvailableMemorySizeTest()
        {
            var memorySize = (long) new ComputerInfo().AvailablePhysicalMemory;
            Assert.NotEqual(0, memorySize);
        }
    }
}
