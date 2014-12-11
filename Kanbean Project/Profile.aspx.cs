using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using System.Text.RegularExpressions;

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

        private void GetDB()
        {
            myAdapter.SelectCommand = mySelectCommand;
            myDataSet.Clear();
            mySelectCommand.CommandText = "SELECT [User].*, Projects.ProjectName FROM [User], Projects "
                                        + "WHERE [User].DefaultProjectID = Projects.ProjectID AND "
                                        + "[User].[UserID] = " + Session["userID"].ToString();
            myAdapter.Fill(myDataSet, "Account");
            mySelectCommand.CommandText = "SELECT ProjectsMembers.ProjectID,Projects.ProjectName, ProjectsMembers.UserID, [User].Username "
                                        + "FROM ProjectsMembers, Projects, [User] "
                                        + "WHERE ProjectsMembers.ProjectID = Projects.ProjectID AND ProjectsMembers.UserID = [User].UserID "
                                        + "AND ProjectsMembers.UserID = " + Session["userID"].ToString();
            myAdapter.Fill(myDataSet, "ParticipatedProjects");
            mySelectCommand.CommandText = "SELECT ProjectsMembers.ProjectID,Projects.ProjectName, ProjectsMembers.UserID, User.Username "
                                        + "FROM ProjectsMembers, Projects, [User] "
                                        + "WHERE ProjectsMembers.ProjectID = Projects.ProjectID AND ProjectsMembers.UserID= [User].UserID "
                                        + "AND ProjectsMembers.UserID <> 1";
            myAdapter.Fill(myDataSet, "ProjectsMembers");
            myAdapter.SelectCommand.CommandText = "SELECT * FROM Projects;";
            myAdapter.Fill(myDataSet, "Projects");
            mySelectCommand.CommandText = "SELECT * From [User] WHERE UserID <> 1 AND UserID <> 2";
            myAdapter.Fill(myDataSet, "Users");
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

            if (Session["userID"] == null)
                Response.Redirect("board.aspx");

            userProfile.Visible = true;
            projectManagement.Visible = false;
            accountManagement.Visible = false;

            tableSummary.Visible = true;
            tableEditProfile.Visible = false;
            tableChangePass.Visible = false;

            myConnection.Open();
            myUpdateCommand.Connection = myConnection;
            mySelectCommand.Connection = myConnection;
            myDeleteCommand.Connection = myConnection;
            myInsertCommand.Connection = myConnection;

            GetDB();
            if (myDataSet.Tables["Account"].Rows[0]["Level"].ToString() != "1")
            {
                projectManagement.Enabled = btnProjectManagement.Visible = btnProjectManagement.Enabled = false;
                accountManagement.Enabled = btnAccountManagement.Visible = btnAccountManagement.Enabled = false;
            }
            FillContent();
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
                Session["username"] = null;
                Session["userID"] = null;
                Session["currentProject"] = null;
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

            GetDB();
            FillContent();
        }

        protected void linkbtnSummary_Click(object sender, EventArgs e)
        {
            tableSummary.Visible = true;
            tableEditProfile.Visible = false;
            tableChangePass.Visible = false;

            GetDB();
            FillContent();
        }

        protected void linkbtnEditProfile_Click(object sender, EventArgs e)
        {
            tableSummary.Visible = false;
            tableEditProfile.Visible = true;
            tableChangePass.Visible = false;

            emailTextbox.Text = myDataSet.Tables["Account"].Rows[0]["Email"].ToString();
            defaultProjectDropDownList.Items.Clear();
            foreach (DataRow row in myDataSet.Tables["ParticipatedProjects"].Rows)
            {
                defaultProjectDropDownList.Items.Add(row["ProjectName"].ToString());
                defaultProjectDropDownList.Items[defaultProjectDropDownList.Items.Count - 1].Value = row["ProjectID"].ToString();
                if (row["ProjectID"].ToString() == myDataSet.Tables["Account"].Rows[0]["DefaultProjectID"].ToString())
                    defaultProjectDropDownList.Items[defaultProjectDropDownList.Items.Count - 1].Selected = true;
            }
        }

        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            if (emailTextbox.Text != "" && Regex.IsMatch(emailTextbox.Text, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*") == false)
                ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('Invalid Email.');", true);
            else
            {
                myUpdateCommand.CommandText = "UPDATE [User] SET [Email] = @Email, DefaultProjectID = @DefaultProjectID WHERE [UserID] = @UserID";
                myUpdateCommand.Parameters.AddWithValue("@Email", emailTextbox.Text);
                myUpdateCommand.Parameters.AddWithValue("@DefaultProjectID", defaultProjectDropDownList.SelectedValue);
                myUpdateCommand.Parameters.AddWithValue("@UserID", myDataSet.Tables["Account"].Rows[0]["UserID"].ToString());
                myUpdateCommand.ExecuteNonQuery();
                GetDB();
                ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('User information is updated.');", true);
            }
        }

        protected void linkbtnChangePassword_Click(object sender, EventArgs e)
        {
            tableSummary.Visible = false;
            tableEditProfile.Visible = false;
            tableChangePass.Visible = true;
        }

        protected void btnChangePass_Click(object sender, EventArgs e)
        {
            if (myDataSet.Tables["Account"].Rows[0]["Password"].ToString() == oldPassTextBox.Text) {
                if (passwordValidator(newPassTextBox.Text))
                {
                    if (newPassTextBox.Text == reenterNewPassTextBox.Text)
                    {
                        myUpdateCommand.CommandText = "UPDATE [User] SET [Password] = @Password WHERE [UserID] = @UserID";
                        myUpdateCommand.Parameters.AddWithValue("@Password", newPassTextBox.Text);
                        myUpdateCommand.Parameters.AddWithValue("@UserID", myDataSet.Tables["Account"].Rows[0]["UserID"].ToString());
                        myUpdateCommand.ExecuteNonQuery();
                        GetDB();
                        ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('Password is changed.');", true);
                    }
                    else
                        ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('New password is not match.');", true);
                }
                else
                    ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('Password should have at least 6 characters, include 1 number, 1 small letter and 1 capital letter.');", true);
            }
            else
                ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('Wrong password.');", true);
            oldPassTextBox.Text = newPassTextBox.Text = reenterNewPassTextBox.Text = "";
        }

        protected bool passwordValidator(string password)
        {
            int isNum = password.IndexOfAny("1234567890".ToCharArray());
            int isLetterSmall = password.IndexOfAny("qwertyuiopåäölkjhgfdsazxcvbnm".ToCharArray());
            int isLetterBig = password.IndexOfAny("QWERTYUIOPÅÄÖLKJHGFDSAZXCVBNM".ToCharArray());
            bool isLongEnough = password.Length > 5;
            if ((isNum > -1) && (isLetterSmall > -1) && (isLetterBig > -1) && isLongEnough)
                return true;
            else
                return false;
        }

        protected void btnProjectManagement_Click(object sender, EventArgs e)
        {
            userProfile.Visible = false;
            projectManagement.Visible = true;
            accountManagement.Visible = false;

            tableCreateProject.Visible = true;
            tableAddMembers.Visible = false;
            tableRemoveMembers.Visible = false;

            newProjectMembersListBox.Items.Clear();
            newProjectMembersListBox.DataSource = myDataSet.Tables["Users"];
            newProjectMembersListBox.DataTextField = "Username";
            newProjectMembersListBox.DataValueField = "UserID";
            newProjectMembersListBox.DataBind();
        }

        protected void linkbtnCreateProject_Click(object sender, EventArgs e)
        {
            tableCreateProject.Visible = true;
            tableAddMembers.Visible = false;
            tableRemoveMembers.Visible = false;

            newProjectMembersListBox.Items.Clear();
            newProjectMembersListBox.DataSource = myDataSet.Tables["Users"];
            newProjectMembersListBox.DataTextField = "Username";
            newProjectMembersListBox.DataValueField = "UserID";
            newProjectMembersListBox.DataBind();
        }

        protected void btnCreateProject_Click(object sender, EventArgs e)
        {
            if (newProjectNameTextBox.Text != "")
            {
                myInsertCommand.CommandText = "INSERT INTO Projects(ProjectName) VALUES (@ProjectName)";
                myInsertCommand.Parameters.AddWithValue("@ProjectName", newProjectNameTextBox.Text);
                myInsertCommand.ExecuteNonQuery();
                mySelectCommand.CommandText = "SELECT MAX(ProjectID) FROM Projects";
                string ProjectID = mySelectCommand.ExecuteScalar().ToString();
                myInsertCommand.CommandText = "INSERT INTO ProjectsMembers(ProjectID, UserID) VALUES (" + ProjectID + ", 1)";
                myInsertCommand.ExecuteNonQuery();
                myInsertCommand.CommandText = "INSERT INTO ProjectsMembers(ProjectID, UserID) VALUES (" + ProjectID + ", 2)";
                myInsertCommand.ExecuteNonQuery();
                foreach (ListItem li in newProjectMembersListBox.Items)
                {
                    if (li.Selected)
                    {
                        myInsertCommand.CommandText = "INSERT INTO ProjectsMembers(ProjectID, UserID) VALUES (" + ProjectID + ", " + li.Value + ")";
                        myInsertCommand.ExecuteNonQuery();
                    }
                }
                ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('New project created.');", true);
                GetDB();
                newProjectNameTextBox.Text = "";
                foreach (ListItem li in newProjectMembersListBox.Items)
                    li.Selected = false;
            }
            else
                ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('Please enter the name of the project.');", true);
        }

        protected void linkbtnAddMembers_Click(object sender, EventArgs e)
        {
            tableCreateProject.Visible = false;
            tableAddMembers.Visible = true;
            tableRemoveMembers.Visible = false;

            FillProject(AddMembersProjectDropDownList, myDataSet.Tables["Projects"]);
            AddProjectMembersListBox.Items.Clear();
        }

        protected void AddMembersProjectDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddProjectMembersListBox.Items.Clear();
            if (AddMembersProjectDropDownList.SelectedIndex != 0)
            {
                mySelectCommand.CommandText = "SELECT * FROM [User] WHERE [User].UserID NOT IN "
                                            + "(SELECT [User].UserID FROM ProjectsMembers, [User] WHERE "
                                            + "ProjectsMembers.ProjectID = " + AddMembersProjectDropDownList.SelectedValue + " AND ProjectsMembers.UserID = [User].UserID)";
                myAdapter.Fill(myDataSet, "OuterMembers");
                foreach (DataRow row in myDataSet.Tables["OuterMembers"].Rows)
                {
                    AddProjectMembersListBox.Items.Add(row["Username"].ToString());
                    AddProjectMembersListBox.Items[AddProjectMembersListBox.Items.Count - 1].Value = row["UserID"].ToString();
                }
            }
        }

        protected void btnAddMembers_Click(object sender, EventArgs e)
        {
            if (AddMembersProjectDropDownList.SelectedIndex != 0)
            {
                if (AddProjectMembersListBox.SelectedIndex != -1)
                {
                    foreach (ListItem li in AddProjectMembersListBox.Items)
                    {
                        if (li.Selected)
                        {
                            myInsertCommand.CommandText = "INSERT INTO ProjectsMembers(ProjectID, UserID) VALUES ("
                                                        + AddMembersProjectDropDownList.SelectedValue + ", " + li.Value + ")";
                            myInsertCommand.ExecuteNonQuery();
                        }
                    }
                    ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('Member(s) added.');", true);
                    GetDB();
                    AddProjectMembersListBox.Items.Clear();
                    mySelectCommand.CommandText = "SELECT * FROM [User] WHERE [User].UserID NOT IN "
                                                + "(SELECT [User].UserID FROM ProjectsMembers, [User] WHERE "
                                                + "ProjectsMembers.ProjectID = " + AddMembersProjectDropDownList.SelectedValue + " AND ProjectsMembers.UserID = [User].UserID)";
                    myAdapter.Fill(myDataSet, "OuterMembers");
                    foreach (DataRow row in myDataSet.Tables["OuterMembers"].Rows)
                    {
                        AddProjectMembersListBox.Items.Add(row["Username"].ToString());
                        AddProjectMembersListBox.Items[AddProjectMembersListBox.Items.Count - 1].Value = row["UserID"].ToString();
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('Please select member(s).');", true);
            }
        }

        protected void linkbtnRemoveMembers_Click(object sender, EventArgs e)
        {
            tableCreateProject.Visible = false;
            tableAddMembers.Visible = false;
            tableRemoveMembers.Visible = true;

            FillProject(RemoveMembersProjectDropDownList, myDataSet.Tables["Projects"]);
            RemoveProjectMembersListBox.Items.Clear();
        }

        protected void RemoveMembersProjectDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemoveProjectMembersListBox.Items.Clear();
            if (RemoveMembersProjectDropDownList.SelectedIndex != 0)
            {
                mySelectCommand.CommandText = "SELECT [User].* FROM ProjectsMembers, [User] WHERE "
                                            + "ProjectsMembers.ProjectID = " + RemoveMembersProjectDropDownList.SelectedValue
                                            + " AND ProjectsMembers.UserID = [User].UserID "
                                            + "AND [User].UserID <> 1 AND [User].UserID <> 2";
                myAdapter.Fill(myDataSet, "InnerMembers");
                foreach (DataRow row in myDataSet.Tables["InnerMembers"].Rows)
                {
                    RemoveProjectMembersListBox.Items.Add(row["Username"].ToString());
                    RemoveProjectMembersListBox.Items[RemoveProjectMembersListBox.Items.Count - 1].Value = row["UserID"].ToString();
                }
            }
        }

        protected void btnRemoveMembers_Click(object sender, EventArgs e)
        {
            if (RemoveMembersProjectDropDownList.SelectedIndex != 0)
            {
                if (RemoveProjectMembersListBox.SelectedIndex != -1)
                {
                    foreach (ListItem li in RemoveProjectMembersListBox.Items)
                    {
                        if (li.Selected)
                        {
                            myDeleteCommand.CommandText = "DELETE FROM ProjectsMembers WHERE ProjectID = "
                                                        + RemoveMembersProjectDropDownList.SelectedValue
                                                        + " AND UserID = " + li.Value;
                            myDeleteCommand.ExecuteNonQuery();
                        }
                    }
                    ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('Member(s) removed.');", true);
                    GetDB();
                    RemoveProjectMembersListBox.Items.Clear();
                    mySelectCommand.CommandText = "SELECT [User].* FROM ProjectsMembers, [User] WHERE "
                                                + "ProjectsMembers.ProjectID = " + RemoveMembersProjectDropDownList.SelectedValue
                                                + " AND ProjectsMembers.UserID = [User].UserID "
                                                + "AND [User].UserID <> 1 AND [User].UserID <> 2";
                    myAdapter.Fill(myDataSet, "InnerMembers");
                    foreach (DataRow row in myDataSet.Tables["InnerMembers"].Rows)
                    {
                        RemoveProjectMembersListBox.Items.Add(row["Username"].ToString());
                        RemoveProjectMembersListBox.Items[RemoveProjectMembersListBox.Items.Count - 1].Value = row["UserID"].ToString();
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('Please select member(s).');", true);
            }
        }

        protected void btnAccountManagement_Click(object sender, EventArgs e)
        {
            userProfile.Visible = false;
            projectManagement.Visible = false;
            accountManagement.Visible = true;

            tableCreateAccount.Visible = true;
            tableEditAccount.Visible = false;

            newAccountUsernameTextBox.Text = "";
            newAccountPasswordTextBox.Text = "";
            newAccountEmailTextBox.Text = "";
            newAccountDefaultProjectDropDownList.Items.Clear();
            foreach (DataRow row in myDataSet.Tables["Projects"].Rows)
            {
                newAccountDefaultProjectDropDownList.Items.Add(row["ProjectName"].ToString());
                newAccountDefaultProjectDropDownList.Items[newAccountDefaultProjectDropDownList.Items.Count - 1].Value = row["ProjectID"].ToString();
            }
        }

        protected void linkbtnCreateAccount_Click(object sender, EventArgs e)
        {
            tableCreateAccount.Visible = true;
            tableEditAccount.Visible = false;

            newAccountUsernameTextBox.Text = "";
            newAccountPasswordTextBox.Text = "";
            newAccountEmailTextBox.Text = "";
            newAccountDefaultProjectDropDownList.Items.Clear();
            foreach (DataRow row in myDataSet.Tables["Projects"].Rows)
            {
                newAccountDefaultProjectDropDownList.Items.Add(row["ProjectName"].ToString());
                newAccountDefaultProjectDropDownList.Items[newAccountDefaultProjectDropDownList.Items.Count - 1].Value = row["ProjectID"].ToString();
            }
        }

        protected void btnCreateAccount_Click(object sender, EventArgs e)
        {
            if (newAccountUsernameTextBox.Text != "" && newAccountPasswordTextBox.Text != "")
            {
                mySelectCommand.CommandText = "SELECT COUNT(UserID) FROM [User] WHERE [Username] ='" + newAccountUsernameTextBox.Text + "'";
                if ((int)mySelectCommand.ExecuteScalar() > 0)
                    ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('This username is existed.');", true);
                else
                {
                    if (newAccountEmailTextBox.Text != "" && Regex.IsMatch(newAccountEmailTextBox.Text, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*") == false)
                            ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('Invalid Email.');", true);
                    else
                    {
                        myInsertCommand.CommandText = "INSERT INTO [User] ([Username], [Password], [Email], [Level], DefaultProjectID) "
                                                    + "VALUES (@Username, @Password, @Email, @Level, @DefaultProjectID)";
                        myInsertCommand.Parameters.AddWithValue("@Username", newAccountUsernameTextBox.Text);
                        myInsertCommand.Parameters.AddWithValue("@Password", newAccountPasswordTextBox.Text);
                        myInsertCommand.Parameters.AddWithValue("@Email", newAccountEmailTextBox.Text);
                        myInsertCommand.Parameters.AddWithValue("@Level", newAccountUserLevelDropDownList.SelectedValue);
                        myInsertCommand.Parameters.AddWithValue("@DefaultProjectID", newAccountDefaultProjectDropDownList.SelectedValue);
                        myInsertCommand.ExecuteNonQuery();
                        mySelectCommand.CommandText = "SELECT MAX(UserID) FROM [User]";
                        string userID = mySelectCommand.ExecuteScalar().ToString();
                        myInsertCommand.CommandText = "INSERT INTO ProjectsMembers(ProjectID, UserID) VALUES ("
                                                    + newAccountDefaultProjectDropDownList.SelectedValue + ", " + userID + ")";
                        myInsertCommand.ExecuteNonQuery();
                        GetDB();
                        ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('Account created.');", true);
                    }
                }
            }
            else
                ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('Username and password are required.');", true);
        }

        protected void linkbtnEditAccount_Click(object sender, EventArgs e)
        {
            tableCreateAccount.Visible = false;
            tableEditAccount.Visible = true;

            selectAccountDropDownList.Items.Clear();
            selectAccountDropDownList.Items.Add("- Choose an account -");
            selectAccountDropDownList.Items[0].Attributes.Add("disabled", "disabled");
            foreach (DataRow row in myDataSet.Tables["Users"].Rows)
            {
                selectAccountDropDownList.Items.Add(row["Username"].ToString());
                selectAccountDropDownList.Items[selectAccountDropDownList.Items.Count - 1].Value = row["UserID"].ToString();
            }
            editPasswordTextBox.Text = "";
            editEmailTextBox.Text = "";
            editUserLevelDropDownList.Items.Clear();
            editDefaultProjectDropDownList.Items.Clear();

        }

        protected void selectAccountDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            editPasswordTextBox.Text = "";
            editEmailTextBox.Text = "";
            editUserLevelDropDownList.Items.Clear();
            editDefaultProjectDropDownList.Items.Clear();

            if (selectAccountDropDownList.SelectedIndex != 0)
            {
                editUserLevelDropDownList.Items.Add("admin");
                editUserLevelDropDownList.Items[0].Value = "1";
                editUserLevelDropDownList.Items.Add("user");
                editUserLevelDropDownList.Items[1].Value = "2";

                foreach (DataRow row in myDataSet.Tables["Users"].Rows)
                {
                    if (row["UserID"].ToString() == selectAccountDropDownList.SelectedValue)
                    {
                        editPasswordTextBox.Text = row["Password"].ToString();
                        editEmailTextBox.Text = row["Email"].ToString();
                        if (row["Level"].ToString() == "1")
                            editUserLevelDropDownList.Items[0].Selected = true;
                        else
                            editUserLevelDropDownList.Items[1].Selected = true;
                        mySelectCommand.CommandText = "SELECT ProjectsMembers.ProjectID,Projects.ProjectName "
                                                    + "FROM ProjectsMembers, Projects, [User] "
                                                    + "WHERE ProjectsMembers.ProjectID = Projects.ProjectID AND ProjectsMembers.UserID = [User].UserID "
                                                    + "AND ProjectsMembers.UserID = " + row["UserID"].ToString();
                        myReader = mySelectCommand.ExecuteReader();
                        bool notEoF = myReader.Read();
                        while (notEoF)
                        {
                            editDefaultProjectDropDownList.Items.Add(myReader["ProjectName"].ToString());
                            editDefaultProjectDropDownList.Items[editDefaultProjectDropDownList.Items.Count - 1].Value = myReader["ProjectID"].ToString();
                            if (myReader["ProjectID"].ToString() == row["DefaultProjectID"].ToString())
                                editDefaultProjectDropDownList.Items[editDefaultProjectDropDownList.Items.Count - 1].Selected = true;
                            notEoF = myReader.Read();
                        }
                        myReader.Close();
                    }
                }
            }
        }

        protected void btnSaveEditAccount_Click(object sender, EventArgs e)
        {
            if (selectAccountDropDownList.SelectedIndex != 0)
            {
                if (editPasswordTextBox.Text != "")
                {
                    if (editEmailTextBox.Text != "" && Regex.IsMatch(editEmailTextBox.Text, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*") == false)
                        ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('Invalid Email.');", true);
                    else
                    {
                        myUpdateCommand.CommandText = "UPDATE [User] SET [Password] = '" + editPasswordTextBox.Text + "', "
                                                    + "[Email] = '" + editEmailTextBox.Text + "', "
                                                    + "[Level] = " + editUserLevelDropDownList.SelectedValue + ", "
                                                    + "DefaultProjectID = " + editDefaultProjectDropDownList.SelectedValue
                                                    + " WHERE UserID = " + selectAccountDropDownList.SelectedValue;
                        //myUpdateCommand.Parameters.AddWithValue("@UserID", selectAccountDropDownList.SelectedValue);
                        //myUpdateCommand.Parameters.AddWithValue("@Password", editPasswordTextBox.Text);
                        //myUpdateCommand.Parameters.AddWithValue("@Email", editEmailTextBox.Text);
                        //myUpdateCommand.Parameters.AddWithValue("@Level", editUserLevelDropDownList.SelectedValue);
                        //myUpdateCommand.Parameters.AddWithValue("@DefaultProjectID", editDefaultProjectDropDownList.SelectedValue);
                        myUpdateCommand.ExecuteNonQuery();
                        GetDB();
                        ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('Account is updated.');", true);
                    }
                }
                else
                    ScriptManager.RegisterStartupScript(accountUpdatePanel, accountUpdatePanel.GetType(), "alert", "alert('Password is required.');", true);
            }
        }

        private void FillContent()
        {
            lblUsername.Text = myDataSet.Tables["Account"].Rows[0]["Username"].ToString();
            lblEmail.Text = myDataSet.Tables["Account"].Rows[0]["Email"].ToString();
            if (myDataSet.Tables["Account"].Rows[0]["Level"].ToString() == "1")
                lblUserLevel.Text = "admin";
            else
                lblUserLevel.Text = "user";
            lblDefaultProject.Text = myDataSet.Tables["Account"].Rows[0]["ProjectName"].ToString();
            lblParticipatedProjects.Text = "";
            foreach (DataRow row in myDataSet.Tables["ParticipatedProjects"].Rows)
                lblParticipatedProjects.Text += row["ProjectName"].ToString() + ", ";
        }

        private void FillProject(DropDownList _Project, DataTable _projectTable)
        {
            _Project.Items.Clear();
            _Project.Items.Add("- Choose a project -");
            _Project.Items[0].Attributes.Add("disabled", "disabled");
            foreach (DataRow row in _projectTable.Rows)
            {
                _Project.Items.Add(row["ProjectName"].ToString());
                _Project.Items[_Project.Items.Count - 1].Value = row["ProjectID"].ToString();
            }
        }



    }
}