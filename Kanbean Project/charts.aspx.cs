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
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["UserSettings"]["Name"] != null)
                    linkBtnUsername.Text = Request.Cookies["UserSettings"]["Name"];
                else
                    Response.Redirect("login.aspx");
            }
            else
                Response.Redirect("login.aspx");

            if (Session["currentProject"] == null)
                Response.Redirect("board.aspx");

            myConnection.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|LanbanDatabase.mdb;";
            myConnection.Open();
            myCommand.Connection = myConnection;
            myAdapter.SelectCommand = myCommand;

            //task table
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

            //task assigned table
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

            //task done table
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

            //Estimation Factor table
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
            
            //task assigned chart
            TaskAssignedChart.Titles.Add(new Title("Task Assigned by Person", Docking.Top, new Font("Calibri", 16f, FontStyle.Bold), Color.Black));
            TaskAssignedChart.Series["TaskAssigned"]["PieLabelStyle"] = "Outside";
            TaskAssignedChart.Series["TaskAssigned"].XValueMember = "User";
            TaskAssignedChart.Series["TaskAssigned"].YValueMembers = "Amount";
            TaskAssignedChart.DataSource = myDataSet.Tables["TaskAssigned"];
            TaskAssignedChart.DataBind();
            TaskAssignedChart.Legends.Add("TaskAssignedLegend");
            TaskAssignedChart.Legends["TaskAssignedLegend"].Docking = Docking.Bottom;
            TaskAssignedChart.Legends["TaskAssignedLegend"].Alignment = System.Drawing.StringAlignment.Center;

            //task done chart
            TaskDoneChart.Titles.Add(new Title("Task Done by Person", Docking.Top, new Font("Calibri", 16f, FontStyle.Bold), Color.Black));
            TaskDoneChart.Series["TaskDone"]["PieLabelStyle"] = "Outside";
            TaskDoneChart.Series["TaskDone"].XValueMember = "User";
            TaskDoneChart.Series["TaskDone"].YValueMembers = "Amount";
            TaskDoneChart.DataSource = myDataSet.Tables["TaskDone"];
            TaskDoneChart.DataBind();
            TaskDoneChart.Legends.Add("TaskDoneLegend");
            TaskDoneChart.Legends["TaskDoneLegend"].Docking = Docking.Bottom;
            TaskDoneChart.Legends["TaskDoneLegend"].Alignment = System.Drawing.StringAlignment.Center;

            //Estimated Point chart
            for (int i = 0; i < myDataSet.Tables["TaskAssigned"].Rows.Count; i++)
            {
                EstimationPointChart.Series["Amount"].Points.AddXY(myDataSet.Tables["TaskAssigned"].Rows[i]["User"].ToString(), Convert.ToInt32(myDataSet.Tables["TaskAssigned"].Rows[i]["Amount"]));
                EstimationPointChart.Series["Point"].Points.AddXY(myDataSet.Tables["TaskAssigned"].Rows[i]["User"].ToString(), Convert.ToInt32(myDataSet.Tables["TaskAssigned"].Rows[i]["Point"]));
            }

            EstimationPointChart.Titles.Add(new Title("Estimated Point by Person", Docking.Top, new Font("Calibri", 16f, FontStyle.Bold), Color.Black));
            EstimationPointChart.Legends.Add("EstimationPointChartLegend");
            EstimationPointChart.Legends["EstimationPointChartLegend"].Docking = Docking.Bottom;
            EstimationPointChart.Legends["EstimationPointChartLegend"].Alignment = System.Drawing.StringAlignment.Center;

            //burn up and burn down chart
            myCommand.CommandText = "SELECT MIN(Tasks.TaskStartDate) FROM Tasks, Backlogs " +
                                    "WHERE Tasks.BacklogID = Backlogs.BacklogID AND Backlogs.ProjectID = " + Session["currentProject"].ToString();
            DateTime startdate = DateTime.Parse(myCommand.ExecuteScalar().ToString());
            myCommand.CommandText = "SELECT MAX(Tasks.TaskDueDate) FROM Tasks, Backlogs " +
                                    "WHERE Tasks.BacklogID = Backlogs.BacklogID AND Backlogs.ProjectID = " + Session["currentProject"].ToString();
            DateTime enddate = DateTime.Parse(myCommand.ExecuteScalar().ToString());
            DateTime currentdate = DateTime.Today;
            if (currentdate > enddate)
                currentdate = enddate;

            myCommand.CommandText = "SELECT SUM(Tasks.TaskEstimationHour) FROM Tasks, Backlogs " +
                                    "WHERE Tasks.BacklogID = Backlogs.BacklogID AND Backlogs.ProjectID = " + Session["currentProject"].ToString() +
                                    " AND (Tasks.TaskStatusID = 1 OR Tasks.TaskStatusID = 2 OR Tasks.TaskCompletedDate >= #" + startdate + "#)" +
                                    " AND Tasks.TaskDueDate <= #" + enddate + "#";

            decimal totalhour = Convert.ToDecimal(myCommand.ExecuteScalar());
            decimal diffstartend = Convert.ToDecimal(enddate.Subtract(startdate).TotalDays);
            decimal diffstartcurrent = Convert.ToDecimal(currentdate.Subtract(startdate).TotalDays);
            decimal gap = totalhour / diffstartend;
            for (int i = 0; i <= diffstartend; i++)
            {
                if (i <= diffstartcurrent)
                {
                    decimal spenthour;
                    myCommand.CommandText = "SELECT SUM(Tasks.TaskEstimationHour) FROM Tasks, Backlogs " +
                                            "WHERE Tasks.BacklogID = Backlogs.BacklogID AND Backlogs.ProjectID = " + Session["currentProject"].ToString() +
                                            " AND (Tasks.TaskStatusID = 1 OR Tasks.TaskStatusID = 2 OR Tasks.TaskCompletedDate >= #" + startdate + "#)" +
                                            " AND Tasks.TaskDueDate <=#" + enddate + "# AND Tasks.TaskCompletedDate <=#" + startdate.AddDays((double)i) + "#";
                    try
                    {
                        spenthour = Convert.ToDecimal(myCommand.ExecuteScalar());
                    }
                    catch
                    {
                        spenthour = 0;
                    }
                    BurnDownChart.Series["Burn-Down"].Points.AddXY(startdate.AddDays((double)i).ToString("dd.MM.yyyy"), totalhour - spenthour);
                    BurnUpChart.Series["Burn-Up"].Points.AddXY(startdate.AddDays((double)i).ToString("dd.MM.yyyy"), spenthour);
                }
                BurnDownChart.Series["Optimal"].Points.AddXY(startdate.AddDays((double)i).ToString("dd.MM.yyyy"), totalhour - i * gap);
                BurnUpChart.Series["Total"].Points.AddXY(startdate.AddDays((double)i).ToString("dd.MM.yyyy"), totalhour);
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
    }
}