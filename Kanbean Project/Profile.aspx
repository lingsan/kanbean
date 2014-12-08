<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="Kanbean_Project.Profile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="/css/style.css" type="text/css" />
    <style type="text/css">
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="user-information-profile">
            <p>Project:
                <asp:DropDownList ID="projectDropDownList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="projectDropDownList_SelectedIndexChanged"></asp:DropDownList>
                Welcome,
                <asp:LinkButton ID="linkBtnUsername" runat="server" OnClick="linkBtnUsername_Click" Text="Username"></asp:LinkButton>. (<asp:LinkButton ID="linkBtnLogout" runat="server" OnClick="EatCookies">Logout</asp:LinkButton>)
            </p>
        </div>
    <div>
        <div style="background-color: deepskyblue"><h1 class="formTitle-profile">Kanbean board</h1></div>
        <table style="width:500px; margin:0 auto">
            <tr>
                <td style ="width:200px">Username</td>
                <td>
                    <asp:TextBox ID="TxtUsername" runat="server" CssClass="TextBox"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Password</td>
                <td>
                    <asp:TextBox ID="TxtPassword" runat="server" CssClass="TextBox"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td>Email</td>
                <td>
                    <asp:TextBox ID="TxtEmail" runat="server" TextMode="Email" CssClass="TextBox"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td>User level</td>
                <td>
                    <asp:TextBox ID="TextLevel" runat="server" CssClass="TextBox"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>Participated Project(s)</td>
                <td>
                    <asp:DropDownList ID="DropProjects" runat="server" CssClass="TextBox">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:LinkButton ID="SetDefault" runat="server" OnClick="SetDefaulProject">Set as Default Project</asp:LinkButton>
                </td>
            </tr>
            <tr>
                <td>Default Project</td>
                <td>
                    <asp:TextBox ID="TextDefaultProject" runat="server" CssClass="TextBox"></asp:TextBox>
                </td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
