<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="board.aspx.cs" Inherits="Kanbean_Project.board" %>
<%@ Register Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" tagPrefix="ajax" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Lanban</title>
    <link rel="stylesheet" href="/css/style.css" type="text/css" />
    <link rel="stylesheet" href="//maxcdn.bootstrapcdn.com/font-awesome/4.2.0/css/font-awesome.min.css" />
</head>
<body>
    <form id="lanbanboard" runat="server">
        <ajaxToolkit:ToolkitScriptManager ID="lanbanScriptManager" runat="server" ></ajaxToolkit:ToolkitScriptManager>
        <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="header">
                    <div id="logo" title="Lanban">
                        <h1>Lanban</h1>
                    </div>
                    <div id="user-information">
                        <p>Welcome, <span id="username">Username</span>. (<a href="#">logout</a>)</p>
                        <p>Project: <asp:DropDownList runat="server" ID="projectDropDownList"></asp:DropDownList></p>
                    </div>
                </div>

                <div class="clear"></div>

                <div id="toolbar">
                    <asp:LinkButton runat="server" id="btnAddBacklog" CssClass="icon" ToolTip="Add new backlog" OnClick="btnAddBacklog_Click"></asp:LinkButton>
                    <asp:LinkButton runat="server" id="btnSearch" CssClass="icon" ToolTip="Search"></asp:LinkButton>
                    <asp:LinkButton runat="server" id="btnFilter" CssClass="icon" ToolTip="Filter"></asp:LinkButton>
                </div>

                <div class="clear"></div>

                <asp:Table id="kanbanboard" border="1" runat="server"></asp:Table>

                <div class="clear"></div>
                <asp:Label ID="test" runat="server" Text=""></asp:Label>

                <ajaxToolkit:ModalPopupExtender ID="addandEditBacklogPopup" runat="server" TargetControlID="addandEditBacklogHiddenField" PopupControlID="addandEditBacklogPanel" CancelControlID="btnCancelAddandEditBacklog" BackgroundCssClass="popupbackground"></ajaxToolkit:ModalPopupExtender>
                <asp:HiddenField ID="addandEditBacklogHiddenField" runat="server" />
                <asp:Panel ID="addandEditBacklogPanel" runat="server" CssClass="popupmodal">
                    <fieldset>
                        <legend id="addandEditBacklogLegend" runat="server"></legend>
                        <div id="addandEditBacklogTable" runat="server" visible="true">
                            <table>
                                <tr>
                                    <td>
                                        <p><label>Title</label><br />
                                        <asp:TextBox ID="titleTextBox" runat="server" Columns="40"></asp:TextBox></p>
                                        <p><label>Description</label><br />
                                        <asp:TextBox ID="descriptionTextBox" runat="server" TextMode="MultiLine" Columns="40" Rows="12"></asp:TextBox></p>
                                    </td>
                                    <td style="vertical-align:bottom;padding:0em 0.5em 1.3em 1em">
                                        <label>Column</label><br />
                                        <asp:DropDownList runat="server" ID="swimlaneDropDownList" Width="160"></asp:DropDownList>
                                        <p><label>Assignee</label><br />
                                        <asp:DropDownList runat="server" ID="assigneeDropDownList" Width="160"></asp:DropDownList></p>
                                        <label>Color</label><br />
                                        <asp:DropDownList runat="server" ID="colorDropDownList" Width="160">
                                            <asp:ListItem Text="Yellow" Value="#ffff4b,#ffff95"></asp:ListItem>
                                            <asp:ListItem Text="Red" Value="#ff4b4b,#ff9898"></asp:ListItem>
                                            <asp:ListItem Text="Purple" Value="#b64bff,#d598ff"></asp:ListItem>
                                            <asp:ListItem Text="Orange" Value="#ffa500,#ffc864"></asp:ListItem>
                                            <asp:ListItem Text="Green" Value="#4bff4b,#98ff98"></asp:ListItem>
                                            <asp:ListItem Text="Cyan" Value="#80ffff,#caffff"></asp:ListItem>
                                            <asp:ListItem Text="Blue" Value="#6464ff,#adadff"></asp:ListItem>
                                        </asp:DropDownList>
                                        <p><label>Complexity</label><br />
                                        <asp:TextBox ID="complexityTextBox" runat="server" TextMode="Number" Width="160"></asp:TextBox></p>
                                        <label>Deadline</label><br />
                                        <asp:TextBox ID="deadlineTextBox" runat="server" Width="160"></asp:TextBox>
                                        <ajax:CalendarExtender ID="deadlineCalendarExtender" runat="server" TargetControlID="deadlineTextBox" Format="MM/dd/yyyy" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:Button ID="btnUpdateBacklog" runat="server" Text="Edit" OnClick="btnUpdateBacklog_Click" OnClientClick="refreshBoard()" />
                                        <asp:Button ID="btnAddNewBacklog" runat="server" Text="Add" OnClick="btnAddNewBacklog_Click" OnClientClick="refreshBoard()" />&nbsp;or&nbsp;
                                        <asp:Button ID="btnCancelAddandEditBacklog" runat="server" Text="Cancel" />
                                        <asp:Button runat="server" ClientIDMode="Static" ID="btnRefresh" />
                                        <script>
                                            function refreshBoard() {
                                                setTimeout(function () { document.getElementById('btnRefresh').click(); }, 200);
                                            }
                                        </script>
                                    </td>
                                </tr>
                            </table>
                        </div>          
                    </fieldset>
                </asp:Panel>

                <ajaxToolkit:ModalPopupExtender ID="viewBacklogandTaskPopup" runat="server" TargetControlID="viewBacklogandTaskHiddenField" PopupControlID="viewBacklogandTaskPanel" CancelControlID="btnCancelView" BackgroundCssClass="popupbackground"></ajaxToolkit:ModalPopupExtender>
                <asp:HiddenField ID="viewBacklogandTaskHiddenField" runat="server" />
                <asp:Panel ID="viewBacklogandTaskPanel" runat="server" CssClass="popupmodal">
                    <fieldset style="padding:1em">
                        <legend id="viewBacklogandTaskLegend" runat="server"></legend>
                        <asp:Literal ID="viewBacklogandTask" runat="server"></asp:Literal>
                        <asp:Button ID="btnEditViewTask" runat="server" Text="Edit" />
                        <asp:Button ID="btnEditViewBacklog" runat="server" Text="Edit" OnClick="btnEditBacklog_Click" />&nbsp;or&nbsp;<asp:Button ID="btnCancelView" runat="server" Text="Cancel" />
                    </fieldset>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
     </form>
</body>
</html>