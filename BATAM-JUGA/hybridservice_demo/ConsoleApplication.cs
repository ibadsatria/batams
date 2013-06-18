using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HybridService
{
    class ConsoleApplication
    {
        public ConsoleApplication(string applicationPath, bool consoleMode)
        {
            //This is where your console application would be

            Console.WriteLine("Hybrid Service Application");
            Console.WriteLine();

            StreamWriter stream = new StreamWriter(applicationPath + "\\output.txt", true);
            stream.WriteLine(DateTime.Now.ToString("f") + "   It works! \n");
            stream.Flush();
            stream.Close();
            stream.Dispose();

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
    }
}
