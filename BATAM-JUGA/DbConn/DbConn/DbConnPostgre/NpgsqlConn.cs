using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using NpgsqlTypes;
namespace DbConnPostgre
{
    
    public class PostGresDB
    {
        #region IDisposable Members
        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if(!this.disposed)
            {
                if(disposing)
                {
                    // Dispose managed resources.
                    disposeAll();
                }

                disposed = true;
            }
        }

        ~PostGresDB()
        {
            Dispose(false);
        }

        #endregion

        private void disposeAll() 
        {
            this.MyDataSet.Clear();
            this.MyDataSet.Dispose();
            this.MyConnection.Dispose();
        }

        public NpgsqlConnection MyConnection = new NpgsqlConnection();
        public System.Data.DataSet MyDataSet = new System.Data.DataSet();

        public void ConnectionString(string host, string Database, string UserId, string Password)
        {            
            MyConnection.ConnectionString = "Server=" + host + ";Port=5432;Database=" + Database + ";User id=" + UserId + ";Password=" + Password + ";Timeout=15";
        }
        
        public bool ConnectionTest(ref Exception Excep)
        {
            try
            {
                MyConnection.Open();
                MyConnection.Close();
                return true;
            }
            catch (Exception Ex)
            {
                Excep = Ex;
                return false;
            }
        }

        public System.Data.DataSet GetDataSet 
        {
            get { return MyDataSet; }
            set { MyDataSet = value; }
        }

        public object GetDataItem(string DataTableName, int RowNumber, int ColumnNumber) 
        {
            return this.MyDataSet.Tables[DataTableName].Rows[RowNumber][ColumnNumber];
        }

        public object GetDataItem(string DataTableName, int RowNumber, string ColumnNumber) 
        {
            return this.MyDataSet.Tables[DataTableName].Rows[RowNumber][ColumnNumber];
        }

        public System.Data.DataTable AddDataTable(string DataTableName) 
        {
            return MyDataSet.Tables.Add(DataTableName);
        }

        public bool isTableAliasExists(string DataTableName)
        {
            if (MyDataSet.Tables[DataTableName] == null)
                return false;
            else
                return true;
        }

        public void RemoveDataTable(string DataTableName, ref  Exception Excep) 
        {
            try 
            {
                MyDataSet.Tables.Remove(DataTableName);
            }
            catch(Exception Ex) 
            {
                Excep = Ex;
            }
        }

        public void RemoveDataTable(System.Data.DataTable idDataTable, ref Exception Excep) 
        {
            try 
            {
                MyDataSet.Tables.Remove(idDataTable);
            }
            catch(Exception Ex)
            {
                Excep = Ex;
            }
        }

        public void ClearDataTable() 
        {
            MyDataSet.Tables.Clear();
        }
        
        public int ExecNonQuerySql(string NonQuery, ref Exception Excep)
        {
            NpgsqlCommand myCommand = new NpgsqlCommand(NonQuery, MyConnection);
            Int32 Hasil = 0;
            try
            {
                myCommand.Connection.Open();
            }
            catch (Exception Ex)
            {
                Excep = Ex;
                return 0;
            }
            try
            {
                Hasil = myCommand.ExecuteNonQuery();
            }
            catch (Exception Ex)
            {
                Excep = Ex;
                MyConnection.Close();
                return 0;
            }

            try
            {
                MyConnection.Close();
            }
            catch (Exception Ex)
            {
                Excep = Ex;
            }
            return Hasil;
        }

        public int ExecQuerySql(string Query, string DataTableName, ref Exception Excep)
        {
            NpgsqlCommand SQLCommand = new NpgsqlCommand(Query, MyConnection);
            NpgsqlDataAdapter AdapterSQL = new NpgsqlDataAdapter();
            try
            {
                SQLCommand.CommandTimeout = 30;
                AdapterSQL.SelectCommand = SQLCommand;
                MyDataSet.Tables[DataTableName].Clear();
                AdapterSQL.Fill(MyDataSet.Tables[DataTableName]);
                return MyDataSet.Tables[DataTableName].Rows.Count;
            }
            catch (Exception Ex)
            {
                Excep = Ex;
                return 0;
            }
            return 0;
        }
        
    }
}
