using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;

namespace ZidiumServerMonitor
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var currentDir = Path.GetDirectoryName(typeof(Program).GetTypeInfo().Assembly.Location);
            if (currentDir != null)
                Directory.SetCurrentDirectory(currentDir);

            if (args != null && args.Length == 1 && args[0].Length > 1 && (args[0][0] == '-' || args[0][0] == '/'))
            {
                switch (args[0].Substring(1).ToLower())
                {
                    case "install":
                    case "i":
                        Install(true);

                        break;
                    case "uninstall":
                    case "u":
                        Install(false);

                        break;
                    default:
                        Console.WriteLine("Wrong command line parameters");
                        break;
                }
            }
            else
            {
                try
                {
                    var service = new Service();
                    if (Environment.UserInteractive)
                    {
                        Console.CancelKeyPress += (x, y) => service.Stop();
                        service.Start();
                        Console.WriteLine("Press any key to stop...");
                        Console.ReadKey();
                        service.Stop();
                    }
                    else
                    {
                        var servicesToRun = new ServiceBase[] { service };
                        ServiceBase.Run(servicesToRun);
                    }
                }
                catch (IOException exception)
                {
                    Console.WriteLine(exception.ToString());
                    Console.Read();
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Unable to run service in console mode" + Environment.NewLine + exception);
                    Console.Read();
                }
            }
        }

        private static void Install(bool isInstall)
        {
            try
            {
                var appAssembly = Assembly.GetEntryAssembly();

                using (var installer = new ProjectInstaller("Zidium Server Monitor", "Service for monitor this server state to Zidium", appAssembly))
                {
                    if (isInstall)
                    {
                        installer.Install();
                    }
                    else
                    {
                        installer.Uninstall();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }
    }
}
