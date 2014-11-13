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

namespace Kanbean_Project
{
    public partial class board : System.Web.UI.Page
    {
        OleDbConnection myConnection = new OleDbConnection();
        OleDbCommand mySelectCommand = new OleDbCommand();
        OleDbCommand myDeleteCommand = new OleDbCommand();
        OleDbCommand myInsertCommand = new OleDbCommand();
        OleDbDataAdapter myAdapter = new OleDbDataAdapter();

        OleDbDataReader myReader;
        OleDbCommand selectSearch = new OleDbCommand();

        DataSet myDataSet = new DataSet();
        private void getDatabase()
        {
            mySelectCommand.Connection = myConnection;
            myAdapter.SelectCommand = mySelectCommand;
            mySelectCommand.CommandText = "SELECT * FROM Swimlanes WHERE ProjectID = 1 ORDER BY SwimlaneID";
            myAdapter.Fill(myDataSet, "mySwimlanes");
            mySelectCommand.CommandText = "SELECT ProjectsMembers.*, Projects.ProjectName, [User].UserID, [User].Username, [User].Level "
                                        + "FROM ProjectsMembers, Projects, [User] " 
                                        + "WHERE ProjectsMembers.ProjectID = Projects.ProjectID AND ProjectsMembers.UserID = [User].UserID AND Projects.ProjectID = 1 " 
                                        + "ORDER BY [User].UserID";
            myAdapter.Fill(myDataSet, "myUsers");
            mySelectCommand.CommandText = "SELECT Backlogs.*, [User].Username, Swimlanes.SwimlaneName, Projects.ProjectName, Status.StatusName "
                                        + "FROM Backlogs, [User], Swimlanes, Projects, Status "
                                        + "WHERE Backlogs.ProjectID = 1 AND Backlogs.BacklogAssigneeID = [User].UserID AND Backlogs.SwimlaneID = Swimlanes.SwimlaneID "
                                        + "AND Backlogs.ProjectID = Projects.ProjectID AND Backlogs.BacklogStatusID = Status.StatusID " 
                                        + "ORDER BY Backlogs.SwimlaneID, Backlogs.BacklogPosition";
            myAdapter.Fill(myDataSet, "myBacklogs");
            mySelectCommand.CommandText = "Select * From Backlogs";
            myAdapter.Fill(myDataSet, "myRawBacklogs");
            mySelectCommand.CommandText = "SELECT Tasks.*, [User].Username, Status.StatusName "
                                        + "FROM Tasks, [User], Backlogs, Status " 
                                        + "WHERE Backlogs.ProjectID = 1 AND Backlogs.BacklogID = Tasks.BacklogID " 
                                        + "AND Tasks.TaskAssigneeID = [User].UserID AND Tasks.TaskStatusID = Status.StatusID " 
                                        + "ORDER BY Tasks.TaskID";
            myAdapter.Fill(myDataSet, "myTasks");
            mySelectCommand.CommandText = "Select * From Tasks";
            myAdapter.Fill(myDataSet, "myRawTasks");
            mySelectCommand.CommandText = "Select * From Status";
            myAdapter.Fill(myDataSet, "myStatus");
        }

        private void createBacklog(string id, string complexity, string title, string deadline, string color, string colorHeader, string swimlaneID, string assignee)
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

            LinkButton btnComplexity = new LinkButton();
            btnComplexity.CssClass = "btnComplexity";
            btnComplexity.ID = "btnBacklogComplexity" + id;
            btnComplexity.ToolTip = "Edit complexity";
            btnComplexity.Text = complexity;
            btnComplexity.Click += new EventHandler(btnComplexity_Click);
            backlogFooterUp.Controls.Add(btnComplexity);

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

            LinkButton btnComment = new LinkButton();
            btnComment.CssClass = "backlogIcon iconComment";
            btnComment.ID = "btnBacklogComment" + id;
            btnComment.ToolTip = "Show the comments";
            btnComment.Click += new EventHandler(btnComment_Click);
            mySelectCommand.CommandText = "SELECT COUNT(CommentID) FROM BacklogsComments WHERE BacklogID=" + id;
            string amountComments = mySelectCommand.ExecuteScalar().ToString();
            if (amountComments != "0")
                btnComment.Text = " " + amountComments;
            backlogFooterDown.Controls.Add(btnComment);

