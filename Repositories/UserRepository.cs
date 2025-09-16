using System;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using ShoeStore.Security;
using ShoeStore.Data;

namespace ShoeStore.Repositories
{
    public class UserRepository
    {
        public long CreateUser(string email, string fullName, string phone, string password, string createdBy = null)
        {
            var passwordHash = PasswordHasher.Hash(password);
            using (var conn = OracleDb.GetOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO APP_USER(EMAIL, PASSWORD_HASH, FULL_NAME, PHONE, STATUS, CREATED_AT, CREATED_BY)
                                    VALUES(:p_email, :p_hash, :p_fullname, :p_phone, 'ACTIVE', SYSTIMESTAMP, :p_createdBy)
                                    RETURNING USER_ID INTO :p_id";
                cmd.Parameters.Add(OracleDb.Param(":p_email", email, OracleDbType.Varchar2));
                cmd.Parameters.Add(OracleDb.Param(":p_hash", passwordHash, OracleDbType.Varchar2));
                cmd.Parameters.Add(OracleDb.Param(":p_fullname", fullName, OracleDbType.Varchar2));
                cmd.Parameters.Add(OracleDb.Param(":p_phone", phone, OracleDbType.Varchar2));
                cmd.Parameters.Add(OracleDb.Param(":p_createdBy", createdBy, OracleDbType.Varchar2));
                var idParam = new OracleParameter(":p_id", OracleDbType.Int64) { Direction = System.Data.ParameterDirection.Output };
                cmd.Parameters.Add(idParam);
                cmd.ExecuteNonQuery();
                return Convert.ToInt64(idParam.Value.ToString());
            }
        }

        public (long userId, string passwordHash, string fullName) GetByEmail(string email)
        {
            using (var conn = OracleDb.GetOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT USER_ID, PASSWORD_HASH, FULL_NAME FROM APP_USER WHERE EMAIL = :p_email AND STATUS = 'ACTIVE'";
                cmd.Parameters.Add(OracleDb.Param(":p_email", email, OracleDbType.Varchar2));
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (reader.GetInt64(0), reader.GetString(1), reader.GetString(2));
                    }
                }
            }
            return (0, null, null);
        }
    }
}
