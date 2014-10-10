<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Frame.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.MultiWindowTest.Frame" %>
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
            <p>Frame.aspx running in FrameFunction</p>
            <p><asp:Label ID="FrameSmartLabel" Text="FrameSmartLabel" ViewStateMode="Disabled" runat="server" /></p>
            <p><remotion:WebButton ID="SimplePostBack" Text="SimplePostBack" runat="server"/></p>
            <p><remotion:WebButton ID="LoadWindowFunctionInNewWindow" Text="Load WindowFunction in new Window" runat="server"/></p>
            <p><remotion:WebButton ID="RefreshMainUpdatePanel" Text="Refresh Main UpdatePanel" runat="server"/></p>
          </ContentTemplate>
        </asp:UpdatePanel>
      </div>
    </form>
</body>
</html>
