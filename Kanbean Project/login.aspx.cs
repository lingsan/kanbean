﻿using System;
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
            Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            string path = @"\App_Data";
            string constr = "Provider=Microsoft.Jet.OleDB.4.0 " +
                "Data Source = " + path + @"\LanbanDatabase.mdb";
            LogInConnection.ConnectionString = constr;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("board.aspx");
        }

        protected void LoginValidation(object source, ServerValidateEventArgs args)
        {
            //connect to DB
            OleDbCommand UserPassConn = new OleDbCommand("SELECT Username, Password FROM User", LogInConnection);
            UserPassConn.CommandType = CommandType.Text;
            //insert username and password in to string lists
            List<string> Username = new List<string>();
            List<string> PassWord = new List<string>();
            OleDbDataReader CheckLoginReader;
            CheckLoginReader = UserPassConn.ExecuteReader();
            bool notEoF = CheckLoginReader.Read();
            while (notEoF)
            {
                Username.Add(CheckLoginReader["Username"].ToString());
                PassWord.Add(CheckLoginReader["Password"].ToString());
                notEoF = CheckLoginReader.Read();
            }
            //check matching between username and password
            string InputUser = usernameTextBox.Text;
            string InputPass = passwordTextBox.Text;
            int UserIndex = Username.IndexOf(InputUser);
            if (InputPass == PassWord[UserIndex])
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
        }
    }
}