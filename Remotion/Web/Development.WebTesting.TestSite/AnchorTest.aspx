<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="AnchorTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.AnchorTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <h3>HtmlAnchor1 - re-motion WebLinkButton</h3>
      <%-- ReSharper disable once Html.PathError --%>
      <remotion:WebLinkButton ID="MyWebLinkButton" Text="MyWebLinkButton" CommandName="MyWebLinkButtonCommand" PostBackUrl="AnchorTest.wxe" runat="server"/>
      <h3>HtmlAnchor2 - re-motion SmartHyperLink</h3>
      <%-- ReSharper disable once Html.PathError --%>
      <remotion:SmartHyperLink ID="MySmartHyperLink" Text="MySmartHyperLink" NavigateUrl="AnchorTest.wxe" runat="server"/>
      <h3>HtmlAnchor3 - ASP.NET LinkButton</h3>
      <asp:LinkButton ID="MyAspLinkButton" Text="MyAspLinkButton" CommandName="MyAspLinkButtonCommand" runat="server"/>
      <h3>HtmlAnchor4 - ASP.NET HyperLink</h3>
      <%-- ReSharper disable once Html.PathError --%>
      <asp:HyperLink ID="MyAspHyperLink" Text="MyAspHyperLink" NavigateUrl="AnchorTest.wxe" runat="server" />
      <div id="scope">
        <h3>HtmlAnchor5 - HTML a</h3>
        <a id="MyHtmlAnchor" href="AnchorTest.wxe" runat="server">MyHtmlAnchor</a>
      </div>
    </ContentTemplate>
  </asp:UpdatePanel>
</asp:Content>