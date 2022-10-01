using System.Reflection;
using Microsoft.Extensions.Options;
using Zidium.Api;

namespace ZidiumServerMonitor
{
    internal class ZidiumComponentsProvider
    {
        public ZidiumComponentsProvider(IClient client, IOptions<ServerOptions> serverOptions)
        {
            _client = client;
            _serverOptions = serverOptions.Value;
        }

        private readonly IClient _client;

        private readonly ServerOptions _serverOptions;

        public IComponentControl GetMonitorComponent()
        {
            lock (_monitorComponentLockObject)
            {
                if (_monitorComponent == null)
                {
                    if (_client.Config.DefaultComponent.Id.HasValue)
                    {
                        _monitorComponent = _client.GetDefaultComponentControl();
                        _monitorComponent.Update(new UpdateComponentData()
                        {
                            Version = typeof(ZidiumComponentsProvider).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion
                        });
                    }
                    else
                    {
                        _monitorComponent = new FakeComponentControl();
                    }
                }

                return _monitorComponent;
            }
        }

        private IComponentControl _monitorComponent;

        private readonly object _monitorComponentLockObject = new object();

        public IComponentControl GetServerComponent()
        {
            lock (_serverComponentLockObject)
            {
                if (_serverComponent == null)
                {
                    _serverComponent = _client.GetComponentControl(_serverOptions.ComponentId);
                }

                return _serverComponent;
            }
        }

        private IComponentControl _serverComponent;

        private readonly object _serverComponentLockObject = new object();
    }
}
