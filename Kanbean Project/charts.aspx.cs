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

        protected void Page_Init(object sender, EventArgs e)
        {
            myConnection.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|LanbanDatabase.mdb;";
            myConnection.Open();
            myCommand.Connection = myConnection;
            myAdapter.SelectCommand = myCommand;
            myCommand.CommandText = "SELECT Tasks.TaskID as [Task ID], Tasks.BacklogID as [Backlog ID], " +
            "Tasks.TaskTitle as Title, Tasks.TaskComplexity as Complexity, " +
            "[User].[Username] as Assginee, Status.StatusName as Status, " +
            "Tasks.TaskEstimationHour as [Estimation Hour], Tasks.TaskSpentTime as [Spent Time], " +
            "FORMAT(Tasks.TaskStartDate, 'dd.mm.yyyy') as [Created Date], " +
            "FORMAT(Tasks.TaskDueDate, 'dd.mm.yyyy') as [Due Date], " +
            "FORMAT(Tasks.TaskCompletedDate, 'dd.mm.yyyy') as [Completed Date] " +
            "FROM Tasks, Backlogs, Status, [User] " +
            "WHERE Tasks.BacklogID = Backlogs.BacklogID AND Tasks.TaskAssigneeID = [User].UserID " +
            "AND Tasks.TaskStatusID = Status.StatusID AND Backlogs.ProjectID = " + Session["currentProject"].ToString();
            myAdapter.Fill(myDataSet, "TasksTable");
            taskGridView.DataSource = myDataSet;
            taskGridView.DataMember = "TasksTable";
            taskGridView.DataBind();
            taskGridView.Caption = "Tasks Table";

            myCommand.CommandText = "SELECT [User].Username as [User], COUNT(Tasks.TaskAssigneeID) as Amount, SUM(Tasks.TaskEstimationHour) as Point FROM Tasks, [User] WHERE Tasks.TaskAssigneeID = [User].UserID AND Tasks.TaskAssigneeID <> 1 GROUP BY Tasks.TaskAssigneeID, [User].Username ORDER BY Tasks.TaskAssigneeID";
            myAdapter.Fill(myDataSet, "TaskAssigned");
            TaskAssignedGridView.DataSource = myDataSet;
            TaskAssignedGridView.DataMember = "TaskAssigned";
            TaskAssignedGridView.DataBind();
            TaskAssignedGridView.Caption = "Task Assigned";

            myCommand.CommandText = "SELECT [User].Username as [User], COUNT(Tasks.TaskAssigneeID) as Amount, SUM(Tasks.TaskEstimationHour) as Point FROM Tasks, [User] WHERE Tasks.TaskAssigneeID = [User].UserID AND Tasks.TaskAssigneeID <> 1 AND Tasks.TaskStatusID = 3 GROUP BY Tasks.TaskAssigneeID, [User].Username ORDER BY Tasks.TaskAssigneeID";
            myAdapter.Fill(myDataSet, "TaskDone");
            TaskDoneGridView.DataSource = myDataSet;
            TaskDoneGridView.DataMember = "TaskDone";
            TaskDoneGridView.DataBind();
            TaskDoneGridView.Caption = "Task Done";


            TaskAssignedChart.Titles.Add(new Title("Task Assigned by Person", Docking.Top, new Font("Calibri", 16f, FontStyle.Bold), Color.Black));
            TaskAssignedChart.Series["TaskAssigned"]["PieLabelStyle"] = "Outside";
            TaskAssignedChart.Series["TaskAssigned"].XValueMember = "User";
            TaskAssignedChart.Series["TaskAssigned"].YValueMembers = "Amount";
            TaskAssignedChart.DataSource = myDataSet.Tables["TaskAssigned"];
            TaskAssignedChart.DataBind();

            TaskDoneChart.Titles.Add(new Title("Task Done by Person", Docking.Top, new Font("Calibri", 16f, FontStyle.Bold), Color.Black));
            TaskDoneChart.Series["TaskDone"]["PieLabelStyle"] = "Outside";
            TaskDoneChart.Series["TaskDone"].XValueMember = "User";
            TaskDoneChart.Series["TaskDone"].YValueMembers = "Amount";
            TaskDoneChart.DataSource = myDataSet.Tables["TaskDone"];
            TaskDoneChart.DataBind();

            for (int i = 0; i < myDataSet.Tables["TaskAssigned"].Rows.Count; i++)
            {
                EstimationPointChart.Series["Amount"].Points.AddXY(myDataSet.Tables["TaskAssigned"].Rows[i]["User"].ToString(), Convert.ToInt32(myDataSet.Tables["TaskAssigned"].Rows[i]["Amount"]));
                EstimationPointChart.Series["Point"].Points.AddXY(myDataSet.Tables["TaskAssigned"].Rows[i]["User"].ToString(), Convert.ToInt32(myDataSet.Tables["TaskAssigned"].Rows[i]["Point"]));
            }

            EstimationPointChart.Titles.Add(new Title("Estimated Point by Person", Docking.Top, new Font("Calibri", 16f, FontStyle.Bold), Color.Black));

        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnBacktheBoard_Click(object sender, EventArgs e)
        {
            myConnection.Close();
            Response.Redirect("board.aspx");
        }

        protected void btnBurnDown_Click(object sender, EventArgs e)
        {
            DateTime startdate = Convert.ToDateTime(startdateBDTextBox.Text);
            DateTime enddate = Convert.ToDateTime(enddateBDTextBox.Text);
            DataTable dt = new DataTable();
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("Hour", typeof(int));
            myCommand.CommandText = "SELECT SUM(TaskEstimationHour) FROM Tasks WHERE TaskDueDate >= #" + startdate + "# AND  TaskDueDate <=#" + enddate + "#";
            int remainhour = Convert.ToInt32(myCommand.ExecuteScalar().ToString());
            dt.Rows.Add(startdate.ToString("dd.MM.yyyy"), remainhour);
            foreach (DateTime date in GetDateRange(startdate, enddate))
            {
                int hour = remainhour;
                myCommand.CommandText = "SELECT SUM(TaskEstimationHour) FROM Tasks WHERE TaskCompletedDate >= #" + startdate + "# AND  TaskCompletedDate <=#" + date + "#";
                if (myCommand.ExecuteScalar() != DBNull.Value)
                    hour = remainhour - Convert.ToInt32(myCommand.ExecuteScalar());
                dt.Rows.Add(date.ToString("dd.MM.yyyy"), hour);
            }
            dt.Rows.Add(enddate.ToString("dd.MM.yyyy"), 0);

            decimal j = (Convert.ToDecimal(dt.Rows[0]["Hour"]) - Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["Hour"])) / (dt.Rows.Count - 3);
            for (int i = 0; i < dt.Rows.Count - 2; i++)
            {
                BurnDownChart.Series["Burn-Down"].Points.AddXY(dt.Rows[i + 1]["Date"].ToString(), Convert.ToDecimal(dt.Rows[i + 1]["Hour"]));
                BurnDownChart.Series["Optimal"].Points.AddXY(dt.Rows[i + 1]["Date"].ToString(), Convert.ToDecimal(dt.Rows[0]["Hour"]) - i * j);
            }
            BurnDownChart.Titles.Add(new Title("Burn-Down Chart", Docking.Top, new Font("Calibri", 16f, FontStyle.Bold), Color.Black));
            BurnDownChart.Legends.Add("BurnDownChartLegend");
            BurnDownChart.Legends["BurnDownChartLegend"].Docking = Docking.Bottom;
            BurnDownChart.Legends["BurnDownChartLegend"].Alignment = System.Drawing.StringAlignment.Center;
        }

        protected void btnBurnUp_Click(object sender, EventArgs e)
        {
            
        }

        private List<DateTime> GetDateRange(DateTime StartingDate, DateTime EndingDate)
        {
            if (StartingDate > EndingDate)
                return null;
            List<DateTime> rv = new List<DateTime>();
            DateTime tmpDate = StartingDate;
            do
            {
                rv.Add(tmpDate);
                tmpDate = tmpDate.AddDays(1);
            } while (tmpDate <= EndingDate);
            return rv;
        }
    }
}