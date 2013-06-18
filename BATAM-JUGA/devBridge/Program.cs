using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace devBridge
{
    class Program
    {
        static void Main(string[] args)
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
