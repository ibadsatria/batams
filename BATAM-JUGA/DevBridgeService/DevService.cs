using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Reflection;

namespace DevBridgeService
{
    public partial class DevService : ServiceBase
    {
        DeviceManager application;

        // http://www.thedavejay.com/2012/04/self-installing-c-windows-service-safe.html

        public DevService()
        {
            //InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string applicationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            application = new DeviceManager(applicationPath, false);
        }

        protected override void OnStop()
        {
            application.Close();
        }
    }
}
