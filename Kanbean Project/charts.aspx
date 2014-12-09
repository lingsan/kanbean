<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="charts.aspx.cs" Inherits="Kanbean_Project.charts" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Lanban</title>
    <link rel="stylesheet" href="/css/style.css" type="text/css" />
    <link rel="stylesheet" href="//maxcdn.bootstrapcdn.com/font-awesome/4.2.0/css/font-awesome.min.css" />
</head>
<body>
    <form id="chart" runat="server">
        <asp:ScriptManager ID="chartScriptManager" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="chartUpdatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="header">
                    <div id="logo" title="Lanban">
                        <h1>Lanban</h1>
                    </div>
                    <div id="user-information">
                        <p>Welcome, <asp:LinkButton ID="linkBtnUsername" runat="server" Text="Username" OnClick="linkBtnUsername_Click"></asp:LinkButton>. (<asp:LinkButton ID="linkBtnLogout" runat="server" OnClick="EatCookies">Logout</asp:LinkButton>)</p>
                    </div>
                </div>

                <div class="clear"></div>
                <div id="toolbar">
                    <asp:LinkButton runat="server" id="btnBacktheBoard" CssClass="icon" ToolTip="Back to the Board" OnClick="btnBacktheBoard_Click"></asp:LinkButton>
                    <b><asp:Label ID="lbl" runat="server" CssClass="icon" Text="Chart"></asp:Label></b>
                </div>
                <table id="chartTable">
                    <tr>
                        <td colspan="3">
                            <asp:Chart ID="BurnDownChart" runat="server" Width="500px">
                                <Series>
                                    <asp:Series Name="Burn-Down" ChartArea="BurnDownChartArea" BorderWidth="4" ChartType="Line"></asp:Series>
                                    <asp:Series Name="Optimal" ChartArea="BurnDownChartArea" BorderWidth="4" ChartType="Line"></asp:Series>
                                </Series>
                                <ChartAreas>
                                    <asp:ChartArea Name="BurnDownChartArea"></asp:ChartArea>
                                </ChartAreas>
                            </asp:Chart>
                        </td>
                        <td colspan="3">
                            <asp:Chart ID="BurnUpChart" runat="server" Width="500px">
                                <Series>
                                    <asp:Series Name="Burn-Up" ChartArea="BurnUpChartArea" ChartType="Column"></asp:Series>
                                    <asp:Series Name="Total" ChartArea="BurnUpChartArea" BorderWidth="4" ChartType="Line"></asp:Series>
                                </Series>
                                <ChartAreas>
                                    <asp:ChartArea Name="BurnUpChartArea"></asp:ChartArea>
                                </ChartAreas>
                            </asp:Chart>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            Start date: <asp:TextBox ID="startdateTextBox" runat="server" TextMode="Date"></asp:TextBox>&nbsp;&nbsp;
                            <ajaxToolkit:CalendarExtender ID="startdateCalendarExtender" runat="server" TargetControlID="startdateTextBox" Format="MM/dd/yyyy" />
                            End date: <asp:TextBox ID="enddateTextBox" runat="server" TextMode="Date"></asp:TextBox>&nbsp;&nbsp;
                            <ajaxToolkit:CalendarExtender ID="enddateCalendarExtender" runat="server" TargetControlID="enddateTextBox" Format="MM/dd/yyyy" />
                            <asp:Button ID="btnCreateChart" runat="server" Text="Burn-Down and Burn-Up Chart" OnClick="btnCreateChart_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Chart ID="TaskAssignedChart" runat="server" >
                                <Series>
                                    <asp:Series Name="TaskAssigned" ChartType="Pie" ChartArea="TaskAssignedChartArea" IsValueShownAsLabel="True" Label="#PERCENT{P2}" BorderWidth="1" BorderColor="#000040" LegendText="#VALX"></asp:Series>
                                </Series>
                                <ChartAreas>
                                    <asp:ChartArea Name="TaskAssignedChartArea"></asp:ChartArea>
                                </ChartAreas>
                                <Legends>
                                    <asp:Legend Name="TaskAssignedLegend" Docking="Bottom" Alignment="Center"></asp:Legend>
                                </Legends>
                            </asp:Chart>
                        </td>
                        <td colspan="2">
                            <asp:Chart ID="TaskDoneChart" runat="server">
                                <Series>
                                    <asp:Series Name="TaskDone" ChartArea="TaskDoneChartArea" ChartType="Pie" IsValueShownAsLabel="True" Label="#PERCENT{P2}" BorderWidth="1" BorderColor="#000040" LegendText="#VALX"></asp:Series>
                                </Series>
                                <ChartAreas>
                                    <asp:ChartArea Name="TaskDoneChartArea"></asp:ChartArea>
                                </ChartAreas>
                                <Legends>
                                    <asp:Legend Name="TaskDoneLegend" Docking="Bottom" Alignment="Center"></asp:Legend>
                                </Legends>
                            </asp:Chart>
                        </td>
                        <td colspan="2">
                            <asp:Chart ID="EstimationPointChart" runat="server">
                                <Series>
                                    <asp:Series Name="Amount" ChartArea="EstimationPointChartArea"></asp:Series>
                                    <asp:Series Name="Point" ChartArea="EstimationPointChartArea"></asp:Series>
                                </Series>
                                <ChartAreas>
                                    <asp:ChartArea Name="EstimationPointChartArea"></asp:ChartArea>
                                </ChartAreas>
                                <Legends>
                                    <asp:Legend Name="EstimationPointChartLegend" Docking="Bottom" Alignment="Center"></asp:Legend>
                                </Legends>
                            </asp:Chart>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:GridView ID="TaskAssignedGridView" runat="server" CellPadding="3" HorizontalAlign="Center"></asp:GridView>
                        </td>
                        <td colspan="2">
                            <asp:GridView ID="TaskDoneGridView" runat="server" CellPadding="3" HorizontalAlign="Center"></asp:GridView>
                        </td>
                        <td colspan="2">
                            <asp:GridView ID="EstimationFactorGridView" runat="server" CellPadding="3" HorizontalAlign="Center"></asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <asp:GridView ID="taskGridView" runat="server" CellPadding="3" HorizontalAlign="Center"></asp:GridView>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
