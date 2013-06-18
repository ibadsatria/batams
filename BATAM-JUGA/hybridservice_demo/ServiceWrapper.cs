using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;

namespace HybridService
{
    public partial class ServiceWrapper : ServiceBase
    {
        ConsoleApplication application;

        public ServiceWrapper()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string applicationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            application = new ConsoleApplication(applicationPath, false);
        }

        protected override void OnStop()
        {
            application.Close();
        }

    }
    
}
