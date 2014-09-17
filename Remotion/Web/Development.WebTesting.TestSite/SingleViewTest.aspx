<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SingleViewTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.SingleViewTest" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Web.Development.WebTesting.TestSite</title>
</head>
<body>
  <form id="form1" runat="server">
    <h1>SingleView</h1>
    <remotion:SingleView ID="MySingleView" runat="server" Height="500">
      <TopControls>
        <span>TopControls</span>
      </TopControls>
      <View>
        <span>Content</span>
      </View>
      <BottomControls>
        <span>BottomControls</span>
      </BottomControls>
    </remotion:SingleView>
    <span>DoNotFindMe</span>
  </form>
</body>
</html>
