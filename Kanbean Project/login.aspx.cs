using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Data.OleDb;

namespace Kanbean_Project
{
    public partial class login : System.Web.UI.Page
    {
        OleDbConnection LogInConnection = new OleDbConnection();

        protected void Page_Init(object sender, EventArgs e)
        {
            Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            LogInConnection.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|LanbanDatabase.mdb;";

            //read cookie to check if User Loged on or not
            if (Request.Cookies["UserSettings"] != null && Request.Cookies["UserSettings"]["Name"] != null)
                Response.Redirect("board.aspx");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        //Write a cookie for username
        private void BakeCookies ()
        {
            String Username = usernameTextBox.Text;
            /*HttpCookie UserCookie = new HttpCookie("UserSetting");
            UserCookie["Name"] = Username;
            //UserCookie.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(UserCookie);*/
            Response.Cookies["UserSettings"]["Name"] = Username;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (this.IsValid)
            {
                BakeCookies();
                Response.Redirect("board.aspx"); 
            }
        }

        protected void LoginValidation(object source, ServerValidateEventArgs args)
        {
            LogInConnection.Open();
            OleDbCommand UserPassConn = new OleDbCommand("SELECT [Password] FROM [User] WHERE [Username]='" + usernameTextBox.Text + "'", LogInConnection);
            UserPassConn.CommandType = CommandType.Text;

            try
            {
                if (passwordTextBox.Text == UserPassConn.ExecuteScalar().ToString())
                    args.IsValid = true;
                else
                    args.IsValid = false;
            }
            catch
            {
                args.IsValid = false;
            }
            LogInConnection.Close();
        }
    }
}