            LinkButton btnTask = new LinkButton();
            btnTask.CssClass = "backlogIcon iconTask";
            btnTask.ID = "btnShowHideTask" + id;
            btnTask.ToolTip = "Show the tasks";
            btnTask.Click += new EventHandler(btnShowHideTask_Click);
            mySelectCommand.CommandText = "SELECT COUNT(TaskID) FROM Tasks WHERE BacklogID=" + id;
            string amountTasks = mySelectCommand.ExecuteScalar().ToString();
            mySelectCommand.CommandText = "SELECT COUNT(TaskID) FROM Tasks WHERE BacklogID=" + id + "AND TaskStatusID=3";
            string amountCompletedTasks = mySelectCommand.ExecuteScalar().ToString();
            if (amountTasks != "0")
                btnTask.Text = " " + amountCompletedTasks + "/" + amountTasks;
            backlogFooterDown.Controls.Add(btnTask);

            kanbanboard.FindControl("columnContent" + swimlaneID).Controls.Add(newBacklog);
        }

        private void createTask(string id, string BacklogID, string complexity, string title, string deadline, string assignee, string statusID)
        {
            Panel task = new Panel();
            task.CssClass = "tasks";
            if (statusID == "1")
                task.Style.Add("background-color", "#fffafa");
            if (statusID == "2")
                task.Style.Add("background-color", "#eee9e9");
            if (statusID == "3")
                task.Style.Add("background-color", "#ddd7d7");
            task.ID = "task" + id;

            Panel taskHeader = new Panel();
            taskHeader.CssClass = "backlog-header";
            taskHeader.Style.Add("background-color", "#cccccc");
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

            LinkButton btnComment = new LinkButton();
            btnComment.CssClass = "backlogIcon iconComment";
            btnComment.ID = "btnTaskComment" + id;
            btnComment.ToolTip = "Show the comments";
            btnComment.Click += new EventHandler(btnComment_Click);
            taskFooterDown.Controls.Add(btnComment);

            kanbanboard.FindControl("backlogArea" + BacklogID).Controls.Add(task);
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
            myConnection.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|LanbanDatabase.mdb;";
            myConnection.Open();
            //mySelectCommand.Connection = myConnection;
            //myAdapter.SelectCommand = mySelectCommand;
            //mySelectCommand.CommandText = "SELECT * FROM Swimlanes WHERE ProjectID = 1 ORDER BY SwimlaneID";
            //myAdapter.Fill(myDataSet, "mySwimlanes");
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
            //myDataSet.Clear();

            getBacklogs();
            getTasks();

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string Username;
            if (Request.Cookies["UserSetting"] != null)
            {
                if (Request.Cookies["UserSetting"]["Name"] != null)
                {
                    Username = Request.Cookies["UserSetting"]["Name"];
                    LblUsername.Text = Username;
                }
            }

        }

        protected void btnAddBacklog_Click(object sender, EventArgs e)
        {
            btnAddNewBacklog.Visible = true;
            btnUpdateBacklog.Visible = false;
            addandEditBacklogLegend.InnerText = "Add new backlog";
            titleTextBox.Text = "";
            descriptionTextBox.Text = "";

            swimlaneDropDownList.Items.Clear();
            foreach (DataRow row in myDataSet.Tables["mySwimlanes"].Rows)
            {
                swimlaneDropDownList.Items.Add(row["SwimlaneName"].ToString());
                swimlaneDropDownList.Items[swimlaneDropDownList.Items.Count - 1].Value = row["SwimlaneID"].ToString();
            }

            assigneeDropDownList.Items.Clear();
            foreach (DataRow row in myDataSet.Tables["myUsers"].Rows)
            {
                assigneeDropDownList.Items.Add(row["Username"].ToString());
                assigneeDropDownList.Items[assigneeDropDownList.Items.Count - 1].Value = row["User.UserID"].ToString();
            }

            for (int i = 0; i < colorDropDownList.Items.Count; i++)
                colorDropDownList.Items[i].Selected = false;
            complexityTextBox.Text = "";
            deadlineTextBox.Text = "";

            addandEditBacklogPopup.Show();
        }

