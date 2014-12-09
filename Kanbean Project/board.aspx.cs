using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Data;
using System.Collections;
using System.Web.Services;
using System.IO;

namespace Kanbean_Project
{
    public partial class board : System.Web.UI.Page
    {
        OleDbConnection myConnection = new OleDbConnection();
        OleDbCommand mySelectCommand = new OleDbCommand();
        OleDbCommand myDeleteCommand = new OleDbCommand();
        OleDbCommand myInsertCommand = new OleDbCommand();
        OleDbCommand myUpdateCommand = new OleDbCommand();
        OleDbDataAdapter myAdapter = new OleDbDataAdapter();

        OleDbDataReader myReader;
        OleDbCommand selectSearch = new OleDbCommand();

        DataSet myDataSet = new DataSet();
        
        private void getDatabase()
        {
            mySelectCommand.Connection = myConnection;
            myAdapter.SelectCommand = mySelectCommand;
            mySelectCommand.CommandText = "SELECT * FROM Swimlanes ORDER BY SwimlaneID";
            myAdapter.Fill(myDataSet, "mySwimlanes");
            mySelectCommand.CommandText = "SELECT Projects.ProjectName, [User].UserID, [User].Username, [User].Level "
                                        + "FROM ProjectsMembers, Projects, [User] " 
                                        + "WHERE ProjectsMembers.ProjectID = Projects.ProjectID AND ProjectsMembers.UserID = [User].UserID "
                                        + "AND Projects.ProjectID = " + Session["currentProject"].ToString() 
                                        + " ORDER BY [User].UserID";
            myAdapter.Fill(myDataSet, "myProjectNembers");
            mySelectCommand.CommandText = "SELECT Backlogs.*, [User].Username, Swimlanes.SwimlaneName, Projects.ProjectName, Status.StatusName "
                                        + "FROM Backlogs, [User], Swimlanes, Projects, Status "
                                        + "WHERE Backlogs.ProjectID = " + Session["currentProject"].ToString() 
                                        + " AND Backlogs.BacklogAssigneeID = [User].UserID AND Backlogs.SwimlaneID = Swimlanes.SwimlaneID "
                                        + "AND Backlogs.ProjectID = Projects.ProjectID AND Backlogs.BacklogStatusID = Status.StatusID " 
                                        + "ORDER BY Backlogs.SwimlaneID, Backlogs.BacklogPosition";
            myAdapter.Fill(myDataSet, "myBacklogs");
            mySelectCommand.CommandText = "Select * From Backlogs";
            myAdapter.Fill(myDataSet, "myRawBacklogs");
            mySelectCommand.CommandText = "SELECT Tasks.*, [User].Username, Status.StatusName "
                                        + "FROM Tasks, [User], Backlogs, Status " 
                                        + "WHERE Backlogs.ProjectID = " + Session["currentProject"].ToString() 
                                        + " AND Backlogs.BacklogID = Tasks.BacklogID " 
                                        + "AND Tasks.TaskAssigneeID = [User].UserID AND Tasks.TaskStatusID = Status.StatusID " 
                                        + "ORDER BY Tasks.TaskID";
            myAdapter.Fill(myDataSet, "myTasks");
            mySelectCommand.CommandText = "Select * From Tasks";
            myAdapter.Fill(myDataSet, "myRawTasks");
            mySelectCommand.CommandText = "Select * From Status";
            myAdapter.Fill(myDataSet, "myStatus");
            mySelectCommand.CommandText = "Select * From Projects";
            myAdapter.Fill(myDataSet, "myProjects");
        }
        
        private void createBacklog(string id, string complexity, string title, string deadline, string color, string colorHeader, string swimlaneID, string assignee)
        {
            if (kanbanboard.FindControl("backlogArea" + id) == null)
            {
                Panel newBacklog = new Panel();
                newBacklog.CssClass = "backlogArea";
                newBacklog.ID = "backlogArea" + id;

                Panel backlog = new Panel();
                backlog.CssClass = "backlog";
                backlog.Style.Add("background-color", color);
                backlog.ID = "backlog" + id;
                backlog.Attributes.Add("draggable", "true");
                backlog.Attributes.Add("ondragstart", "drag(event)");
                newBacklog.Controls.Add(backlog);

                Panel backlogHeader = new Panel();
                backlogHeader.CssClass = "backlog-header";
                backlogHeader.Style.Add("background-color", colorHeader);
                backlogHeader.ID = "backlogHeader" + id;
                backlog.Controls.Add(backlogHeader);

                Panel backlogContent = new Panel();
                backlogContent.CssClass = "backlog-content";
                backlogContent.ID = "backlogContent" + id;
                backlog.Controls.Add(backlogContent);

                Panel backlogFooterUp = new Panel();
                backlogFooterUp.CssClass = "backlog-footer";
                backlogFooterUp.ID = "backlogFooterUp" + id;
                backlog.Controls.Add(backlogFooterUp);

                Panel backlogFooterDown = new Panel();
                backlogFooterDown.CssClass = "backlog-footer";
                backlogFooterDown.ID = "backlogFooterDown" + id;
                backlog.Controls.Add(backlogFooterDown);

                Label lblID = new Label();
                lblID.CssClass = "lblID";
                lblID.ID = "lblBacklogID" + id;
                lblID.ToolTip = "backlog ID";
                lblID.Text = "#" + id;
                backlogHeader.Controls.Add(lblID);

                LinkButton btnDelete = new LinkButton();
                btnDelete.CssClass = "backlogIcon iconDelete";
                btnDelete.ID = "btnDeleteBacklog" + id;
                btnDelete.ToolTip = "Delete the backlog";
                btnDelete.Click += new EventHandler(btnDelete_Click);
                backlogHeader.Controls.Add(btnDelete);

                LinkButton btnEdit = new LinkButton();
                btnEdit.CssClass = "backlogIcon iconEdit";
                btnEdit.ID = "btnEditBacklog" + id;
                btnEdit.ToolTip = "Edit the backlog";
                btnEdit.Click += new EventHandler(btnEditBacklog_Click);
                backlogHeader.Controls.Add(btnEdit);

                LinkButton btnAddTask = new LinkButton();
                btnAddTask.CssClass = "backlogIcon iconAdd";
                btnAddTask.ID = "btnAddTask" + id;
                btnAddTask.ToolTip = "Add new task";
                btnAddTask.Click += new EventHandler(btnAddTask_Click);
                backlogHeader.Controls.Add(btnAddTask);

                LinkButton backlogTitle = new LinkButton();
                backlogTitle.CssClass = "backlog-title";
                backlogTitle.ID = "backlogTitle" + id;
                backlogTitle.Text = title;
                backlogTitle.ToolTip = "View the backlog";
                backlogTitle.Click += new EventHandler(showDetail_Click);
                backlogContent.Controls.Add(backlogTitle);

                if (complexity != "")
                {
                    LinkButton btnComplexity = new LinkButton();
                    btnComplexity.CssClass = "btnComplexity";
                    btnComplexity.ID = "btnBacklogComplexity" + id;
                    btnComplexity.ToolTip = "Edit complexity";
                    btnComplexity.Text = complexity;
                    btnComplexity.Click += new EventHandler(btnComplexity_Click);
                    backlogFooterUp.Controls.Add(btnComplexity);
                }
                
                LinkButton btnAssignee = new LinkButton();
                btnAssignee.CssClass = "btnAssignee";
                btnAssignee.ID = "btnBacklogAssignee" + id;
                btnAssignee.ToolTip = "Edit assignee";
                if (assignee != "")
                    btnAssignee.Text = "Assignee: " + assignee;
                btnAssignee.Click += new EventHandler(btnAssignee_Click);
                backlogFooterUp.Controls.Add(btnAssignee);

                LinkButton btnDueDate = new LinkButton();
                btnDueDate.CssClass = "btnDueDate";
                btnDueDate.ID = "btnBacklogDueDate" + id;
                btnDueDate.ToolTip = "Edit deadline";
                if (deadline != "")
                    btnDueDate.Text = Convert.ToDateTime(deadline).ToString("dd.MM.yyyy");
                btnDueDate.Click += new EventHandler(btnDueDate_Click);
                backlogFooterDown.Controls.Add(btnDueDate);

                mySelectCommand.CommandText = "SELECT COUNT(CommentID) FROM BacklogsComments WHERE BacklogID=" + id;
                string amountComments = mySelectCommand.ExecuteScalar().ToString();
                LinkButton btnComment = new LinkButton();
                btnComment.CssClass = "backlogIcon iconComment";
                btnComment.ID = "btnBacklogComment" + id;
                btnComment.ToolTip = "Show the comments";
                btnComment.Click += new EventHandler(btnComment_Click);
                if (amountComments != "0")
                    btnComment.Text = " " + amountComments;
                backlogFooterDown.Controls.Add(btnComment);

                mySelectCommand.CommandText = "SELECT COUNT(TaskID) FROM Tasks WHERE BacklogID=" + id;
                string amountTasks = mySelectCommand.ExecuteScalar().ToString();
                if (amountTasks != "0")
                {
                    LinkButton btnTask = new LinkButton();
                    btnTask.CssClass = "backlogIcon iconTask";
                    btnTask.ID = "btnShowHideTask" + id;
                    btnTask.ToolTip = "Show/Hide the tasks";
                    btnTask.Click += new EventHandler(btnShowHideTask_Click);
                    mySelectCommand.CommandText = "SELECT COUNT(TaskID) FROM Tasks WHERE BacklogID=" + id + "AND TaskStatusID=3";
                    string amountCompletedTasks = mySelectCommand.ExecuteScalar().ToString();
                    btnTask.Text = " " + amountCompletedTasks + "/" + amountTasks;
                    backlogFooterDown.Controls.Add(btnTask);
                }
                kanbanboard.FindControl("columnContent" + swimlaneID).Controls.Add(newBacklog);
            }
        }

