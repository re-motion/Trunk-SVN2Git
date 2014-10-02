<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="HtmlAnchorTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.HtmlAnchorTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>HtmlAnchor</h3>
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <asp:LinkButton ID="MyHtmlAnchor" Text="MyHtmlAnchor" CommandName="MyCommand" runat="server"/>
      <div id="scope">
        <%-- ReSharper disable once Html.PathError --%>
        <asp:HyperLink ID="MyHtmlAnchor2" Text="MyHtmlAnchor2" NavigateUrl="HtmlAnchorTest.wxe" runat="server" />
      </div>
    </ContentTemplate>
  </asp:UpdatePanel>
</asp:Content>