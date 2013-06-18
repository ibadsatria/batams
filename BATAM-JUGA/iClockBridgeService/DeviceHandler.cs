using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Collections;
using System.IO;

namespace iClockBridgeService
{
    class DeviceHandler : IDisposable
    {
        Thread oneThread;
        bool threadExit;
        bool threadExited;
        bool asRegistratorDevice = false;
        string devIP = "";
        int devPort = 0;
        string ApplicationPath = "";

        //Create Standalone SDK class dynamicly.
        public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();
        private bool bIsConnected = false;//the boolean value identifies whether the device is connected
        private int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.
        int idwErrorCode = 0;

        #region Process Initiator
        public DeviceHandler()
        {
        }

        public bool asRegistrator
        {
            set { asRegistratorDevice = value; }
            get { return asRegistratorDevice; }
        }

        public void start(string deviceIP, int devicePort, string AppPath)
        {
            ApplicationPath = AppPath;
            devIP = deviceIP; devPort = devicePort;
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
        #endregion

        #region Device Connection
        private void DisconnectDevice()
        {
            axCZKEM1.Disconnect();
            bIsConnected = false;
        }

        private bool ConnectDevice()
        {
            bIsConnected = axCZKEM1.Connect_Net(devIP, devPort);
            if (bIsConnected == true)
            {
                iMachineNumber = 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                axCZKEM1.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                return true;
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                // LOG("Unable to connect the device,ErrorCode=" + idwErrorCode.ToString(), "Error");
                writeLogLine("ERROR: ConnectDevice, Unable to connect the device,ErrorCode=" + idwErrorCode.ToString());
                Console.WriteLine("Unable to connect the device,ErrorCode=" + idwErrorCode.ToString() + " Error");
                return false;
            }
        }
        #endregion

        #region Username access
        public struct UserInfoStruct
        {
            public int ID;
            public string Name;
            public string Password;
            public string CardSerialNumber;
            public bool Enabled;
            public int Privilege;
        }
        private bool GetUserInfo(int UserID, out UserInfoStruct UserInfo)
        {
            //UserInfoStruct userInfo;
            UserInfo.ID = UserID;
            UserInfo.Name = "";
            UserInfo.Password = "";
            UserInfo.Privilege = 0;
            UserInfo.Enabled = false;
            UserInfo.CardSerialNumber = "";


            if (bIsConnected == false)
            {
                // LOG("Please connect the device first!", "Error");
                writeLogLine("ERROR: GetUserInfo, Device not connected!");
                Console.WriteLine("Please connect the device first!");
                return false;
            }

            int idwErrorCode = 0;
            UserInfo.ID = UserID;

            bool bEnabled = true;

            string sdwEnrollNumber = UserID.ToString();
            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;

            string sExtendCard = "";

            axCZKEM1.EnableDevice(iMachineNumber, false);

            if (axCZKEM1.SSR_GetUserInfo(iMachineNumber, sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))//upload the user's information(card number included)
            {
                UserInfo.Name = sName;
                UserInfo.Password = sPassword;
                UserInfo.Privilege = iPrivilege;
                UserInfo.Enabled = bEnabled;

                if (axCZKEM1.GetExtUserInfo(iMachineNumber, Convert.ToInt32(sdwEnrollNumber), out sExtendCard))
                    UserInfo.CardSerialNumber = sExtendCard;
                else
                    UserInfo.CardSerialNumber = "";
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                // LOG ("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                writeLogLine("ERROR: GetUserInfo, Operation failed,ErrorCode=" + idwErrorCode.ToString());
                Console.WriteLine("Operation failed,ErrorCode=" + idwErrorCode.ToString());
                return false;
            }
            axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
            axCZKEM1.EnableDevice(iMachineNumber, true);
            return true;
        }

        public bool SetUserInfo(UserInfoStruct UserInfo)
        {
            if (bIsConnected == false)
            {
                // LOG("Please connect the device first!", "Error");
                writeLogLine("ERROR: SetUserInfo, Device not connected!");
                Console.WriteLine("Please connect the device first!");
                return false;
            }

            if (UserInfo.Name == "")
            {
                // LOG("UserID,Privilege,must be inputted first!", "Error");
                writeLogLine("ERROR: SetUserInfo; UserID and Privilege, must not be empty!");
                Console.Write("UserID,Privilege, must not be empty!");
                return false;
            }
            int idwErrorCode = 0;

            bool bEnabled = UserInfo.Enabled;

            string sdwEnrollNumber = UserInfo.ID.ToString();
            string sName = UserInfo.Name;
            string sPassword = UserInfo.Password;
            int iPrivilege = UserInfo.Privilege;

            string sExtendCard = UserInfo.CardSerialNumber;

            axCZKEM1.EnableDevice(iMachineNumber, false);
            bool isfailed = false;

            if (axCZKEM1.SSR_SetUserInfo(iMachineNumber, sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload the user's information(card number included)
            {
                if (!axCZKEM1.SetExtUserInfo(iMachineNumber, Convert.ToInt32(sdwEnrollNumber), sExtendCard))
                {
                    // LOG
                    writeLogLine("ERROR: SetUserInfo, Failed to set CardSerialNumber on UserID: " + sdwEnrollNumber);
                    Console.WriteLine("SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString() + " FAILED");
                    isfailed = true;
                }
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                // LOG("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                writeLogLine("ERROR: SetUserInfo, Operation failed,ErrorCode=" + idwErrorCode.ToString());
                Console.WriteLine("Operation failed,ErrorCode=" + idwErrorCode.ToString());
            }
            axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
            axCZKEM1.EnableDevice(iMachineNumber, true);
            return !isfailed;
        }
        public bool DeleteUser(int UserID)
        {
            if (bIsConnected == false)
            {
                // LOG("Please connect the device first!", "Error");
                writeLogLine("ERROR: DeleteUser, Device not connected!");
                Console.WriteLine("Please connect the device first!");
                return false;
            }

            int idwErrorCode = 0;
            string sdwEnrollNumber = UserID.ToString();
            int idwBackupNumber = 12;

            axCZKEM1.EnableDevice(iMachineNumber, false);
            bool isfailed = false;
            if (!axCZKEM1.SSR_DeleteEnrollData(iMachineNumber, sdwEnrollNumber, idwBackupNumber))//upload the user's information(card number included)
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                // LOG("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                writeLogLine("ERROR: DeleteUser, Operation failed,ErrorCode=" + idwErrorCode.ToString());
                Console.WriteLine("Operation failed,ErrorCode=" + idwErrorCode.ToString());
                isfailed = true;
            }
            axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
            axCZKEM1.EnableDevice(iMachineNumber, true);
            return !isfailed;
        }
        #endregion

        #region WorkCode
        private bool GetWorkCode(int WorkCodeID, ref string WorkCodeName)
        {
            WorkCodeName = "";
            if (bIsConnected == false)
            {
                // LOG("Please connect the device first!", "Error");
                writeLogLine("ERROR: GetWorkCode, Device not connected!");
                Console.WriteLine("Please connect the device first!");
                return false;
            }

            int idwErrorCode = 0;
            int idwEnrollNumber = WorkCodeID;
            string sdwCodeName = "";
            bool isfailed = false;
            axCZKEM1.EnableDevice(iMachineNumber, false);

            if (axCZKEM1.SSR_GetWorkCode(idwEnrollNumber, out sdwCodeName))//upload the user's information(card number included)
            {
                WorkCodeName = sdwCodeName;
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                // LOG("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                writeLogLine("ERROR: GetWorkCode, Operation failed,ErrorCode=" + idwErrorCode.ToString());
                Console.WriteLine("Operation failed,ErrorCode=" + idwErrorCode.ToString());
                isfailed = true;
            }
            axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
            axCZKEM1.EnableDevice(iMachineNumber, true);
            return !isfailed;
        }

        private bool SetWorkCode(int WorkCodeID, string WorkCodeName)
        {
            if (bIsConnected == false)
            {
                // LOG("Please connect the device first!", "Error");
                writeLogLine("ERROR: SetWorkCode, Device not connected!");
                Console.WriteLine("Please connect the device first!");
                return false;
            }

            int idwErrorCode = 0;
            int idwEnrollNumber = WorkCodeID;
            string sdwCodeName = WorkCodeName;
            bool isfailed = false;

            axCZKEM1.EnableDevice(iMachineNumber, false);

            if (!axCZKEM1.SSR_SetWorkCode(idwEnrollNumber, sdwCodeName))//upload the user's information(card number included)
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                // LOG("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                writeLogLine("ERROR: SetWorkCode, Operation failed,ErrorCode=" + idwErrorCode.ToString());
                Console.WriteLine("Operation failed,ErrorCode=" + idwErrorCode.ToString());
                isfailed = true;
            }
            axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
            axCZKEM1.EnableDevice(iMachineNumber, true);
            return !isfailed;
        }

        private bool DeleteWorkCode(int WorkCodeID)
        {
            if (bIsConnected == false)
            {
                // LOG("Please connect the device first!", "Error");
                writeLogLine("ERROR: DeleteWorkCode, Device not connected!");
                Console.WriteLine("Please connect the device first!");
                return false;
            }

            int idwErrorCode = 0;
            int idwEnrollNumber = WorkCodeID;
            bool isfailed = false;

            axCZKEM1.EnableDevice(iMachineNumber, false);

            if (!axCZKEM1.SSR_DeleteWorkCode(idwEnrollNumber))//upload the user's information(card number included)
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                // LOG("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                writeLogLine("ERROR: DeleteWorkCode, Operation failed,ErrorCode=" + idwErrorCode.ToString());
                Console.WriteLine("Operation failed,ErrorCode=" + idwErrorCode.ToString());
                isfailed = true;
            }
            axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
            axCZKEM1.EnableDevice(iMachineNumber, true);
            return !isfailed;
        }

        #endregion

        #region Attendance logs
        //Get the count of attendance records in from terminal(For both Black&White and TFT screen devices).
        private bool GetRecordsCount()
        {
            if (bIsConnected == false)
            {
                // LOG("Please connect the device first!", "Error");
                writeLogLine("ERROR: GetRecordsCount, Device not connected!");
                Console.WriteLine("Please connect the device first!");
                return false;
            }

            int idwErrorCode = 0;
            int iValue = 0;
            bool isfailed = false;

            axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device
            if (!axCZKEM1.GetDeviceStatus(iMachineNumber, 6, ref iValue)) //Here we use the function "GetDeviceStatus" to get the record's count.The parameter "Status" is 6.
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                // LOGS("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                writeLogLine("ERROR: GetDeviceRecordsCount, Operation failed,ErrorCode=" + idwErrorCode.ToString());
                Console.WriteLine("Operation failed,ErrorCode=" + idwErrorCode.ToString());
                isfailed = true;
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device
            return !isfailed;
        }

        //Download the attendance records from the device(For both Black&White and TFT screen devices).
        private bool GetDeviceRecords()
        {
            if (bIsConnected == false)
            {
                // LOG("Please connect the device first!", "Error");
                writeLogLine("ERROR: GetDeviceRecords, Device not connected!");
                Console.WriteLine("Please connect the device first!");
                return false;
            }

            // TODO pastikan koneksi database available disini

            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;

            string sdwCard = "";
            string sdwWorkcode = "";

            int idwUserMode = 0;
            int idwDeviceID = 0;
            int idwErrorCode = 0;

            bool isfailed = false;
            string aRecord = "";
            List<string> recordList = new List<string>();

            axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device
            if (axCZKEM1.ReadAttNewLogData(iMachineNumber))//read all the attendance records to the memory
            {
                while (axCZKEM1.GetAttNewLogData(iMachineNumber, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond,
                    out sdwCard, out sdwWorkcode, out idwUserMode, out idwDeviceID))//get records from the memory
                {
                    // simpan sementara di file, menjaga agar data tidak hilang sebelum masuk database
                    aRecord = devIP + ", ";
                    aRecord += idwYear.ToString().PadLeft(4, '0') + idwMonth.ToString().PadLeft(2, '0') + idwDay.ToString().PadLeft(2, '0')
                        + idwHour.ToString().PadLeft(2, '0') + idwMinute.ToString().PadLeft(2, '0') + idwSecond.ToString().PadLeft(2, '0') + ", ";
                    aRecord += sdwWorkcode + ", ";
                    aRecord += idwUserMode.ToString() + ", ";
                    aRecord += sdwCard + ", ";
                    aRecord += idwDeviceID.ToString();
                    recordList.Add(aRecord);
                }
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);

                if (idwErrorCode != 0)
                {
                    isfailed = true;
                    // LOG("Reading data from terminal failed,ErrorCode: " + idwErrorCode.ToString(), "Error");
                    writeLogLine("ERROR: GetDeviceRecords, Reading data from terminal failed, ErrorCode: " + idwErrorCode.ToString());
                    Console.WriteLine("Reading data from terminal failed,ErrorCode: " + idwErrorCode.ToString());
                }
                else
                {
                    // LOG("No data from terminal returns!", "Error");
                    //writeLogLine("ERROR: GetDeviceRecords, No data from terminal returns!");
                    Console.WriteLine("No data from terminal returns!");
                }
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device
            if (recordList.Count > 0) writeRecordsLog(recordList);
            recordList.Clear();
            return !isfailed;
        }

        //Clear all attendance records from terminal (For both Black&White and TFT screen devices).
        private bool ClearDeviceRecords()
        {
            if (bIsConnected == false)
            {
                // LOG("Please connect the device first!", "Error");
                writeLogLine("ERROR: ClearDeviceRecords, Device not connected!");
                Console.WriteLine("Please connect the device first!");
                return false;
            }
            int idwErrorCode = 0;
            bool isfailed = false;

            axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device
            if (!axCZKEM1.ClearGLog(iMachineNumber))
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                // LOG("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                writeLogLine("ERROR: ClearDeviceRecords, Operation failed,ErrorCode=" + idwErrorCode.ToString());
                Console.WriteLine("Operation failed,ErrorCode=" + idwErrorCode.ToString());
                isfailed = true;
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device
            return !isfailed;
        }

        #endregion

        #region Log Writer
        private void writeRecordsLog(List<string> recordList)
        {
            string path = ApplicationPath + "\\RecordsTemp";
            string filepath = path + "\\AttRecord-" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

            using (StreamWriter sw = File.AppendText(filepath))
            {
                for (int i = 0; i < recordList.Count; i++)
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmss") + ": " + recordList[i]);
                }
            }
        }

