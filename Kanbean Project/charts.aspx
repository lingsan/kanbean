<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="charts.aspx.cs" Inherits="Kanbean_Project.charts" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Lanban</title>
    <link rel="stylesheet" href="/css/style.css" type="text/css" />
    <link rel="stylesheet" href="//maxcdn.bootstrapcdn.com/font-awesome/4.2.0/css/font-awesome.min.css" />
</head>
<body>
    <form id="chart" runat="server">
        <h1>Time Table</h1>
        <asp:GridView ID="taskGridView" runat="server"></asp:GridView>
    </form>
</body>
</html>
