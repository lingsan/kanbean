﻿using System;
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

        private void GetDB(string _UserID)
        {
            myConnection.Open();
            myAdapter.SelectCommand = mySelectCommand;
            mySelectCommand.Connection = myConnection;
            mySelectCommand.CommandType = CommandType.Text;
            myDataSet.Clear();
            mySelectCommand.CommandText = "SELECT [User].*, Projects.ProjectName FROM [User], Projects " 
                                        + "WHERE [User].DefaultProjectID = Projects.ProjectID AND "
                                        +   "[User].[Username] = '" + _UserID + "';";
            myAdapter.Fill(myDataSet, "User");
            mySelectCommand.CommandText = "SELECT ProjectsMembers.ProjectID, ProjectsMembers.UserID, Projects.ProjectName, User.Username "
                                        + "FROM ProjectsMembers, Projects, [User] "
                                        + "WHERE (((ProjectsMembers.ProjectID)=[Projects].[ProjectID]) AND "
                                        +   "((ProjectsMembers.UserID)=[User].[UserID])) AND "
                                        + "[User].[Username] = '" + _UserID + "';";
            myAdapter.Fill(myDataSet, "ProjectsMembers");
            myConnection.Close();
        }


        private void GetDB()
        {
            myConnection.Open();
            myAdapter.SelectCommand = mySelectCommand;
            mySelectCommand.Connection = myConnection;
            mySelectCommand.CommandType = CommandType.Text;
            myDataSet.Clear();
            mySelectCommand.CommandText = "SELECT * From [User];";
            myAdapter.Fill(myDataSet, "User");
            mySelectCommand.CommandText = "SELECT ProjectsMembers.ProjectID,Projects.ProjectName, ProjectsMembers.UserID,  User.Username "
                                        + "FROM ProjectsMembers, Projects, [User] "
                                        + "WHERE (((ProjectsMembers.ProjectID)=[Projects].[ProjectID]) AND "
                                        + "((ProjectsMembers.UserID)=[User].[UserID]));";
            myAdapter.Fill(myDataSet, "ProjectsMembers");
            myAdapter.SelectCommand.CommandText = "SELECT * FROM Projects;";
            myAdapter.Fill(myDataSet, "Projects");
            myConnection.Close();
        }

        private void FillContent()
        {
            lblUsername.Text = myDataSet.Tables["User"].Rows[0]["Username"].ToString();
            lblEmail.Text = myDataSet.Tables["User"].Rows[0]["Email"].ToString();
            if (myDataSet.Tables["User"].Rows[0]["Level"].ToString() == "1"){
                lblUserLevel.Text = "admin";
            }
            else { lblUserLevel.Text = "User"; }
            lblDefaultProject.Text = myDataSet.Tables["User"].Rows[0]["ProjectName"].ToString();
            lblParticipatedProjects.Text = "";
            foreach (DataRow row in myDataSet.Tables["ProjectsMembers"].Rows)
            {
                lblParticipatedProjects.Text += row["ProjectName"].ToString() + ", ";
            }
        }



        private void Update(string table, string updatecol, string updateval, string where, string filter)
        {
            myConnection.Open();
            if (IsString(where)) { filter = "'" + filter + "'"; }
            string updatestring = "UPDATE " + table + " SET " + updatecol + " = '" + updateval + "' WHERE " + where + " = " + filter + ";";
            myUpdateCommand.Connection = myConnection;
            myUpdateCommand.CommandType = CommandType.Text;
            myUpdateCommand.CommandText = updatestring;
            myUpdateCommand.ExecuteNonQuery();
            myConnection.Close();
        }

        private bool IsString(string _ColName)
        {
            bool IsString;
            IsString = _ColName == "Username" || _ColName == "Password" || _ColName == "ProjectName" || _ColName == "Email" 
                || _ColName == "[Username]" || _ColName == "[Password]" || _ColName == "[ProjectName]" || _ColName == "[Email]";
            return IsString;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            //reading cookies
            string Username = "";
            if (Request.Cookies["UserSettings"] != null) {
                if (Request.Cookies["UserSettings"]["Name"] != null) {
                    Username = Request.Cookies["UserSettings"]["Name"];
                    linkBtnUsername.Text = Username;
                } else Response.Redirect("login.aspx");
            } else Response.Redirect("login.aspx");

            userProfile.Visible = true;
            projectManagement.Visible = false;
            accountManagement.Visible = false;

            tableSummary.Visible = true;
            tableEditProfile.Visible = false;
            tableChangePass.Visible = false;

            //myConnection.Open();
            GetDB(Session["username"].ToString());
            if (myDataSet.Tables["User"].Rows[0]["Level"].ToString() != "1")
            {
                projectManagement.Enabled = btnProjectManagement.Visible = btnProjectManagement.Enabled = false;
                accountManagement.Enabled = btnUserManagement.Visible = btnUserManagement.Enabled = false;
            }
            FillContent();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnBacktheBoard_Click(object sender, EventArgs e)
        {
            //myConnection.Close();
            Response.Redirect("board.aspx");
        }

        protected void linkBtnUsername_Click(object sender, EventArgs e)
        {
            //myConnection.Close();
            Response.Redirect("Profile.aspx?userID=" + Session["userID"].ToString());
        }

        protected void EatCookies(object sender, EventArgs e)
        {
            if (Request.Cookies["UserSettings"] != null)
            {
                Response.Cookies["UserSettings"].Expires = DateTime.Now.AddDays(-1);
                //myConnection.Close();
                Response.Redirect("login.aspx");
            }
        }

        protected void btnUserProfile_Click(object sender, EventArgs e)
        {
            userProfile.Visible = true;
            projectManagement.Visible = false;
            accountManagement.Visible = false;

            backSummary();
        }

        private void backSummary()
        {
            //back to summary
            tableSummary.Visible = true;
            tableEditProfile.Visible = false;
            tableChangePass.Visible = false;
            GetDB(Session["username"].ToString());
            FillContent();
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
            GetDB();
        }

        protected void linkbtnSummary_Click(object sender, EventArgs e)
        {
            tableSummary.Visible = true;
            tableEditProfile.Visible = false;
            tableChangePass.Visible = false;
            GetDB(Session["username"].ToString());
            FillContent();
        }

        protected void linkbtnEditProfile_Click(object sender, EventArgs e)
        {
            FillProject(defaultProjectDropDownList, myDataSet.Tables["ProjectsMembers"]);
            tableSummary.Visible = false;
            tableEditProfile.Visible = true;
            tableChangePass.Visible = false;

        }

        private void FillProject(DropDownList _Project,DataTable _projectTable)
        {

            //fill projectlist
            _Project.Items.Clear();
            _Project.Items.Add("Choose a project!");
            _Project.Items[0].Value = "0";
            foreach (DataRow row in _projectTable.Rows)
            {
                _Project.Items.Add(row["ProjectName"].ToString());
                _Project.Items[_Project.Items.Count - 1].Value = row["ProjectID"].ToString();
            }
        }

        protected void linkbtnChangePassword_Click(object sender, EventArgs e)
        {
            tableSummary.Visible = false;
            tableEditProfile.Visible = false;
            tableChangePass.Visible = true;
        }

        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            //update Default Project
            if (defaultProjectDropDownList.SelectedValue != "0") {
                Update("[User]", "DefaultProjectID",defaultProjectDropDownList.SelectedValue.ToString(), "[Username]", Session["username"].ToString());
            }
            //up date email
            Update("[User]", "Email", emailTextbox.Text, "[Username]", Session["username"].ToString());
            backSummary();
        }

        protected void btnChangePass_Click(object sender, EventArgs e)
        {
            if (myDataSet.Tables["User"].Rows[0]["Password"].ToString() == oldPassTextBox.Text) {
                if (newPassTextBox.Text == reenterNewPassTextBox.Text) {
                    Update("[User]", "[Password]", newPassTextBox.Text, "[UserID]", myDataSet.Tables["User"].Rows[0]["UserID"].ToString());
                    oldPassTextBox.Text = newPassTextBox.Text = reenterNewPassTextBox.Text = "";
                }
            }
            backSummary();
        }

        protected void linkbtnCreateProject_Click(object sender, EventArgs e)
        {
            tableCreateProject.Visible = true;
            tableAddMembers.Visible = false;
            tableRemoveMembers.Visible = false;
            GetDB();
        }

        protected void linkbtnAddMembers_Click(object sender, EventArgs e)
        {
            GetDB();
            FillProject(AddMembersProjectDropDownList, myDataSet.Tables["Projects"]);
            tableCreateProject.Visible = false;
            tableAddMembers.Visible = true;
            tableRemoveMembers.Visible = false;
        }

        protected void linkbtnRemoveMembers_Click(object sender, EventArgs e)
        {
            GetDB();
            FillProject(RemoveMembersProjectDropDownList,myDataSet.Tables["Projects"]);
            tableCreateProject.Visible = false;
            tableAddMembers.Visible = false;
            tableRemoveMembers.Visible = true;
        }

        protected void btnCreateProject_Click(object sender, EventArgs e)
        {
            myConnection.Open();
            myUpdateCommand.Connection = myConnection;
            myUpdateCommand.CommandType = CommandType.Text;
            myAdapter.SelectCommand.CommandText = "SELECT * FROM Projects;";
            myAdapter.Fill(myDataSet, "Project");
            DataRow Inrow = myDataSet.Tables["Project"].NewRow();
            Inrow["ProjectName"] = newProjectNameTextBox.Text;
            myDataSet.Tables["Project"].Rows.Add(Inrow);
            OleDbCommandBuilder InSert = new OleDbCommandBuilder(myAdapter);
            myUpdateCommand = InSert.GetInsertCommand(true);
            myAdapter.InsertCommand = myUpdateCommand;
            myAdapter.Update(myDataSet, "Projects");
            myConnection.Close();
        }

        protected void InnerMem(object sender, EventArgs e)
        {
            GetDB();
            DataTable MemList = new DataTable("InnerMem");
            RemoveProjectMembersListBox.Items.Clear();
            foreach(DataRow row in myDataSet.Tables["ProjectsMembers"].Rows){
                if (row["ProjectID"].ToString() == RemoveMembersProjectDropDownList.SelectedValue.ToString())
                {
                    RemoveProjectMembersListBox.Items.Add(row["Username"].ToString());
                    RemoveProjectMembersListBox.Items[RemoveProjectMembersListBox.Items.Count - 1].Value = row["UserID"].ToString();
                }
            }
        }
        protected void OuterMem(object sender, EventArgs e)
        {
            GetDB();
            DataTable MemList = new DataTable("InnerMem");
            AddProjectMembersListBox.Items.Clear();
            foreach(DataRow row in myDataSet.Tables["ProjectsMembers"].Rows){
                if (row["ProjectID"].ToString() == AddMembersProjectDropDownList.SelectedValue.ToString())
                {
                    foreach (DataRow r1 in myDataSet.Tables["User"].Rows)
                    {
                        if (r1["UserID"].ToString() == row["UserID"].ToString()) { r1["UserID"] = "-1"; }
                    }
                }
            }
            foreach (DataRow row in myDataSet.Tables["User"].Rows)
            {
                if (row["UserID"].ToString() != "-1")
                {
                    AddProjectMembersListBox.Items.Add(row["Username"].ToString());
                    AddProjectMembersListBox.Items[AddProjectMembersListBox.Items.Count - 1].Value = row["UserID"].ToString();
                }
            }
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