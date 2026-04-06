using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
// 
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace CSE355BYS
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conStr"].ToString();

            SqlConnection con = new SqlConnection(connectionString);

            try
            {
                con.Open();
            }
            catch (Exception)
            {
                con.Close();
                return;
                throw;
            }


            DataSet ds = new DataSet();
            string sqlstr = "select * from student where studentID=" +TextBox1.Text;

            SqlDataAdapter da = new SqlDataAdapter(sqlstr, con);
            da.Fill(ds);

            string fName = ds.Tables[0].Rows[0]["fName"].ToString();
            string lName = ds.Tables[0].Rows[0]["lName"].ToString();
            string studentID = ds.Tables[0].Rows[0]["studentID"].ToString();
            con.Close();

            Session["fName"] = fName;
            Session["lName"] = lName;
            Session["studentID"] = studentID;

            Response.Redirect("student.aspx");
        }
    }
}