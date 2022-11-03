using System;

namespace ZidiumServerMonitor
{
    public class MemoryInfoDataboxService : BaseDataboxService<MemoryInfoDatabox>
    {
        protected override MemoryInfoDatabox Copy(MemoryInfoDatabox value)
        {
            return new MemoryInfoDatabox()
            {
                MinAvailablePhysical = value.MinAvailablePhysical
            };
        }

        public void Set(ulong availablePhysical)
        {
            Update(data =>
            {
                data.MinAvailablePhysical = data.MinAvailablePhysical.HasValue
                    ? Math.Min(data.MinAvailablePhysical.Value, availablePhysical)
                    : availablePhysical;
            });
        }
    }
}
