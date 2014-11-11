﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="board.aspx.cs" Inherits="Kanbean_Project.board" %>
<%@ Register Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" tagPrefix="ajax" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Lanban</title>
    <link rel="stylesheet" href="/css/style.css" type="text/css" />
    <link rel="stylesheet" href="//maxcdn.bootstrapcdn.com/font-awesome/4.2.0/css/font-awesome.min.css" />
    <script>
        function refreshBoard() {
            setTimeout(function () { document.getElementById('btnRefresh').click(); }, 200);
        }

        function dragover(ev) {
            ev.preventDefault();
        }

        function drag(ev) {
            var node = findParentNode(ev.target);
            ev.dataTransfer.setData("id", node.id);
            ev.dataTransfer.setData("oldSwimlane", node.parentElement.id);
        }

        function drop(ev) {
            ev.preventDefault();
            var data = ev.dataTransfer.getData("id");
            var backlogID, newSwimlane, newSwimlaneID, oldSwimlane, oldSwimlaneID;
            backlogID = data.replace('backlogArea', '');
            oldSwimlane = ev.dataTransfer.getData("oldSwimlane");
            oldSwimlaneID = oldSwimlane.replace('columnContent', '');
            if (ev.target.className == "board-content") {
                ev.target.appendChild(document.getElementById(data));
                newSwimlane = ev.target.id;
            }
            else {
                var node = findParentNode(ev.target);
                node.parentElement.insertBefore(document.getElementById(data), node);
                newSwimlane = node.parentElement.id;
            }
            newSwimlaneID = newSwimlane.replace('columnContent', '');

            var oldSwimlaneBacklog = [], oldSwimlaneBacklogPos = [];
            for (var i = 0; i < document.getElementById(oldSwimlane).childNodes.length; i++) {
                oldSwimlaneBacklog[i] = document.getElementById(oldSwimlane).childNodes[i].id.replace('backlogArea', '');
                oldSwimlaneBacklogPos[i] = i;

            }
            var newSwimlaneBacklog = [], newSwimlaneBacklogPos = [];
            for (var i = 0; i < document.getElementById(newSwimlane).childNodes.length; i++) {
                newSwimlaneBacklog[i] = document.getElementById(newSwimlane).childNodes[i].id.replace('backlogArea', '');
                newSwimlaneBacklogPos[i] = i;
            }

            PageMethods.updatePosition(oldSwimlaneID, oldSwimlaneBacklog, oldSwimlaneBacklogPos);
            PageMethods.updatePosition(newSwimlaneID, newSwimlaneBacklog, newSwimlaneBacklogPos);
            refreshBoard();
        }

        function findParentNode(childObj) {
            var testObj = childObj.parentNode;
            var count = 1;
            while (testObj.getAttribute('class') != "backlogArea") {
                testObj = testObj.parentNode;
                count++;
            }
            return testObj;
        }

    </script>
