using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace iClockBridgeService
{
    public class DeviceManager : IDisposable
    {
        int TotDevice = 0;
        string ApplicationPath = "";
        DeviceHandler[] devHandlerList;

        /*
        public DeviceManager(string applicationPath, bool consoleMode)
        {
            Console.WriteLine("I have written to " + applicationPath + "\\output.txt");
            Console.WriteLine("Press any key to exit...");
            if (consoleMode) Console.ReadKey();

            //If you need to exit the application before it reaches this point, you need to use the line below, or the service will not stop running.
            //But if you are running a socket server or something that needs to stay alive after this method has been executed, remove the line below.
            Environment.Exit(0);
        }
        */
        public DeviceManager(string AppPath, int TotalDevice)
        {
            ApplicationPath = AppPath;
            TotDevice = TotalDevice;
            devHandlerList = new DeviceHandler[TotDevice];
            for (int i = 0; i < TotDevice; i++)
            {
                // Create device handler thread
                devHandlerList[i] = new DeviceHandler();
            }
        }

        private string getDevIP(int index)
        {
            // nanti ambil dari database
            string devIP = "192.168.1.";
            return devIP + (index+2).ToString();
        }
        private int getDevPort(int index)
        {
            // nanti ambil dari database
            int devPort = 4370;
            return devPort;
        }

        public void Start()
        {
            //ManualResetEvent resetEvent = new ManualResetEvent(false);

            // Start workers.
            for (int i = 0; i < TotDevice; i++)
            {
                // Create device handler thread
                // baca seting koneksi device dari database
                devHandlerList[i].start(getDevIP(i), getDevPort(i), ApplicationPath);
                if (i == 0) devHandlerList[i].asRegistrator = true;
            }

/*            // Wait for workers.
            //            resetEvent.WaitOne();
            Console.WriteLine("Enter to close all");
            Console.ReadLine();
            Console.WriteLine("Closing threads.");
            for (int i = 0; i < TotDevice; i++)
            {
                // Create device handler thread
                devHandlerList[i].stop();
            }
            Console.WriteLine("Finished... Enter to exit");
            Console.ReadLine();
 */
        }

        public void Stop()
        {
            // Stop workers.
            for (int i = 0; i < TotDevice; i++)
            {
                // Create device handler thread
                // baca seting koneksi device dari database
                devHandlerList[i].stop();
                devHandlerList[i].Dispose();
            }
            //Application is exiting. This is where your cleanup code should be. For example, a socket server would need "mySocketListener.Close();"
//            Environment.Exit(0);
        }

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
        ~DeviceManager()
        {
            this.Dispose(false);
        }

    }
}
