<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="WebTreeViewTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.WebTreeViewTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>WebTreeView1</h3>
  <remotion:WebTreeView ID="MyWebTreeView" runat="server"/>
  <div id="scope">
    <h3>WebTreeView2</h3>
    <remotion:WebTreeView ID="MyWebTreeView2" runat="server"/>
  </div>
</asp:Content>