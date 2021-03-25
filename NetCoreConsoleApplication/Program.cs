using System;
using System.Linq;
using System.Threading;

namespace ZidiumServerMonitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var application = new Application();
            application.Start();

            bool runAsService = args.Any(x => string.Compare("--service", x, StringComparison.OrdinalIgnoreCase) == 0);

            if (runAsService)
            {
                Thread.Sleep(Timeout.Infinite);
            }
            else
            {
                Console.WriteLine("Press any key to stop...");
                Console.ReadKey();
                application.Stop();
            }
        }
    }
}