        protected void btnAddNewBacklog_Click(object sender, EventArgs e)
        {
            DataRow row = myDataSet.Tables["myRawBacklogs"].NewRow();
            row["ProjectID"] = 1;
            row["SwimlaneID"] = Convert.ToInt32(swimlaneDropDownList.SelectedValue);
            if (swimlaneDropDownList.SelectedValue == "5")
                row["BacklogStatusID"] = 3;
            else if (swimlaneDropDownList.SelectedValue == "4")
                row["BacklogStatusID"] = 2;
            else
                row["BacklogStatusID"] = 1;
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
            getTasks();
            addandEditBacklogPopup.Hide();
        }

        protected void showDetail_Click(object sender, EventArgs e)
        {
            if (((Control)sender).ID.Substring(0, 4) == "back")
            {
                string id = ((Control)sender).ID.Remove(0, 12);
                viewBacklogandTaskLegend.InnerText = "Backlog ID #" + id;
                btnEditViewBacklog.Visible = true;
                btnEditViewTask.Visible = false;

                foreach (DataRow row in myDataSet.Tables["myBacklogs"].Rows)
                {
                    if (row["BacklogID"].ToString() == id)
                    {
                        viewBacklogandTask.Text = "Backlog in Project \"" + row["ProjectName"].ToString() + "\"";
                        viewBacklogandTask.Text += "<ul><li><b>Title: </b>" + row["BacklogTitle"].ToString() + "</li>";
                        if (row["BacklogDescription"].ToString() != "")
                            viewBacklogandTask.Text += "<li><b>Description: </b>" + row["BacklogDescription"].ToString() + "</li>";
                        if (row["Username"].ToString() != "")
                            viewBacklogandTask.Text += "<li><b>Assignee: </b>" + row["Username"].ToString() + "</li>";
                        if (row["BacklogComplexity"].ToString() != "")
                            viewBacklogandTask.Text += "<li><b>Complexity: </b>" + row["BacklogComplexity"].ToString() + "</li>";
                        viewBacklogandTask.Text += "<li><b>Backlog Status: </b>" + row["StatusName"].ToString() + "</li>";
                        viewBacklogandTask.Text += "<li><b>Column on the board: </b>" + row["SwimlaneName"].ToString() + "</li>";
                        viewBacklogandTask.Text += "<li>Created on <b>" + Convert.ToDateTime(row["BacklogStartDate"]).ToString("ddd, dd MMM yyyy") + "</b></li>";
                        if (row["BacklogDueDate"].ToString() != "")
                            viewBacklogandTask.Text += "<li>Must be done before <b>" + Convert.ToDateTime(row["BacklogDueDate"]).ToString("ddd, dd MMM yyyy") + "</b></li>";
                        viewBacklogandTask.Text += "</ul>";
                    }
                }
            }
            if (((Control)sender).ID.Substring(0, 4) == "task")
            {
                string id = ((Control)sender).ID.Remove(0, 9);
                viewBacklogandTaskLegend.InnerText = "Task ID #" + id;
                btnEditViewBacklog.Visible = false;
                btnEditViewTask.Visible = true;

                foreach (DataRow row in myDataSet.Tables["myTasks"].Rows)
                {
                    if (row["TaskID"].ToString() == id)
                    {
                        viewBacklogandTask.Text = "Task of Backlog #" + row["BacklogID"].ToString();
                        viewBacklogandTask.Text += "<ul><li><b>Title: </b>" + row["TaskTitle"].ToString() + "</li>";
                        if (row["Username"].ToString() != "")
                            viewBacklogandTask.Text += "<li><b>Assignee: </b>" + row["Username"].ToString() + "</li>";
                        if (row["TaskComplexity"].ToString() != "")
                            viewBacklogandTask.Text += "<li><b>Complexity: </b>" + row["TaskComplexity"].ToString() + "</li>";
                        if (row["TaskEstimationHour"].ToString() != "")
                            viewBacklogandTask.Text += "<li><b>Estimation Hour: </b>" + row["TaskEstimationHour"].ToString() + " hour(s)</li>";
                        if (row["TaskSpentTime"].ToString() != "")
                            viewBacklogandTask.Text += "<li><b>Time Spent: </b>" + row["TaskSpentTime"].ToString() + " hour(s)</li>";
                        viewBacklogandTask.Text += "<li><b>Task Status: </b>" + row["StatusName"].ToString() + "</li>";
                        viewBacklogandTask.Text += "<li>Created on <b>" + Convert.ToDateTime(row["TaskStartDate"]).ToString("ddd, dd MMM yyyy") + "</b></li>";
                        if (row["TaskDueDate"].ToString() != "")
                            viewBacklogandTask.Text += "<li>Must be done before <b>" + Convert.ToDateTime(row["TaskDueDate"]).ToString("ddd, dd MMM yyyy") + "</b></li>";
                        viewBacklogandTask.Text += "</ul>";
                    }
                }
            }
            viewBacklogandTaskPopup.Show();
        }

