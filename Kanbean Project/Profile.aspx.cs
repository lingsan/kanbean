using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;

namespace Kanbean_Project
{
    public partial class Profile : System.Web.UI.Page
    {
        OleDbConnection myConnection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|LanbanDatabase.mdb;");
        OleDbCommand mySelectCommand = new OleDbCommand();
        OleDbCommand myUpdateCommand = new OleDbCommand();
        OleDbCommand myDeleteCommand = new OleDbCommand();
        OleDbDataAdapter myAdapter = new OleDbDataAdapter();
        DataSet myDataSet = new DataSet();
        OleDbDataReader myReader;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["UserSettings"]["Name"] != null)
                    linkBtnUsername.Text = Request.Cookies["UserSettings"]["Name"];
                else
                    Response.Redirect("login.aspx");
            }
            else
                Response.Redirect("login.aspx");

            userProfile.Visible = true;
            projectManagement.Visible = false;
            accountManagement.Visible = false;

            tableSummary.Visible = true;
            tableEditProfile.Visible = false;
            tableChangePass.Visible = false;

            myConnection.Open();
            mySelectCommand.Connection = myConnection;
            mySelectCommand.CommandText = "SELECT [User].*, Projects.ProjectName FROM [User], Projects " +
                                        "WHERE [User].DefaultProjectID = Projects.ProjectID AND [User].[Username] = '" + linkBtnUsername.Text + "';";
            //string DefaultProject = mySelectCommand.ExecuteScalar().ToString();
            myReader = mySelectCommand.ExecuteReader();
            bool notEoF = myReader.Read();
            while (notEoF)
            {
                lblUsername.Text = myReader["Username"].ToString();
                lblEmail.Text = myReader["Email"].ToString();
                lblUserLevel.Text = myReader["Level"].ToString();
                lblDefaultProject.Text = myReader["ProjectName"].ToString();
                Session["userID"] = myReader["UserID"].ToString();
                notEoF = myReader.Read();
            }
            myReader.Close();
            mySelectCommand.CommandText = "SELECT ProjectsMembers.*, Projects.ProjectName FROM ProjectsMembers, Projects " +
                                        "WHERE ProjectsMembers.ProjectID = Projects.ProjectID AND ProjectsMembers.UserID = " + Session["userID"].ToString();
            myReader = mySelectCommand.ExecuteReader();
            notEoF = myReader.Read();
            while (notEoF)
            {
                lblParticipatedProjects.Text += myReader["ProjectName"].ToString() + ", ";
                notEoF = myReader.Read();
            }
            myReader.Close();

        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnBacktheBoard_Click(object sender, EventArgs e)
        {
            myConnection.Close();
            Response.Redirect("board.aspx");
        }

        protected void linkBtnUsername_Click(object sender, EventArgs e)
        {
            myConnection.Close();
            Response.Redirect("Profile.aspx");
        }

        protected void EatCookies(object sender, EventArgs e)
        {
            if (Request.Cookies["UserSettings"] != null)
            {
                Response.Cookies["UserSettings"].Expires = DateTime.Now.AddDays(-1);
                myConnection.Close();
                Response.Redirect("login.aspx");
            }
        }

        protected void btnUserProfile_Click(object sender, EventArgs e)
        {
            userProfile.Visible = true;
            projectManagement.Visible = false;
            accountManagement.Visible = false;

            tableSummary.Visible = true;
            tableEditProfile.Visible = false;
            tableChangePass.Visible = false;
        }

        protected void btnProjectManagement_Click(object sender, EventArgs e)
        {
            userProfile.Visible = false;
            projectManagement.Visible = true;
            accountManagement.Visible = false;

            tableCreateProject.Visible = true;
            tableAddMembers.Visible = false;
            tableRemoveMembers.Visible = false;
        }

        protected void btnUserManagement_Click(object sender, EventArgs e)
        {
            userProfile.Visible = false;
            projectManagement.Visible = false;
            accountManagement.Visible = true;

            tableCreateAccount.Visible = true;
            tableEditAccount.Visible = false;
        }

        protected void linkbtnSummary_Click(object sender, EventArgs e)
        {
            tableSummary.Visible = true;
            tableEditProfile.Visible = false;
            tableChangePass.Visible = false;
        }

        protected void linkbtnEditProfile_Click(object sender, EventArgs e)
        {
            tableSummary.Visible = false;
            tableEditProfile.Visible = true;
            tableChangePass.Visible = false;
        }

        protected void linkbtnChangePassword_Click(object sender, EventArgs e)
        {
            tableSummary.Visible = false;
            tableEditProfile.Visible = false;
            tableChangePass.Visible = true;
        }

        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {

        }

        protected void btnChangePass_Click(object sender, EventArgs e)
        {

        }

        protected void linkbtnCreateProject_Click(object sender, EventArgs e)
        {
            tableCreateProject.Visible = true;
            tableAddMembers.Visible = false;
            tableRemoveMembers.Visible = false;
        }

        protected void linkbtnAddMembers_Click(object sender, EventArgs e)
        {
            tableCreateProject.Visible = false;
            tableAddMembers.Visible = true;
            tableRemoveMembers.Visible = false;
        }

        protected void linkbtnRemoveMembers_Click(object sender, EventArgs e)
        {
            tableCreateProject.Visible = false;
            tableAddMembers.Visible = false;
            tableRemoveMembers.Visible = true;
        }

        protected void btnCreateProject_Click(object sender, EventArgs e)
        {

        }

        protected void btnAddMembers_Click(object sender, EventArgs e)
        {

        }

        protected void btnRemoveMembers_Click(object sender, EventArgs e)
        {

        }

        protected void linkbtnCreateAccount_Click(object sender, EventArgs e)
        {
            tableCreateAccount.Visible = true;
            tableEditAccount.Visible = false;
        }

        protected void linkbtnEditAccount_Click(object sender, EventArgs e)
        {
            tableCreateAccount.Visible = false;
            tableEditAccount.Visible = true;
        }

        protected void btnCreateAccount_Click(object sender, EventArgs e)
        {

        }

        protected void btnSaveEditAccount_Click(object sender, EventArgs e)
        {

        }
    }
}