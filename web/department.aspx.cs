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
    public partial class department : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack==false)
            {
                string sqlstr;
                sqlstr = "select Staff.staffID as id,fName+ ' ' +lName as name" +
                " from STAFF, MANAGER" +
                " where STAFF.staffID = MANAGER.staffID";

                DBConnection dbcon = new DBConnection();
                DataSet ds = new DataSet();
                ds = dbcon.getSelect(sqlstr);

                DropDownList1.DataTextField = ds.Tables[0].Columns["name"].ToString();
                DropDownList1.DataValueField = ds.Tables[0].Columns["id"].ToString();

                DropDownList1.DataSource = ds.Tables[0];
                DropDownList1.DataBind();

                DropDownList1.Items.Insert(0, new ListItem("Please Select", String.Empty));
                DropDownList1.SelectedIndex = 0;

            }


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
            string sqlstr = "select * from DEPARTMENT order by dName";

            SqlDataAdapter da = new SqlDataAdapter(sqlstr, con);
            da.Fill(ds);

            GridView1.DataSource = ds;
            GridView1.DataBind();
            con.Close();

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            int success = 0;
            string sqlstr = "UPDATE DEPARTMENT SET dName='" + TextBox2.Text +"'";
            sqlstr = sqlstr + " where deptCode='" + TextBox1.Text+"'";

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

            SqlCommand exec = new SqlCommand(sqlstr, con);

            
            try
            {
            exec.ExecuteNonQuery();
                success = 1;
            }
            catch (Exception)
            {
                throw;
            }
            con.Close();

            if (success==1)
            {
                Response.Write("<script LANGUAGE='JavaScript'> alert('Successfully Updated')  </script>");
            }
            else
            {
                Response.Write("<script LANGUAGE='JavaScript'> alert('Error')  </script>");
            }

        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conStr"].ToString();

            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("insertDepartment", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@deptCode", SqlDbType.VarChar).Value=TextBox1.Text;
            cmd.Parameters.Add("@dName", SqlDbType.VarChar).Value = TextBox2.Text;
            // cmd.Parameters.Add("@managerID", SqlDbType.SmallInt).Value = TextBox3.Text;
           
            cmd.Parameters.Add("@managerID", SqlDbType.SmallInt).Value =  DropDownList1.SelectedItem.Value;

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();


        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conStr"].ToString();

            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("deleteDepartment", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@deptCode", SqlDbType.VarChar).Value = TextBox1.Text;
            cmd.Parameters.Add(new SqlParameter( "@result", SqlDbType.Int));
            cmd.Parameters["@result"].Direction = ParameterDirection.Output;

            con.Open();
                cmd.ExecuteNonQuery();
            int result = (int)cmd.Parameters["@result"].Value;

            con.Close();

            if (result==1)
            {
                Response.Write("<script LANGUAGE='JavaScript'> alert('Successfully Deleted')  </script>");
            }
            else
            {
                Response.Write("<script LANGUAGE='JavaScript'> alert('Corresponding Department code does not exit')  </script>");

            }


        }
    }
}