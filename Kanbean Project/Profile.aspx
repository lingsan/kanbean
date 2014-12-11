<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="Kanbean_Project.Profile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Lanban</title>
    <link rel="stylesheet" href="/css/style.css" type="text/css" />
    <link rel="stylesheet" href="//maxcdn.bootstrapcdn.com/font-awesome/4.2.0/css/font-awesome.min.css" />
</head>
<body>
    <form id="userAccountForm" runat="server">
        <asp:ScriptManager ID="accountScriptManager" runat="server"></asp:ScriptManager>
        <asp:UpdatePanel ID="accountUpdatePanel" runat="server" UpdateMode="Conditional">
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
                    <asp:LinkButton runat="server" id="btnUserProfile" CssClass="icon" ToolTip="User Profile" OnClick="btnUserProfile_Click"></asp:LinkButton>
                    <asp:LinkButton runat="server" id="btnProjectManagement" CssClass="icon" ToolTip="Project Management" OnClick="btnProjectManagement_Click"></asp:LinkButton>
                    <asp:LinkButton runat="server" id="btnAccountManagement" CssClass="icon" ToolTip="Account Management" OnClick="btnAccountManagement_Click"></asp:LinkButton>
                </div>
                <div id="content">
                    <asp:Table runat="server" ID="userProfile">
                        <asp:TableRow>
                            <asp:TableCell ColumnSpan="2">
                                <h1>Profile</h1>
                                <hr />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell>
                                <ul>
                                    <li><asp:LinkButton ID="linkbtnSummary" runat="server" Text="Summary" OnClick="linkbtnSummary_Click"></asp:LinkButton></li>
                                    <li><asp:LinkButton ID="linkbtnEditProfile" runat="server" Text="Edit Profile" OnClick="linkbtnEditProfile_Click"></asp:LinkButton></li>
                                    <li><asp:LinkButton ID="linkbtnChangePassword" runat="server" Text="Change Password" OnClick="linkbtnChangePassword_Click"></asp:LinkButton></li>
                                </ul>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:Table runat="server" ID="tableSummary">
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="2"><h2>Summary</h2></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Username: </asp:TableCell>
                                        <asp:TableCell><asp:Label ID="lblUsername" runat="server" Text=""></asp:Label></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Email: </asp:TableCell>
                                        <asp:TableCell><asp:Label ID="lblEmail" runat="server" Text=""></asp:Label></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>User level: </asp:TableCell>
                                        <asp:TableCell><asp:Label ID="lblUserLevel" runat="server" Text=""></asp:Label></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Participated project(s): </asp:TableCell>
                                        <asp:TableCell><asp:Label ID="lblParticipatedProjects" runat="server" Text=""></asp:Label></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Default Project: </asp:TableCell>
                                        <asp:TableCell><asp:Label ID="lblDefaultProject" runat="server" Text=""></asp:Label></asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <asp:Table runat="server" ID="tableEditProfile">
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="2"><h2>Edit Profile</h2></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Email: </asp:TableCell>
                                        <asp:TableCell><asp:TextBox ID="emailTextbox" CssClass="accounttextbox" runat="server"></asp:TextBox></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Default Project: </asp:TableCell>
                                        <asp:TableCell><asp:DropDownList ID="defaultProjectDropDownList" CssClass="accountselectbox" runat="server"></asp:DropDownList></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="2">
                                            <asp:Button ID="btnSaveProfile" CssClass="buttonSubmit" runat="server" Text="Save" OnClick="btnSaveProfile_Click" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <asp:Table runat="server" ID="tableChangePass">
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="2"><h2>Change Password</h2></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Old password: </asp:TableCell>
                                        <asp:TableCell><asp:TextBox ID="oldPassTextBox" CssClass="accounttextbox" runat="server" TextMode="Password"></asp:TextBox></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>New password: </asp:TableCell>
                                        <asp:TableCell><asp:TextBox ID="newPassTextBox" CssClass="accounttextbox" runat="server" TextMode="Password"></asp:TextBox></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Re-enter new password: </asp:TableCell>
                                        <asp:TableCell><asp:TextBox ID="reenterNewPassTextBox" CssClass="accounttextbox" runat="server" TextMode="Password"></asp:TextBox></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="2">
                                            <asp:Button ID="btnChangePass" runat="server" CssClass="buttonSubmit" Text="Change Password" OnClick="btnChangePass_Click" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>

                    <asp:Table runat="server" ID="projectManagement">
                        <asp:TableRow>
                            <asp:TableCell ColumnSpan="2">
                                <h1>Project Management</h1>
                                <hr />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell>
                                <ul>
                                    <li><asp:LinkButton ID="linkbtnCreateProject" runat="server" Text="Create new project" OnClick="linkbtnCreateProject_Click"></asp:LinkButton></li>
                                    <li><asp:LinkButton ID="linkbtnAddMembers" runat="server" Text="Add project members" OnClick="linkbtnAddMembers_Click"></asp:LinkButton></li>
                                    <li><asp:LinkButton ID="linkbtnRemoveMembers" runat="server" Text="Remove project members" OnClick="linkbtnRemoveMembers_Click"></asp:LinkButton></li>
                                </ul>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:Table runat="server" ID="tableCreateProject">
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="2"><h2>Create new Project</h2></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Project name: </asp:TableCell>
                                        <asp:TableCell><asp:TextBox ID="newProjectNameTextBox" CssClass="accounttextbox" runat="server"></asp:TextBox></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Project members: </asp:TableCell>
                                        <asp:TableCell><asp:ListBox ID="newProjectMembersListBox" runat="server" CssClass="accountlistbox" SelectionMode="Multiple"></asp:ListBox></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="2">
                                            <asp:Button ID="btnCreateProject" runat="server" CssClass="buttonSubmit" Text="Create New Project" OnClick="btnCreateProject_Click" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <asp:Table runat="server" ID="tableAddMembers">
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="2"><h2>Add Project Members</h2></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Project: </asp:TableCell>
                                        <asp:TableCell><asp:DropDownList ID="AddMembersProjectDropDownList" CssClass="accountselectbox" runat="server" AutoPostBack ="true" OnSelectedIndexChanged="AddMembersProjectDropDownList_SelectedIndexChanged"></asp:DropDownList></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Add Member(s): </asp:TableCell>
                                        <asp:TableCell><asp:ListBox ID="AddProjectMembersListBox" runat="server" CssClass="accountlistbox" SelectionMode="Multiple"></asp:ListBox></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="2">
                                            <asp:Button ID="btnAddMembers" CssClass="buttonSubmit" runat="server" Text="Add" OnClick="btnAddMembers_Click" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <asp:Table runat="server" ID="tableRemoveMembers">
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="2"><h2>Remove Project Members</h2></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Project: </asp:TableCell>
                                        <asp:TableCell><asp:DropDownList ID="RemoveMembersProjectDropDownList" CssClass="accountselectbox" runat="server" AutoPostBack="true" OnSelectedIndexChanged="RemoveMembersProjectDropDownList_SelectedIndexChanged"></asp:DropDownList></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Remove Member(s): </asp:TableCell>
                                        <asp:TableCell><asp:ListBox ID="RemoveProjectMembersListBox" CssClass="accountlistbox" runat="server" SelectionMode="Multiple"></asp:ListBox></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="2">
                                            <asp:Button ID="btnRemoveMembers" CssClass="buttonSubmit" runat="server" Text="Remove" OnClick="btnRemoveMembers_Click" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>

                    <asp:Table runat="server" ID="accountManagement">
                        <asp:TableRow>
                            <asp:TableCell ColumnSpan="2">
                                <h1>Account Management</h1>
                                <hr />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell>
                                <ul>
                                    <li><asp:LinkButton ID="linkbtnCreateAccount" runat="server" Text="Create new account" OnClick="linkbtnCreateAccount_Click"></asp:LinkButton></li>
                                    <li><asp:LinkButton ID="linkbtnEditAccount" runat="server" Text="Edit Account" OnClick="linkbtnEditAccount_Click"></asp:LinkButton></li>
                                </ul>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:Table runat="server" ID="tableCreateAccount">
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="2"><h2>Create new Account</h2></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Username: </asp:TableCell>
                                        <asp:TableCell><asp:TextBox ID="newAccountUsernameTextBox" CssClass="accounttextbox" runat="server"></asp:TextBox></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Password: </asp:TableCell>
                                        <asp:TableCell><asp:TextBox ID="newAccountPasswordTextBox" CssClass="accounttextbox" runat="server"></asp:TextBox></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Email: </asp:TableCell>
                                        <asp:TableCell><asp:TextBox ID="newAccountEmailTextBox" CssClass="accounttextbox" runat="server"></asp:TextBox></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>User level: </asp:TableCell>
                                        <asp:TableCell>
                                            <asp:DropDownList ID="newAccountUserLevelDropDownList" CssClass="accountselectbox" runat="server">
                                                <asp:ListItem Text="admin" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="user" Value="2" Selected="True"></asp:ListItem>
                                            </asp:DropDownList>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Default Project: </asp:TableCell>
                                        <asp:TableCell><asp:DropDownList ID="newAccountDefaultProjectDropDownList" CssClass="accountselectbox" runat="server"></asp:DropDownList></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="2">
                                            <asp:Button ID="btnCreateAccount" CssClass="buttonSubmit" runat="server" Text="Create New Account" OnClick="btnCreateAccount_Click" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>

                                <asp:Table runat="server" ID="tableEditAccount">
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="2"><h2>Edit Account</h2></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Select Account: </asp:TableCell>
                                        <asp:TableCell><asp:DropDownList ID="selectAccountDropDownList" CssClass="accountselectbox" runat="server" AutoPostBack="true" OnSelectedIndexChanged="selectAccountDropDownList_SelectedIndexChanged"></asp:DropDownList></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Password: </asp:TableCell>
                                        <asp:TableCell><asp:TextBox ID="editPasswordTextBox" CssClass="accounttextbox" runat="server"></asp:TextBox></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Email: </asp:TableCell>
                                        <asp:TableCell><asp:TextBox ID="editEmailTextBox" CssClass="accounttextbox" runat="server"></asp:TextBox></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>User level: </asp:TableCell>
                                        <asp:TableCell><asp:DropDownList ID="editUserLevelDropDownList" CssClass="accountselectbox" runat="server" AutoPostBack ="true" ></asp:DropDownList></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell>Default Project: </asp:TableCell>
                                        <asp:TableCell><asp:DropDownList ID="editDefaultProjectDropDownList" CssClass="accountselectbox" runat="server"></asp:DropDownList></asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="2">
                                            <asp:Button ID="btnSaveEditAccount" CssClass="buttonSubmit" runat="server" Text="Save" OnClick="btnSaveEditAccount_Click" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>

                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
