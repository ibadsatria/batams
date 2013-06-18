using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DevBridgeService
{
    class DeviceHandler : IDisposable
    {
        Thread oneThread;
        bool threadExit;
        bool threadExited;

        public DeviceHandler()
        {
        }

        public void start()
        {
            threadExit = false;
            oneThread = new Thread(new ThreadStart(this.ThreadProcSafe));
            oneThread.Start();
        }

        public void stop()
        {
            if (oneThread == null) return;
            threadExit = true;
            while (!threadExited)
            {
                Thread.Sleep(50);
            }
        }

        void ThreadProcSafe()
        {
            threadExited = false;
            while (!threadExit)
            {
                // eksekusi komunikasi dan database disini
                Thread.Sleep(200);
                Console.Write("ID: " + Thread.CurrentThread.ManagedThreadId + ", ");
            }
            threadExited = true;
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
        ~DeviceHandler()
        {
            this.Dispose(false);
        }

    }
}
