<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TabbedMultiViewTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.TabbedMultiViewTest" %>
<%@ Import Namespace="System" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Web.Development.WebTesting.TestSite</title>
</head>
<body>
  <form id="form1" runat="server">
    <h1>TabbedMultiView</h1>
    <remotion:TabbedMultiView ID="MyTabbedMultiView" runat="server" Height="500">
      <TopControls>
        <span>TopControls</span>
      </TopControls>
      <Views>
        <remotion:TabView ID="Tab1" Title="Tab1Title" runat="server">
          <span>Content1</span>
        </remotion:TabView>
        <remotion:TabView ID="Tab2" Title="Tab2Title" runat="server">
          <span>Content2</span>
        </remotion:TabView>
      </Views>
      <BottomControls>
        <span>BottomControls</span>
      </BottomControls>
    </remotion:TabbedMultiView>
    <span>DoNotFindMe</span>
  </form>
</body>
</html>