        private void writeRecordLine(string dataRecord)
        {
            string path = ApplicationPath + "\\RecordsTemp";
            string filepath = path + "\\AttRecord-" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

            using (StreamWriter sw = File.AppendText(filepath))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmss") + ": " + dataRecord);
            }
        }

        private void writeLogLine(string dataLog)
        {
            string path = ApplicationPath + "\\Logs";
            if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
            path += "\\" + devIP;
            if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
            string filepath = path + "\\AttLog-" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            using (StreamWriter sw = File.AppendText(filepath))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmss") + ": " + dataLog);
            }
        }

        #endregion

        /// <summary>
        /// Business Process Thread untuk setiap device
        /// </summary>
        void ThreadProcSafe()
        {
            // business process disini
            threadExited = false;
            int ctr = 0;
            while (!threadExit)
            {
                // eksekusi business process disini dan database di thread lain
                Thread.Sleep(100);
                ctr++;
                if (asRegistrator)
                {
                    if (ctr < 300) continue;    // per 30 detik
                }
                else
                {
                    if (ctr < 600) continue;    // per 1 menit
                }
                ctr = 0;
                //Console.Write("ID: " + Thread.CurrentThread.ManagedThreadId + ", ");
                if (asRegistrator)
                {
                    // business process untuk device registrator
                    // TODO panggil fungsinya
                    Load semua WorkCode dan semua UserID untuk ke database
                }
                else
                {
                    // business process untuk slave device
                    // TODO panggil fungsinya
                    Jika ada yg harus di set di alat dari database
                    Load semua transaksi absensi
                        if(GetDeviceRecords())
                        {
                            ClearDeviceRecords();
                        }
                }
            }
            threadExited = true;
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
                axCZKEM1 = null;
                this.disposed = true;
            }
        }
        private void disposeAll()
        {
            // disini dispose semua yang bisa di dispose
            if (oneThread != null)
            {
                try
                {
                    oneThread.Abort();
                    oneThread.Join(5000);
                }
                catch { }
            }
        }
        ~DeviceHandler()
        {
            this.Dispose(false);
        }
        #endregion
    }
}
