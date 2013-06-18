using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PushUsersTask
{
    public class DevPush : IDisposable
    {
        DeviceHandler DevHandlerPush;
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
            ApplicationPath = appPath;
            DevHandlerPush = new DeviceHandler();
            DevHandlerPush.init(devIP, devPort, appPath);
            Console.WriteLine("Connectings to IP: " + devIP);
            if (!DevHandlerPush.ConnectDevice())
            {
                Console.WriteLine("Can't connect to device " + devIP);
                DevHandlerPush.Dispose();
                return;
            }
            Console.WriteLine("Connected to IP: " + devIP);

            // get UserInfo to List<DeviceHandler.UserInfoStruct>
//            List<DeviceHandler.UserInfoStruct> userList = new List<DeviceHandler.UserInfoStruct>();
            DeviceHandler.UserInfoStruct[] userList = BatamDBUtils.GetUserInfo(BatamDBUtils.GetDb());

            foreach (DeviceHandler.UserInfoStruct userInfo in userList)
            {
                Console.Write("Set UserInfo " + userInfo.ID.ToString() + " to IP: " + devIP);
                if (!DevHandlerPush.SetUserInfo(userInfo))
                {
                    Console.WriteLine(" failed");
                    DevHandlerPush.DisconnectDevice();
                    DevHandlerPush.Dispose();
                    return;
                }
                Console.WriteLine(" success");
            }
            Console.WriteLine("Set user info to " + devIP + " done.");

            Console.Write("Clear WorkCode of IP: " + devIP);
            if (!DevHandlerPush.ClearWorkCode())
            {
                Console.WriteLine(" failed");
                DevHandlerPush.DisconnectDevice();
                DevHandlerPush.Dispose();
                return;
            }
            Console.WriteLine(" success");

            // get WorkCode info to List<DeviceHandler.WorkCodeStruct>
//            List<DeviceHandler.WorkCodeStruct> workCodeList = new List<DeviceHandler.WorkCodeStruct>();
            DeviceHandler.WorkCodeStruct[] workCodeList = BatamDBUtils.GetWorkCode(BatamDBUtils.GetDb());

            foreach (DeviceHandler.WorkCodeStruct workCode in workCodeList)
            {
                Console.Write("Set WorkCode " + workCode.ID.ToString() + " to IP: " + devIP);
                if (!DevHandlerPush.SetWorkCode(workCode.ID, workCode.Name))
                {
                    Console.WriteLine(" failed");
                    DevHandlerPush.DisconnectDevice();
                    DevHandlerPush.Dispose();
                    return;
                }
                Console.WriteLine(" success");
            }
            Console.WriteLine("Set WorkCode to " + devIP + " done.");
            Console.WriteLine("Disconnecting device " + devIP);
            DevHandlerPush.DisconnectDevice();
            DevHandlerPush.Dispose();
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
        ~DevPush()
        {
            this.Dispose(false);
        }
        #endregion
    }
}