</head>
<body>
    <form id="lanbanboard" runat="server">
        <ajaxToolkit:ToolkitScriptManager ID="lanbanScriptManager" EnablePageMethods="true" runat="server" ></ajaxToolkit:ToolkitScriptManager>
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
                    <asp:LinkButton runat="server" id="btnSearch" CssClass="icon" ToolTip="Search" OnClick="btnSearch_Click"></asp:LinkButton>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:TextBox ID="tbxSearch" runat="server" Visible="False"></asp:TextBox>
                    &nbsp;<asp:LinkButton runat="server" id="btnFilter" CssClass="icon" ToolTip="Filter" Enabled="False" OnClick="btnFilter_Click"></asp:LinkButton>
                    <asp:DropDownList ID="dropdownFilter" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropdownFilter_SelectedIndexChanged" Visible="False">
                        <asp:ListItem>All categories</asp:ListItem>
                        <asp:ListItem>Tasks</asp:ListItem>
                        <asp:ListItem>Users</asp:ListItem>
                        <asp:ListItem>Comments</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="clear"></div>

                <asp:Table id="kanbanboard" border="1" runat="server"></asp:Table>

                <div class="clear"></div>
                <asp:Label ID="test" runat="server" Text=""></asp:Label>
                <asp:Button runat="server" ClientIDMode="Static" ID="btnRefresh" />

                <ajaxToolkit:ModalPopupExtender ID="addandEditBacklogPopup" runat="server" TargetControlID="addandEditBacklogHiddenField" PopupControlID="addandEditBacklogPanel" CancelControlID="btnCancelAddandEditBacklog" BackgroundCssClass="popupbackground"></ajaxToolkit:ModalPopupExtender>
                <asp:HiddenField ID="addandEditBacklogHiddenField" runat="server" />
                <asp:Panel ID="addandEditBacklogPanel" runat="server" CssClass="popupmodal">
                    <fieldset>
                        <legend id="addandEditBacklogLegend" runat="server"></legend>
                        <div id="addandEditBacklogTable" runat="server" visible="true">
                            <table>
                                <tr>
                                    <td>
                                        <label>Title</label><br />
                                        <asp:TextBox ID="titleTextBox" runat="server" Width="340"></asp:TextBox>
                                    </td>
                                    <td>
                                        <label>Column</label><br />
                                        <asp:DropDownList runat="server" ID="swimlaneDropDownList" Width="160"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td rowspan="4">
                                        <label>Description</label><br />
                                        <asp:TextBox ID="descriptionTextBox" runat="server" TextMode="MultiLine" Columns="40" Rows="12"></asp:TextBox>
                                    </td>
                                    <td>
                                        <label>Assignee</label><br />
                                        <asp:DropDownList runat="server" ID="assigneeDropDownList" Width="160"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
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
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Complexity</label><br />
                                        <asp:TextBox ID="complexityTextBox" runat="server" TextMode="Number" Width="157"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Deadline</label><br />
                                        <asp:TextBox ID="deadlineTextBox" runat="server" Width="157"></asp:TextBox>
                                        <ajax:CalendarExtender ID="deadlineCalendarExtender" runat="server" TargetControlID="deadlineTextBox" Format="MM/dd/yyyy" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:Button ID="btnUpdateBacklog" runat="server" Text="Edit" OnClick="btnUpdateBacklog_Click" OnClientClick="refreshBoard()" />
                                        <asp:Button ID="btnAddNewBacklog" runat="server" Text="Add" OnClick="btnAddNewBacklog_Click" OnClientClick="refreshBoard()" />&nbsp;or&nbsp;
                                        <asp:Button ID="btnCancelAddandEditBacklog" runat="server" Text="Cancel" />
                                    </td>
                                </tr>
                            </table>
                        </div>          
                    </fieldset>
                </asp:Panel>

                <ajaxToolkit:ModalPopupExtender ID="addandEditTaskPopup" runat="server" TargetControlID="addandEditTaskHiddenField" PopupControlID="addandEditTaskPanel" CancelControlID="btnCancelAddandEditTask" BackgroundCssClass="popupbackground"></ajaxToolkit:ModalPopupExtender>
                <asp:HiddenField ID="addandEditTaskHiddenField" runat="server" />
                <asp:Panel ID="addandEditTaskPanel" runat="server" CssClass="popupmodal">
                    <fieldset>
                        <legend id="addandEditTaskLegend" runat="server"></legend>
                        <div id="addandEditTaskTable" runat="server" visible="true">
                            <table>
								<tr>
									<td colspan="2">
										<label>Title</label><br />
										<asp:TextBox ID="titleTaskTextBox" runat="server" Width="340"></asp:TextBox>
									</td>
								</tr>
								<tr>
									<td colspan="2">
										<label>Assignee</label><br />
                                        <asp:DropDownList runat="server" ID="assigneeTaskDropDownList" Width="160"></asp:DropDownList>
									</td>
								</tr>
								<tr>
                                    <td>
                                        <label>Estimation Hour</label><br />
                                        <asp:TextBox ID="estimationHourTaskTextBox" runat="server" TextMode="Number" Width="160"></asp:TextBox>
                                    </td>
                                    <td>
                                        <label>Time Spent</label><br />
                                        <asp:TextBox ID="timeSpentTaskTextBox" runat="server" TextMode="Number" Width="160"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Complexity</label><br />
                                        <asp:TextBox ID="complexityTaskTextBox" runat="server" TextMode="Number" Width="160"></asp:TextBox>
                                    </td>
                                    <td>
                                        <label>Deadline</label><br />
                                        <asp:TextBox ID="deadlineTaskTextBox" runat="server" Width="160"></asp:TextBox>
                                        <ajax:CalendarExtender ID="deadlineTaskCalendarExtender" runat="server" TargetControlID="deadlineTaskTextBox" Format="MM/dd/yyyy" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:Button ID="btnUpdateTask" runat="server" Text="Edit" OnClick="btnUpdateTask_Click" OnClientClick="refreshBoard()" />
                                        <asp:Button ID="btnAddNewTask" runat="server" Text="Add" OnClick="btnAddNewTask_Click" OnClientClick="refreshBoard()" />&nbsp;or&nbsp;
                                        <asp:Button ID="btnCancelAddandEditTask" runat="server" Text="Cancel" />
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

                <ajaxToolkit:ModalPopupExtender ID="deleteBacklogandTaskPopup" runat="server" TargetControlID="deleteBacklogandTaskHiddenField" PopupControlID="deleteBacklogandTaskPanel" CancelControlID="btnCancelDelete" BackgroundCssClass="popupbackground"></ajaxToolkit:ModalPopupExtender>
                <asp:HiddenField ID="deleteBacklogandTaskHiddenField" runat="server" />
                <asp:Panel ID="deleteBacklogandTaskPanel" runat="server" CssClass="popupmodal">
                    <fieldset style="padding:1em">
                        <legend id="deleteBacklogorTaskLegend" runat="server"></legend>
                        Do you want to delete <asp:Label ID="lblDeleteItem" runat="server" Text=""></asp:Label>?<br /><br />
                        <asp:Button ID="btnDeleteBacklogorTask" runat="server" Text="Delete" OnClick="btnDeleteBacklogorTask_Click" OnClientClick="refreshBoard()" />&nbsp;or&nbsp;<asp:Button ID="btnCancelDelete" runat="server" Text="Cancel" />
                    </fieldset>
                </asp:Panel>

            </ContentTemplate>
        </asp:UpdatePanel>
     </form>
</body>
</html>