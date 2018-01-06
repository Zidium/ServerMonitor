using System;

namespace ZidiumServerMonitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var worker = new Worker();
            worker.Start();
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
            worker.Stop();
        }
    }
}
