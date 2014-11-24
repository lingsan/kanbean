using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Web.UI.DataVisualization;
using System.Web.UI.DataVisualization.Charting;

namespace Kanbean_Project
{
    public partial class charts : System.Web.UI.Page
    {
        OleDbConnection myConnection = new OleDbConnection();
        OleDbCommand myCommand = new OleDbCommand();
        OleDbDataAdapter myAdapter = new OleDbDataAdapter();
        DataSet myDataSet = new DataSet();
        protected void Page_Load(object sender, EventArgs e)
        {
            myConnection.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|LanbanDatabase.mdb;";
            myConnection.Open();
            myCommand.Connection = myConnection;
            myAdapter.SelectCommand = myCommand;
            myCommand.CommandText = "SELECT Tasks.TaskID as [Task ID], Tasks.BacklogID as [Backlog ID], " +
            "Tasks.TaskTitle as Title, Tasks.TaskComplexity as Complexity, " +
            "[User].[Username] as Assginee, Status.StatusName as Status, " +
            "Tasks.TaskEstimationHour as [Estimation Hour], Tasks.TaskSpentTime as [Spent Time], " +
            "Tasks.TaskStartDate as [Created Date], Tasks.TaskDueDate as [Due Date], " +
            "Tasks.TaskCompletedDate as [Completed Date] " +
            "FROM Tasks, Backlogs, Status, [User] " +
            "WHERE Tasks.BacklogID = Backlogs.BacklogID AND Tasks.TaskAssigneeID = [User].UserID " +
            "AND Tasks.TaskStatusID = Status.StatusID AND Backlogs.ProjectID = " + Session["currentProject"].ToString();
            myAdapter.Fill(myDataSet, "myTasks");
            taskGridView.DataSource = myDataSet;
            taskGridView.DataMember = "myTasks";
            taskGridView.DataBind();
            taskGridView.Caption = "Tasks Table";

            myCommand.CommandText = "SELECT [User].Username as [User], COUNT(Tasks.TaskAssigneeID) as Amount, SUM(Tasks.TaskEstimationHour) as Point FROM Tasks, [User] WHERE Tasks.TaskAssigneeID = [User].UserID AND Tasks.TaskAssigneeID <> 1 GROUP BY Tasks.TaskAssigneeID, [User].Username ORDER BY Tasks.TaskAssigneeID";
            myAdapter.Fill(myDataSet, "TaskAssigned");
            TaskAssignedGridView.DataSource = myDataSet;
            TaskAssignedGridView.DataMember = "TaskAssigned";
            TaskAssignedGridView.DataBind();
            TaskAssignedGridView.Caption = "Task Assigned";

            Chart1.Titles.Add(new Title("Task Assigned by Person", Docking.Top, new Font("Calibri", 16f, FontStyle.Bold), Color.Black));
            Chart1.Series["Series1"].ChartType = SeriesChartType.Pie;
            Chart1.Series["Series1"]["PieLabelStyle"] = "Outside";
            Chart1.Series["Series1"].IsValueShownAsLabel = true;
            Chart1.Series["Series1"].Label = "#PERCENT{P2}";
            Chart1.Series["Series1"].BorderWidth = 1;
            Chart1.Series["Series1"].BorderColor = System.Drawing.Color.FromArgb(26, 59, 105);
            Chart1.Series["Series1"].XValueMember = "User";
            Chart1.Series["Series1"].YValueMembers = "Amount";
            Chart1.Legends.Add("Legend1");
            Chart1.Legends["Legend1"].Docking = Docking.Bottom;
            Chart1.Legends["Legend1"].Alignment = System.Drawing.StringAlignment.Center;
            Chart1.Series["Series1"].LegendText = "#VALX";
            Chart1.DataSource = myDataSet.Tables["TaskAssigned"];
            Chart1.DataBind();

            myCommand.CommandText = "SELECT [User].Username as [User], COUNT(Tasks.TaskAssigneeID) as Amount, SUM(Tasks.TaskEstimationHour) as Point FROM Tasks, [User] WHERE Tasks.TaskAssigneeID = [User].UserID AND Tasks.TaskAssigneeID <> 1 AND Tasks.TaskStatusID = 3 GROUP BY Tasks.TaskAssigneeID, [User].Username ORDER BY Tasks.TaskAssigneeID";
            myAdapter.Fill(myDataSet, "TaskDone");
            TaskDoneGridView.DataSource = myDataSet;
            TaskDoneGridView.DataMember = "TaskDone";
            TaskDoneGridView.DataBind();
            TaskDoneGridView.Caption = "Task Done";

            Chart2.Titles.Add(new Title("Task Done by Person", Docking.Top, new Font("Calibri", 16f, FontStyle.Bold), Color.Black));
            Chart2.Series["Series1"].ChartType = SeriesChartType.Pie;
            Chart2.Series["Series1"]["PieLabelStyle"] = "Outside";
            Chart2.Series["Series1"].IsValueShownAsLabel = true;
            Chart2.Series["Series1"].Label = "#PERCENT";
            Chart2.Series["Series1"].BorderWidth = 1;
            Chart2.Series["Series1"].BorderColor = System.Drawing.Color.FromArgb(26, 59, 105);
            Chart2.Series["Series1"].XValueMember = "User";
            Chart2.Series["Series1"].YValueMembers = "Amount";
            Chart2.Legends.Add("Legend1");
            Chart2.Legends["Legend1"].Docking = Docking.Bottom;
            Chart2.Legends["Legend1"].Alignment = System.Drawing.StringAlignment.Center;
            Chart2.Series["Series1"].LegendText = "#VALX";
            Chart2.DataSource = myDataSet.Tables["TaskDone"];
            Chart2.DataBind();

            Chart3.Titles.Add(new Title("Estimated Point by Person", Docking.Top, new Font("Calibri", 16f, FontStyle.Bold), Color.Black));
            Chart3.Series["Series1"].ChartType = SeriesChartType.Column;
            Chart3.Legends.Add("Legend1");
            Chart3.Legends["Legend1"].Docking = Docking.Bottom;
            Chart3.Legends["Legend1"].Alignment = System.Drawing.StringAlignment.Center;
            Chart3.DataBindTable((myDataSet.Tables["TaskAssigned"] as System.ComponentModel.IListSource).GetList(), "User");


        }
    }
}