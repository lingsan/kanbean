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
            linkBtnUsername.Text = Session["username"].ToString();

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

            myCommand.CommandText = "SELECT [User].Username as [User], COUNT(Tasks.TaskAssigneeID) as Amount, " +
                                    "SUM(Tasks.TaskEstimationHour) as Point FROM Tasks, [User], Backlogs " +
                                    "WHERE Tasks.BacklogID = Backlogs.BacklogID AND Backlogs.ProjectID = " + Session["currentProject"].ToString() + 
                                    " AND Tasks.TaskAssigneeID = [User].UserID AND Tasks.TaskAssigneeID <> 1 " + 
                                    "GROUP BY Tasks.TaskAssigneeID, [User].Username ORDER BY Tasks.TaskAssigneeID";
            myAdapter.Fill(myDataSet, "TaskAssigned");
            TaskAssignedGridView.DataSource = myDataSet;
            TaskAssignedGridView.DataMember = "TaskAssigned";
            TaskAssignedGridView.DataBind();
            TaskAssignedGridView.Caption = "Task Assigned";

            myCommand.CommandText = "SELECT [User].Username as [User], COUNT(Tasks.TaskAssigneeID) as Amount, " +
                                    "SUM(Tasks.TaskEstimationHour) as Point FROM Tasks, [User], Backlogs " +
                                    "WHERE Tasks.BacklogID = Backlogs.BacklogID AND Backlogs.ProjectID = " + Session["currentProject"].ToString() + 
                                    " AND Tasks.TaskAssigneeID = [User].UserID AND Tasks.TaskAssigneeID <> 1 " +
                                    "AND Tasks.TaskStatusID = 3 GROUP BY Tasks.TaskAssigneeID, [User].Username " + 
                                    "ORDER BY Tasks.TaskAssigneeID";
            myAdapter.Fill(myDataSet, "TaskDone");
            TaskDoneGridView.DataSource = myDataSet;
            TaskDoneGridView.DataMember = "TaskDone";
            TaskDoneGridView.DataBind();
            TaskDoneGridView.Caption = "Task Done";

            myCommand.CommandText = "SELECT Tasks.TaskID as Task, Tasks.TaskEstimationHour as [Estimation Hour], " +
                                    "Tasks.TaskSpentTime as [Spent Time], Format (" +
                                    "SWITCH(Tasks.TaskEstimationHour >= Tasks.TaskSpentTime," +
                                            "1 - Tasks.TaskEstimationHour/Tasks.TaskSpentTime," +
                                            "Tasks.TaskEstimationHour < Tasks.TaskSpentTime," +
                                            "Tasks.TaskSpentTime/Tasks.TaskEstimationHour - 1)," +
                                    "'Percent') as [Factor] FROM Tasks, Backlogs " +
                                    "WHERE Tasks.TaskStatusID = 3 AND Tasks.BacklogID = Backlogs.BacklogID " +
                                    "AND Backlogs.ProjectID = " + Session["currentProject"].ToString() + 
                                    " UNION ALL SELECT 'Total' As Task, Sum(Tasks.TaskEstimationHour) As [Estimation Hour], " +
                                    "Sum(Tasks.TaskSpentTime) As [Spent Time], Format (" +
                                    "SWITCH(Sum(Tasks.TaskEstimationHour) >= Sum(Tasks.TaskSpentTime)," +
                                            "1 - Sum(Tasks.TaskEstimationHour)/Sum(Tasks.TaskSpentTime)," +
                                            "Sum(Tasks.TaskEstimationHour) < Sum(Tasks.TaskSpentTime)," +
                                            "Sum(Tasks.TaskSpentTime)/Sum(Tasks.TaskEstimationHour) - 1)," +
                                    "'Percent') as [Factor] FROM Tasks, Backlogs " +
                                    "WHERE Tasks.TaskStatusID = 3 AND Tasks.BacklogID = Backlogs.BacklogID " +
                                    "AND Backlogs.ProjectID = " + Session["currentProject"].ToString();
            myAdapter.Fill(myDataSet, "EstimationFactor");
            EstimationFactorGridView.DataSource = myDataSet;
            EstimationFactorGridView.DataMember = "EstimationFactor";
            EstimationFactorGridView.DataBind();
            EstimationFactorGridView.Caption = "Estimation Factor";
            
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


            myCommand.CommandText = "SELECT MIN(Tasks.TaskStartDate) FROM Tasks, Backlogs " +
                        "WHERE Tasks.BacklogID = Backlogs.BacklogID AND Backlogs.ProjectID = " + 1;
            DateTime startdate = DateTime.Parse(myCommand.ExecuteScalar().ToString());
            startdateTextBox.Text = Convert.ToDateTime(myCommand.ExecuteScalar()).ToShortDateString();

            myCommand.CommandText = "SELECT MAX(Tasks.TaskDueDate) FROM Tasks, Backlogs " +
                                    "WHERE Tasks.BacklogID = Backlogs.BacklogID AND Backlogs.ProjectID = " + 1;
            DateTime enddate = DateTime.Parse(myCommand.ExecuteScalar().ToString());
            enddateTextBox.Text = Convert.ToDateTime(myCommand.ExecuteScalar()).ToShortDateString();
            //DateTime currentdate = DateTime.Parse("28/11/2014");
            //if (Convert.ToDateTime(myCommand.ExecuteScalar()) < DateTime.Today)
            //    enddateTextBox.Text = DateTime.Today.ToString();
            //else
            //    enddateTextBox.Text = myCommand.ExecuteScalar().ToString();

            decimal gap = Convert.ToDecimal(enddate.Subtract(startdate).TotalDays) - 1;

            DataTable dt = new DataTable();
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("SpentHour", typeof(decimal));
            dt.Columns.Add("RemainingHour", typeof(decimal));
            myCommand.CommandText = "SELECT SUM(Tasks.TaskEstimationHour) FROM Tasks, Backlogs " +
                                    "WHERE Tasks.BacklogID = Backlogs.BacklogID AND Backlogs.ProjectID = 1" +
                                    " AND (Tasks.TaskStatusID = 1 OR Tasks.TaskStatusID = 2 OR Tasks.TaskCompletedDate >= #" + startdate + "#)" +
                                    " AND Tasks.TaskDueDate <= #" + enddate + "#";
            if (myCommand.ExecuteScalar() != DBNull.Value)
            {
                decimal totalhour = Convert.ToDecimal(myCommand.ExecuteScalar());
                foreach (DateTime date in GetDateRange(startdate, enddate))
                {
                    decimal spenthour = 0;
                    myCommand.CommandText = "SELECT SUM(Tasks.TaskEstimationHour) FROM Tasks, Backlogs " +
                                            "WHERE Tasks.BacklogID = Backlogs.BacklogID AND Backlogs.ProjectID = " + 1 +
                                            " AND (Tasks.TaskStatusID = 1 OR Tasks.TaskStatusID = 2 OR Tasks.TaskCompletedDate >= #" + startdate + "#)" +
                                            " AND Tasks.TaskDueDate <=#" + enddate + "# AND Tasks.TaskCompletedDate <=#" + date + "#";
                    if (myCommand.ExecuteScalar() != DBNull.Value)
                        spenthour = Convert.ToDecimal(myCommand.ExecuteScalar());
                    dt.Rows.Add(date.ToString("dd.MM.yyyy"), spenthour, totalhour - spenthour);
                }


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BurnDownChart.Series["Burn-Down"].Points.AddXY(dt.Rows[i]["Date"].ToString(), Convert.ToDecimal(dt.Rows[i]["RemainingHour"]));
                    BurnDownChart.Series["Optimal"].Points.AddXY(dt.Rows[i]["Date"].ToString(), totalhour - i * gap);
                    BurnUpChart.Series["Burn-Up"].Points.AddXY(dt.Rows[i]["Date"].ToString(), Convert.ToDecimal(dt.Rows[i]["SpentHour"]));
                    BurnUpChart.Series["Total"].Points.AddXY(dt.Rows[i]["Date"].ToString(), totalhour);
                }
                BurnDownChart.Titles.Add(new Title("Burn-Down Chart", Docking.Top, new Font("Calibri", 16f, FontStyle.Bold), Color.Black));
                BurnDownChart.Legends.Add("BurnDownChartLegend");
                BurnDownChart.Legends["BurnDownChartLegend"].Docking = Docking.Bottom;
                BurnDownChart.Legends["BurnDownChartLegend"].Alignment = System.Drawing.StringAlignment.Center;

                BurnUpChart.Titles.Add(new Title("Burn-Up Chart", Docking.Top, new Font("Calibri", 16f, FontStyle.Bold), Color.Black));
                BurnUpChart.Legends.Add("BurnUpChartLegend");
                BurnUpChart.Legends["BurnUpChartLegend"].Docking = Docking.Bottom;
                BurnUpChart.Legends["BurnUpChartLegend"].Alignment = System.Drawing.StringAlignment.Center;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnBacktheBoard_Click(object sender, EventArgs e)
        {
            myConnection.Close();
            Response.Redirect("board.aspx");
        }

        protected void btnCreateChart_Click(object sender, EventArgs e)
        {
            DateTime startdate = Convert.ToDateTime(startdateTextBox.Text);
            DateTime enddate = Convert.ToDateTime(enddateTextBox.Text);
            DataTable dt = new DataTable();
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("SpentHour", typeof(decimal));
            dt.Columns.Add("RemainingHour", typeof(decimal));
            myCommand.CommandText = "SELECT SUM(Tasks.TaskEstimationHour) FROM Tasks, Backlogs " +
                                    "WHERE Tasks.BacklogID = Backlogs.BacklogID AND Backlogs.ProjectID = " + Session["currentProject"].ToString() +
                                    " AND (Tasks.TaskStatusID = 1 OR Tasks.TaskStatusID = 2 OR Tasks.TaskCompletedDate >= #" + startdate + "#)" +
                                    " AND Tasks.TaskDueDate <=#" + enddate + "#";
            if (myCommand.ExecuteScalar() != DBNull.Value)
            {
                decimal totalhour = Convert.ToDecimal(myCommand.ExecuteScalar());
                foreach (DateTime date in GetDateRange(startdate, enddate))
                {
                    decimal spenthour = 0;
                    myCommand.CommandText = "SELECT SUM(Tasks.TaskEstimationHour) FROM Tasks, Backlogs " +
                                            "WHERE Tasks.BacklogID = Backlogs.BacklogID AND Backlogs.ProjectID = " + Session["currentProject"].ToString() +
                                            " AND (Tasks.TaskStatusID = 1 OR Tasks.TaskStatusID = 2 OR Tasks.TaskCompletedDate >= #" + startdate + "#)" +
                                            " AND Tasks.TaskDueDate <=#" + enddate + "# AND Tasks.TaskCompletedDate <=#" + date + "#";
                    if (myCommand.ExecuteScalar() != DBNull.Value)
                        spenthour = Convert.ToDecimal(myCommand.ExecuteScalar());
                    dt.Rows.Add(date.ToString("dd.MM.yyyy"), spenthour, totalhour - spenthour);
                }

                decimal gap = totalhour / (dt.Rows.Count - 1);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BurnDownChart.Series["Burn-Down"].Points.AddXY(dt.Rows[i]["Date"].ToString(), Convert.ToDecimal(dt.Rows[i]["RemainingHour"]));
                    BurnDownChart.Series["Optimal"].Points.AddXY(dt.Rows[i]["Date"].ToString(), totalhour - i * gap);
                    BurnUpChart.Series["Burn-Up"].Points.AddXY(dt.Rows[i]["Date"].ToString(), Convert.ToDecimal(dt.Rows[i]["SpentHour"]));
                    BurnUpChart.Series["Total"].Points.AddXY(dt.Rows[i]["Date"].ToString(), totalhour);
                }
                BurnDownChart.Titles.Add(new Title("Burn-Down Chart", Docking.Top, new Font("Calibri", 16f, FontStyle.Bold), Color.Black));
                BurnDownChart.Legends.Add("BurnDownChartLegend");
                BurnDownChart.Legends["BurnDownChartLegend"].Docking = Docking.Bottom;
                BurnDownChart.Legends["BurnDownChartLegend"].Alignment = System.Drawing.StringAlignment.Center;

                BurnUpChart.Titles.Add(new Title("Burn-Up Chart", Docking.Top, new Font("Calibri", 16f, FontStyle.Bold), Color.Black));
                BurnUpChart.Legends.Add("BurnUpChartLegend");
                BurnUpChart.Legends["BurnUpChartLegend"].Docking = Docking.Bottom;
                BurnUpChart.Legends["BurnUpChartLegend"].Alignment = System.Drawing.StringAlignment.Center;
            }
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
    }
}