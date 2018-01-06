using Xunit;

namespace ZidiumServerMonitor.Tests
{
    public class FreeSpaceHelperTests
    {
        [Fact]
        public void GetExistingDriveFreeSpaceTest()
        {
            var space = FreeSpaceHelper.GetDriveFreeSpace("C");
            Assert.NotNull(space);
            Assert.NotEqual(0, space.Value);
        }

        [Fact]
        public void GetNonExistingDriveFreeSpaceTest()
        {
            var space = FreeSpaceHelper.GetDriveFreeSpace("-");
            Assert.Null(space);
        }
    }
}
