<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TabbedMultiViewTest.aspx.cs" Inherits="Remotion.Web.Test.Theming.TabbedMultiViewTest" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Tabbed multi-view theming test</title>
    <remotion:HtmlHeadContents runat="server" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="test">
    <remotion:TabbedMultiView runat="server">
      <TopControls>
        <h1>Tabbed multi-view theming test</h1>
      </TopControls>
      <Views>
        <remotion:TabView ID="TabView1" runat="server" Title="View 1">
          <p>This is view no. 1</p>
        </remotion:TabView>
        <remotion:TabView ID="TabView2" runat="server" Title="View 2">
          <p>This is view no. 2</p>
        </remotion:TabView>
      </Views>
      <BottomControls>
        <p>This is a test of the theming capabilities of the TabbedMultiViewControl.</p>
      </BottomControls>
    </remotion:TabbedMultiView>
    </div>
    </form>
</body>
</html>
