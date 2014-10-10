<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest.Main" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MultiWindowTest</title>
</head>
<body>
    <form id="form1" runat="server">
      <asp:ScriptManager ID="ScriptManager" EnablePartialRendering="true" AsyncPostBackTimeout="3600" runat="server" />
      <div>
        <asp:UpdatePanel ID="UpdatePanel" UpdateMode="Always" runat="server">
          <ContentTemplate>
            <p>Main.aspx running in MainFunction</p>
            <p><asp:Label ID="MainSmartLabel" Text="MainSmartLabel" ViewStateMode="Disabled" runat="server" /></p>
            <p><remotion:WebButton ID="SimplePostBack" Text="Simple PostBack" runat="server"/></p>
            <p><remotion:WebButton ID="LoadFrameFunctionInFrame" Text="Load FrameFunction in Frame" runat="server"/></p>
            <p><remotion:WebButton ID="LoadFrameFunctionAsSubInFrame" Text="Load FrameFunction (as sub function) in Frame" runat="server"/></p>
            <p><remotion:WebButton ID="LoadWindowFunctionInFrame" Text="Load WindowFunction in Frame" runat="server"/></p>
            <p><remotion:WebButton ID="LoadMainAutoRefreshingFrameFunctionInFrame" Text="Load Main-auto-refreshing FrameFunction in Frame" runat="server"/></p>
            <p><remotion:WebButton ID="LoadWindowFunctionInNewWindow" Text="Load WindowFunction in new Window" runat="server"/></p>
          </ContentTemplate>
        </asp:UpdatePanel>
        <div style="border: 1px solid black">
          <iframe name="frame" src="Frame.wxe?AlwaysRefreshMain=False" height="500" width="1000"></iframe>
        </div>
      </div>
    </form>
</body>
</html>
