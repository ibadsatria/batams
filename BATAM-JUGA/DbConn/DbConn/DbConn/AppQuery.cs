using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DbConn
{
    // Class Name   : AppQuery
    // Creator      : Iwan Supratman (deong)
    // Create Date  : June, 13 2013
    // Mail         : deong84@gmail.com
    // Globat Matra Aditama, PT.

    class AppQuery
    {
        DbCommon dbc;
        public AppQuery(DbCommon db) 
        {
            dbc = db;
        }

        public int TestInsertTable()
        {
            // ::test insert database ke table attendance_course 10 row data
            // INSERT INTO table (field0, field1, field2, field3, field4) VALUES (value0, value1, value3, value4);

            DataTable record = new DataTable("attendance_course");
            record.Columns.Add("id", typeof(string));
            record.Columns.Add("course_id", typeof(string));
            record.Columns.Add("start_time", typeof(string));
            record.Columns.Add("device_id", typeof(string));
            record.Columns.Add("status", typeof(int));

            // test 10 row                        
            record.Rows.Add("A17A31", "EL-3406", "2013-06-13 08:00:00", "2", 0);
            record.Rows.Add("A17A32", "EL-3406", "2013-06-13 09:00:00", "2", 0);
            record.Rows.Add("A17A33", "EL-3406", "2013-06-13 10:00:00", "2", 0);
            record.Rows.Add("A17A35", "EL-3406", "2013-06-13 11:00:00", "2", 0);
            record.Rows.Add("A17A36", "EL-3406", "2013-06-13 12:00:00", "2", 0);
            record.Rows.Add("A17A37", "EL-3406", "2013-06-13 13:00:00", "2", 0);
            record.Rows.Add("A17A38", "EL-3406", "2013-06-13 14:00:00", "2", 0);
            record.Rows.Add("A17A39", "EL-3406", "2013-06-13 15:00:00", "2", 0);
            record.Rows.Add("A17A40", "EL-3406", "2013-06-13 16:00:00", "2", 0);
            record.Rows.Add("A17A41", "EL-3406", "2013-06-13 17:00:00", "2", 0);

            return InsertDb(record); // jika return 1 berhasil, else gagal di rollback kabeh porses insert na.
        }

        public int TestUpdateTable()
        {
            // ::test update database ke table attendance_course
            // Example QUERY : UPDATE attendance_course SET course_id = 'EL-3406',start_time = '2013-06-13 09:00:00', device_id = '2', status = 0  WHERE id =  'A17A31';

            DataTable record = new DataTable("attendance_course");
            record.Columns.Add("id", typeof(string));
            record.Columns.Add("course_id", typeof(string));
            record.Columns.Add("start_time", typeof(string));
            record.Columns.Add("device_id", typeof(string));
            record.Columns.Add("status", typeof(int));

            // test update 10 row, field id sebagai kondisi WHERE
            record.Rows.Add("A17A31", "EL-3406", "2013-06-13 08:00:00", "2", 1);
            record.Rows.Add("A17A32", "EL-3406", "2013-06-13 09:00:00", "2", 1);
            record.Rows.Add("A17A33", "EL-3406", "2013-06-13 10:00:00", "2", 1);
            record.Rows.Add("A17A35", "EL-3406", "2013-06-13 11:00:00", "2", 1);
            record.Rows.Add("A17A36", "EL-3406", "2013-06-13 12:00:00", "2", 1);
            record.Rows.Add("A17A37", "EL-3406", "2013-06-13 13:00:00", "2", 1);
            record.Rows.Add("A17A38", "EL-3406", "2013-06-13 14:00:00", "2", 1);
            record.Rows.Add("A17A39", "EL-3406", "2013-06-13 15:00:00", "2", 1);
            record.Rows.Add("A17A40", "EL-3406", "2013-06-13 16:00:00", "2", 1);
            record.Rows.Add("A17A41", "EL-3406", "2013-06-13 17:00:00", "2", 1);

            //record.PrimaryKey = record.Columns[0];
            record.PrimaryKey = new DataColumn[] { record.Columns["id"] };            
            return UpdateDb(record);
        }

        public int TestDeleteRow()
        {
            // ::test delete row database ke table attendance_course
            // Example QUERY : DELETE FROM attendance_course WHERE id = 'A17A31';

            DataTable record = new DataTable("attendance_course");
            record.Columns.Add("id", typeof(string));
            record.Columns.Add("course_id", typeof(string));
            record.Rows.Add("A17A31");
            record.Rows.Add("A17A32");
            record.Rows.Add("A17A33");
            record.Rows.Add("A17A35");
            record.Rows.Add("A17A36");
            record.Rows.Add("A17A37");
            record.Rows.Add("A17A38");
            record.Rows.Add("A17A39");
            record.Rows.Add("A17A40");
            record.Rows.Add("A17A41");
            record.PrimaryKey = new DataColumn[] { record.Columns["id"] };
            return DeleteDb(record);
        }

        public DataTable TestReadTable()
        {
            // QUERY Example : SELECT * FROM table_name 
            //                 WHERE cond1 = val1 AND cand2 = val2, cond3 NOT IN (in1,in2,in3) 
            //                 ORDER BY field1, field2, field3 ASC 
            //                 OFFSET 0 
            //                 LIMIT 10;

            string tblname = "attendance_course";
            string[] selectfield = new string[1];       // bisa lebih dari 1, kalau isinya di kasih * artinya semua field.

            // NOTE:: belum ada sample yang di join, sementara gk di impelemtasikan liat kebutuhan saja!!.
            // string[] joinedtable = new string[0];      
            // contoh join table : 
            // INNER JOIN tbl1 ON cond1 = cond2, 
            // LEFT JOIN tbl1 ON cond1 = cond2
            // INNER OUTER JOIN tbl1 ON cond1 = cond2

            string[] conditionfield = new string[3];    // bisa lebih dari 3
            string[] conditionoperator = new string[3]; // bisa lebih dari 3
            string[] conditionvalue = new string[3];    // bisa lebih dari 3
            string[] orderby = new string[3];           // bisa lebih dari 3
            string[] additionalcondition = new string[2];// bisa lebih dari 2

            string sortby = "ASC"; // ASC = ascending, DESC = descending.
            string limit = "10";    // return record max 10
            string offset = "0";    // return record akan di return dari mulai offset/record ke 0 berdasrkan SORT data record.

            selectfield[0] = "*";   // atau bisa juga jadi gini : selectfield[0] = "id as id"; selectfield[1] = "device_id as device"; dan selanjutnya.

            conditionfield[0] = "course_id";    // isi ini harus sesuai yang ada di table tblname
            conditionfield[1] = "date(start_time)";   // isi ini harus sesuai yang ada di table tblname
            conditionfield[2] = "date(start_time)";   // isi ini harus sesuai yang ada di table tblname

            // opearot bisa berupa : LIKE , = , IN, < , <= , > , >= , !=, NOT IN, IS, IS NOT.
            conditionoperator[0] = "LIKE";
            conditionoperator[1] = ">=";
            conditionoperator[2] = "<=";

            conditionvalue[0] = "'%%EL-34%%'";      // % ngaruh ke awalan dan akiran, % di belakang akan nyari ke belakang yang mirip dan % di depan akan nyari ke arah depan yang mirip.
            conditionvalue[1] = "'2013-06-01'";     // kalau text/sting/varchar/tanggal pakai tanda kutip satu ' pada value nya
            conditionvalue[2] = "'2013-06-30'";     // kalau numeric jangan pakai tanda kutip satu ' pada value nya

            orderby[0] = "id";
            orderby[1] = "course_id";
            orderby[2] = "start_time";

            additionalcondition[0] = "device_id = '2'";
            additionalcondition[1] = "id NOT IN ('A15A06', 'A15A08', 'A15A10', 'A15A12')";

            return DbQuery(tblname,selectfield, conditionfield, conditionoperator, conditionvalue, additionalcondition, orderby, sortby, limit, offset);
        }
        
        public string TestDbGetOne()
        {
            // Fungsi test ini digunakan hanya untuk dapetin satu field dan satu value dari table tertentu.
            // QUERY EXAMPLE : SELECT status FROM attendance_course WHERE id = 'A15A06'; 
            // kalau kondisi seperti ini pasti hanya akan return satu field dan satu value saja. 

            string tblname = "attendance_course";
            string[] selectfield = new string[1];       // bisa lebih dari 1, kalau isinya di kasih * artinya semua field.

            // NOTE:: belum ada sample yang di join, sementara gk di impelemtasikan liat kebutuhan saja!!.
            // string[] joinedtable = new string[0];      
            // contoh join table : 
            // INNER JOIN tbl1 ON cond1 = cond2, 
            // LEFT JOIN tbl1 ON cond1 = cond2
            // INNER OUTER JOIN tbl1 ON cond1 = cond2

            string[] conditionfield = new string[1];    // bisa lebih dari 1
            string[] conditionoperator = new string[1]; // bisa lebih dari 1
            string[] conditionvalue = new string[1];    // bisa lebih dari 1            
            string[] additionalcondition = new string[0];// bisa lebih dari 0
            
            selectfield[0] = "status";   // atau bisa juga jadi gini : selectfield[0] = "id as id"; selectfield[1] = "device_id as device"; dan selanjutnya.
            conditionfield[0] = "id";    // isi ini harus sesuai yang ada di table tblname            
            // opearot bisa berupa : LIKE , = , IN, < , <= , > , >= , !=, NOT IN, IS, IS NOT.
            conditionoperator[0] = "=";

            conditionvalue[0] = "'A15A06'";         // yang hanya nilai itu saja.
            return DbGetOne(tblname, selectfield, conditionfield, conditionoperator, conditionvalue, additionalcondition);
        }

        // fungsi untuk isi row data pada table tertentu
        public int InsertDb(DataTable record)
        {
            string sql = "";
            string sqlheader = "INSERT INTO "+ record.TableName.ToString() +" (";
            string sqlval = "";
            bool failed = false;
            dbc.StartTrans();
            for (int i = 0; i < record.Columns.Count; i++)
            {
                if (i == (record.Columns.Count - 1))
                    sqlheader += record.Columns[i];
                else
                    sqlheader += record.Columns[i] + ",";
            }
            sqlheader += ") VALUES (";

            for (int i = 0; i < record.Rows.Count; i++)
            {
                sqlval = "";
                DataRow row = record.Rows[i];
                int j = 0;
                foreach(object item in row.ItemArray)
                {
                    if (item is int)
                    {
                        if (j == (row.ItemArray.Length - 1))
                            sqlval += "'" + item + "'";
                        else
                            sqlval += "'" + item + "',";                        
                    }
                    else if (item is string)
                    {
                        if (j == (row.ItemArray.Length - 1))
                            sqlval += "'" + item + "'";
                        else
                            sqlval += "'" + item + "',";
                    }
                    j++;
                }
                sqlval += ")";
                sql = sqlheader + sqlval;
                dbc.SetQuery(sql);
                Console.WriteLine("YOUR QUERY : ");
                Console.WriteLine(sql);                
                if (dbc.ExecuteQuery() == 0)
                {
                    // roolback trans
                    dbc.RollBackTrans();
                    failed = true;
                    break;
                }                
            }

            if (!failed)
            {
                // commit trans
                dbc.CommitTrans();
                return 1;         
            }
            else 
            {
                return 0;         
            }            
        }

        // fungsi untuk merubah row data pada table tertentu berdasarkan ID tertentu
        public int UpdateDb(DataTable record)
        {
            string sql = "";
            string sqlheader = "UPDATE " + record.TableName.ToString() + " SET ";
           
            bool failed = false;
           
            string condval = "";
            // start trans
            dbc.StartTrans();
            string sqlval = "";
            for (int i = 0; i < record.Rows.Count; i++)
            {
                sqlval = "";
                DataRow row = record.Rows[i];
                int j = 0;
                foreach (object item in row.ItemArray)
                {
                    string a = record.Columns[j].ToString();
                    string b = record.PrimaryKey.GetValue(0).ToString();
                    if (a == b)
                    {
                        if (item is int)
                            condval = item.ToString();
                        else if (item is string)
                            condval = "'" + item + "'";
                        j++;
                        continue;
                    }
                    else
                    {
                        sqlval += record.Columns[j] + "=";
                    }

                    if (item is int)
                    {
                        if (j == (row.ItemArray.Length - 1))
                            sqlval += "'" + item + "'";
                        else
                            sqlval += "'" + item + "',";
                    }
                    else if (item is string)
                    {
                        if (j == (row.ItemArray.Length - 1))
                            sqlval += "'" + item + "'";
                        else
                            sqlval += "'" + item + "',";
                    }
                    j++;
                }
                sqlval += " WHERE " + record.PrimaryKey.GetValue(0).ToString() + "=" + condval + ";";
                dbc.SetQuery(sqlheader + sqlval);
                Console.WriteLine("YOUR QUERY : ");
                Console.WriteLine(sqlval);
                
                if (dbc.ExecuteQuery() == 0)
                {
                    // roolback trans
                    dbc.RollBackTrans();
                    failed = true;
                    break;
                }
            }

            if (!failed)
            {
                // commit trans
                dbc.CommitTrans();
                return 1;
            }
            else
            {
                return 0;
            }
        }

        // fungsi untuk menghapus row data pada table tertentu berdasarkan ID tertentu
        public int DeleteDb(DataTable record)
        {
            string sql = "";
            string sqlheader = "DELETE FROM " + record.TableName.ToString();
            sqlheader += " WHERE " + record.PrimaryKey.GetValue(0).ToString() + "=";

            bool failed = false;

            string sqlval = "";
            // start trans
            dbc.StartTrans();

            for (int i = 0; i < record.Rows.Count; i++)
            {
                sqlval = "";
                DataRow row = record.Rows[i];
                int j = 0;

                foreach (object item in row.ItemArray)
                {
                    string a = record.Columns[j].ToString();
                    string b = record.PrimaryKey.GetValue(0).ToString();
                    if (a == b)
                    {
                        if (item is int)
                            sqlval += item.ToString();
                        else if (item is string)
                            sqlval += "'" + item + "'";                        
                    }
                    j++;
                }

                dbc.SetQuery(sqlheader + sqlval);
                Console.WriteLine("YOUR QUERY : {0} ", sqlheader + sqlval);
                
                if (dbc.ExecuteQuery() == 0)
                {
                    // roolback trans
                    dbc.RollBackTrans();
                    failed = true;
                    break;
                }
            }

            if (!failed)
            {
                // commit trans
                dbc.CommitTrans();
                return 1;
            }
            else
            {
                return 0;
            }
        }     

        // fungsi untuk baca row pada table tertentu
        public DataTable DbQuery(string tblname, string[] selectfield, string[] conditionfield, string[] conditionoperator, string[] conditionvalue, string[] additionalcondition, string[] orderby, string sortby, string limit, string offset)            
        {            
            // header query
            string sql = "SELECT ";

            if (selectfield.Length > 0)
            {
                for (int i = 0; i < selectfield.Length; i++)
                {
                    if (i == (selectfield.Length - 1))
                        sql += selectfield[i];
                    else
                        sql += selectfield[i] + ", ";
                }
            }
            else 
            {
                sql += " * ";
            }

            // from table what
            sql += " FROM " + tblname;

            // confition WHERE
            bool flagwhere = false;
            if (conditionfield.Length > 0 && conditionoperator.Length > 0 && conditionvalue.Length > 0)
            {
                if ((conditionfield.Length == conditionoperator.Length) && (conditionvalue.Length == conditionoperator.Length))
                {
                    sql += " WHERE ";
                    flagwhere = true;
                    for (int i = 0; i < conditionfield.Length; i++)
                    {
                        if (i == (conditionfield.Length - 1))
                            sql += conditionfield[i] + " " + conditionoperator[i] + " " + conditionvalue[i];
                        else
                            sql += conditionfield[i] + " " + conditionoperator[i] + " " + conditionvalue[i] + " AND ";
                    }
                }
            }

            // aditional condition
            if (additionalcondition.Length > 0)
            {
                if (!flagwhere)
                    sql += " WHERE ";
                else
                    sql += " AND ";

                for (int i = 0; i < additionalcondition.Length; i++)
                {
                    if (i == (additionalcondition.Length - 1))
                        sql += additionalcondition[i];
                    else
                        sql += additionalcondition[i] + " AND ";
                }                
            }

            // sorting data
            if (orderby.Length > 0)
            {
                sql += " ORDER BY ";
                for (int i = 0; i < orderby.Length; i++)
                {
                    if (i == (orderby.Length - 1))
                        sql += orderby[i];
                    else
                        sql += orderby[i] + ", ";
                }
            }
            sql += " " + sortby;

            // offset
            if (offset == "")
                sql += " OFFSET 0 ";
            else
                sql += " OFFSET " + offset + " ";

            // limit
            if (limit != "")
                sql += " LIMIT " + limit;
            
            dbc.SetQuery(sql);
            Console.WriteLine("YOUR QUERY : ");
            Console.WriteLine(sql);
            dbc.AddDataTbl(tblname);


            if (dbc.DbQuery(tblname) > 0) // ada data dan tidak terjadi error.
            {
                DataTable record = dbc.GetResultData(tblname);
                dbc.RemoveDataTbl(tblname);
                return record;
            }
            else
                return null;
            
        }

        // fungsi untuk isi 1 field tertentu pada table tertentu
        public string DbGetOne(string tblname, string[] selectfield, string[] conditionfield, string[] conditionoperator, string[] conditionvalue, string[]  additionalcondition)                      
        {
            // header query
            string sql = "SELECT ";

            if (selectfield.Length > 0)
            {
                for (int i = 0; i < selectfield.Length; i++)
                {
                    if (i == (selectfield.Length - 1))
                        sql += selectfield[i];
                    else
                        sql += selectfield[i] + ", ";
                }
            }
            else
            {
                sql += " * ";
            }

            // from table what
            sql += " FROM " + tblname;

            // confition WHERE
            bool flagwhere = false;
            if (conditionfield.Length > 0 && conditionoperator.Length > 0 && conditionvalue.Length > 0)
            {
                if ((conditionfield.Length == conditionoperator.Length) && (conditionvalue.Length == conditionoperator.Length))
                {
                    sql += " WHERE ";
                    flagwhere = true;
                    for (int i = 0; i < conditionfield.Length; i++)
                    {
                        if (i == (conditionfield.Length - 1))
                            sql += conditionfield[i] + " " + conditionoperator[i] + " " + conditionvalue[i];
                        else
                            sql += conditionfield[i] + " " + conditionoperator[i] + " " + conditionvalue[i] + " AND ";
                    }
                }
            }

            // aditional condition
            if (additionalcondition.Length > 0)
            {
                if (!flagwhere)
                    sql += " WHERE ";
                else
                    sql += " AND ";

                for (int i = 0; i < additionalcondition.Length; i++)
                {
                    if (i == (additionalcondition.Length - 1))
                        sql += additionalcondition[i];
                    else
                        sql += additionalcondition[i] + " AND ";
                }
            }            
            
            dbc.SetQuery(sql);
            Console.WriteLine("YOUR QUERY : ");
            Console.WriteLine(sql);
            dbc.AddDataTbl(tblname);

            if (dbc.DbQuery(tblname) > 0) // ada data dan tidak terjadi error.
            {
                DataTable record = dbc.GetResultData(tblname);
                dbc.RemoveDataTbl(tblname);
                DataRow row = record.Rows[0];
                string res = "";
                foreach (object item in row.ItemArray)
                {
                    res = item.ToString();                    
                }
                return res;                    
            }
            else
                return "";
        }

        public int DbFreeExecute(string tblname, string sql)
        {
            dbc.SetQuery(sql);
            Console.WriteLine("YOUR QUERY : ");
            Console.WriteLine(sql);
            return dbc.ExecuteQuery();
        }

        public DataTable DbFreeQuery(string tblname, string sql)
        {
            dbc.SetQuery(sql);
            Console.WriteLine("YOUR QUERY : ");
            Console.WriteLine(sql);
            dbc.AddDataTbl(tblname);

            if (dbc.DbQuery(tblname) > 0) // ada data dan tidak terjadi error.
            {
                DataTable record = dbc.GetResultData(tblname);
                dbc.RemoveDataTbl(tblname);
                return record;
            }
            else
                return null;
        }
    }
}