        protected void btnEditBacklog_Click(object sender, EventArgs e)
        {
            btnAddNewBacklog.Visible = false;
            btnUpdateBacklog.Visible = true;
            string id = "";
            if (((Control)sender).ID.Substring(7, 4) == "Back")
                id = ((Control)sender).ID.Remove(0, 14);
            if (((Control)sender).ID.Substring(7, 4) == "View")
                id = viewBacklogandTaskLegend.InnerText.Remove(0, 12);
            addandEditBacklogLegend.InnerText = "Edit backlog ID #" + id;

            foreach (DataRow row in myDataSet.Tables["myBacklogs"].Rows)
            {
                if (row["BacklogID"].ToString() == id)
                {
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
                    foreach (DataRow r in myDataSet.Tables["myUsers"].Rows)
                    {
                        assigneeDropDownList.Items.Add(r["Username"].ToString());
                        assigneeDropDownList.Items[assigneeDropDownList.Items.Count - 1].Value = r["User.UserID"].ToString();
                        if (row["BacklogAssigneeID"].ToString() == r["User.UserID"].ToString())
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
                }
            }
            addandEditBacklogPopup.Show();
        }

        protected void btnUpdateBacklog_Click(object sender, EventArgs e)
        {
            string id = addandEditBacklogLegend.InnerText.Remove(0, 17);
            foreach (DataRow row in myDataSet.Tables["myRawBacklogs"].Rows)
            {
                if (row["BacklogID"].ToString() == id)
                {
                    row["SwimlaneID"] = Convert.ToInt32(swimlaneDropDownList.SelectedValue);
                    row["BacklogTitle"] = titleTextBox.Text;
                    row["BacklogDescription"] = descriptionTextBox.Text;
                    row["BacklogColor"] = colorDropDownList.SelectedValue.Split(',')[1].ToString();
                    row["BacklogColorHeader"] = colorDropDownList.SelectedValue.Split(',')[0].ToString();
                    if (complexityTextBox.Text != "")
                        row["BacklogComplexity"] = Convert.ToInt32(complexityTextBox.Text);
                    if (deadlineTextBox.Text != "")
                        row["BacklogDueDate"] = Convert.ToDateTime(deadlineTextBox.Text);
                    row["BacklogAssigneeID"] = Convert.ToInt32(assigneeDropDownList.SelectedValue);
                }
            }

            myAdapter.SelectCommand.CommandText = "Select * From Backlogs";
            OleDbCommandBuilder myCommandBuilder = new OleDbCommandBuilder(myAdapter);
            myAdapter.UpdateCommand = myCommandBuilder.GetUpdateCommand();
            myAdapter.Update(myDataSet, "myRawBacklogs");
            myDataSet.Clear();

            getDatabase();
            getBacklogs();
            getTasks();
            addandEditBacklogPopup.Hide();
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
                myDeleteCommand.CommandText = "DELETE FROM Tasks WHERE TaskID = " + lblDeleteItem.Text.Remove(0, 9);
                myDeleteCommand.ExecuteNonQuery();
            }
            myDataSet.Clear();

            getDatabase();
            getBacklogs();
            getTasks();
            deleteBacklogandTaskPopup.Hide();
        }

        protected void btnAddTask_Click(object sender, EventArgs e)
        {
            btnAddNewTask.Visible = true;
            btnUpdateTask.Visible = false;
            addandEditTaskLegend.InnerText = "Add new task";
            lblAddedBacklogID.Text = ((Control)sender).ID.Remove(0, 10);
            titleTaskTextBox.Text = "";

            assigneeTaskDropDownList.Items.Clear();
            foreach (DataRow row in myDataSet.Tables["myUsers"].Rows)
            {
                assigneeTaskDropDownList.Items.Add(row["Username"].ToString());
                assigneeTaskDropDownList.Items[assigneeTaskDropDownList.Items.Count - 1].Value = row["User.UserID"].ToString();
            }

            statusTaskDropDownList.Items.Clear();
            foreach (DataRow row in myDataSet.Tables["myStatus"].Rows)
            {
                statusTaskDropDownList.Items.Add(row["StatusName"].ToString());
                statusTaskDropDownList.Items[statusTaskDropDownList.Items.Count - 1].Value = row["StatusID"].ToString();
            }

            complexityTaskTextBox.Text = "";
            deadlineTaskTextBox.Text = "";

            addandEditTaskPopup.Show();
        }

