<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest.Main" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MultiWindowTest</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <p>Main.aspx running in MainFunction</p>
      <p><remotion:SmartLabel ID="MainSmartLabel" Text="MainSmartLabel" runat="server" /></p>
      <p><remotion:WebButton ID="LoadFrameFunction" Text="LoadFrameFunction" runat="server"/></p>
      <p><remotion:WebButton ID="LoadAutoMainRefreshingFrameFunction" Text="LoadAutoMainRefreshingFrameFunction" runat="server"/></p>
      <p><remotion:WebButton ID="OpenNewWindowFromMain" Text="OpenNewWindowFromMain" runat="server"/></p>
      <p><remotion:WebButton ID="OpenNewWindowAndFunctionFromMain" Text="OpenNewWindowAndFunctionFromMain" runat="server"/></p>
      <div style="border: 1px solid black">
        <iframe src="Frame.wxe" height="500" width="500"></iframe>
      </div>
    </div>
    </form>
</body>
</html>
