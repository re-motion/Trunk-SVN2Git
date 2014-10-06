<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="WebButtonTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.WebButtonTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <h3>WebButton1</h3>
      <remotion:WebButton ID="MyWebButton1Sync" Text="SyncButton" CommandName="Sync" RequiresSynchronousPostBack="true"  runat="server" />
      <h3>WebButton2</h3>
      <remotion:WebButton ID="MyWebButton2Async" Text="AsyncButton" CommandName="Async" runat="server" />
      <div id="scope">
        <h3>WebButton3</h3>
        <%-- ReSharper disable once Html.PathError --%>
        <remotion:WebButton ID="MyWebButton3Href" Text="HrefButton" PostBackUrl="WebButtonTest.wxe" runat="server" />
      </div>
    </ContentTemplate>
  </asp:UpdatePanel>
</asp:Content>