<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SingleSignOn.aspx.cs" Inherits="SingleSignOn" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script language="javascript"><!--
    function SubmitLoginForm() {
        document.forms[0].submit();
    }
    //-->
    </script>
</head>
<body onload="SubmitLoginForm();">
<div style="display: none;">
    <form id="FormSamlResponse" runat="server">
        <asp:Literal ID="ltrPostData" runat="server"></asp:Literal>
        <input type="submit" value="Submit SAML Response" />
    </form>
</div>
<i>Loading Google Application ...</i>
</body>
</html>
