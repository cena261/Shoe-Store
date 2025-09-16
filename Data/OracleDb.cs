using System;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace ShoeStore.Data
{
    public static class OracleDb
    {
        private static string ConnStr => ConfigurationManager.ConnectionStrings["OracleConn"].ConnectionString;

        public static OracleConnection GetOpenConnection()
        {
            var conn = new OracleConnection(ConnStr);
            conn.Open();
            return conn;
        }

        public static OracleParameter Param(string name, object value, OracleDbType dbType)
        {
            var p = new OracleParameter(name, dbType);
            p.Value = value ?? DBNull.Value;
            return p;
        }
    }
}
