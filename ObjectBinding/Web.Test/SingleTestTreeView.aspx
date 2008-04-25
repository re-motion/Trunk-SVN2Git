<%@ Page Language="c#" Codebehind="SingleTestTreeView.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.SingleTestTreeView" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
  <title>SingleTestTreeView</title>
  <remotion:HtmlHeadContents runat="server" ID="HtmlHeadContents" />
</head>
<body>
  <form id="Form" method="post" runat="server">
  <asp:ScriptManager ID="ScriptManager" runat="server" />
    <h1>
      SingleTest TreeView</h1>
    <table width="100%">
      <tr>
        <td valign="top" width="33%">
          <ros:PersonTreeView ID="PersonTreeView" runat="server" DataSourceControl="CurrentObject" CssClass="TreeBlock" EnableTopLevelExpander="False" EnableLookAheadEvaluation="True" />
          <asp:Button ID="RefreshPesonTreeViewButton" runat="server" Text="Refresh"></asp:Button>
        </td>
        <td valign="top" width="33%">
          <remotion:WebTreeView ID="WebTreeView" runat="server" CssClass="TreeBlock" Width="150px" EnableScrollBars="True" />
          <p>
            <asp:Button ID="PostBackButton" runat="server" Text="PostBack"></asp:Button></p>
          <p>
            <asp:Label ID="TreeViewLabel" runat="server" EnableViewState="False">#</asp:Label></p>
          <p>
            <asp:Button ID="Node332Button" runat="server" Text="Node 332"></asp:Button></p>
        </td>
        <td valign="top" width="33%">
          <asp:UpdatePanel ID="UpdatePanel" runat="server">
            <ContentTemplate>
              <ros:PersonTreeView ID="PersonTreeViewWithMenus" runat="server" DataSourceControl="CurrentObject" CssClass="TreeBlock" EnableTopLevelExpander="False" EnableLookAheadEvaluation="True" />
            </ContentTemplate>
          </asp:UpdatePanel>
        </td>
      </tr>
    </table>
    <remotion:FormGridManager ID="FormGridManager" runat="server" />
    <remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
  </form>
</body>
</html>
