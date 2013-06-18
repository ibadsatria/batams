using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DevBridgeService
{
    class DeviceManager
    {
        public DeviceManager(string applicationPath, bool consoleMode)
        {
            Console.WriteLine("I have written to " + applicationPath + "\\output.txt");
            Console.WriteLine("Press any key to exit...");
            if (consoleMode) Console.ReadKey();

            //If you need to exit the application before it reaches this point, you need to use the line below, or the service will not stop running.
            //But if you are running a socket server or something that needs to stay alive after this method has been executed, remove the line below.
            Environment.Exit(0);
        }

        public void Close()
        {
            //Application is exiting. This is where your cleanup code should be. For example, a socket server would need "mySocketListener.Close();"
        }

        private void appStart()
        {
            int numThreads = 60;
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            int toProcess = numThreads;

            DeviceHandler[] devHandlerList = new DeviceHandler[numThreads];

            // Start workers.
            for (int i = 0; i < numThreads; i++)
            {
                // Create device handler thread
                devHandlerList[i] = new DeviceHandler();
                devHandlerList[i].start();
            }

            // Wait for workers.
            //            resetEvent.WaitOne();
            Console.WriteLine("Enter to close all");
            Console.ReadLine();
            Console.WriteLine("Closing threads.");
            for (int i = 0; i < numThreads; i++)
            {
                // Create device handler thread
                devHandlerList[i].stop();
            }
            Console.WriteLine("Finished... Enter to exit");
            Console.ReadLine();
        }

    }
}