        protected void btnAddNewTask_Click(object sender, EventArgs e)
        {
            DataRow row = myDataSet.Tables["myRawTasks"].NewRow();
            row["BacklogID"] = Convert.ToInt32(lblAddedBacklogID.Text);
            row["TaskTitle"] = titleTaskTextBox.Text;
            if (complexityTaskTextBox.Text != "")
                row["TaskComplexity"] = Convert.ToInt32(complexityTaskTextBox.Text);
            row["TaskAssigneeID"] = Convert.ToInt32(assigneeTaskDropDownList.SelectedValue);
            row["TaskStatusID"] = Convert.ToInt32(statusTaskDropDownList.SelectedValue);
            if (estimationHourTaskTextBox.Text != "")
                row["TaskEstimationHour"] = Convert.ToInt32(estimationHourTaskTextBox.Text);
            if (timeSpentTaskTextBox.Text != "")
                row["TaskSpentTime"] = Convert.ToInt32(timeSpentTaskTextBox.Text);
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
            getBacklogs();
            addandEditTaskPopup.Hide();

        }

        protected void btnEditTask_Click(object sender, EventArgs e)
        {

        }

        protected void btnUpdateTask_Click(object sender, EventArgs e)
        {

        }

        protected void btnUpdateEditComplex_Click(object sender, EventArgs e)
        {
            if (editComplexityLegend.InnerText.Substring(24, 4) == "Back")
            {
                string id = editComplexityLegend.InnerText.Remove(0, 33);
                foreach (DataRow row in myDataSet.Tables["myRawBacklogs"].Rows)
                {
                    if (row["BacklogID"].ToString() == id)
                        row["BacklogComplexity"] = Convert.ToInt32(txtBacklogComplexity.Text);
                }
                myAdapter.SelectCommand.CommandText = "Select * From Backlogs";
            }
            if (editComplexityLegend.InnerText.Substring(24, 4) == "Task")
            {
                string idTask = editComplexityLegend.InnerText.Remove(0, 30);
                foreach (DataRow row in myDataSet.Tables["myRawTasks"].Rows)
                {
                    if (row["TaskID"].ToString() == idTask)
                        row["TaskComplexity"] = Convert.ToInt32(txtBacklogComplexity.Text);
                }
                myAdapter.SelectCommand.CommandText = "Select * From Tasks";
            }
            OleDbCommandBuilder myCommandBuilder = new OleDbCommandBuilder(myAdapter);
            myAdapter.UpdateCommand = myCommandBuilder.GetUpdateCommand();
            myAdapter.Update(myDataSet, "myRawBacklogs");
            myDataSet.Clear();
            getDatabase();
            getBacklogs();
            getTasks();
        }
        protected void btnComplexity_Click(object sender, EventArgs e)
        {
            string id = "";
            if (((Control)sender).ID.Substring(3, 4) == "Back")
            {
                id = ((Control)sender).ID.Remove(0, 20);
                editComplexityLegend.InnerText = "Edit the complexity for Backlog #" + id;
                foreach (DataRow row in myDataSet.Tables["myBacklogs"].Rows)
                {
                    if (row["BacklogID"].ToString() == id)
                        txtBacklogComplexity.Text = row["BacklogComplexity"].ToString();
                }
            }
            else if (((Control)sender).ID.Substring(3, 4) == "Task")
            {
                id = ((Control)sender).ID.Remove(0, 17);
                editComplexityLegend.InnerText = "Edit the complexity for Task #" + id;
                foreach (DataRow row in myDataSet.Tables["myTasks"].Rows)
                {
                    if (row["TaskID"].ToString() == id)
                        txtBacklogComplexity.Text = row["TaskComplexity"].ToString();
                }
            }
            editComplexityPopup.Show();
        }

        protected void btnAssignee_Click(object sender, EventArgs e)
        {
            //lblTest.Text = ((Control)sender).ID;
        }

