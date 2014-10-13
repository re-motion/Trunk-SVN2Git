<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Window.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest.Window" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MyWindow</title>
</head>
<body>
    <form id="form1" runat="server">
      <div>
        <p>Window.aspx running in WindowFunction</p>
        <p><asp:Label ID="WindowLabel" Text="WindowLabel" runat="server" /></p>
        <p><remotion:WebButton ID="SimplePostBack" Text="Simple PostBack" runat="server"/></p>
        <p><remotion:WebButton ID="Close" Text="Close" runat="server"/></p>
        <p><remotion:WebButton ID="CloseAndRefreshMainAsWell" Text="Close (and refresh Main even if started from Frame)" runat="server"/></p>
      </div>
    </form>
</body>
</html>
