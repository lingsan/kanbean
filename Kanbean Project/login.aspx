<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="Kanbean_Project.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Lanban</title>
    <link rel="stylesheet" href="/css/style.css" type="text/css" />
</head>
<body>
        <form id="loginForm" runat="server">
        <h1 class="formTitle">Lanban board</h1>
            
        <asp:Panel ID="loginPanel" runat="server" GroupingText="Login">
            <asp:CustomValidator ID="LoginValidator" runat="server" ErrorMessage="your Username and password do not match!!!" CssClass="validatorField" ControlToValidate="passwordTextBox" OnServerValidate="LoginValidation"></asp:CustomValidator>
            <table>
                <tr>
                    <td class="registerLabel">Username</td>
                    <td>
                        <asp:RequiredFieldValidator ID="usernameRequiredFieldValidator" runat="server" ErrorMessage="* Please enter your username" ControlToValidate="usernameTextBox" EnableClientScript="False" CssClass="validatorField" Font-Italic="True"></asp:RequiredFieldValidator><br />
                        <asp:TextBox ID="usernameTextBox" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="registerLabel">Password</td>
                    <td>
                        <asp:RequiredFieldValidator ID="passwordRequiredFieldValidator" runat="server" ErrorMessage="* Please enter your password" ControlToValidate="passwordTextBox" EnableClientScript="False" CssClass="validatorField" Font-Italic="True"></asp:RequiredFieldValidator><br />
                        <asp:TextBox ID="passwordTextBox" runat="server" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <a href="#">Forgot username and password ???</a>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" id="btnRegiterField">
                        <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" Width="80px" Height="30px" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </form>
</body>
</html>
