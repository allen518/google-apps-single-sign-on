<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AuthGmail.aspx.cs" Inherits="AuthGmail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    username:<asp:TextBox ID="TextBox1" runat="server" placeholder="please key in your username" autofocus="true" />
    password:<asp:TextBox ID="TextBox2" runat="server" placeholder="please key in your password" TextMode="Password" />
    <button runat="server" onserverclick="Button1_Click" ID="Button1">
        <span class="state">登入</span>
    </button>
    </div>
    </form>
</body>
</html>
