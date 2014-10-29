<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="User_Register_GUI.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server" >
 <meta name="viewport" content="width=device-width, height=device-height, initial-scale=1"  />
    
 <style>
        html {
        height:100%; overflow:hidden;border:0;
        }
        body {
         height:100%; overflow:hidden;margin:0;padding:0;
        }
        div {
            height:100%;
        }
        h1 {
        font-family:'Myriad Web Pro';
        }
        #nav {
        font-family:'Myriad Web Pro';
        line-height:30px;
        background-color:#eeeeee;
        height:400px;
        width:150px;
        float:left;
        padding:5px;
            
        }
        a {
        color:black;
        }
        #header {
            background-color:black;
            color:white;
            text-align:left;
            padding:4px;
        }
        legend {
        font-family:'Myriad Web Pro';
        font-size:30px;
         }
        lable.field {
            text-align:right;
            width:150px;
            float:right;
            display:block;
        }
        input.tb {
        float:left;
        }
        fieldset p {
        clear:both;
        padding:5px;
        }
        #footer {
            background-color:black;
            text-align:center;
            color:white;
            clear:both;
            height:40px;
        }
 </style>
    
</head>
<body style="height: 568px" >
    <form id="form1" runat="server">
            <div id="header">
           <h1>Lanban borard</h1>
            </div>

        <div id="nav">
          <a href="#">  Project </a></br>
           <a href="#"> ASD</a></br>
           <a href="#"> DSV</a></br>
         </div>
         <fieldset>
            <legend>User Registration</legend>
             <p>
            <asp:Label CssClass="field" ID="Label1" runat="server" Text="UserName" ></asp:Label>    
            <asp:TextBox CssClass="tb" ID="tbUser" runat="server" ></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbUser" ErrorMessage="Please enter User Name" ForeColor="Red"></asp:RequiredFieldValidator>
            </p>
            <p>      
            <asp:Label CssClass="field" ID="Label2" runat="server" Text="PassWord"></asp:Label> 
            <asp:TextBox CssClass="tb" ID="tbPass" runat="server"></asp:TextBox>
             
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbPass" ForeColor="Red">Please enter Password</asp:RequiredFieldValidator>
            </p>
            <p>
            <asp:Label CssClass="field" ID="Label3" runat="server" Text="Password confirmation"></asp:Label>
                                

            <asp:TextBox CssClass="tb" ID="tbPassConF" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="tbPassConF" ErrorMessage="Please confirm your password" ForeColor="Red"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="Please re-write your password" ControlToCompare="tbPass" ControlToValidate="tbPassConF" ForeColor="Red"></asp:CompareValidator>
             </p>
             <p>
            <asp:Label CssClass="field" ID="Label4" runat="server" Text="Email address"></asp:Label>
            <asp:TextBox CssClass="tb" ID="tbEmail" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="tbEmail" ErrorMessage="Please enter email address" ForeColor="Red"></asp:RequiredFieldValidator>
             </p>
             <p>
            <asp:Button ID="btnSub" runat="server" Text="Register" OnClick="btnSub_Click" />
     
            <asp:Button ID="btnCan" runat="server" Text="Cancel" />
            </p>
          </fieldset>
        <br />
        <div id="footer">
            </div>
     
    </form>
</body>
</html>
