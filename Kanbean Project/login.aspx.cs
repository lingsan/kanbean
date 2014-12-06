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
        protected void Page_Load(object sender, EventArgs e)
        {
            //read cookie to check if User Loged on or not
            if ( Request.Cookies["UserSetting"] != null && Request.Cookies["UserSetting"]["Name"] != null)
            {
                Response.Redirect("Board.aspx");
            }

            Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|LanbanDatabase.mdb;";
            LogInConnection.ConnectionString = constr;
        }

        //Write a cookie for username
        private void BakeCookies ()
        {
            String Username = usernameTextBox.Text;
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
            //connect to DB
            LogInConnection.Open();
            string UserName = usernameTextBox.Text;
            OleDbCommand UserPassConn = new OleDbCommand("SELECT [Password] FROM [User] WHERE [Username]='" + UserName + "'", LogInConnection);
            UserPassConn.CommandType = CommandType.Text;
            /*OleDbDataReader CheclogInReader;
            CheclogInReader = UserPassConn.ExecuteReader();*/
            try {
                if (passwordTextBox.Text == UserPassConn.ExecuteScalar().ToString())
                {
                    args.IsValid = true;
                } else
                {
                    args.IsValid = false;
                } 
            } catch
            {
                args.IsValid = false;
            }
            LogInConnection.Close();
        }
    }
}