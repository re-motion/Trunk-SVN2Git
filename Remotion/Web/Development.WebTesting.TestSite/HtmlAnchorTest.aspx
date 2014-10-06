<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="HtmlAnchorTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.HtmlAnchorTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <h3>HtmlAnchor1 - LinkButton</h3>
      <asp:LinkButton ID="MyHtmlAnchor" Text="MyHtmlAnchor" CommandName="MyCommand" runat="server"/>
      <div id="scope">
        <h3>HtmlAnchor2 - HyperLink</h3>
        <%-- ReSharper disable once Html.PathError --%>
        <asp:HyperLink ID="MyHtmlAnchor2" Text="MyHtmlAnchor2" NavigateUrl="HtmlAnchorTest.wxe" runat="server" />
      </div>
    </ContentTemplate>
  </asp:UpdatePanel>
</asp:Content>