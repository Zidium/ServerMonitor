using System;

namespace ZidiumServerMonitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var application = new Application();
            application.Start();
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
            application.Stop();
        }
    }
}
