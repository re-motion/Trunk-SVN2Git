<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TabStripTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.TabStripTest" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Web.Development.WebTesting.TestSite</title>
  <style type="text/css">
    div.container {
      margin-top: 1em;
      padding: 0 1em 1em;
      border: 1px dotted black;
    }
  </style>
</head>
<body>
  <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" EnablePartialRendering="true" AsyncPostBackTimeout="3600" runat="server" />
    <div class="container">
      <h3>TabStrip1</h3>
      <remotion:WebTabStrip ID="MyTabStrip1" runat="server">
        <Tabs>
          <remotion:WebTab ItemID="Tab1" Text="Tab1Label"/>
          <remotion:WebTab ItemID="Tab2" Text="Tab2Label"/>
        </Tabs>
      </remotion:WebTabStrip>
      <div id="scope">
        <h3>TabStrip2</h3>
        <remotion:WebTabStrip ID="MyTabStrip2" runat="server">
          <Tabs>
            <remotion:WebTab ItemID="Tab1" Text="Tab1Label"/>
            <remotion:WebTab ItemID="Tab2" Text="Tab2Label"/>
          </Tabs>
        </remotion:WebTabStrip>
      </div>
    </div>
    <div class="container">
      <h3>Test output:</h3>
      <asp:UpdatePanel ID="MainUpdatePanel" UpdateMode="Always" runat="server">
        <ContentTemplate>
          <asp:Label ID="TestOutputLabel" ViewStateMode="Disabled" runat="server"></asp:Label>
        </ContentTemplate>
      </asp:UpdatePanel>
    </div>
  </form>
</body>
</html>
