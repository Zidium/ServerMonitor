using System.ServiceProcess;

namespace ZidiumServerMonitor
{
    public class Service : ServiceBase
    {
        public void Start()
        {
            _application = new Application();
            _application.Start();
        }

        protected override void OnStart(string[] args)
        {
            Start();
        }

        public new void Stop()
        {
            _application.Stop();
        }

        protected override void OnStop()
        {
            Stop();
        }

        private Application _application;
    }
}
