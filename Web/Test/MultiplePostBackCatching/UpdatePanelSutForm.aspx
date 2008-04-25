<%@ Page Language="c#" Codebehind="UpdatePanelSutForm.aspx.cs" AutoEventWireup="True"
  Inherits="Remotion.Web.Test.MultiplePostBackCatching.UpdatePanelSutForm" SmartNavigation="False" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head" runat="server">
  <title>MultiplePostbackCatching Inside UpdatePanel</title>
  <remotion:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
</head>
<body>
  <form id="MyForm" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" />
    <asp:UpdatePanel ID="UpdatePanel" runat="server">
      <ContentTemplate>
        <asp:PlaceHolder ID="SutPlaceHolder" runat="server" />
      </ContentTemplate>
    </asp:UpdatePanel>
  </form>
</body>
</html>
