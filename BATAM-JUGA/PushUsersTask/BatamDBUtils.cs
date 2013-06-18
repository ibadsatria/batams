using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PushUsersTask
{
    class BatamDBUtils
    {

        private static DbCommon db = null;

        public static DbCommon GetDb()
        {
            if (db == null)
            {
                db = new DbCommon("180.250.39.107", "attendance_batam", "daksa", "d4ksasangsangkuriang");
                if (db.ConnectDb() == 1)
                {
                    Console.WriteLine("Database connected.");
                }
                else
                {
                    Console.WriteLine("Database not connected.");
                }
            }
            return db;
        }

        /**
         * kalo dbc = null, bakal return array 1 element, data dummy
         * kalo dbc ambil dari BatamDBUtils.getDBObject() dan koneksi valid:
         *     kalo ada isinya, bakal return isinya
         *     kalo ga ada isinya, bakal return array dengan member 0
         */
        public static DeviceHandler.devAddress[] GetDeviceAddresses(DbCommon dbc)
        {
            //if input null
            if (dbc == null)
            {
                DeviceHandler.devAddress[] dummy = new DeviceHandler.devAddress[1];
                DeviceHandler.devAddress dummyTerminal = new DeviceHandler.devAddress();
                dummyTerminal.IP = "192.168.1.10";
                dummyTerminal.Port = 4370;
                dummy[0] = dummyTerminal;
                return dummy;
            }
            dbc.SetQuery("select ip, port FROM devices");
            dbc.AddDataTbl("devices");
            DeviceHandler.devAddress[] returnValue = null;
            if (dbc.DbQuery("devices") > 0) // ada data dan tidak terjadi error.
            {
                DataTable record = dbc.GetResultData("devices");
                dbc.RemoveDataTbl("devices");
                returnValue = new DeviceHandler.devAddress[record.Rows.Count];
                for (int i = 0; i < record.Rows.Count; i++)
                {
                    DataRow row = record.Rows[i];
                    DeviceHandler.devAddress dev = new DeviceHandler.devAddress();
                    dev.IP = row.ItemArray[0].ToString();
                    dev.Port = int.Parse(row.ItemArray[1].ToString());
                    returnValue[i] = dev;
                }
            }
            else
            {
                returnValue = new DeviceHandler.devAddress[0];

            }
            return returnValue;
        }

        /**
         * kalo dbc = null, bakal return array 1 element, data dummy
         * kalo dbc ambil dari BatamDBUtils.getDBObject() dan koneksi valid:
         *     kalo ada isinya, bakal return isinya
         *     kalo ga ada isinya, bakal return array dengan member 0
         */
        public static DeviceHandler.UserInfoStruct[] GetUserInfo(DbCommon dbc)
        {
            //if input null
            if (dbc == null)
            {
                DeviceHandler.UserInfoStruct[] dummy = new DeviceHandler.UserInfoStruct[1];
                DeviceHandler.UserInfoStruct dummyUser = new DeviceHandler.UserInfoStruct();
                dummyUser.ID = 7;
                dummyUser.Name = "Riza Bond";
                dummyUser.Password = "007";
                dummyUser.CardSerialNumber = "007007";
                dummyUser.Enabled = true;
                dummyUser.Privilege = 1;
                dummy[0] = dummyUser;
                return dummy;
            }
            dbc.SetQuery("select id, name, card_serial_number, password, active, authority FROM authorized_user");
            dbc.AddDataTbl("authorized_user");
            DeviceHandler.UserInfoStruct[] returnValue = null;
            if (dbc.DbQuery("authorized_user") > 0) // ada data dan tidak terjadi error.
            {
                DataTable record = dbc.GetResultData("authorized_user");
                dbc.RemoveDataTbl("authorized_user");
                returnValue = new DeviceHandler.UserInfoStruct[record.Rows.Count];
                for (int i = 0; i < record.Rows.Count; i++)
                {
                    DataRow row = record.Rows[i];
                    DeviceHandler.UserInfoStruct user = new DeviceHandler.UserInfoStruct();
                    //user.IP = row.ItemArray[0].ToString();
                    //user.Port = int.Parse(row.ItemArray[1].ToString());
                    user.ID = int.Parse(row.ItemArray[0].ToString());
                    user.Name = row.ItemArray[1].ToString();
                    user.CardSerialNumber = row.ItemArray[2].ToString();
                    user.Password = row.ItemArray[3].ToString();
                    user.Enabled = bool.Parse(row.ItemArray[4].ToString());
                    user.Privilege = int.Parse(row.ItemArray[5].ToString());
                    returnValue[i] = user;
                }
            }
            else
            {
                returnValue = new DeviceHandler.UserInfoStruct[0];

            }
            return returnValue;

        }


        private static String[] ATTENDANCE_TABLES = { "insert into student_attendance (card_serial_number, checkin_time, device_id) values ", 
                                                      "insert into attendance_course (course_id, start_time, device_id) values " };

        /**
         * 
         */
        public static bool SaveAttendanceLog(DbCommon dbc, DeviceHandler.AttendanceLog log)
        {
            bool success = false;

            String IDinContext = log.Usermode == 1 ? log.Workcode : log.CardSerialNumber;
            StringBuilder query = new StringBuilder(ATTENDANCE_TABLES[log.Usermode]);
            query.Append("('").Append(IDinContext).Append("','").Append(log.DateTime)
                 .Append("','").Append(log.DevID).Append("')");
            Console.WriteLine(query.ToString());

            String sql = query.ToString();
            dbc.SetQuery(sql);
            if (dbc.ExecuteQuery() == 0)
            {
                // roolback trans
                dbc.RollBackTrans();
            }
            else
            {
                success = true;
            }
            if (success) { dbc.CommitTrans(); }
            return success;
        }

        public static bool SaveAttendanceLog(DbCommon dbc, DeviceHandler.AttendanceLog[] logs)
        {
            bool success = false;
            StringBuilder query = new StringBuilder();
            for (int i = 0; i < logs.Length; ++i)
            {
                DeviceHandler.AttendanceLog log = logs[i];
                String IDinContext = log.Usermode == 1 ? log.Workcode : log.CardSerialNumber;
                query.Append(ATTENDANCE_TABLES[log.Usermode]);
                query.Append("('").Append(IDinContext).Append("','").Append(log.DateTime)
                     .Append("','").Append(log.DevID).Append("'); ");
                Console.WriteLine(query.ToString());
            }
            String sql = query.ToString();
            dbc.SetQuery(sql);
            if (dbc.ExecuteQuery() == 0)
            {
                // roolback trans
                dbc.RollBackTrans();
            }
            else
            {
                success = true;
            }
            if (success) { dbc.CommitTrans(); }
            return success;
        }
        public static DeviceHandler.WorkCodeStruct[] GetWorkCode(DbCommon dbc)
        {
            //if input null
            if (dbc == null)
            {
                DeviceHandler.WorkCodeStruct[] dummy = new DeviceHandler.WorkCodeStruct[1];
                DeviceHandler.WorkCodeStruct dummyUser = new DeviceHandler.WorkCodeStruct();
                dummyUser.ID = 7;
                dummyUser.Name = "Riza Bond";
                dummy[0] = dummyUser;
                return dummy;
            }
            dbc.SetQuery("select id, name FROM workcode");
            dbc.AddDataTbl("workcode");
            DeviceHandler.WorkCodeStruct[] returnValue = null;
            if (dbc.DbQuery("workcode") > 0) // ada data dan tidak terjadi error.
            {
                DataTable record = dbc.GetResultData("workcode");
                dbc.RemoveDataTbl("workcode");
                returnValue = new DeviceHandler.WorkCodeStruct[record.Rows.Count];
                for (int i = 0; i < record.Rows.Count; i++)
                {
                    DataRow row = record.Rows[i];
                    DeviceHandler.WorkCodeStruct user = new DeviceHandler.WorkCodeStruct();
                    //user.IP = row.ItemArray[0].ToString();
                    //user.Port = int.Parse(row.ItemArray[1].ToString());
                    user.ID = int.Parse(row.ItemArray[0].ToString());
                    user.Name = row.ItemArray[1].ToString();
                    returnValue[i] = user;
                }
            }
            else
            {
                returnValue = new DeviceHandler.WorkCodeStruct[0];

            }
            return returnValue;
        }
    }

}
