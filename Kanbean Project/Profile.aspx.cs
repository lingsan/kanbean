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
        private void getDB()
        {
            myConnection.Open();
            mySelectCommand.Connection = myConnection;
            mySelectCommand.CommandType = CommandType.Text;
            myAdapter.SelectCommand = mySelectCommand;
            mySelectCommand.CommandText = "SELECT Projects.ProjectID, Projects.ProjectName, User.* "
                                        + "FROM [User], Projects "
                                        + "WHERE (((User.UserID)=" + Request.QueryString["UserID"] + ") AND "
                                        +   "((Projects.ProjectID)=[User].[DefaultProjectID]));";
            myAdapter.Fill(myDataSet, "User");
            mySelectCommand.CommandText = "SELECT Tasks.TaskTitle, Tasks.TaskComplexity, Tasks.TaskStatusID, Tasks.TaskEstimationHour, "
                                        +   "Tasks.TaskSpentTime, Tasks.TaskDueDate, Projects.ProjectID, Tasks.TaskID, Projects.ProjectName "
                                        + "FROM [User], Projects, ProjectsMembers, Backlogs, Tasks "
                                        + "WHERE (((Tasks.BacklogID)=[Backlogs].[BacklogID]) AND "
                                        +   "((Projects.ProjectID)=[Backlogs].[ProjectID]) AND "
                                        +   "((ProjectsMembers.UserID)=[User].[UserID]) AND "
                                        +   "((ProjectsMembers.ProjectID)=[Projects].[ProjectID]) AND "
                                        +   "((User.UserID)=" + Request.QueryString["UserID"] + ") AND "
                                        +   "((User.UserID)=[Tasks].[TaskAssigneeID])) "
                                        + "ORDER BY Tasks.TaskID;";
            myAdapter.Fill(myDataSet, "Task");
            mySelectCommand.CommandText = "SELECT User.UserID, Projects.* "
                                        + "FROM [User], Projects, ProjectsMembers "
                                        + "WHERE (((User.UserID)=" + Request.QueryString["UserID"] + ") AND "
                                        +   "((ProjectsMembers.ProjectID)=[Projects].[ProjectID]) AND "
                                        +   "((ProjectsMembers.UserID)=[user].[UserID])) "
                                        + "ORDER BY ProjectsMembers.ProjectID;";
            myAdapter.Fill(myDataSet, "Projects");
            mySelectCommand.CommandText = "SELECT * FROM Projects ORDER BY ProjectID;";
            myAdapter.Fill(myDataSet, "RawProjects");
            myConnection.Close();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //reading cookies
            string Username = "";
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["UserSettings"]["Name"] != null)
                {
                    Username = Request.Cookies["UserSettings"]["Name"];
                    linkBtnUsername.Text = Username;
                }
                else Response.Redirect("login.aspx");
            }
            else Response.Redirect("login.aspx");
            FillContent();
            //fill project drop down list
            foreach (DataRow row in myDataSet.Tables["RawProjects"].Rows)
            {
                projectDropDownList.Items.Add(row["ProjectName"].ToString());
                projectDropDownList.Items[projectDropDownList.Items.Count - 1].Value = row["ProjectID"].ToString();
            }
        }

        private void FillContent()
        {
            //get database base on UserID
            getDB();
            //fill content
            foreach (DataRow row in myDataSet.Tables["User"].Rows)
            {
                TxtUsername.Text = row["Username"].ToString(); TxtUsername.Enabled = false;
                string password = row["Password"].ToString();
                foreach (char c in password)
                {
                    if (password.IndexOf(c) != 0 && password.IndexOf(c) != (password.Length - 1))
                    {
                        char[] Char = password.ToCharArray();
                        Char[password.IndexOf(c)] = '*';
                        password = new string(Char);
                    }
                } TxtPassword.Text = password; TxtPassword.Enabled = false;

                if (row["Email"].ToString() == "" || row["Email"].ToString() == null)
                {
                    TxtEmail.Text = "Unknown";
                }
                else
                {
                    TxtEmail.Text = row["Email"].ToString();
                } TxtEmail.Enabled = false;

                if (row["Level"].ToString() == "1") { TextLevel.Text = "admin"; }
                else { TextLevel.Text = "User"; } TextLevel.Enabled = false;

                TextDefaultProject.Text = row["ProjectName"].ToString(); TextDefaultProject.Enabled = false;

                foreach (DataRow row1 in myDataSet.Tables["Projects"].Rows)
                {
                    DropProjects.Items.Add(row1["ProjectName"].ToString());
                    DropProjects.Items[DropProjects.Items.Count - 1].Value = row1["ProjectID"].ToString();
                }
            }
        }
        protected void linkBtnUsername_Click(object sender, EventArgs e)
        {
            myConnection.Close();
            Response.Redirect("Profile.aspx?userID=" + Session["userID"].ToString());
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


        protected void projectDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            myConnection.Close();
            Response.Redirect("board.aspx?projectID=" + projectDropDownList.SelectedValue);
        }

        protected void SetDefaulProject(object sender, EventArgs e)
        {

        }
    }
}