<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="CommandTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.CommandTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <h3>Command1</h3>
      <testsite:TestCommand ID="Command1" ItemID="Command1ItemID" Text="Command1" CommandType="Event" runat="server"></testsite:TestCommand>
      <div id="scope">
        <h3>Command2</h3>
        <testsite:TestCommand ID="Command2" ItemID="Command2ItemID" Text="Command2" CommandType="Href" HrefCommandInfo-Href="CommandTest.wxe" runat="server"></testsite:TestCommand>
      </div>
    </ContentTemplate>
  </asp:UpdatePanel>
</asp:Content>