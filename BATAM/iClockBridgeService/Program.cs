using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Reflection;
using System.IO;

namespace iClockBridgeService
{
    public class Program : ServiceBase
    {
        public static string InstallServiceName = "Attendance Bridge Service";
        static string applicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName.Replace(".vshost", "");
        static string applicationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        DeviceManager devManager;
        int TotalDevice = 60;   // sementara fix

        static void Main(string[] args)
        {
            bool debugMode = false;
            if (args.Length > 0)
            {
                for (int ii = 0; ii < args.Length; ii++)
                {
                    switch (args[ii].ToUpper())
                    {
                        case "/NAME":
                            if (args.Length > ii + 1)
                            {
                                InstallServiceName = args[++ii];
                            }
                            break;
                        case "/I":
                            InstallService();
                            return;
                        case "/U":
                            UninstallService();
                            return;
                        case "/D":
                            debugMode = true;
                            break;
                        default:
                            break;
                    }
                }
            }

            if (debugMode)
            {
                Program service = new Program();
                service.OnStart(null);
                Console.WriteLine("Service Started...");
                Console.WriteLine("<press any key to exit...>");
                Console.Read();
                Console.Write("Please wait while stoping service... ");
                service.OnStop();
                Console.WriteLine();
                Console.WriteLine("Done");
                Console.WriteLine("Service Closed...");
                Environment.Exit(0);
            }
            else
            {
                System.ServiceProcess.ServiceBase.Run(new Program());
            }
        }

        public Program()
        {
            //set initializers here
            devManager = new DeviceManager(applicationPath, TotalDevice);
        }

        /// <summary>
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        protected override void OnStart(string[] args)
        {
            //start any threads or http listeners etc
            devManager.Start();
        }

        /// <SUMMARY>
        /// Stop this service.
        /// </SUMMARY>
        protected override void OnStop()
        {
            //stop any threads here and wait for them to be stopped.
            devManager.Stop();
        }

        protected override void Dispose(bool disposing)
        {
            //clean your resources if you have to
            devManager.Dispose();
            base.Dispose(disposing);
        }

        private static bool IsServiceInstalled()
        {
            return ServiceController.GetServices().Any(s => s.ServiceName == InstallServiceName);
        }

        private static void InstallService()
        {
            if (IsServiceInstalled())
            {
                UninstallService();
            }

            ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
        }

        private static void UninstallService()
        {
            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
        } 


    }
}