        protected void btnDueDate_Click(object sender, EventArgs e)
        {
            //lblTest.Text = ((Control)sender).ID;
        }

        protected void btnShowHideTask_Click(object sender, EventArgs e)
        {
            //lblTest.Text = ((Control)sender).ID;
        }

        protected void btnComment_Click(object sender, EventArgs e)
        {
            //lblTest.Text = ((Control)sender).ID;
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            dropdownFilter.Visible = true;

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            tbxSearch.Visible = true;
            btnFilter.Enabled = true;
        }

        protected void dropdownFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> links = new List<string>();
            selectSearch.Connection = myConnection;
            if (myConnection.State == ConnectionState.Closed)
                myConnection.Open();

            if (dropdownFilter.SelectedItem.Text == "Users")
            {
                selectSearch.CommandText = "SELECT [Username],[Email] FROM [User] "
                                        + "WHERE ([Username] LIKE '%" + tbxSearch.Text + "%' OR [Email] LIKE '%" + tbxSearch.Text + "%')"
                                        + "AND UserID IN (SELECT UserID FROM ProjectsMembers " 
                                        + "WHERE ProjectID LIKE '" + projectDropDownList.SelectedItem.Text + "') ";

                myReader = selectSearch.ExecuteReader();
                bool notEoF;
                notEoF = myReader.Read();
                while (notEoF)
                {
                    //string linkItem = myReader["Username"].ToString() + ", " + myReader["Email"].ToString();
                    //linkItem.Value = myReader["UserID"].ToString();
                    links.Add(myReader["Username"].ToString() + ", " + myReader["Email"].ToString());
                    notEoF = myReader.Read();
                }
            }
            else if (dropdownFilter.SelectedItem.Text == "Tasks")
            {
                selectSearch.CommandText = "SELECT TaskTitle, TaskComplexity, TaskStartDate, TaskDueDate, [Username] " 
                                        + "FROM Tasks INNER JOIN [User] ON Tasks.TaskAssigneeID = [User].UserID " 
                                        + "WHERE ProjectID LIKE '" + projectDropDownList.SelectedItem.Text + "' " 
                                        + "AND (TaskTitle LIKE '%" + tbxSearch.Text + "%' "
                                        + "OR TaskComplexity LIKE '" + tbxSearch.Text + "' " 
                                        + "OR TaskStartDate LIKE '%" + tbxSearch.Text + "%' " 
                                        + "OR TaskDueDate LIKE '%" + tbxSearch.Text + "%')";

                myReader = selectSearch.ExecuteReader();
                bool notEoF;
                notEoF = myReader.Read();
                while (notEoF)
                {
                    links.Add(myReader["TaskTitle"].ToString() + ", complexity: " + myReader["TaskComplexity"].ToString() 
                                + ". Period: " + myReader["TaskStartDate"] + " - " + myReader["TaskDueDate"] 
                                + ", assignee: " + myReader["Username"]);
                    notEoF = myReader.Read();
                }
            }
            else if (dropdownFilter.SelectedItem.Text == "Comments")
            {
                selectSearch.CommandText = "SELECT CommentContent, [Username] FROM BacklogsComments " 
                                        + "INNER JOIN [User] ON [User].UserID = BacklogsComments.CommenterID " 
                                        + "WHERE CommentContent LIKE '%" + tbxSearch.Text + "%' " 
                                        + "AND BacklogID IN (SELECT BacklogID FROM Backlogs WHERE ProjectID LIKE '" + projectDropDownList.SelectedItem.Text + "')";

                myReader = selectSearch.ExecuteReader();
                bool notEoF;
                notEoF = myReader.Read();
                while (notEoF)
                {
                    links.Add(myReader["CommentContent"].ToString() + ", - " + myReader["Username"].ToString());
                    notEoF = myReader.Read();
                }
            }
            myConnection.Close();
            Session["links"] = links;
            //Server.Transfer("SearchResults.aspx", true);
            Response.Redirect("SearchResults.aspx");
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
                myCommand.CommandText = "UPDATE Backlogs SET BacklogPosition = " + swimlaneBacklogPos[i] 
                                        + ", swimlaneID = " + swimlane + ", BacklogStatusID = " + statusID 
                                        + " WHERE backlogID = " + swimlaneBacklog[i];
                myCommand.ExecuteNonQuery();
            }
            conn.Close();
        }

    }
}