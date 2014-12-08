<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="board.aspx.cs" Inherits="Kanbean_Project.board" %>
<%@ Register Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" tagPrefix="ajax" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Lanban</title>
    <link rel="stylesheet" href="/css/style.css" type="text/css" />
    <link rel="stylesheet" href="//maxcdn.bootstrapcdn.com/font-awesome/4.2.0/css/font-awesome.min.css" />
    <script>

        //$(function () {
        //    $('#LogoutLink').click(function () {
        //        document.cookie = "UserSettings=; expires=Thu, 01 Jan 1970 00:00:00 UTC)";
        //    });
        //});

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
        <ajaxToolkit:ToolkitScriptManager ID="lanbanScriptManager" EnablePartialRendering="true" EnablePageMethods="true" runat="server" ></ajaxToolkit:ToolkitScriptManager>
        <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="header">
                    <div id="logo" title="Lanban">
                        <h1>Lanban</h1>
                    </div>
                    <div id="user-information">
                        <p>Welcome, <asp:LinkButton ID="linkBtnUsername" runat="server" Text="Username" OnClick="linkBtnUsername_Click"></asp:LinkButton>. (<asp:LinkButton ID="linkBtnLogout" runat="server" OnClick="EatCookies">Logout</asp:LinkButton>)</p>
                        <p>Project: <asp:DropDownList runat="server" ID="projectDropDownList" OnSelectedIndexChanged="projectDropDownList_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></p>
                    </div>
                </div>

                <div class="clear"></div>

                <div id="toolbar">
                    <asp:LinkButton runat="server" id="btnAddBacklog" CssClass="icon" ToolTip="Add new backlog" OnClick="btnAddBacklog_Click"></asp:LinkButton>
                    <asp:LinkButton runat="server" id="btnChart" CssClass="icon" ToolTip="Click to display the chart" OnClick="btnChart_Click"></asp:LinkButton>
                    <asp:LinkButton runat="server" id="btnSearch" CssClass="icon" ToolTip="Click to get the search field" OnClick="btnSearch_Click"></asp:LinkButton>
                    <asp:TextBox ID="tbxSearch" runat="server" Visible="False"></asp:TextBox>
                    <asp:LinkButton runat="server" id="btnFilter" CssClass="icon" ToolTip="Click to select category" Enabled="False" OnClick="btnFilter_Click"></asp:LinkButton>
                    <asp:DropDownList ID="dropdownFilter" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropdownFilter_SelectedIndexChanged" Visible="False">
                        <asp:ListItem Selected="True"></asp:ListItem>
                        <asp:ListItem>Tasks</asp:ListItem>
                        <asp:ListItem>Users</asp:ListItem>
                        <asp:ListItem>Comments</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="clear"></div>

                <asp:Table id="kanbanboard" border="1" runat="server"></asp:Table>

                <div class="clear"></div>
                <asp:Button runat="server" ClientIDMode="Static" ID="btnRefresh" />

                <ajaxToolkit:ModalPopupExtender ID="addandEditBacklogPopup" runat="server" TargetControlID="addandEditBacklogHiddenField" PopupControlID="addandEditBacklogPanel" CancelControlID="btnCancelAddandEditBacklog" BackgroundCssClass="popupbackground"></ajaxToolkit:ModalPopupExtender>
                <asp:HiddenField ID="addandEditBacklogHiddenField" runat="server" />
                <asp:Panel ID="addandEditBacklogPanel" runat="server" CssClass="popupmodal">
                    <fieldset>
                        <legend id="addandEditBacklogLegend" runat="server"></legend>
                        <div id="addandEditBacklogTable" runat="server" visible="true">
                            <table>
                                <tr>
                                    <td colspan="2">
                                        <asp:Label ID="lblBacklogNotice" runat="server" Text="" CssClass="noticeField"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Title</label><br />
                                        <asp:TextBox ID="titleTextBox" runat="server" Width="95%"></asp:TextBox>
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
                                        <asp:TextBox ID="deadlineTextBox" runat="server" Width="157" TextMode="Date"></asp:TextBox>
                                        <ajax:CalendarExtender ID="deadlineCalendarExtender" runat="server" TargetControlID="deadlineTextBox" Format="MM/dd/yyyy" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:Button ID="btnUpdateBacklog" runat="server" Text="Edit" OnClick="btnUpdateBacklog_Click" />
                                        <asp:Button ID="btnAddNewBacklog" runat="server" Text="Add" OnClick="btnAddNewBacklog_Click" />&nbsp;or&nbsp;
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
                            <asp:Label ID="lblAddedBacklogID" runat="server" visible="false"></asp:Label>
                            <table>
                                <tr>
                                    <td colspan="2">
                                        <asp:Label ID="lblTaskNotice" runat="server" Text="" CssClass="noticeField"></asp:Label>
                                    </td>
                                </tr>
								<tr>
									<td colspan="2">
										<label>Title</label><br />
										<asp:TextBox ID="titleTaskTextBox" runat="server" Width="340"></asp:TextBox>
									</td>
								</tr>
								<tr>
									<td>
										<label>Assignee</label><br />
                                        <asp:DropDownList runat="server" ID="assigneeTaskDropDownList" Width="95%"></asp:DropDownList>
									</td>
                                    <td>
										<label>Task Status</label><br />
                                        <asp:DropDownList runat="server" ID="statusTaskDropDownList" Width="160"></asp:DropDownList>
									</td>
								</tr>
								<tr>
                                    <td>
                                        <label>Estimation Hour</label><br />
                                        <asp:TextBox ID="estimationHourTaskTextBox" runat="server" TextMode="Number" Width="160"></asp:TextBox>
                                    </td>
                                    <td>
                                        <label>Time Spent</label><br />
                                        <asp:TextBox ID="spentTimeTaskTextBox" runat="server" TextMode="Number" Width="160"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label>Complexity</label><br />
                                        <asp:TextBox ID="complexityTaskTextBox" runat="server" TextMode="Number" Width="160"></asp:TextBox>
                                    </td>
                                    <td>
                                        <label>Deadline</label><br />
                                        <asp:TextBox ID="deadlineTaskTextBox" runat="server" Width="160" TextMode="Date"></asp:TextBox>
                                        <ajax:CalendarExtender ID="deadlineTaskCalendarExtender" runat="server" TargetControlID="deadlineTaskTextBox" Format="MM/dd/yyyy" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:Button ID="btnUpdateTask" runat="server" Text="Edit" OnClick="btnUpdateTask_Click" />
                                        <asp:Button ID="btnAddNewTask" runat="server" Text="Add" OnClick="btnAddNewTask_Click" />&nbsp;or&nbsp;
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
                    <fieldset>
                        <legend id="viewBacklogandTaskLegend" runat="server"></legend>
                        <asp:Literal ID="viewBacklogandTask" runat="server"></asp:Literal>
                        <asp:Button ID="btnEditViewTask" runat="server" Text="Edit" OnClick="btnEditTask_Click" />
                        <asp:Button ID="btnEditViewBacklog" runat="server" Text="Edit" OnClick="btnEditBacklog_Click" />&nbsp;or&nbsp;<asp:Button ID="btnCancelView" runat="server" Text="Cancel" />
                    </fieldset>
                </asp:Panel>

                <ajaxToolkit:ModalPopupExtender ID="deleteBacklogandTaskPopup" runat="server" TargetControlID="deleteBacklogandTaskHiddenField" PopupControlID="deleteBacklogandTaskPanel" CancelControlID="btnCancelDelete" BackgroundCssClass="popupbackground"></ajaxToolkit:ModalPopupExtender>
                <asp:HiddenField ID="deleteBacklogandTaskHiddenField" runat="server" />
                <asp:Panel ID="deleteBacklogandTaskPanel" runat="server" CssClass="popupmodal">
                    <fieldset>
                        <legend id="deleteBacklogorTaskLegend" runat="server"></legend>
                        Do you want to delete <asp:Label ID="lblDeleteItem" runat="server" Text=""></asp:Label>?<br /><br />
                        <asp:Button ID="btnDeleteBacklogorTask" runat="server" Text="Delete" OnClick="btnDeleteBacklogorTask_Click" />&nbsp;or&nbsp;<asp:Button ID="btnCancelDelete" runat="server" Text="Cancel" />
                    </fieldset>
                </asp:Panel>

                <ajaxToolkit:ModalPopupExtender ID="editComplexityPopup" runat="server" TargetControlID="viewEditComplexityHiddenField" PopupControlID="editComplexityPanel" CancelControlID="btnCancelEditC" BackgroundCssClass="popupbackground"></ajaxToolkit:ModalPopupExtender>
                <asp:HiddenField ID="viewEditComplexityHiddenField" runat="server" />
                <asp:Panel ID="editComplexityPanel" runat="server" CssClass="popupmodal">
                    <fieldset>
                        <legend id="editComplexityLegend" runat="server"></legend>
                        <asp:Label ID="lblComplexityNotice" runat="server" Text="" CssClass="noticeField"></asp:Label>
                        <br />
                        <asp:Label ID="lblEditComplexity" runat="server" Text="Complexity: "></asp:Label>
                        <asp:TextBox ID="txtBacklogComplexity" runat="server" TextMode="Number"></asp:TextBox>
                        <br />
                        <br />
                        <asp:Button ID="btnEditComplexity" runat="server" Text="Edit" OnClick="btnUpdateEditComplex_Click" />&nbsp;or&nbsp;<asp:Button ID="btnCancelEditC" runat="server" Text="Cancel" />
                    </fieldset>
                </asp:Panel>

                <ajaxToolkit:ModalPopupExtender runat="server" ID="editAssigneePopup" TargetControlID="viewEditAssigneeHiddenField" PopupControlID="editAssigneePanel" CancelControlID="btnCancelEditA" BackgroundCssClass="popupbackground"></ajaxToolkit:ModalPopupExtender> 
                <asp:HiddenField ID="viewEditAssigneeHiddenField" runat="server"/>
                <asp:Panel ID="editAssigneePanel" runat="server" CssClass="popupmodal">
                    <fieldset>
                        <legend id="editAssigneeLegend" runat="server"></legend>
                        <asp:Label ID="lblEditAssign" runat="server" Text="Assignee: "></asp:Label>
                        <asp:DropDownList ID="editAssigneeDropdownList" runat="server" Width="160"></asp:DropDownList>
                        <br />
                        <br />
                        <asp:Button ID="btnEditAssignee" runat="server" Text="Edit" OnClick="updateAssignee_Click" />&nbsp;or&nbsp;<asp:Button ID="btnCancelEditA" runat="server" Text="Cancel" />
                    </fieldset>
                </asp:Panel>

                <ajaxToolkit:ModalPopupExtender runat="server" ID="editDueDatePopup" TargetControlID="editDueDateHiddenField" PopupControlID="editDueDatePanel" CancelControlID="btnCancelEditDD" BackgroundCssClass="popupbackground"></ajaxToolkit:ModalPopupExtender>
                <asp:HiddenField ID="editDueDateHiddenField" runat="server" />
                <asp:Panel ID="editDueDatePanel" runat="server" CssClass="popupmodal">
                    <fieldset>
                        <legend id="editDueDateLegend" runat="server"></legend>
                        <asp:Label ID="lblDueDateNotice" runat="server" Text="" CssClass="noticeField"></asp:Label>
                        <br />
                        <asp:Label ID="editDueDateLabel" runat="server" Text="Deadline: "></asp:Label> 
                        <asp:TextBox ID="editDueDateTextBox" runat="server" Width="160" TextMode="Date"></asp:TextBox>
                        <ajax:CalendarExtender ID="editDueDateCalendarExtender" runat="server" TargetControlID="editDueDateTextBox" Format="MM/dd/yyyy" /><br /><br />
                        <asp:Button ID="btnUpdateDueDate" runat="server" Text="Edit" OnClick="btnUpdateDueDate_Click" />&nbsp;or&nbsp;<asp:Button ID="btnCancelEditDD" runat="server" Text="Cancel" />
                    </fieldset>
                </asp:Panel>

                <ajaxToolkit:ModalPopupExtender runat="server" ID="addCommentPopup" TargetControlID="addCommentHiddenField" PopupControlID="addCommentPanel" CancelControlID="btnCloseCmt" BackgroundCssClass="popupbackground"></ajaxToolkit:ModalPopupExtender>
                <asp:HiddenField ID="addCommentHiddenField" runat="server" />
                <asp:Panel ID="addCommentPanel" runat="server" CssClass="popupmodal">
                    <fieldset>
                        <legend id="addCommentLegend" runat="server"></legend>
                        <asp:GridView ID="commentGridView" runat="server" AutoGenerateColumns="false" BorderWidth="0" CellPadding="3" EmptyDataText = "No Comment">
                            <Columns>
                                <asp:BoundField DataField="User" ItemStyle-BorderWidth="0" HeaderStyle-BorderWidth="0" HeaderText="User" />
                                <asp:BoundField DataField="Comment" ItemStyle-BorderWidth="0" HeaderStyle-BorderWidth="0" HeaderText="Comment" />
                                <asp:TemplateField ItemStyle-BorderWidth="0" HeaderStyle-BorderWidth="0">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDeleteComment" CssClass="btnDeleteFileIcon" ToolTip="Delete Comment"  CommandArgument = '<%# Eval("ID") %>' runat = "server" OnClick="btnDeleteComment_Click" ></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <hr />
                        <asp:TextBox ID="addCommentTextBox" runat="server" ></asp:TextBox>
                        <asp:Button ID="btnAddComment" runat="server" Text="Add" OnClick="btnAddComment_Click" />&nbsp;&nbsp;<asp:Button ID="btnCloseCmt" runat="server" Text="Close" />
                    </fieldset>
                </asp:Panel>

                <ajaxToolkit:ModalPopupExtender runat="server" ID="showAttachedFilesPopup" TargetControlID="showAttachedFilesHiddenField" PopupControlID="showAttachedFilesPanel" CancelControlID="btnCloseFiles" BackgroundCssClass="popupbackground"></ajaxToolkit:ModalPopupExtender>
                <asp:HiddenField ID="showAttachedFilesHiddenField" runat="server" />
                <asp:Panel ID="showAttachedFilesPanel" runat="server" CssClass="popupmodal">
                    <fieldset>
                        <legend id="showAttachedFilesLegend" runat="server"></legend>
                        <asp:GridView ID="showAttachedFilesGridView" runat="server" AutoGenerateColumns="false" BorderWidth="0" CellPadding="3" EmptyDataText = "No files uploaded">
                            <Columns>
                                <asp:BoundField DataField="Text" ItemStyle-BorderWidth="0" HeaderStyle-BorderWidth="0" HeaderText="File Name" />
                                <asp:TemplateField ItemStyle-BorderWidth="0" HeaderStyle-BorderWidth="0">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDownloadFile" CssClass="btnDownloadFileIcon" ToolTip="Download file"  CommandArgument = '<%# Eval("Value") %>' runat="server" OnClick="btnDownloadFile_Click"></asp:LinkButton>&nbsp;&nbsp;
                                        <asp:LinkButton ID="btnDeleteFile" CssClass="btnDeleteFileIcon" ToolTip="Delete File"  CommandArgument = '<%# Eval("Value") %>' runat = "server" OnClick="btnDeleteFile_Click"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <hr />
                        <asp:FileUpload ID="AttachedFileUpload" runat="server" />
                        <asp:Button ID="btnUploadFile" runat="server" Text="Upload" OnClick="btnUploadFile_Click" />
                        &nbsp;or&nbsp;<asp:Button ID="btnCloseFiles" runat="server" Text="Close" />
                    </fieldset>
                </asp:Panel>
                

               <ajaxToolkit:ModalPopupExtender runat="server" ID="showUserinformationPopup" TargetControlID="showUserinformationHiddenField" PopupControlID="showUserInformationPanel" BackgroundCssClass="popupbackground"></ajaxToolkit:ModalPopupExtender>\
               <asp:HiddenField id="showUserinformationHiddenField" runat="server" />
                <asp:Panel ID="showUserInformationPanel" runat="server" CssClass="popupmodal">
                    <fieldset style="padding:1em">
                        <table id="registerTable">
                
                <tr>
                    
                    <td class="registerLabel">UserID</td>
                    <td>
                        <asp:Label ID="UserIDLabel" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="registerLabel">
                        <br />
                        Username</td>
                    <td>
                        <br />
                        <asp:Label ID="UsernameLabel" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="registerLabel">
                        <br />
                        Email</td>
                    <td>
                        <br />
                        <asp:Label ID="EmailLabel" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="registerLabel">
                        <br />
                        Level</td>
                    <td>
                        <br />
                        <asp:Label ID="LevelLabel" runat="server"></asp:Label>
                    </td>
                </tr>
               <tr>
                   <td class="registerLabel">
                       <br />
                       Project</td>
                   <td>
                       <br />
                       <asp:Label ID="ProjectLabel" runat="server"></asp:Label>
                   </td>
               </tr>
               <tr>
                   <br />
                   <td class="registerLabel">
                    <asp:Button ID="btnBack" runat="server" Text="Back" />
                   </td>
               </tr>
               
            </table>
                
               </fieldset>
               </asp:Panel>
                
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnUploadFile" />
                <asp:PostBackTrigger ControlID="showAttachedFilesGridView" />
            </Triggers>
        </asp:UpdatePanel>
     </form>
</body>
</html>