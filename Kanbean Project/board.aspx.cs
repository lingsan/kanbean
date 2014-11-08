﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Data;

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
        DataSet myDataSet = new DataSet();
        private void getDatabase()
        {
            mySelectCommand.Connection = myConnection;
            myAdapter.SelectCommand = mySelectCommand;
            mySelectCommand.CommandText = "SELECT * FROM Swimlanes WHERE ProjectID = 1 ORDER BY SwimlaneID";
            myAdapter.Fill(myDataSet, "mySwimlanes");
            mySelectCommand.CommandText = "SELECT ProjectsMembers.*, Projects.ProjectName, [User].UserID, [User].Username, [User].Level FROM ProjectsMembers, Projects, [User] WHERE ProjectsMembers.ProjectID = Projects.ProjectID AND ProjectsMembers.UserID = [User].UserID AND Projects.ProjectID = 1 ORDER BY [User].UserID";
            myAdapter.Fill(myDataSet, "myUsers");
            mySelectCommand.CommandText = "SELECT Backlogs.*, [User].Username, Swimlanes.SwimlaneName, Projects.ProjectName FROM Backlogs, [User], Swimlanes, Projects WHERE Backlogs.ProjectID = 1 AND Backlogs.BacklogAssigneeID = [User].UserID AND Backlogs.SwimlaneID = Swimlanes.SwimlaneID AND Backlogs.ProjectID = Projects.ProjectID ORDER BY Backlogs.BacklogID";
            myAdapter.Fill(myDataSet, "myBacklogs");
            mySelectCommand.CommandText = "Select * From Backlogs";
            myAdapter.Fill(myDataSet, "myRawBacklogs");
            mySelectCommand.CommandText = "SELECT Tasks.*, [User].Username FROM Tasks, [User] WHERE Tasks.ProjectID = 1 AND Tasks.TaskAssigneeID = [User].UserID ORDER BY Tasks.TaskID";
            myAdapter.Fill(myDataSet, "myTasks");
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
            backlogFooterDown.ID = "backlogFooterrDown" + id;
            backlog.Controls.Add(backlogFooterDown);

            Label lblID = new Label();
            lblID.CssClass = "lblID";
            lblID.ID = "backlogID" + id;
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
            backlogFooterDown.Controls.Add(btnComment);

            LinkButton btnTask = new LinkButton();
            btnTask.CssClass = "backlogIcon iconTask";
            btnTask.ID = "btnTask" + id;
            btnTask.ToolTip = "Show the tasks";
            btnTask.Click += new EventHandler(btnTask_Click);
            backlogFooterDown.Controls.Add(btnTask);

            foreach (TableCell cell in kanbanboard.Rows[1].Cells)
            {
                if (cell.ID.Remove(0, 13) == swimlaneID)
                    cell.Controls.Add(newBacklog);
            }
        }

        private void getBacklogs()
        {
            DataTable backlogsTable = myDataSet.Tables["myBacklogs"];
            foreach (DataRow row in backlogsTable.Rows)
                createBacklog(row["BacklogID"].ToString(), row["BacklogComplexity"].ToString(), row["BacklogTitle"].ToString(), row["BacklogDueDate"].ToString(), row["BacklogColor"].ToString(), row["BacklogColorHeader"].ToString(), row["SwimlaneID"].ToString(), row["Username"].ToString());
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            myConnection.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|LanbanDatabase.mdb;";
            myConnection.Open();
            mySelectCommand.Connection = myConnection;
            myAdapter.SelectCommand = mySelectCommand;
            mySelectCommand.CommandText = "SELECT * FROM Swimlanes WHERE ProjectID = 1 ORDER BY SwimlaneID";
            myAdapter.Fill(myDataSet, "mySwimlanes");
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
                tRow.Cells.Add(tCell);
            }
            kanbanboard.Controls.Add(thRow);
            kanbanboard.Controls.Add(tRow);
            myDataSet.Clear();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            getDatabase();
            getBacklogs();
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
            //row["BacklogPosition"] = 1;
            myDataSet.Tables["myRawBacklogs"].Rows.Add(row);
            myAdapter.SelectCommand.CommandText = "Select * From Backlogs";
            OleDbCommandBuilder myCommandBuilder = new OleDbCommandBuilder(myAdapter);
            myAdapter.InsertCommand = myCommandBuilder.GetInsertCommand();
            myAdapter.Update(myDataSet, "myRawBacklogs");
            myDataSet.Clear();

            getDatabase();
            getBacklogs();
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
                        if (row["BacklogComplexity"].ToString() != "")
                            viewBacklogandTask.Text += "<li><b>Complexity: </b>" + row["BacklogComplexity"].ToString() + "</li>";
                        if (row["Username"].ToString() != "")
                            viewBacklogandTask.Text += "<li><b>Assignee: </b>" + row["Username"].ToString() + "</li>";
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
                id = viewBacklogandTaskLegend.InnerText.Remove(0,12);

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
                    //row["BacklogPosition"] = 1;
                }
            }
            myDataSet.Tables["myRawBacklogs"].AcceptChanges();

            myAdapter.SelectCommand.CommandText = "Select * From Backlogs";
            OleDbCommandBuilder myCommandBuilder = new OleDbCommandBuilder(myAdapter);
            myAdapter.UpdateCommand = myCommandBuilder.GetUpdateCommand();
            myAdapter.Update(myDataSet, "myRawBacklogs");
            myDataSet.Clear();

            getDatabase();
            getBacklogs();
            addandEditBacklogPopup.Hide();
        }
        

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            //lblTest.Text = ((Control)sender).ID;
        }

        protected void btnAddTask_Click(object sender, EventArgs e)
        {
            //lblTest.Text = ((Control)sender).ID;
        }

        protected void btnComplexity_Click(object sender, EventArgs e)
        {
            //lblTest.Text = ((Control)sender).ID;
        }

        protected void btnAssignee_Click(object sender, EventArgs e)
        {
            //lblTest.Text = ((Control)sender).ID;
        }

        protected void btnDueDate_Click(object sender, EventArgs e)
        {
            //lblTest.Text = ((Control)sender).ID;
        }

        protected void btnTask_Click(object sender, EventArgs e)
        {
            //lblTest.Text = ((Control)sender).ID;
        }

        protected void btnComment_Click(object sender, EventArgs e)
        {
            //lblTest.Text = ((Control)sender).ID;
        }



    }
}