        private void createTask(string id, string BacklogID, string complexity, string title, string deadline, string assignee, string statusID)
        {
            if (kanbanboard.FindControl("task" + id) == null)
            {
                Panel task = new Panel();
                task.CssClass = "tasks";
                if (statusID == "1")
                    task.Style.Add("background-color", "#fffafa");
                if (statusID == "2")
                    task.Style.Add("background-color", "#f8f8ff");
                if (statusID == "3")
                    task.Style.Add("background-color", "#eee9e9");
                task.ID = "task" + id;
                task.Visible = false;

                Panel taskHeader = new Panel();
                taskHeader.CssClass = "backlog-header";
                taskHeader.Style.Add("background-color", "#ddd7d7");
                taskHeader.ID = "taskHeader" + id;
                task.Controls.Add(taskHeader);

                Panel taskContent = new Panel();
                taskContent.CssClass = "backlog-content";
                taskContent.ID = "taskContent" + id;
                task.Controls.Add(taskContent);

                Panel taskFooterUp = new Panel();
                taskFooterUp.CssClass = "backlog-footer";
                taskFooterUp.ID = "taskFooterUp" + id;
                task.Controls.Add(taskFooterUp);

                Panel taskFooterDown = new Panel();
                taskFooterDown.CssClass = "backlog-footer";
                taskFooterDown.ID = "taskFooterDown" + id;
                task.Controls.Add(taskFooterDown);

                LinkButton btnDelete = new LinkButton();
                btnDelete.CssClass = "backlogIcon iconDelete";
                btnDelete.ID = "btnDeleteTask" + id;
                btnDelete.ToolTip = "Delete the task";
                btnDelete.Click += new EventHandler(btnDelete_Click);
                taskHeader.Controls.Add(btnDelete);

                LinkButton btnEdit = new LinkButton();
                btnEdit.CssClass = "backlogIcon iconEdit";
                btnEdit.ID = "btnEditTask" + id;
                btnEdit.ToolTip = "Edit the task";
                btnEdit.Click += new EventHandler(btnEditTask_Click);
                taskHeader.Controls.Add(btnEdit);

                LinkButton taskTitle = new LinkButton();
                taskTitle.CssClass = "backlog-title";
                taskTitle.ID = "taskTitle" + id;
                taskTitle.Text = title;
                taskTitle.ToolTip = "View the task";
                taskTitle.Click += new EventHandler(showDetail_Click);
                taskContent.Controls.Add(taskTitle);

                LinkButton btnComplexity = new LinkButton();
                btnComplexity.CssClass = "btnComplexity";
                btnComplexity.ID = "btnTaskComplexity" + id;
                btnComplexity.ToolTip = "Edit complexity";
                if (complexity != "")
                    btnComplexity.Text = complexity;
                btnComplexity.Click += new EventHandler(btnComplexity_Click);
                taskFooterUp.Controls.Add(btnComplexity);

                LinkButton btnAssignee = new LinkButton();
                btnAssignee.CssClass = "btnAssignee";
                btnAssignee.ID = "btnTaskAssignee" + id;
                btnAssignee.ToolTip = "Edit assignee";
                if (assignee != "")
                    btnAssignee.Text = "Assignee: " + assignee;
                btnAssignee.Click += new EventHandler(btnAssignee_Click);
                taskFooterUp.Controls.Add(btnAssignee);

                LinkButton btnDueDate = new LinkButton();
                btnDueDate.CssClass = "btnDueDate";
                btnDueDate.ID = "btnTaskDueDate" + id;
                btnDueDate.ToolTip = "Edit deadline";
                if (deadline != "")
                    btnDueDate.Text = Convert.ToDateTime(deadline).ToString("dd.MM.yyyy");
                btnDueDate.Click += new EventHandler(btnDueDate_Click);
                taskFooterDown.Controls.Add(btnDueDate);

                mySelectCommand.CommandText = "SELECT COUNT(CommentID) FROM TasksComments WHERE TaskID=" + id;
                string amountComments = mySelectCommand.ExecuteScalar().ToString();
                LinkButton btnComment = new LinkButton();
                btnComment.CssClass = "backlogIcon iconComment";
                btnComment.ID = "btnTaskComment" + id;
                btnComment.ToolTip = "Show the comments";
                if (amountComments != "0")
                    btnComment.Text = " " + amountComments;
                btnComment.Click += new EventHandler(btnComment_Click);
                taskFooterDown.Controls.Add(btnComment);

                int amountFiles = 0;
                if (Directory.Exists(Server.MapPath("~/Files/task" + id + "/")))
                    amountFiles = Directory.GetFiles(Server.MapPath("~/Files/task" + id + "/")).Length;
                LinkButton btnAttachedFile = new LinkButton();
                btnAttachedFile.CssClass = "backlogIcon iconAttachedFile";
                btnAttachedFile.ID = "btnTaskAttachedFile" + id;
                btnAttachedFile.ToolTip = "Show the attached files";
                if (amountFiles != 0)
                    btnAttachedFile.Text = " " + amountFiles.ToString();
                btnAttachedFile.Click += new EventHandler(btnAttachedFile_Click);
                taskFooterDown.Controls.Add(btnAttachedFile);

                kanbanboard.FindControl("backlogArea" + BacklogID).Controls.Add(task);
            }
        }

        private void getBacklogs()
        {
            DataTable backlogsTable = myDataSet.Tables["myBacklogs"];
            foreach (DataRow row in backlogsTable.Rows)
                createBacklog(row["BacklogID"].ToString(), row["BacklogComplexity"].ToString(), row["BacklogTitle"].ToString(), row["BacklogDueDate"].ToString(), row["BacklogColor"].ToString(), row["BacklogColorHeader"].ToString(), row["SwimlaneID"].ToString(), row["Username"].ToString());
        }

        private void getTasks()
        {
            DataTable tasksTable = myDataSet.Tables["myTasks"];
            foreach (DataRow row in tasksTable.Rows)
                createTask(row["TaskID"].ToString(), row["BacklogID"].ToString(), row["TaskComplexity"].ToString(), row["TaskTitle"].ToString(), row["TaskDueDate"].ToString(), row["Username"].ToString(), row["TaskStatusID"].ToString());
        }

        protected void Page_Init(object sender, EventArgs e)
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
                else 
                    Response.Redirect("login.aspx");
            }
            else 
                Response.Redirect("login.aspx");

