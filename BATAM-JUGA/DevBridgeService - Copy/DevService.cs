using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace DevBridgeService
{
    public partial class DevService : ServiceBase
    {
        static void Main(string[] args)
        {
            DevService service = new DevService();

            if (Environment.UserInteractive)
            {
                service.OnStart(args);
                Console.WriteLine("Press any key to stop program");
                Console.Read();
                service.OnStop();
            }
            else
            {
                ServiceBase.Run(service);
            }

        }
        public DevService()
        {
            InitializeComponent();
        }

        internal override void OnStart(string[] args)
        {
        }

        internal override void OnStop()
        {
        }
    }
}
