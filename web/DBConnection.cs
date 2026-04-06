using System.Data;
using Microsoft.Data.SqlClient; 

namespace SipernaWeb
{
    public class DBConnection
    {
        private readonly string _connectionString = "Server=DESKTOP-VEBT1KK;Database=SipernaDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public DataSet getSelect(string sqlstr)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(sqlstr, con);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
        }

        public bool execute(string sqlstr)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(sqlstr, con);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}