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
        OleDbCommand myInsertCommand = new OleDbCommand();
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
                                        +   "[User].[UserID] = " + _UserID + ";";
            myAdapter.Fill(myDataSet, "User");
            mySelectCommand.CommandText = "SELECT ProjectsMembers.ProjectID, ProjectsMembers.UserID, Projects.ProjectName, User.Username "
                                        + "FROM ProjectsMembers, Projects, [User] "
                                        + "WHERE (((ProjectsMembers.ProjectID)=[Projects].[ProjectID]) AND "
                                        +   "((ProjectsMembers.UserID)=[User].[UserID])) AND "
                                        + "[User].[UserID] = " + _UserID + ";";
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
            GetDB(Request.QueryString["UserID"].ToString());
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
            GetDB(Request.QueryString["UserID"].ToString());
            FillContent();
        }

        protected void btnProjectManagement_Click(object sender, EventArgs e)
        {
            GetDB();
            userProfile.Visible = false;
            projectManagement.Visible = true;
            accountManagement.Visible = false;

            tableCreateProject.Visible = true;
            tableAddMembers.Visible = false;
            tableRemoveMembers.Visible = false;

            newProjectMembersListBox.DataSource = myDataSet.Tables["User"];
            newProjectMembersListBox.DataTextField = "Username";
            newProjectMembersListBox.DataValueField = "UserID";
            newProjectMembersListBox.DataBind();
        }

        protected void btnUserManagement_Click(object sender, EventArgs e)
        {
            userProfile.Visible = false;
            projectManagement.Visible = false;
            accountManagement.Visible = true;

            tableCreateAccount.Visible = true;
            tableEditAccount.Visible = false;
            GetDB();
            FillProject(newAccountDefaultProjectDropDownList, myDataSet.Tables["Projects"]);
            newAccountUserLevelDropDownList.Items.Add("Choose User Level");
            newAccountUserLevelDropDownList.Items.Add("1");
            newAccountUserLevelDropDownList.Items.Add("2");
        }

        protected void linkbtnSummary_Click(object sender, EventArgs e)
        {
            tableSummary.Visible = true;
            tableEditProfile.Visible = false;
            tableChangePass.Visible = false;
            GetDB(Request.QueryString["UserID"].ToString());
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
        private void FillUser(DropDownList _Account, DataTable _projectTable)
        {

            //fill projectlist
            _Account.Items.Clear();
            _Account.Items.Add("Choose a User Account!");
            _Account.Items[0].Value = "0";
            foreach (DataRow row in _projectTable.Rows)
            {
                _Account.Items.Add(row["Username"].ToString());
                _Account.Items[_Account.Items.Count - 1].Value = row["Username"].ToString();
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
                Update("[User]", "DefaultProjectID",defaultProjectDropDownList.SelectedValue.ToString(), "[UserID]", Request.QueryString["UserID"].ToString());
            }
            //up date email
            Update("[User]", "Email", emailTextbox.Text, "[UserID]", Request.QueryString["UserID"].ToString());
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
            GetDB();
            myConnection.Open();
            myInsertCommand.Connection = myConnection;
            myInsertCommand.CommandType = CommandType.Text;
            bool valid = true;
            foreach (DataRow r in myDataSet.Tables["Projects"].Rows)
            {
                if (r["ProjectName"].ToString() == newProjectNameTextBox.Text) { valid = false; }
            }

            if (valid && newProjectNameTextBox.Text != "the entered project nam has already exist!" && newProjectNameTextBox.Text!="")
            {   //Error with the command
                myInsertCommand.CommandText = "INSERT INTO Projects (ProjectName) "
                                                + "VALUES (@ProjectName)";
                myInsertCommand.Parameters.AddWithValue("@ProjectName", newProjectNameTextBox.Text);
                myInsertCommand.ExecuteNonQuery();
                mySelectCommand.CommandText = "SELECT ProjectID FROM Projects Where ProjectName = '" + newProjectNameTextBox.Text + "';";
                myReader = mySelectCommand.ExecuteReader();
                string ProjectID = "";
                bool notEoF;
                //read first row from database
                notEoF = myReader.Read();
                //read row by row until the last row
                while (notEoF)
                {
                    ProjectID = myReader["ProjectID"].ToString();
                    //read next row
                    notEoF = myReader.Read();
                }
                foreach (ListItem li in newProjectMembersListBox.Items)
                {
                    if (li.Selected)
                    {
                        myUpdateCommand.CommandText = "INSERT INTO ProjectsMembers(ProjectID,UserID) VALUE (" + ProjectID + "," + li.Value.ToString() + ");";

                    }
                }
            }
            else
            {
                newProjectNameTextBox.Text = "the entered project nam has already exist!";
            }
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
            myConnection.Open();
            myInsertCommand.Connection = myConnection;
            myInsertCommand.CommandType = CommandType.Text;
            foreach (ListItem li in AddProjectMembersListBox.Items)
            {
                if (li.Selected)
                {

                    myInsertCommand.CommandText = "INSERT INTO ProjectsUsers (UserID, ProjectID) "
                                                        + "VALUES (@UserID, @ProjectID)";
                    myInsertCommand.Parameters.AddWithValue("@UserID", li.Value.ToString());
                    myInsertCommand.Parameters.AddWithValue("@ProjectID", AddMembersProjectDropDownList.SelectedValue.ToString());
                    myInsertCommand.ExecuteNonQuery();
                }
            }
            myConnection.Close();
            OuterMem(new object(), EventArgs.Empty);
            
        }

        protected void btnRemoveMembers_Click(object sender, EventArgs e)
        {
            myConnection.Open();
            myDeleteCommand.CommandType = CommandType.Text;
            myDeleteCommand.Connection = myConnection;
            foreach (ListItem li in RemoveProjectMembersListBox.Items)
            {
                if (li.Selected)
                {
                    myDeleteCommand.CommandText = "DELETE FROM ProjectsMembers "
                                                + "WHERE (UserID = @UserID) AND (ProjectID = @ProjectID);";
                    myDeleteCommand.Parameters.AddWithValue("@UserID", li.Value.ToString());
                    myDeleteCommand.Parameters.AddWithValue("@ProjectID", RemoveMembersProjectDropDownList.SelectedValue.ToString());
                    myDeleteCommand.ExecuteNonQuery();
                }
            }
            myConnection.Close();
            InnerMem(new object(), EventArgs.Empty);
        }

        protected void linkbtnCreateAccount_Click(object sender, EventArgs e)
        {
            tableCreateAccount.Visible = true;
            tableEditAccount.Visible = false;
            GetDB();
            FillProject(newAccountDefaultProjectDropDownList, myDataSet.Tables["Projects"]);
        }

        protected void linkbtnEditAccount_Click(object sender, EventArgs e)
        {
            tableCreateAccount.Visible = false;
            tableEditAccount.Visible = true;
            GetDB();
            FillUser(selectAccountDropDownList, myDataSet.Tables["User"]);
            GetDB();
            FillProject(editDefaultProjectDropDownList, myDataSet.Tables["Projects"]);

            editUserLevelDropDownList.Items.Add("Choose User Level");
            editUserLevelDropDownList.Items.Add("1");
            editUserLevelDropDownList.Items.Add("2");
        }

        protected void btnCreateAccount_Click(object sender, EventArgs e)
        {
            myInsertCommand.Connection = myConnection;
            myInsertCommand.CommandType = CommandType.Text;

            myInsertCommand.CommandText = "INSERT INTO User (Username, Password, Email, Level, DefaultProjectID) "
                                                + "VALUES (@Username, @Password, @Email, @Level, @DefaultProjectID)";
            myInsertCommand.Parameters.AddWithValue("@Username", newAccountUsernameTextBox.Text);
            myInsertCommand.Parameters.AddWithValue("@Password", newAccountPasswordTextBox.Text);
            myInsertCommand.Parameters.AddWithValue("@Email", newAccountEmailTextBox2.Text);
            myInsertCommand.Parameters.AddWithValue("@Level", newAccountUserLevelDropDownList.SelectedValue);
            myInsertCommand.Parameters.AddWithValue("@DefaultProjectID", newAccountDefaultProjectDropDownList.SelectedValue);

            myInsertCommand.ExecuteNonQuery();
            myDataSet.Clear();
            GetDB();
          
        }

        protected void btnSaveEditAccount_Click(object sender, EventArgs e)
        {
            myUpdateCommand.Connection = myConnection;
            myUpdateCommand.CommandType = CommandType.Text;

            myUpdateCommand.CommandText = "UPDATE User SET Password = @Password, Email=@Email, Level=@Level, DefaultProjectID=@DefaultProjectID WHERE Username =@Username";
            myUpdateCommand.Parameters.AddWithValue("@Username", selectAccountDropDownList.SelectedItem.Text);
            myUpdateCommand.Parameters.AddWithValue("@Password", editPasswordTextBox.Text);
            myUpdateCommand.Parameters.AddWithValue("@Email", editEmailTextBox.Text);
            myUpdateCommand.Parameters.AddWithValue("@Level", editUserLevelDropDownList.SelectedValue.ToString());
            myUpdateCommand.Parameters.AddWithValue("@DefaultProjectID", editDefaultProjectDropDownList.SelectedValue.ToString());

            myUpdateCommand.ExecuteNonQuery();
            myDataSet.Clear();
            GetDB();
        }
    }
}