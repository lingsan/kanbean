<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.aspx.cs" Inherits="Kanbean_Project.SearchResults" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Lanban</title>
    <link rel="stylesheet" href="/css/style.css" type="text/css" />
    <link rel="stylesheet" href="//maxcdn.bootstrapcdn.com/font-awesome/4.2.0/css/font-awesome.min.css" />
</head>
<body>
    <form id="searchResultForm" runat="server">
        <asp:ScriptManager ID="searchResultScriptManager" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="searchResultUpdatePanel" runat="server" UpdateMode="Conditional">
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
                    <b><asp:Label ID="lbl" runat="server" CssClass="icon" Text="Search Result"></asp:Label></b>
                </div>
                <asp:Table ID="TableSearchResults" border="1" runat="server"></asp:Table>    
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
