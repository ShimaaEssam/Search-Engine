<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="search_project.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="fborm1" runat="server">
        <p style="font-size:x-large; font-family:'Blackadder ITC'; color:green"><b> Bang Bang ^^ </b></p>
    <div style="background-image:url(bg.jpg) ; background-repeat:no-repeat" />&nbsp;<asp:Label ID="Label1" runat="server" Text="Enter Your Query"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="txt_searchQuery" runat="server"></asp:TextBox>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button1" runat="server" OnClick="Search" Text="Search" />
        <br />
        <br />
        <asp:RadioButton ID="RadioButton1" runat="server" Text="Spelling Coorelation" />
        &nbsp;<br />
        <br />
        <asp:RadioButton ID="RadioButton2" runat="server" Text="Soundex" />
    
        <br />
        <br />
        <br />
        <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button6" runat="server" OnClick="Inverted_Index" Text="Inverted_Index" />
&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button7" runat="server" OnClick="Bi_Gram" Text="Bi_Gram" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <br />
        <br />
        <asp:TextBox ID="Result" runat="server" Height="224px" Width="1061px" style="margin-top: 181px"></asp:TextBox>
        <br />
    
    </div>
    </form>
</body>
</html>
