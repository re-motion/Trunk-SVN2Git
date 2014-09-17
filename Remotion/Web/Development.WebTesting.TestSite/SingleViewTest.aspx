<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SingleViewTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.SingleViewTest" %>
<%@ Import Namespace="System" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Web.Development.WebTesting.TestSite</title>
</head>
<body>
  <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" EnablePartialRendering="true" AsyncPostBackTimeout="3600" runat="server" />
    <remotion:FormGridManager runat="server"></remotion:FormGridManager>
    <h1>SingleView</h1>
    <remotion:SingleView ID="MySingleView" runat="server" Height="500">
      <TopControls>
        <span class="findmetop">FindMeInTopControls</span>
      </TopControls>
      <View>
        <span class="findme">FindMe</span>
      </View>
      <BottomControls>
        <span class="findmebottom">FindMeInBottomControls</span>
      </BottomControls>
    </remotion:SingleView>
    <span class="donotfindme">DoNotFindMe</span>
  </form>
</body>
</html>
