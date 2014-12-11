using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Kanbean_Project
{
    public partial class SearchResults : System.Web.UI.Page
    {
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

            if (Session["userID"] == null)
                Response.Redirect("board.aspx");

            List<string> results = (List<string>)Session["links"];
            TableHeaderRow thr = new TableHeaderRow();
            TableHeaderCell thc = new TableHeaderCell();
            TableHeaderCell thc1 = new TableHeaderCell();
            TableHeaderCell thc2 = new TableHeaderCell();
            TableHeaderCell thc3 = new TableHeaderCell();
            thr.Cells.Add(thc);
            thc.Text = "Title";
            thr.Cells.Add(thc1);
            thc1.Text = "Start Date";
            thr.Cells.Add(thc2);
            thc2.Text = "End Date";
            thr.Cells.Add(thc3);
            thc3.Text = "Assignee";

            TableSearchResults.Rows.Add(thr);
            foreach (string li in results)
            {
                TableRow tr = new TableRow();

                string[] str = li.Split('+');
                TableCell tc = new TableCell();
                TableCell tc1 = new TableCell();
                TableCell tc2 = new TableCell();
                TableCell tc3 = new TableCell();

                tc.Text = str[0];
                tc1.Text = str[1];
                tc2.Text = str[2];
                tc3.Text = str[3];

                tr.Cells.Add(tc);
                tr.Cells.Add(tc1);
                tr.Cells.Add(tc2);
                tr.Cells.Add(tc3);

                TableSearchResults.Rows.Add(tr);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.Write("Halo");
        }

        protected void btnBacktheBoard_Click(object sender, EventArgs e)
        {
            Response.Redirect("board.aspx");
        }

        protected void linkBtnUsername_Click(object sender, EventArgs e)
        {
            Response.Redirect("Profile.aspx");
        }

        protected void EatCookies(object sender, EventArgs e)
        {
            if (Request.Cookies["UserSettings"] != null)
            {
                Response.Cookies["UserSettings"].Expires = DateTime.Now.AddDays(-1);
                Session["username"] = null;
                Session["userID"] = null;
                Session["currentProject"] = null;
                Response.Redirect("login.aspx");
            }
        }
    }
}