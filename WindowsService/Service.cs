using System.ServiceProcess;

namespace ZidiumServerMonitor
{
    public class Service : ServiceBase
    {
        public void Start()
        {
            _worker = new Worker();
            _worker.Start();
        }

        protected override void OnStart(string[] args)
        {
            Start();
        }

        public new void Stop()
        {
            _worker.Stop();
        }

        protected override void OnStop()
        {
            Stop();
        }

        private Worker _worker;
    }
}
