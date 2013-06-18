using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;

namespace DevBridgeService
{
    static class Program
    {
        static string applicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName.Replace(".vshost", "");
        static string applicationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        static string applicationTitle = "DevBridge Service";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (Environment.CommandLine.Contains("-service"))
            {
                if (ServiceCheck(true) == false)
                {
                    ServiceController controller = new ServiceController(applicationName);
                    controller.Start();
                    return;
                }

                ServiceBase[] services = new ServiceBase[] { new DevService() };
                ServiceBase.Run(services);
            }
            else if (Environment.CommandLine.Contains("-noservice"))
            {
                if (ServiceCheck(false))
                {
                    ServiceController controller = new ServiceController(applicationName);
                    if (controller.Status == ServiceControllerStatus.Running) controller.Stop();
                    ServiceInstaller.UnInstallService(applicationName);
                }
            }
            else
            {
                DeviceManager application = new DeviceManager(applicationPath, true);
            }
        }

        static bool ServiceCheck(bool autoInstall)
        {
            bool installed = false;

            Console.WriteLine("AppService: " + applicationName);

            ServiceController[] controllers = ServiceController.GetServices();
            foreach (ServiceController con in controllers)
            {
                if (con.ServiceName == applicationName)
                {
                    installed = true;
                    break;
                }
            }

            if (installed) return true;

            if (autoInstall)
            {
                Console.WriteLine("AutoInstalling");
                ServiceInstaller.InstallService("\"" + applicationPath + "\\" + applicationName + ".exe\" -service", applicationName, applicationTitle, true, false);
                Console.WriteLine("AutoInstalling done");
            }

            return false;
        }

    }
}
