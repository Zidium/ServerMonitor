using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Zidium.Api;

namespace ZidiumServerMonitor
{
    public static class ZidiumHelper
    {
        public static IComponentControl ServerComponent
        {
            get
            {
                lock (typeof(ZidiumHelper))
                {
                    if (_serverComponent == null)
                    {
                        var settings = DependencyInjection.Services.GetRequiredService<Settings>();
                        var client = Client.Instance;
                        _serverComponent = client.GetComponentControl(settings.ServerId);
                    }

                    return _serverComponent;
                }
            }
        }

        private static IComponentControl _serverComponent;

        public static IComponentControl MonitorComponent
        {
            get
            {
                lock (typeof(ZidiumHelper))
                {
                    if (_monitorComponent == null)
                    {
                        _monitorComponent = Client.Instance.GetDefaultComponentControl();
                        _monitorComponent.Update(new UpdateComponentData()
                        {
                            Version = typeof(ZidiumHelper).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion
                        });
                    }

                    return _monitorComponent;
                }
            }
        }

        private static IComponentControl _monitorComponent;
    }
}
