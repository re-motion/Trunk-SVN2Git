<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SingleViewTest.aspx.cs" 
    Inherits="Remotion.Web.Test.Theming.SingleViewTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<!-- <!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > -->


<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>SingleView theming test</title>
    <remotion:HtmlHeadContents runat="server" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <remotion:SingleView ID="MySingleView" runat="server">
      <TopControls>
        <h1>Single View Theming Test</h1>
      </TopControls>
      <View>
        <p>There should be something here.</p>
      </View>
      <BottomControls>
        <p>This is a test of the theming capabilities of the SingleViewControl.</p>
      </BottomControls>
    </remotion:SingleView>
    </div>
    </form>
</body>
</html>
