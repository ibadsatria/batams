using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace devBridge
{
    class TCPClient
    {
        private const int BufferSize = 0x400;
        private ManualResetEvent connectDone;
        private bool disposed;
        private bool fManualDisconnect;
        private bool hasilConnect;
        private bool inConnectCallback;
        private List<string> lstToSend;

        public delegate void onTCPConnected();
        public delegate void onTCPDataBinaryReceived(byte[] msg);
        public delegate void onTCPDataReceived(string msg);
        public delegate void onTCPDataSent(int count);
        public delegate void onTCPDisconnected();


        private ManualResetEvent receiveDone;
        private ManualResetEvent sendDone;
        private object syncRec;
        private byte[] thisbuffer;
        public Socket thisSocket;

        // Events
        public event onTCPConnected onConnected;
        public event onTCPDataBinaryReceived onDataBinaryReceived;
        public event onTCPDataReceived onDataReceived;
        public event onTCPDataSent onDataSent;
        public event onTCPDisconnected onDisconnected;

        public TCPClient()
        {
            this.thisbuffer = new byte[0x400];
            this.connectDone = new ManualResetEvent(false);
            this.sendDone = new ManualResetEvent(false);
            this.receiveDone = new ManualResetEvent(false);
            this.syncRec = new object();
            this.lstToSend = new List<string>();
        }
        public bool connect(string hostOrIpAddress, int port)
        {
            if (this.thisSocket != null)
            {
                return false;
            }
            this.fManualDisconnect = false;
            IPAddress iAddr = null;
            try
            {
                if (!this.GetIP(hostOrIpAddress, ref iAddr))
                {
                    //iAddr = Dns.Resolve(hostOrIpAddress).AddressList[0];
                    iAddr = Dns.GetHostEntry(hostOrIpAddress).AddressList[0];
                }
                this.thisSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.thisSocket.Connect(iAddr, port);
                if (this.thisSocket != null)
                {
                    this.thisSocket.BeginReceive(this.thisbuffer, 0, 1024, SocketFlags.None, new AsyncCallback(this.ReadCallback), this.thisSocket);
                    if (this.thisSocket != null)
                    {
                        if (this.onConnected != null)
                        {
                            this.onConnected();
                        }
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                if ((this.thisSocket != null) && this.thisSocket.Connected)
                {
                    try
                    {
                        this.thisSocket.Shutdown(SocketShutdown.Both);
                        this.thisSocket.Disconnect(false);
                    }
                    catch
                    {
                    }
                }
                try
                {
                    this.thisSocket.Close();
                }
                catch
                {
                }
                this.thisSocket = null;
                return false;
            }
        }
        public bool connect2(string hostOrIpAddress, int port)
        {
            if (this.thisSocket != null)
            {
                return false;
            }
            this.fManualDisconnect = false;
            IPAddress iAddr = null;
            try
            {
                if (!this.GetIP(hostOrIpAddress, ref iAddr))
                {
                    //iAddr = Dns.Resolve(hostOrIpAddress).AddressList[0];
                    iAddr = Dns.GetHostEntry(hostOrIpAddress).AddressList[0];
                }
                this.thisSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.thisSocket.BeginConnect(new IPEndPoint(iAddr, port), new AsyncCallback(this.ConnectCallback), this.thisSocket);
                this.inConnectCallback = true;
                while (this.inConnectCallback)
                {
                    Thread.Sleep(1);
                }
                Thread.Sleep(100);
                return this.hasilConnect;
            }
            catch
            {
                return false;
            }
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            this.inConnectCallback = true;
            Socket client = null;
            try
            {
                client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                this.thisSocket.BeginReceive(this.thisbuffer, 0, 0x400, SocketFlags.None, new AsyncCallback(this.ReadCallback), this.thisSocket);
                this.hasilConnect = true;
                this.inConnectCallback = false;
                if (this.onConnected != null)
                {
                    this.onConnected();
                }
            }
            catch
            {
                this.hasilConnect = false;
                this.inConnectCallback = false;
                if ((client != null) && client.Connected)
                {
                    try
                    {
                        client.Shutdown(SocketShutdown.Both);
                        client.Disconnect(false);
                    }
                    catch
                    {
                    }
                }
                try
                {
                    client.Close();
                }
                catch
                {
                }
                client = null;
            }
        }
        private Socket ConnectSocket(string server, int port)
        {
            foreach (IPAddress address in Dns.GetHostEntry(server).AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                tempSocket.Connect(ipe);
                if (tempSocket.Connected)
                {
                    return tempSocket;
                }
            }
            return null;
        }
        public void disconnect()
        {
            this.fManualDisconnect = true;
            if (this.thisSocket != null)
            {
                try
                {
                    this.thisSocket.Shutdown(SocketShutdown.Both);
                    this.thisSocket.Disconnect(false);
                }
                catch
                {
                }
                try
                {
                    this.thisSocket.Close();
                }
                catch
                {
                }
                this.thisSocket = null;
                if (this.onDisconnected != null)
                {
                    this.onDisconnected();
                }
            }
        }
        private bool GetIP(string IP, ref IPAddress ipAddr)
        {
            try
            {
                ipAddr = IPAddress.Parse(IP);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void ReadCallback(IAsyncResult ar)
        {
            Socket handler = (Socket)ar.AsyncState;
            int bytesRead = 0;
            try
            {
                if (handler.Connected)
                {
                    bytesRead = handler.EndReceive(ar);
                }
            }
            catch
            {
                if (!this.fManualDisconnect)
                {
                    if (this.onDisconnected != null)
                    {
                        this.onDisconnected();
                    }
                    if ((this.thisSocket != null) && this.thisSocket.Connected)
                    {
                        try
                        {
                            this.thisSocket.Shutdown(SocketShutdown.Both);
                            this.thisSocket.Disconnect(false);
                        }
                        catch
                        {
                        }
                        try
                        {
                            this.thisSocket.Close();
                        }
                        catch
                        {
                        }
                    }
                    this.thisSocket = null;
                }
                return;
            }
            if (bytesRead > 0)
            {
                string aa = Encoding.GetEncoding(1252).GetString(this.thisbuffer, 0, bytesRead);
                byte[] ab = new byte[bytesRead];
                Array.Copy(this.thisbuffer, ab, bytesRead);
                lock (this.syncRec)
                {
                    if (this.onDataReceived != null)
                    {
                        this.onDataReceived(aa);
                    }
                    if (this.onDataBinaryReceived != null)
                    {
                        this.onDataBinaryReceived(ab);
                    }
                }
                bytesRead = 0;
                try
                {
                    handler.BeginReceive(this.thisbuffer, 0, 1024, SocketFlags.None, new AsyncCallback(this.ReadCallback), handler);
                    aa = "";
                    return;
                }
                catch
                {
                    if (!this.fManualDisconnect)
                    {
                        if (this.onDisconnected != null)
                        {
                            this.onDisconnected();
                        }
                        if ((this.thisSocket != null) && this.thisSocket.Connected)
                        {
                            try
                            {
                                this.thisSocket.Disconnect(false);
                                this.thisSocket.Close();
                            }
                            catch
                            {
                            }
                        }
                        try
                        {
                            this.thisSocket.Close();
                        }
                        catch
                        {
                        }
                        this.thisSocket = null;
                    }
                    return;
                }
            }
            if (!this.fManualDisconnect)
            {
                if (this.onDisconnected != null)
                {
                    this.onDisconnected();
                }
                if ((this.thisSocket != null) && this.thisSocket.Connected)
                {
                    try
                    {
                        this.thisSocket.Shutdown(SocketShutdown.Both);
                        this.thisSocket.Disconnect(false);
                    }
                    catch
                    {
                    }
                }
                try
                {
                    this.thisSocket.Close();
                }
                catch
                {
                }
                this.thisSocket = null;
            }
        }
        private void sendCallback(IAsyncResult ar)
        {
            int bytesSent = 0;
            try
            {
                bytesSent = ((Socket)ar.AsyncState).EndSend(ar);
            }
            catch
            {
                try
                {
                    this.sendDone.Set();
                }
                catch
                {
                }
                return;
            }
            if (this.onDataSent != null)
            {
                this.onDataSent(bytesSent);
            }
            try
            {
                this.sendDone.Set();
            }
            catch
            {
            }
        }
        public bool sendData(string msg)
        {
            if (this.disposed)
            {
                return false;
            }
            if (this.thisSocket == null)
            {
                return false;
            }
            if (!this.thisSocket.Connected)
            {
                return false;
            }
            byte[] byteData = Encoding.GetEncoding(0x4e4).GetBytes(msg);
            try
            {
                int i = this.thisSocket.Send(byteData);
                if (this.onDataSent != null)
                {
                    this.onDataSent(i);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool sendData(byte[] data)
        {
            if (this.disposed)
            {
                return false;
            }
            if (this.thisSocket == null)
            {
                return false;
            }
            if (!this.thisSocket.Connected)
            {
                return false;
            }
            try
            {
                int i = this.thisSocket.Send(data);
                if (this.onDataSent != null)
                {
                    this.onDataSent(i);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
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
            if (this.thisSocket != null)
            {
                try
                {
                    if (this.thisSocket.Connected)
                    {
                            try
                            {
                                this.thisSocket.Shutdown(SocketShutdown.Both);
                                this.thisSocket.Disconnect(false);
                            }
                            catch
                            {
                            }
                    }
                }
                catch
                {
                }
                finally
                {
                    try
                    {
                        this.thisSocket.Close();
                    }
                    catch
                    {
                    }
                }
                this.thisSocket = null;
            }
            this.onConnected = null;
            this.onDataReceived = null;
            this.onDataSent = null;
            this.onDisconnected = null;
            try
            {
                this.connectDone.Close();
            }
            catch
            {
            }
            try
            {
                this.sendDone.Close();
            }
            catch
            {
            }
            try
            {
                this.receiveDone.Close();
            }
            catch
            {
            }
            this.connectDone = null;
            this.sendDone = null;
            this.receiveDone = null;
            Array.Resize<byte>(ref this.thisbuffer, 0);
            this.thisbuffer = null;
        }
        ~TCPClient()
        {
            this.Dispose(false);
        }
    }
}
