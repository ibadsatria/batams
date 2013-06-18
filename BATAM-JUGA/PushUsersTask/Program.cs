using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Threading;

namespace PushUsersTask
{
    class Program
    {
        static DeviceHandler PushUsersClass = new DeviceHandler();
        static string applicationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        static int TotalDevice = 60;
        static bool threaded = true;
        static bool pushMode = true;
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0].ToUpper())
                {
                    case "NOTHREAD":
                        threaded = false;
                        pushMode = true;
                        break;
                    case "PULL":
                        pushMode = false;
                        break;
                    case "PULLNOTHREAD":
                        threaded = false;
                        pushMode = false;
                        break;
                    default:    // PUSH
                        threaded = true;
                        pushMode = true;
                        break;
                }
            }

            if (pushMode)
            {
                // Push Mode
                PushModeProc();
            }
            else
            {
                // Pull Mode
                PullModeProc();
            }
        }

        static void PushModeProc()
        {
            DevPush[] devPushList = new DevPush[TotalDevice];
            // Get deviceList dr DB
//            List<DeviceHandler.devAddress> devList = new List<DeviceHandler.devAddress>();
            DeviceHandler.devAddress[] devList = BatamDBUtils.GetDeviceAddresses(BatamDBUtils.GetDb());
            TotalDevice = devList.Length;
            for (int i = 0; i < TotalDevice; i++)
            {
                // Create device handler thread
                devPushList[i] = new DevPush();
                devPushList[i].Start(devList[i].IP, devList[i].Port, applicationPath, threaded);
            }

            if (threaded)
            {
                bool isAllFinished = false;
                while (!isAllFinished)
                {
                    // wait all thread
                    isAllFinished = true;
                    for (int i = 0; i < TotalDevice; i++)
                    {
                        isAllFinished &= devPushList[i].isFinished;
                        if (!isAllFinished) break;
                    }
                    Thread.Sleep(500);
                }
            }
        }

        static void PullModeProc()
        {
            DevPull[] devPullList = new DevPull[TotalDevice];
            // Get deviceList dr DB
//            List<DeviceHandler.devAddress> devList = new List<DeviceHandler.devAddress>();
            DeviceHandler.devAddress[] devList = BatamDBUtils.GetDeviceAddresses(BatamDBUtils.GetDb());
            TotalDevice = devList.Length;
            for (int i = 0; i < TotalDevice; i++)
            {
                // Create device handler thread
                devPullList[i] = new DevPull();
                devPullList[i].Start(devList[i].IP, devList[i].Port, applicationPath, threaded);
            }

            if (threaded)
            {
                bool isAllFinished = false;
                while (!isAllFinished)
                {
                    // wait all thread
                    isAllFinished = true;
                    for (int i = 0; i < TotalDevice; i++)
                    {
                        isAllFinished &= devPullList[i].isFinished;
                        if (!isAllFinished) break;
                    }
                    Thread.Sleep(500);
                }
            }
        }
    }
}
