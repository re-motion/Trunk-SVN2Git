<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Window.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest.Window" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MultiWindowTest</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <p>Window.aspx running in WindowFunction</p>
      <p><remotion:SmartLabel ID="WindowSmartLabel" Text="WindowSmartLabel" runat="server" /></p>
      <p><remotion:WebButton ID="DoSomething" Text="DoSomething" runat="server"/></p>
      <p><remotion:WebButton ID="Close" Text="Close" runat="server"/></p>
      <p><remotion:WebButton ID="CloseAndRefreshParentAsWell" Text="CloseAndRefreshParentAsWell" runat="server"/></p>
    </div>
    </form>
</body>
</html>
