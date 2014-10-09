<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Frame.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest.Frame" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MultiWindowTest</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <p>Frame.aspx running in FrameFunction</p>
      <p><remotion:SmartLabel ID="FrameSmartLabel" Text="FrameSmartLabel" runat="server" /></p>
      <p><remotion:WebButton ID="OpenNewWindowFromFrame" Text="OpenNewWindowFromFrame" runat="server"/></p>
      <p><remotion:WebButton ID="OpenNewWindowAndFunctionFromFrame" Text="OpenNewWindowAndFunctionFromFrame" runat="server"/></p>
      <p><remotion:WebButton ID="RefreshMainAsWell" Text="RefreshMainAsWell" runat="server"/></p>
    </div>
    </form>
</body>
</html>
