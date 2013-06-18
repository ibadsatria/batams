using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PushUsersTask
{
    public class DevPull : IDisposable
    {
        DeviceHandler DevHandlerPull;
        Thread oneThread;
        bool threadExited = true;
        string devIP = "";
        int devPort = 0;
        string ApplicationPath = "";

        public bool isFinished
        {
            get { return threadExited; }
        }

        public void Work(string devIP, int devPort, string appPath)
        {
            int logCount = 0;
            ApplicationPath = appPath;
            DevHandlerPull = new DeviceHandler();
            DevHandlerPull.init(devIP, devPort, appPath);
            Console.WriteLine("Connectings to IP: " + devIP);
            if (!DevHandlerPull.ConnectDevice())
            {
                Console.WriteLine("Can't connect to device " + devIP);
                DevHandlerPull.Dispose();
                return;
            }
            Console.WriteLine("Connected to IP: " + devIP);

            //            List<DeviceHandler.AttendanceLog> AttLogList = new List<DeviceHandler.AttendanceLog>();

            Console.Write("Get records count from: " + devIP);
            if (!DevHandlerPull.GetRecordsCount(ref logCount))
            {
                Console.WriteLine(" failed");
                DevHandlerPull.EnableDevice();
                DevHandlerPull.DisconnectDevice();
                DevHandlerPull.Dispose();
                return;
            }
            Console.WriteLine(" success");

            DeviceHandler.AttendanceLog[] AttLogList = new DeviceHandler.AttendanceLog[logCount];

            Console.Write("Get Attendance records from: " + devIP);
            if (!DevHandlerPull.GetDeviceRecords(ref AttLogList))
            {
                Console.WriteLine(" failed");
                DevHandlerPull.EnableDevice();
                DevHandlerPull.DisconnectDevice();
                DevHandlerPull.Dispose();
                return;
            }
            Console.WriteLine(" success");

            Console.WriteLine("Pushing records from " + devIP + " to database");
            // simpan ke database
            if (!BatamDBUtils.SaveAttendanceLog(BatamDBUtils.GetDb(), AttLogList))
            {
                Console.WriteLine(" failed");
                DevHandlerPull.EnableDevice();
                DevHandlerPull.DisconnectDevice();
                DevHandlerPull.Dispose();
                return;
            }
            Console.WriteLine(" success");

            // jika sukses kirim ke database, clear log
            DevHandlerPull.ClearDeviceRecords();

            Console.WriteLine("Enabling " + devIP);
            DevHandlerPull.EnableDevice();

            Console.WriteLine("Disconnecting device " + devIP);
            DevHandlerPull.DisconnectDevice();
            DevHandlerPull.Dispose();
        }

        void Work2()
        {
            Work(devIP, devPort, ApplicationPath);
            threadExited = true;
        }
        public void Start(string deviceIP, int devicePort, string AppPath, bool threaded)
        {
            ApplicationPath = AppPath;
            devIP = deviceIP; devPort = devicePort;
            if (!threaded)
            {
                Work(devIP, devPort, ApplicationPath);
                return;
            }
            threadExited = false;
            oneThread = new Thread(new ThreadStart(this.Work2));
            oneThread.Start();
        }

        #region Disposable
        private bool disposed;
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.disposeAll();
                }
                this.disposed = true;
            }
        }
        private void disposeAll()
        {
            // disini dispose semua yang bisa di dispose
        }
        ~DevPull()
        {
            this.Dispose(false);
        }
        #endregion
    }
}
