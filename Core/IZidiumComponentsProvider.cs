using System;
using Zidium.Api;

namespace ZidiumServerMonitor
{
    public interface IZidiumComponentsProvider
    {
        IComponentControl GetMonitorComponent();

        IComponentControl GetServerComponent();

        void ConnectLoggerToZidium(Guid? componentId, string loggerName);
    }
}