using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PushUsersTask
{
    // Class Name   : DbCommon
    // Creator      : Iwan Supratman (deong)
    // Create Date  : June, 13 2013
    // Mail         : deong84@gmail.com
    // Globat Matra Aditama, PT.

    class DbCommon
    {
        private string dbhost = "";
        private string dbname = "";
        private string dbuser = "";
        private string dbpasswd = "";
        private string dbSql = "";
        private DbConnPostgre.PostGresDB dbposgres;
        private Exception dbex;

        public DbCommon(string _dbhost, string _dbname, string _dbuser, string _dbpasswd) 
        {
            dbhost = _dbhost;
            dbname = _dbname;
            dbuser = _dbuser;
            dbpasswd = _dbpasswd;    
        }

        public int ConnectDb()
        {
            try
            {
                dbposgres = new DbConnPostgre.PostGresDB();
                dbex = new Exception();
                dbposgres.ConnectionString(dbhost, dbname, dbuser, dbpasswd);
                return 1;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
            return 0;
        }

        public int DisconnectDb()
        {
            try
            {
                dbposgres.Dispose();
                return 1;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0;
        }

        public void SetQuery(string sql)
        {
            dbSql = sql;            
        }

        public int StartTrans()
        {
            return dbposgres.ExecNonQuerySql("BEGIN;", ref dbex);
        }

        public int CommitTrans()
        {
            return dbposgres.ExecNonQuerySql("COMMIT;", ref dbex);
        }

        public int RollBackTrans()
        {
            return dbposgres.ExecNonQuerySql("ROLLBACK;", ref dbex);
        }

        public int ExecuteQuery()
        {
            return dbposgres.ExecNonQuerySql(dbSql, ref dbex);
        }

        public void AddDataTbl(string dbtable)
        {
            dbposgres.AddDataTable(dbtable);
        }

        public void RemoveDataTbl(string dbtable)
        {
            dbposgres.RemoveDataTable(dbtable, ref dbex);            
        }

        public DataTable GetResultData(string dbtable)
        {            
            //DataTable record = new DataTable(dbtable);
            //if(dbposgres.isTableAliasExists(dbtable))
            //{
                //for (int i = 0; i < dbposgres.MyDataSet.Tables[dbtable].Columns; i++)
                //{
                //record.Columns.Add("id", typeof(string));
                //}

                //for (int i = 0; i < dbposgres.MyDataSet.Tables[dbtable].Rows.Count; i++ )
                //{                    

                //}
            //}
            return dbposgres.MyDataSet.Tables[dbtable];
        }

        public int DbQuery(string dbtable)
        {
            return dbposgres.ExecQuerySql(dbSql, dbtable, ref dbex);                    
        }

        //public int Get()
        //{
        //}
    }
}
