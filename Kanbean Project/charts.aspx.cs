using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
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
        }
    }
}