            //open connection
            myConnection.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|LanbanDatabase.mdb;";
            myConnection.Open();
            //get the Default Project by Username
            mySelectCommand.Connection = myConnection;
            mySelectCommand.CommandText = "SELECT * FROM [User] WHERE [Username] = '" + Username + "';";
            //string DefaultProject = mySelectCommand.ExecuteScalar().ToString();
            myReader = mySelectCommand.ExecuteReader();
            bool notEoF = myReader.Read();
            while (notEoF)
            {
                Session["username"] = myReader["Username"].ToString();
                Session["userID"] = myReader["UserID"].ToString();
                Session["currentProject"] = myReader["DefaultProjectID"].ToString();
                notEoF = myReader.Read();
            }
            myReader.Close();
            if (Request["projectID"] != null)
                Session["currentProject"] = Request["projectID"].ToString();
            if (Session["currentProject"].ToString() != "0")
            {
                getDatabase();

                //Initialize the kanbanboard with swimlane information from database
                DataTable boardTable = myDataSet.Tables["mySwimlanes"];
                TableRow thRow = new TableRow();
                TableRow tRow = new TableRow();

                foreach (DataRow row in boardTable.Rows)
                {
                    //the board header
                    TableHeaderCell thCell = new TableHeaderCell();
                    thCell.CssClass = "board-header";
                    thCell.ID = "columnHeader" + row["SwimlaneID"].ToString();
                    thCell.Text = row["SwimlaneName"].ToString();
                    thCell.Width = new Unit(100 / boardTable.Rows.Count, UnitType.Percentage);
                    thRow.Cells.Add(thCell);

                    //the board content
                    TableCell tCell = new TableCell();
                    tCell.CssClass = "board-content";
                    tCell.ID = "columnContent" + row["SwimlaneID"].ToString();
                    tCell.Width = new Unit(100 / boardTable.Rows.Count, UnitType.Percentage);
                    tCell.Attributes.Add("ondrop", "drop(event)");
                    tCell.Attributes.Add("ondragover", "dragover(event)");
                    tRow.Cells.Add(tCell);
                }
                kanbanboard.Controls.Add(thRow);
                kanbanboard.Controls.Add(tRow);

                foreach (DataRow row in myDataSet.Tables["myProjects"].Rows)
                {
                    projectDropDownList.Items.Add(row["ProjectName"].ToString());
                    projectDropDownList.Items[projectDropDownList.Items.Count - 1].Value = row["ProjectID"].ToString();
                    if (row["ProjectID"].ToString() == Session["currentProject"].ToString())
                        projectDropDownList.Items[projectDropDownList.Items.Count - 1].Selected = true;
                }
                getBacklogs();
                getTasks();
            }
            else
            {
                ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "alert", "alert('You aren't member of any project.\r\nPlease contact with admin.');", true);
                Response.Cookies["UserSettings"].Expires = DateTime.Now.AddDays(-1);
                Response.Redirect("login.aspx");
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //if ((string[])Session["Controls"] != null)
            //{
            //    foreach (string control in (string[])Session["Controls"])
            //    {
            //        Control c = kanbanboard.FindControl(control);
            //        if (c != null)
            //            for (int i = 0; i < c.Controls.Count; i++)
            //                c.Controls[i].Visible = true;
            //        else for (int i = 0; i < c.Controls.Count; i++)
            //                c.Controls[i].Visible = false;
            //        Session["Controls"] = null;
            //    }
            //}
            //if ((string)Session["Popup"] == "true")
            //{
            //    attachFile((object)Session["BacklogUpload"]);
            //    Session["Popup"] = null;
            //    Session["BacklogUpload"] = null;
            //}
        }

        protected void btnAddBacklog_Click(object sender, EventArgs e)
        {
            btnAddNewBacklog.Visible = true;
            btnUpdateBacklog.Visible = false;
            addandEditBacklogLegend.InnerText = "Add new backlog";
            lblBacklogNotice.Text = "";
            titleTextBox.Text = "";
            descriptionTextBox.Text = "";

            swimlaneDropDownList.Items.Clear();
            foreach (DataRow row in myDataSet.Tables["mySwimlanes"].Rows)
            {
                swimlaneDropDownList.Items.Add(row["SwimlaneName"].ToString());
                swimlaneDropDownList.Items[swimlaneDropDownList.Items.Count - 1].Value = row["SwimlaneID"].ToString();
            }

            assigneeDropDownList.Items.Clear();
            foreach (DataRow row in myDataSet.Tables["myProjectNembers"].Rows)
            {
                assigneeDropDownList.Items.Add(row["Username"].ToString());
                assigneeDropDownList.Items[assigneeDropDownList.Items.Count - 1].Value = row["UserID"].ToString();
            }

            for (int i = 0; i < colorDropDownList.Items.Count; i++)
                colorDropDownList.Items[i].Selected = false;
            complexityTextBox.Text = "";
            deadlineTextBox.Text = "";

            addandEditBacklogPopup.Show();
        }

        protected void btnAddNewBacklog_Click(object sender, EventArgs e)
        {
            if (titleTextBox.Text == "")
            {
                lblBacklogNotice.Text = "Backlog Title is required!";
                addandEditBacklogPopup.Show();
            }
            else if (complexityTextBox.Text != "" && !IsNumeric(complexityTextBox.Text))
            {
                lblBacklogNotice.Text = "Backlog Complexity must be a number!";
                addandEditBacklogPopup.Show();
            }
            else if (deadlineTextBox.Text != "" && !IsDate(deadlineTextBox.Text))
            {
                lblBacklogNotice.Text = "Backlog Due date must be a date!";
                addandEditBacklogPopup.Show();
            }
            else
            {
                DataRow row = myDataSet.Tables["myRawBacklogs"].NewRow();
                row["ProjectID"] = Session["currentProject"].ToString();
                row["SwimlaneID"] = Convert.ToInt32(swimlaneDropDownList.SelectedValue);
                if (swimlaneDropDownList.SelectedValue == "5")
                {
                    row["BacklogStatusID"] = 3;
                    row["BacklogCompletedDate"] = DateTime.Today;
                }
                else if (swimlaneDropDownList.SelectedValue == "4")
                {
                    row["BacklogStatusID"] = 2;
                    row["BacklogCompletedDate"] = DBNull.Value;
                }
                else
                {
                    row["BacklogStatusID"] = 1;
                    row["BacklogCompletedDate"] = DBNull.Value;
                }
                row["BacklogTitle"] = titleTextBox.Text;
                row["BacklogDescription"] = descriptionTextBox.Text;
                row["BacklogColor"] = colorDropDownList.SelectedValue.Split(',')[1].ToString();
                row["BacklogColorHeader"] = colorDropDownList.SelectedValue.Split(',')[0].ToString();
                if (complexityTextBox.Text != "")
                    row["BacklogComplexity"] = Convert.ToInt32(complexityTextBox.Text);
                row["BacklogStartDate"] = DateTime.Today;
                if (deadlineTextBox.Text != "")
                    row["BacklogDueDate"] = Convert.ToDateTime(deadlineTextBox.Text);
                row["BacklogAssigneeID"] = Convert.ToInt32(assigneeDropDownList.SelectedValue);
                mySelectCommand.CommandText = "SELECT COUNT(SwimlaneID) FROM Backlogs WHERE SwimlaneID=" + swimlaneDropDownList.SelectedValue;
                row["BacklogPosition"] = Convert.ToInt32(mySelectCommand.ExecuteScalar().ToString());
                myDataSet.Tables["myRawBacklogs"].Rows.Add(row);
                myAdapter.SelectCommand.CommandText = "Select * From Backlogs";
                OleDbCommandBuilder myCommandBuilder = new OleDbCommandBuilder(myAdapter);
                myAdapter.InsertCommand = myCommandBuilder.GetInsertCommand();
                myAdapter.Update(myDataSet, "myRawBacklogs");
                myDataSet.Clear();

                getDatabase();
                getBacklogs();
                //ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "refreshBoard", "refreshBoard();", true);
                addandEditBacklogPopup.Hide();
            }
        }

        protected void showDetail_Click(object sender, EventArgs e)
        {
            if (((Control)sender).ID.Substring(0, 4) == "back")
            {
                string id = ((Control)sender).ID.Remove(0, 12);
                viewBacklogandTaskLegend.InnerText = "Backlog ID #" + id;
                btnEditViewBacklog.Visible = true;
                btnEditViewTask.Visible = false;
                DataRow row = myDataSet.Tables["myBacklogs"].Select("BacklogID = " + id)[0];
                viewBacklogandTask.Text = "Backlog in Project \"" + row["ProjectName"].ToString() + "\"";
                viewBacklogandTask.Text += "<table><tr><td><b>Title: </b></td><td>" + row["BacklogTitle"].ToString() + "</td></tr>";
                if (row["BacklogDescription"].ToString() != "")
                    viewBacklogandTask.Text += "<tr><td><b>Description: </b></td><td>" + row["BacklogDescription"].ToString() + "</td></tr>";
                if (row["Username"].ToString() != "")
                    viewBacklogandTask.Text += "<tr><td><b>Assignee: </b></td><td>" + row["Username"].ToString() + "</td></tr>";
                if (row["BacklogComplexity"].ToString() != "")
                    viewBacklogandTask.Text += "<tr><td><b>Complexity: </b></td><td>" + row["BacklogComplexity"].ToString() + "</td></tr>";
                viewBacklogandTask.Text += "<tr><td><b>Backlog Status: </b></td><td>" + row["StatusName"].ToString() + "</td></tr>";
                viewBacklogandTask.Text += "<tr><td><b>Column on the board: </b></td><td>" + row["SwimlaneName"].ToString() + "</td></tr>";
                viewBacklogandTask.Text += "<tr><td><b>Created Date: </b></td><td>" + Convert.ToDateTime(row["BacklogStartDate"]).ToString("ddd, dd MMM yyyy") + "</td></tr>";
                if (row["BacklogDueDate"].ToString() != "")
                    viewBacklogandTask.Text += "<tr><td><b>Deadline: <b></td><td>" + Convert.ToDateTime(row["BacklogDueDate"]).ToString("ddd, dd MMM yyyy") + "</td></tr>";
                if (row["BacklogCompletedDate"].ToString() != "")
                    viewBacklogandTask.Text += "<tr><td><b>Completed Date: <b></td><td>" + Convert.ToDateTime(row["BacklogCompletedDate"]).ToString("ddd, dd MMM yyyy") + "</td></tr>";
                viewBacklogandTask.Text += "</table>";
            }
            if (((Control)sender).ID.Substring(0, 4) == "task")
            {
                string id = ((Control)sender).ID.Remove(0, 9);
                viewBacklogandTaskLegend.InnerText = "Task ID #" + id;
                btnEditViewBacklog.Visible = false;
                btnEditViewTask.Visible = true;
                DataRow row = myDataSet.Tables["myTasks"].Select("TaskID = " + id)[0];
                viewBacklogandTask.Text = "Task of Backlog #" + row["BacklogID"].ToString();
                viewBacklogandTask.Text += "<table><tr><td><b>Title: </b></td><td>" + row["TaskTitle"].ToString() + "</td></tr>";
                if (row["Username"].ToString() != "")
                    viewBacklogandTask.Text += "<tr><td><b>Assignee: </b></td><td>" + row["Username"].ToString() + "</td></tr>";
                if (row["TaskComplexity"].ToString() != "")
                    viewBacklogandTask.Text += "<tr><td><b>Complexity: </b></td><td>" + row["TaskComplexity"].ToString() + "</td></tr>";
                if (row["TaskEstimationHour"].ToString() != "")
                    viewBacklogandTask.Text += "<tr><td><b>Estimation Hour: </b></td><td>" + row["TaskEstimationHour"].ToString() + " hour(s)</td></tr>";
                if (row["TaskSpentTime"].ToString() != "")
                    viewBacklogandTask.Text += "<tr><td><b>Spent Time: </b></td><td>" + row["TaskSpentTime"].ToString() + " hour(s)</td></tr>";
                viewBacklogandTask.Text += "<tr><td><b>Task Status: </b></td><td>" + row["StatusName"].ToString() + "</td></tr>";
                viewBacklogandTask.Text += "<tr><td><b>Created Date: </b></td><td>" + Convert.ToDateTime(row["TaskStartDate"]).ToString("ddd, dd MMM yyyy") + "</td></tr>";
                if (row["TaskDueDate"].ToString() != "")
                    viewBacklogandTask.Text += "<tr><td><b>Deadline: <b></td><td>" + Convert.ToDateTime(row["TaskDueDate"]).ToString("ddd, dd MMM yyyy") + "</td></tr>";
                if (row["TaskCompletedDate"].ToString() != "")
                    viewBacklogandTask.Text += "<tr><td><b>Completed Date: <b></td><td>" + Convert.ToDateTime(row["TaskCompletedDate"]).ToString("ddd, dd MMM yyyy") + "</td></tr>";
                viewBacklogandTask.Text += "</table>";

            }
            viewBacklogandTaskPopup.Show();
        }

        protected void btnEditBacklog_Click(object sender, EventArgs e)
        {
            btnAddNewBacklog.Visible = false;
            btnUpdateBacklog.Visible = true;
            lblBacklogNotice.Text = "";
            string id = "";
            if (((Control)sender).ID.Substring(7, 4) == "Back")
                id = ((Control)sender).ID.Remove(0, 14);
            if (((Control)sender).ID.Substring(7, 4) == "View")
                id = viewBacklogandTaskLegend.InnerText.Remove(0, 12);
            addandEditBacklogLegend.InnerText = "Edit backlog ID #" + id;

            DataRow row = myDataSet.Tables["myBacklogs"].Select("BacklogID = " + id)[0];

            titleTextBox.Text = row["BacklogTitle"].ToString();
            descriptionTextBox.Text = row["BacklogDescription"].ToString();

            swimlaneDropDownList.Items.Clear();
            foreach (DataRow r in myDataSet.Tables["mySwimlanes"].Rows)
            {
                swimlaneDropDownList.Items.Add(r["SwimlaneName"].ToString());
                swimlaneDropDownList.Items[swimlaneDropDownList.Items.Count - 1].Value = r["SwimlaneID"].ToString();
                if (row["SwimlaneID"].ToString() == r["SwimlaneID"].ToString())
                    swimlaneDropDownList.Items[swimlaneDropDownList.Items.Count - 1].Selected = true;
            }

            assigneeDropDownList.Items.Clear();
            foreach (DataRow r in myDataSet.Tables["myProjectNembers"].Rows)
            {
                assigneeDropDownList.Items.Add(r["Username"].ToString());
                assigneeDropDownList.Items[assigneeDropDownList.Items.Count - 1].Value = r["UserID"].ToString();
                if (row["BacklogAssigneeID"].ToString() == r["UserID"].ToString())
                    assigneeDropDownList.Items[assigneeDropDownList.Items.Count - 1].Selected = true;
            }

            for (int i = 0; i < colorDropDownList.Items.Count; i++)
            {
                if (row["BacklogColorHeader"].ToString() == colorDropDownList.Items[i].Value.Split(',')[0].ToString())
                    colorDropDownList.Items[i].Selected = true;
                else
                    colorDropDownList.Items[i].Selected = false;
            }
            complexityTextBox.Text = row["BacklogComplexity"].ToString();
            if (row["BacklogDueDate"].ToString() != "")
                deadlineTextBox.Text = Convert.ToDateTime(row["BacklogDueDate"]).ToShortDateString();

            addandEditBacklogPopup.Show();
        }

        protected void btnUpdateBacklog_Click(object sender, EventArgs e)
        {
            if (titleTextBox.Text == "")
            {
                lblBacklogNotice.Text = "Backlog Title is required!";
                addandEditBacklogPopup.Show();
            }
            else if (complexityTextBox.Text != "" && !IsNumeric(complexityTextBox.Text))
            {
                lblBacklogNotice.Text = "Backlog Complexity must be a number!";
                addandEditBacklogPopup.Show();
            }
            else if (deadlineTextBox.Text != "" && !IsDate(deadlineTextBox.Text))
            {
                lblBacklogNotice.Text = "Backlog Due date must be a date!";
                addandEditBacklogPopup.Show();
            }
            else
            {
                string id = addandEditBacklogLegend.InnerText.Remove(0, 17);
                DataRow row = myDataSet.Tables["myRawBacklogs"].Select("BacklogID = " + id)[0];

                row["SwimlaneID"] = Convert.ToInt32(swimlaneDropDownList.SelectedValue);
                row["BacklogTitle"] = titleTextBox.Text;
                row["BacklogDescription"] = descriptionTextBox.Text;
                row["BacklogColor"] = colorDropDownList.SelectedValue.Split(',')[1].ToString();
                row["BacklogColorHeader"] = colorDropDownList.SelectedValue.Split(',')[0].ToString();
                if (complexityTextBox.Text != "")
                    row["BacklogComplexity"] = Convert.ToInt32(complexityTextBox.Text);
                else
                    row["BacklogComplexity"] = DBNull.Value;
                if (deadlineTextBox.Text != "")
                    row["BacklogDueDate"] = Convert.ToDateTime(deadlineTextBox.Text);
                else
                    row["BacklogDueDate"] = DBNull.Value;
                row["BacklogAssigneeID"] = Convert.ToInt32(assigneeDropDownList.SelectedValue);

                myAdapter.SelectCommand.CommandText = "Select * From Backlogs";
                OleDbCommandBuilder myCommandBuilder = new OleDbCommandBuilder(myAdapter);
                myAdapter.UpdateCommand = myCommandBuilder.GetUpdateCommand();
                myAdapter.Update(myDataSet, "myRawBacklogs");
                myDataSet.Clear();

                getDatabase();
                ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "refreshBoard", "refreshBoard();", true);
                addandEditBacklogPopup.Hide();
            }
            
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (((Control)sender).ID.Substring(9, 4) == "Back")
                lblDeleteItem.Text = "backlog ID #" + ((Control)sender).ID.Remove(0, 16).ToString(); deleteBacklogorTaskLegend.InnerText = "Delete Backlog";
            if (((Control)sender).ID.Substring(9, 4) == "Task")
                lblDeleteItem.Text = "task ID #" + ((Control)sender).ID.Remove(0, 13).ToString(); deleteBacklogorTaskLegend.InnerText = "Delete Task";
            deleteBacklogandTaskPopup.Show();
        }

        protected void btnDeleteBacklogorTask_Click(object sender, EventArgs e)
        {
            myDeleteCommand.Connection = myConnection;
            if (lblDeleteItem.Text.Substring(0, 4) == "back")
            {
                string id = lblDeleteItem.Text.Remove(0, 12);
                myDeleteCommand.CommandText = "DELETE FROM Backlogs WHERE BacklogID = " + id;
                myDeleteCommand.ExecuteNonQuery();
                myDeleteCommand.CommandText = "DELETE FROM BacklogsComments WHERE BacklogID = " + id;
                myDeleteCommand.ExecuteNonQuery();
                myDeleteCommand.CommandText = "DELETE FROM Tasks WHERE BacklogID = " + id;
                myDeleteCommand.ExecuteNonQuery();
            }

            if (lblDeleteItem.Text.Substring(0, 4) == "task")
            {
                string id = lblDeleteItem.Text.Remove(0, 9);
                myDeleteCommand.CommandText = "DELETE FROM Tasks WHERE TaskID = " + id;
                myDeleteCommand.ExecuteNonQuery();
                myDeleteCommand.CommandText = "DELETE FROM TasksComments WHERE TaskID = " + id;
                myDeleteCommand.ExecuteNonQuery();
            }
            myDataSet.Clear();

            getDatabase();
            ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "refreshBoard", "refreshBoard();", true);
            deleteBacklogandTaskPopup.Hide();
        }

        protected void btnAddTask_Click(object sender, EventArgs e)
        {
            btnAddNewTask.Visible = true;
            btnUpdateTask.Visible = false;
            addandEditTaskLegend.InnerText = "Add new task";
            lblAddedBacklogID.Text = ((Control)sender).ID.Remove(0, 10);
            lblTaskNotice.Text = "";
            titleTaskTextBox.Text = "";

            assigneeTaskDropDownList.Items.Clear();
            foreach (DataRow row in myDataSet.Tables["myProjectNembers"].Rows)
            {
                assigneeTaskDropDownList.Items.Add(row["Username"].ToString());
                assigneeTaskDropDownList.Items[assigneeTaskDropDownList.Items.Count - 1].Value = row["UserID"].ToString();
            }

            statusTaskDropDownList.Items.Clear();
            foreach (DataRow row in myDataSet.Tables["myStatus"].Rows)
            {
                statusTaskDropDownList.Items.Add(row["StatusName"].ToString());
                statusTaskDropDownList.Items[statusTaskDropDownList.Items.Count - 1].Value = row["StatusID"].ToString();
            }

            estimationHourTaskTextBox.Text = "";
            spentTimeTaskTextBox.Text = "";
            complexityTaskTextBox.Text = "";
            deadlineTaskTextBox.Text = "";

            addandEditTaskPopup.Show();
        }

        protected void btnAddNewTask_Click(object sender, EventArgs e)
        {
            if (titleTaskTextBox.Text == "")
            {
                lblTaskNotice.Text = "Task Title is required!";
                addandEditTaskPopup.Show();
            }
            else if (complexityTaskTextBox.Text != "" && !IsNumeric(complexityTaskTextBox.Text))
            {
                lblTaskNotice.Text = "Task Complexity must be a number!";
                addandEditTaskPopup.Show();
            }
            else if (estimationHourTaskTextBox.Text != "" && !IsNumeric(estimationHourTaskTextBox.Text))
            {
                lblTaskNotice.Text = "Estimation hour must be a number!";
                addandEditTaskPopup.Show();
            }
            else if (spentTimeTaskTextBox.Text != "" && !IsNumeric(spentTimeTaskTextBox.Text))
            {
                lblTaskNotice.Text = "Spent time must be a number!";
                addandEditTaskPopup.Show();
            }
            else if (deadlineTaskTextBox.Text != "" && !IsDate(deadlineTaskTextBox.Text))
            {
                lblTaskNotice.Text = "Task Due date must be a date!";
                addandEditTaskPopup.Show();
            }
            else
            {
                DataRow row = myDataSet.Tables["myRawTasks"].NewRow();
                row["BacklogID"] = Convert.ToInt32(lblAddedBacklogID.Text);
                row["TaskTitle"] = titleTaskTextBox.Text;
                if (complexityTaskTextBox.Text != "")
                    row["TaskComplexity"] = Convert.ToInt32(complexityTaskTextBox.Text);
                row["TaskAssigneeID"] = Convert.ToInt32(assigneeTaskDropDownList.SelectedValue);
                row["TaskStatusID"] = Convert.ToInt32(statusTaskDropDownList.SelectedValue);
                if (statusTaskDropDownList.SelectedValue == "3")
                    row["TaskCompletedDate"] = DateTime.Today;
                else
                    row["TaskCompletedDate"] = DBNull.Value;
                if (estimationHourTaskTextBox.Text != "")
                    row["TaskEstimationHour"] = Convert.ToInt32(estimationHourTaskTextBox.Text);
                if (spentTimeTaskTextBox.Text != "")
                    row["TaskSpentTime"] = Convert.ToInt32(spentTimeTaskTextBox.Text);
                row["TaskStartDate"] = DateTime.Today;
                if (deadlineTaskTextBox.Text != "")
                    row["TaskDueDate"] = Convert.ToDateTime(deadlineTaskTextBox.Text);
                myDataSet.Tables["myRawTasks"].Rows.Add(row);
                myAdapter.SelectCommand.CommandText = "Select * From Tasks";
                OleDbCommandBuilder myCommandBuilder = new OleDbCommandBuilder(myAdapter);
                myAdapter.InsertCommand = myCommandBuilder.GetInsertCommand();
                myAdapter.Update(myDataSet, "myRawTasks");
                myDataSet.Clear();

                getDatabase();
                getTasks();
                ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "refreshBoard", "refreshBoard();", true);
                addandEditTaskPopup.Hide();
            }
        }

        protected void btnEditTask_Click(object sender, EventArgs e)
        {
            btnAddNewTask.Visible = false;
            btnUpdateTask.Visible = true;
            string id = "";
            if (((Control)sender).ID.Substring(7, 4) == "Task")
                id = ((Control)sender).ID.Remove(0, 11);
            if (((Control)sender).ID.Substring(7, 4) == "View")
                id = viewBacklogandTaskLegend.InnerText.Remove(0, 9);
            addandEditTaskLegend.InnerText = "Edit task ID #" + id;
            DataRow row = myDataSet.Tables["myTasks"].Select("TaskID = " + id)[0];
            lblTaskNotice.Text = "";

            titleTaskTextBox.Text = row["TaskTitle"].ToString();

            assigneeTaskDropDownList.Items.Clear();
            foreach (DataRow r in myDataSet.Tables["myProjectNembers"].Rows)
            {
                assigneeTaskDropDownList.Items.Add(r["Username"].ToString());
                assigneeTaskDropDownList.Items[assigneeTaskDropDownList.Items.Count - 1].Value = r["UserID"].ToString();
                if (row["TaskAssigneeID"].ToString() == r["UserID"].ToString())
                    assigneeTaskDropDownList.Items[assigneeTaskDropDownList.Items.Count - 1].Selected = true;
            }

            statusTaskDropDownList.Items.Clear();
            foreach (DataRow r in myDataSet.Tables["myStatus"].Rows)
            {
                statusTaskDropDownList.Items.Add(r["StatusName"].ToString());
                statusTaskDropDownList.Items[statusTaskDropDownList.Items.Count - 1].Value = r["StatusID"].ToString();
                if (row["TaskStatusID"].ToString() == r["StatusID"].ToString())
                    statusTaskDropDownList.Items[statusTaskDropDownList.Items.Count - 1].Selected = true;
            }
            estimationHourTaskTextBox.Text = row["TaskEstimationHour"].ToString();
            spentTimeTaskTextBox.Text = row["TaskSpentTime"].ToString();
            complexityTaskTextBox.Text = row["TaskComplexity"].ToString();
            if (row["TaskDueDate"].ToString() != "")
                deadlineTaskTextBox.Text = Convert.ToDateTime(row["TaskDueDate"]).ToShortDateString();

            addandEditTaskPopup.Show();
        }

        protected void btnUpdateTask_Click(object sender, EventArgs e)
        {
            if (titleTaskTextBox.Text == "")
            {
                lblTaskNotice.Text = "Task Title is required!";
                addandEditTaskPopup.Show();
            }
            else if (complexityTaskTextBox.Text != "" && !IsNumeric(complexityTaskTextBox.Text))
            {
                lblTaskNotice.Text = "Task Complexity must be a number!";
                addandEditTaskPopup.Show();
            }
            else if (estimationHourTaskTextBox.Text != "" && !IsNumeric(estimationHourTaskTextBox.Text))
            {
                lblTaskNotice.Text = "Estimation hour must be a number!";
                addandEditTaskPopup.Show();
            }
            else if (spentTimeTaskTextBox.Text != "" && !IsNumeric(spentTimeTaskTextBox.Text))
            {
                lblTaskNotice.Text = "Spent time must be a number!";
                addandEditTaskPopup.Show();
            }
            else if (deadlineTaskTextBox.Text != "" && !IsDate(deadlineTaskTextBox.Text))
            {
                lblTaskNotice.Text = "Task Due date must be a date!";
                addandEditTaskPopup.Show();
            }
            else
            {
                string id = addandEditTaskLegend.InnerText.Remove(0, 14);
                DataRow row = myDataSet.Tables["myRawTasks"].Select("TaskID = " + id)[0];

                row["TaskTitle"] = titleTaskTextBox.Text;
                row["TaskAssigneeID"] = Convert.ToInt32(assigneeTaskDropDownList.SelectedValue);
                row["TaskStatusID"] = Convert.ToInt32(statusTaskDropDownList.SelectedValue);
                if (statusTaskDropDownList.SelectedValue == "3")
                    row["TaskCompletedDate"] = DateTime.Today;
                else
                    row["TaskCompletedDate"] = DBNull.Value;
                if (complexityTaskTextBox.Text != "")
                    row["TaskComplexity"] = Convert.ToInt32(complexityTaskTextBox.Text);
                else
                    row["TaskComplexity"] = DBNull.Value;
                if (estimationHourTaskTextBox.Text != "")
                    row["TaskEstimationHour"] = Convert.ToInt32(estimationHourTaskTextBox.Text);
                else
                    row["TaskEstimationHour"] = DBNull.Value;
                if (spentTimeTaskTextBox.Text != "")
                    row["TaskSpentTime"] = Convert.ToInt32(spentTimeTaskTextBox.Text);
                else
                    row["TaskSpentTime"] = DBNull.Value;
                if (deadlineTaskTextBox.Text != "")
                    row["TaskDueDate"] = Convert.ToDateTime(deadlineTaskTextBox.Text);
                else
                    row["TaskDueDate"] = DBNull.Value;

                myAdapter.SelectCommand.CommandText = "Select * From Tasks";
                OleDbCommandBuilder myCommandBuilder = new OleDbCommandBuilder(myAdapter);
                myAdapter.UpdateCommand = myCommandBuilder.GetUpdateCommand();
                myAdapter.Update(myDataSet, "myRawTasks");
                myDataSet.Clear();

                getDatabase();
                ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "refreshBoard", "refreshBoard();", true);
                addandEditTaskPopup.Hide();
            }
        }

        protected void btnUpdateEditComplex_Click(object sender, EventArgs e)
        {
            if (txtBacklogComplexity.Text != "" && !IsNumeric(txtBacklogComplexity.Text))
            {
                lblComplexityNotice.Text = "Complexity must be a number!";
                editComplexityPopup.Show();
            }
            else
            {
                myUpdateCommand.Connection = myConnection;
                string value = "";
                if (editComplexityLegend.InnerText.Substring(24, 4) == "Back")
                {
                    if (txtBacklogComplexity.Text != "")
                        value = txtBacklogComplexity.Text;
                    myUpdateCommand.CommandText = "UPDATE Backlogs SET BacklogComplexity = " + value + " WHERE backlogID = " + editComplexityLegend.InnerText.Remove(0, 33);
                    myUpdateCommand.ExecuteNonQuery();
                }
                if (editComplexityLegend.InnerText.Substring(24, 4) == "Task")
                {
                    if (txtBacklogComplexity.Text != "")
                        value = txtBacklogComplexity.Text;
                    myUpdateCommand.CommandText = "UPDATE Tasks SET TaskComplexity = " + value + " WHERE taskID = " + editComplexityLegend.InnerText.Remove(0, 30);
                    myUpdateCommand.ExecuteNonQuery();
                }

                myDataSet.Clear();
                getDatabase();
                ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "refreshBoard", "refreshBoard();", true);
                editComplexityPopup.Hide();
            }
        }

        protected void btnComplexity_Click(object sender, EventArgs e)
        {
            string id = "";
            lblComplexityNotice.Text = "";
            if (((Control)sender).ID.Substring(3, 4) == "Back")
            {
                id = ((Control)sender).ID.Remove(0, 20);
                editComplexityLegend.InnerText = "Edit the complexity for Backlog #" + id;
                DataRow row = myDataSet.Tables["myBacklogs"].Select("BacklogID = " + id)[0];
                txtBacklogComplexity.Text = row["BacklogComplexity"].ToString();
            }
            else if (((Control)sender).ID.Substring(3, 4) == "Task")
            {
                id = ((Control)sender).ID.Remove(0, 17);
                editComplexityLegend.InnerText = "Edit the complexity for Task #" + id;
                DataRow row = myDataSet.Tables["myTasks"].Select("TaskID = " + id)[0];
                txtBacklogComplexity.Text = row["TaskComplexity"].ToString();
            }
            editComplexityPopup.Show();
        }

        protected void updateAssignee_Click(object sender, EventArgs e)
        {
            myUpdateCommand.Connection = myConnection;
            string value = "";
            if (editAssigneeLegend.InnerText.Substring(22, 4) == "Back")
            {
                if (editAssigneeDropdownList.SelectedItem.Text != "")
                    value = editAssigneeDropdownList.SelectedValue;
                myUpdateCommand.CommandText = "UPDATE Backlogs SET BacklogAssigneeID = " + value + " WHERE BacklogID = " + editAssigneeLegend.InnerText.Remove(0, 31);
                myUpdateCommand.ExecuteNonQuery();
            }
            if (editAssigneeLegend.InnerText.Substring(22, 4) == "Task")
            {
                if (editAssigneeDropdownList.SelectedItem.Text != "")
                    value = editAssigneeDropdownList.SelectedValue;
                myUpdateCommand.CommandText = "UPDATE Tasks SET TaskAssigneeID = " + value + " WHERE TaskID = " + editAssigneeLegend.InnerText.Remove(0, 28);
                myUpdateCommand.ExecuteNonQuery();
            }

            myDataSet.Clear();
            getDatabase();
            ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "refreshBoard", "refreshBoard();", true);
            editAssigneePopup.Hide();
        }

        protected void btnAssignee_Click(object sender, EventArgs e)
        {
            string id = "";
            if (((Control)sender).ID.Substring(3, 4) == "Back")
            {
                id = ((Control)sender).ID.Remove(0, 18);
                editAssigneeLegend.InnerText = "Edit the Assignee for Backlog #" + id;
                editAssigneeDropdownList.Items.Clear();

                foreach (DataRow r in myDataSet.Tables["myProjectNembers"].Rows)
                {
                    editAssigneeDropdownList.Items.Add(r["Username"].ToString());
                    DataRow row = myDataSet.Tables["myBacklogs"].Select("BacklogID = " + id)[0];
                    editAssigneeDropdownList.Items[editAssigneeDropdownList.Items.Count - 1].Value = r["UserID"].ToString();
                    if (row["BacklogAssigneeID"].ToString() == r["UserID"].ToString())
                        editAssigneeDropdownList.Items[editAssigneeDropdownList.Items.Count - 1].Selected = true;

                }
            }
            else if (((Control)sender).ID.Substring(3, 4) == "Task")
            {
                id = ((Control)sender).ID.Remove(0, 15);
                editAssigneeLegend.InnerText = "Edit the Assignee for Task #" + id;
                editAssigneeDropdownList.Items.Clear();
                foreach (DataRow r in myDataSet.Tables["myProjectNembers"].Rows)
                {
                    editAssigneeDropdownList.Items.Add(r["Username"].ToString());
                    DataRow row = myDataSet.Tables["myTasks"].Select("TaskID = " + id)[0];
                    editAssigneeDropdownList.Items[editAssigneeDropdownList.Items.Count - 1].Value = r["UserID"].ToString();
                    if (row["TaskAssigneeID"].ToString() == r["UserID"].ToString())
                        editAssigneeDropdownList.Items[editAssigneeDropdownList.Items.Count - 1].Selected = true;

                }
            }
            editAssigneePopup.Show();
        }

        protected void btnUpdateDueDate_Click(object sender, EventArgs e)
        {
            if (editDueDateTextBox.Text != "" && !IsDate(editDueDateTextBox.Text))
            {
                lblDueDateNotice.Text = "Due date must be a date!";
                editDueDatePopup.Show();
            }
            else
            {
                myUpdateCommand.Connection = myConnection;
                DateTime? value = null;
                if (editDueDateLegend.InnerText.Substring(22, 4) == "Back")
                {
                    if (editDueDateTextBox.Text != "")
                        value = Convert.ToDateTime(editDueDateTextBox.Text);
                    myUpdateCommand.CommandText = "UPDATE Backlogs SET BacklogDueDate = '" + value + "' WHERE BacklogID = " + editDueDateLegend.InnerText.Remove(0, 31);
                    myUpdateCommand.ExecuteNonQuery();
                }
                if (editDueDateLegend.InnerText.Substring(22, 4) == "Task")
                {
                    if (editDueDateTextBox.Text != "")
                        value = Convert.ToDateTime(editDueDateTextBox.Text);
                    myUpdateCommand.CommandText = "UPDATE Tasks SET TaskDueDate = '" + value + "' WHERE TaskID = " + editDueDateLegend.InnerText.Remove(0, 28);
                    myUpdateCommand.ExecuteNonQuery();
                }

                myDataSet.Clear();
                getDatabase();
                ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "refreshBoard", "refreshBoard();", true);
                editDueDatePopup.Hide();
            }
            
        }
        
        protected void btnDueDate_Click(object sender, EventArgs e)
        {
            string id = "";
            lblDueDateNotice.Text = "";
            if (((Control)sender).ID.Substring(3, 4) == "Back")
            {
                id = ((Control)sender).ID.Remove(0, 17);
                editDueDateLegend.InnerText = "Edit the Deadline for Backlog #" + id;
                DataRow row = myDataSet.Tables["myBacklogs"].Select("BacklogID = " + id)[0];
                editDueDateTextBox.Text = Convert.ToDateTime(row["BacklogDueDate"].ToString()).ToShortDateString();
            }
            else if (((Control)sender).ID.Substring(3, 4) == "Task")
            {
                id = ((Control)sender).ID.Remove(0, 14);
                editDueDateLegend.InnerText = "Edit the Deadline for Task #" + id;
                DataRow row = myDataSet.Tables["myTasks"].Select("TaskID = " + id)[0];
                editDueDateTextBox.Text = Convert.ToDateTime(row["TaskDueDate"].ToString()).ToShortDateString();
            }
            editDueDatePopup.Show();
        }

        protected void btnShowHideTask_Click(object sender, EventArgs e)
        {
            string id = "backlogArea" + ((Control)sender).ID.Remove(0, 15);
            Control c = kanbanboard.FindControl(id);
            if (c.Controls[1].Visible == true)
            {
                for (int i = 1; i < c.Controls.Count; i++)
                    c.Controls[i].Visible = false;
            }
            else
            {
                for (int i = 1; i < c.Controls.Count; i++)
                    c.Controls[i].Visible = true;
            }
        }

        protected void btnComment_Click(object sender, EventArgs e)
        {
            string id = "";
            if (((Control)sender).ID.Substring(3, 4) == "Back")
            {
                id = ((Control)sender).ID.Remove(0, 17);
                addCommentLegend.InnerText = "Comment for Backlog #" + id;
                mySelectCommand.CommandText = "SELECT BacklogsComments.CommentID as [ID], [User].Username as [User], BacklogsComments.CommentContent as [Comment]"
                                              + " FROM BacklogsComments, [User] WHERE BacklogsComments.BacklogID = " + id
                                              + " AND BacklogsComments.CommenterID =[User].UserID ORDER BY BacklogsComments.CommentID ";
            }
            if (((Control)sender).ID.Substring(3, 4) == "Task")
            {
                id = ((Control)sender).ID.Remove(0, 14);
                addCommentLegend.InnerText = "Comment for Task #" + id;
                mySelectCommand.CommandText = "SELECT TasksComments.CommentID as [ID], [User].Username as [User], TasksComments.CommentContent as [Comment]"
                                              + " FROM TasksComments, [User] WHERE TasksComments.TaskID = " + id
                                              + " AND TasksComments.CommenterID =[User].UserID ORDER BY TasksComments.CommentID ";
            }
            myAdapter.Fill(myDataSet, "myComments");
            commentGridView.DataSource = myDataSet.Tables["myComments"];
            commentGridView.DataBind();
            addCommentPopup.Show();
        }

        protected void btnDeleteComment_Click(object sender, EventArgs e)
        {
            string id = (sender as LinkButton).CommandArgument;
            myDeleteCommand.Connection = myConnection;
            if (addCommentLegend.InnerText.Substring(12, 4) == "Back")
                myDeleteCommand.CommandText = "DELETE FROM BacklogsComments WHERE CommentID = " + id;
            if (addCommentLegend.InnerText.Substring(12, 4) == "Task")
                myDeleteCommand.CommandText = "DELETE FROM TasksComments WHERE CommentID = " + id;
            myDeleteCommand.ExecuteNonQuery();
            myDataSet.Clear();
            getDatabase();
            ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "refreshBoard", "refreshBoard();", true);
        }

        protected void btnAddComment_Click(object sender, EventArgs e)
        {
            myInsertCommand.Connection = myConnection;
            if (addCommentLegend.InnerText.Substring(12, 4) == "Back")
            {
                string id = addCommentLegend.InnerText.Remove(0, 21);
                myInsertCommand.CommandText = "INSERT INTO BacklogsComments (CommentContent,CommenterID,BacklogID) "
                                            + "VALUES ('" + addCommentTextBox.Text + "','"+ Session["userID"].ToString() +"','" + id + "')";
                myInsertCommand.ExecuteNonQuery();
            }
            else if (addCommentLegend.InnerText.Substring(12, 4) == "Task")
            {
                string id = addCommentLegend.InnerText.Remove(0, 18);
                myInsertCommand.CommandText = "INSERT INTO TasksComments (CommentContent,CommenterID,TaskID) "
                                            + "VALUES ('" + addCommentTextBox.Text + "','" + Session["userID"].ToString() + "','" + id + "')";
                myInsertCommand.ExecuteNonQuery();
            }
            myDataSet.Clear();
            getDatabase();
            ScriptManager.RegisterStartupScript(updatePanel, updatePanel.GetType(), "refreshBoard", "refreshBoard();", true);
            addCommentTextBox.Text = "";

        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            //dropdownFilter.Visible = true;

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //tbxSearch.Visible = true;
            //btnFilter.Enabled = true;
        }

        protected void dropdownFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            //List<string> links = new List<string>();
            //selectSearch.Connection = myConnection;
            //if (myConnection.State == ConnectionState.Closed)
            //    myConnection.Open();

            //if (dropdownFilter.SelectedItem.Text == "Users")
            //{
            //    selectSearch.CommandText = "SELECT [Username],[Email] FROM [User] "
            //                            + "WHERE ([Username] LIKE '%" + tbxSearch.Text + "%' OR [Email] LIKE '%" + tbxSearch.Text + "%')"
            //                            + "AND UserID IN (SELECT UserID FROM ProjectsMembers " 
            //                            + "WHERE ProjectID LIKE '" + projectDropDownList.SelectedItem.Text + "') ";

            //    myReader = selectSearch.ExecuteReader();
            //    bool notEoF;
            //    notEoF = myReader.Read();
            //    while (notEoF)
            //    {
            //        //string linkItem = myReader["Username"].ToString() + ", " + myReader["Email"].ToString();
            //        //linkItem.Value = myReader["UserID"].ToString();
            //        links.Add(myReader["Username"].ToString() + ", " + myReader["Email"].ToString());
            //        notEoF = myReader.Read();
            //    }
            //}
            //else if (dropdownFilter.SelectedItem.Text == "Tasks")
            //{
            //    selectSearch.CommandText = "SELECT TaskTitle, TaskComplexity, TaskStartDate, TaskDueDate, [Username] " 
            //                            + "FROM Tasks INNER JOIN [User] ON Tasks.TaskAssigneeID = [User].UserID " 
            //                            + "WHERE ProjectID LIKE '" + projectDropDownList.SelectedItem.Text + "' " 
            //                            + "AND (TaskTitle LIKE '%" + tbxSearch.Text + "%' "
            //                            + "OR TaskComplexity LIKE '" + tbxSearch.Text + "' " 
            //                            + "OR TaskStartDate LIKE '%" + tbxSearch.Text + "%' " 
            //                            + "OR TaskDueDate LIKE '%" + tbxSearch.Text + "%')";

            //    myReader = selectSearch.ExecuteReader();
            //    bool notEoF;
            //    notEoF = myReader.Read();
            //    while (notEoF)
            //    {
            //        links.Add(myReader["TaskTitle"].ToString() + ", complexity: " + myReader["TaskComplexity"].ToString() 
            //                    + ". Period: " + myReader["TaskStartDate"] + " - " + myReader["TaskDueDate"] 
            //                    + ", assignee: " + myReader["Username"]);
            //        notEoF = myReader.Read();
            //    }
            //}
            //else if (dropdownFilter.SelectedItem.Text == "Comments")
            //{
            //    selectSearch.CommandText = "SELECT CommentContent, [Username] FROM BacklogsComments " 
            //                            + "INNER JOIN [User] ON [User].UserID = BacklogsComments.CommenterID " 
            //                            + "WHERE CommentContent LIKE '%" + tbxSearch.Text + "%' " 
            //                            + "AND BacklogID IN (SELECT BacklogID FROM Backlogs WHERE ProjectID LIKE '" + projectDropDownList.SelectedItem.Text + "')";

            //    myReader = selectSearch.ExecuteReader();
            //    bool notEoF;
            //    notEoF = myReader.Read();
            //    while (notEoF)
            //    {
            //        links.Add(myReader["CommentContent"].ToString() + ", - " + myReader["Username"].ToString());
            //        notEoF = myReader.Read();
            //    }
            //}
            //Session["links"] = links;
            ////Server.Transfer("SearchResults.aspx", true);
            //Response.Redirect("SearchResults.aspx");
        }

        [WebMethod]
        public static void updatePosition(string swimlane, List<string> swimlaneBacklog, List<string> swimlaneBacklogPos)
        {
            OleDbConnection conn = new OleDbConnection();
            OleDbCommand myCommand = new OleDbCommand();
            conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|LanbanDatabase.mdb;";
            conn.Open();
            myCommand.Connection = conn;
            int statusID = 1;
            if (swimlane == "5")
                statusID = 3;
            if (swimlane == "4")
                statusID = 2;
            for (int i = 0; i < swimlaneBacklog.Count; i++)
            {
                
                if (swimlane == "5")
                {
                    myCommand.CommandText = "UPDATE Backlogs SET BacklogPosition = " + swimlaneBacklogPos[i]
                                        + ", swimlaneID = " + swimlane + ", BacklogStatusID = " + statusID + ", BacklogCompletedDate = '" + DateTime.Today
                                        + "' WHERE backlogID = " + swimlaneBacklog[i];
                    myCommand.ExecuteNonQuery();
                    myCommand.CommandText = "UPDATE Tasks SET TaskStatusID = " + statusID + ", TaskCompletedDate = '" + DateTime.Today
                                        + "' WHERE backlogID = " + swimlaneBacklog[i] + " AND TaskStatusID <> 3";
                    myCommand.ExecuteNonQuery();
                }
                else
                {
                    myCommand.CommandText = "UPDATE Backlogs SET BacklogPosition = " + swimlaneBacklogPos[i]
                                        + ", swimlaneID = " + swimlane + ", BacklogStatusID = " + statusID + ", BacklogCompletedDate = null"
                                        + " WHERE backlogID = " + swimlaneBacklog[i];
                    myCommand.ExecuteNonQuery();
                }
            }
            conn.Close();
        }

        public bool IsNumeric(string input) 
        {
            int test;
            return int.TryParse(input, out test); 
        }

        public bool IsDate(string input)
        {
            DateTime dt;
            return DateTime.TryParse(input, out dt);
        }

        protected void EatCookies(object sender, EventArgs e){
            if (Request.Cookies["UserSettings"] != null)
            {
                Response.Cookies["UserSettings"].Expires = DateTime.Now.AddDays(-1);
                myConnection.Close();
                Response.Redirect("login.aspx");
            }
        }

        protected void btnChart_Click(object sender, EventArgs e)
        {
            myConnection.Close();
            Response.Redirect("charts.aspx");
        }

        protected void btnAttachedFile_Click(object sender, EventArgs e)
        {
            //attachFile(((Control)sender).ID.Remove(0, 19));
            string id = ((Control)sender).ID.Remove(0, 19);
            List<ListItem> files = new List<ListItem>();
            if (Directory.Exists(Server.MapPath("~/Files/task" + id + "/")))
            {
                string[] filePaths = Directory.GetFiles(Server.MapPath("~/Files/task" + id + "/"));
                foreach (string filePath in filePaths)
                {
                    files.Add(new ListItem(Path.GetFileName(filePath), filePath));
                }
            }
            showAttachedFilesGridView.DataSource = files;
            showAttachedFilesGridView.DataBind();
            showAttachedFilesLegend.InnerText = "Attached File of Task #" + id;
            showAttachedFilesPopup.Show();
        }

        private void attachFile(object id) //- transfered from btnAttachedFile code
        {
            List<ListItem> files = new List<ListItem>();
            if (Directory.Exists(Server.MapPath("~/Files/task" + id + "/")))
            {
                string[] filePaths = Directory.GetFiles(Server.MapPath("~/Files/task" + id + "/"));
                foreach (string filePath in filePaths)
                {
                    files.Add(new ListItem(Path.GetFileName(filePath), filePath));
                }
            }
            showAttachedFilesGridView.DataSource = files;
            showAttachedFilesGridView.DataBind();
            showAttachedFilesLegend.InnerText = "Attached File of Task #" + id;
            showAttachedFilesPopup.Show();
        }

        protected void btnUploadFile_Click(object sender, EventArgs e)
        {
            string id = showAttachedFilesLegend.InnerText.Remove(0, 23);
            if (AttachedFileUpload.HasFile)
            {
                string fileName = Path.GetFileName(AttachedFileUpload.PostedFile.FileName);
               
                if (Directory.Exists(Server.MapPath("~/Files/task" + id + "/")) == false)
                    Directory.CreateDirectory(Server.MapPath("~/Files/task" + id + "/"));
                AttachedFileUpload.PostedFile.SaveAs(Server.MapPath("~/Files/task" + id + "/") + fileName);
                //Response.Redirect(Request.Url.AbsoluteUri);
            }
            //else
            //{
            //    attachFile(id);
            //}

            //saveBacklogsToSession();
            //saveAttachedFilesPopup(id);
        }

        protected void btnDownloadFile_Click(object sender, EventArgs e)
        {
            string filePath = (sender as LinkButton).CommandArgument;
            Response.ContentType = ContentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
            Response.WriteFile(filePath);
            Response.End();
        }

        protected void btnDeleteFile_Click(object sender, EventArgs e)
        {
            //string id = showAttachedFilesLegend.InnerText.Remove(0, 23);
            string filePath = (sender as LinkButton).CommandArgument;
            File.Delete(filePath);

            //saveBacklogsToSession();
            //saveAttachedFilesPopup(id);
            //Response.Redirect(Request.Url.AbsoluteUri);
        }

        private void saveAttachedFilesPopup(object blid)
        {
            if (showAttachedFilesLegend.Visible == true)
            {
                Session["Popup"] = "true";
                Session["BacklogUpload"] = blid;
            }
            else Session["Popup"] = "false";
        }

        private void saveBacklogsToSession()
        {
            string backlogs = "";

            for (int j = 1; j <= myDataSet.Tables["myRawBacklogs"].Rows.Count; j++)
            {
                string backlogid = "backlogArea" + j;
                Control c = kanbanboard.FindControl(backlogid);
                if (c.Controls.Count > 1 && c.Controls[1].Visible == true)
                {
                    backlogs += backlogid + " ";
                }
            }

            backlogs = backlogs.Remove(backlogs.Length - 1);


            Session["Controls"] = backlogs.Split(' ');
        }

        protected void linkBtnUsername_Click(object sender, EventArgs e)
        {
            myConnection.Close();
            Response.Redirect("Profile.aspx");
        }

        protected void projectDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            myConnection.Close();
            Response.Redirect("board.aspx?projectID=" + projectDropDownList.SelectedValue);
        }

    }
}