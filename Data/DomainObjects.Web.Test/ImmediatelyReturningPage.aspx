<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImmediatelyReturningPage.aspx.cs" Inherits="Remotion.Data.DomainObjects.Web.Test.ImmediatelyReturningPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Immediately returning page</title>
</head>
<body>
    <p>Returning...</p>
    <form id="form1" runat="server">
    <div>
    <asp:Button ID="ReturnButton" runat="server" Text="Return" OnClick="ReturnButton_Click" />
    </div>
    </form>
</body>
</html>
