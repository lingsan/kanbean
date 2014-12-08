<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="registration.aspx.cs" Inherits="Kanbean_Project.registration" EnableSessionState="True"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Lanban</title>
    <link rel="stylesheet" href="/css/style.css" type="text/css" />
</head>
<body>
    <form id="registerForm" runat="server">
        <h1 class="formTitle">Lanban</h1>
        <asp:Panel ID="RegisterPanel" runat="server" GroupingText="User Registration">
            <table id="registerTable">
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
                        <asp:CustomValidator ID="passwordCustomValidator" runat="server" ErrorMessage="* Password should have at least 6 characters,<br/>&nbsp;&nbsp;&nbsp;include 1 number and 1 capital letter" ControlToValidate="passwordTextBox" OnServerValidate="passwordCustomValidator_ServerValidate" EnableClientScript="False" CssClass="validatorField" Font-Italic="True"></asp:CustomValidator><br />
                        <asp:RequiredFieldValidator ID="passwordRequiredFieldValidator" runat="server" ErrorMessage="* Please enter your password" ControlToValidate="passwordTextBox" EnableClientScript="False" CssClass="validatorField" Font-Italic="True"></asp:RequiredFieldValidator><br />
                        <asp:TextBox ID="passwordTextBox" runat="server" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="registerLabel">Password Confirmation</td>
                    <td>
                        <asp:CompareValidator ID="passwordCompareValidator" runat="server" ErrorMessage="* Password must be the same" ControlToCompare="passwordTextBox" ControlToValidate="passwordconfirmTextBox" EnableClientScript="False" CssClass="validatorField" Font-Italic="True"></asp:CompareValidator><br />
                        <asp:RequiredFieldValidator ID="passwordconfirmRequiredFieldValidator" runat="server" ErrorMessage="* Please re-enter your password" ControlToValidate="passwordconfirmTextBox" EnableClientScript="False" CssClass="validatorField" Font-Italic="True"></asp:RequiredFieldValidator><br />
                        <asp:TextBox ID="passwordconfirmTextBox" runat="server" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="registerLabel">Email</td>
                    <td>
                        <asp:RegularExpressionValidator ID="emailRegularExpressionValidator" runat="server" ErrorMessage="* Please write the email in right format" ControlToValidate="emailTextBox" EnableClientScript="False" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" CssClass="validatorField" Font-Italic="True"></asp:RegularExpressionValidator><br />
                        <asp:RequiredFieldValidator ID="emailRequiredFieldValidator" runat="server" ErrorMessage="* Please enter your email address" ControlToValidate="emailTextBox" EnableClientScript="False" CssClass="validatorField" Font-Italic="True"></asp:RequiredFieldValidator><br />
                        <asp:TextBox ID="emailTextBox" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" id="btnRegiterField">
                        <asp:Button ID="btnRegiter" runat="server" Text="Register" OnClick="btnRegiter_Click" Width="80px" Height="30px" />
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <ajaxToolkit:ToolkitScriptManager ID="registerFormToolkitscript" runat="server" ></ajaxToolkit:ToolkitScriptManager>
        <asp:UpdatePanel ID="registerFormUpdatePanel" runat="server">
            <ContentTemplate>
                <ajaxToolkit:ModalPopupExtender ID="registerFormPopup" runat="server" TargetControlID="registerFormHiddenField" PopupControlID="registerFormPanel" CancelControlID="btnCancel" BackgroundCssClass="popupbackground"></ajaxToolkit:ModalPopupExtender>
                <asp:HiddenField ID="registerFormHiddenField" runat="server" />
                <asp:Panel ID="registerFormPanel" runat="server" CssClass="popupmodal">
                    <fieldset>
                        <asp:Label ID="resultLabel" runat="server" Text=""></asp:Label><br />
                        <asp:Button ID="btnOK" runat="server" Text="OK" OnClick="btnOK_Click" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
                    </fieldset>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
