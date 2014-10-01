<%@ Page Title="" Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="ControlTestForm.aspx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.ControlTestForm" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <asp:PlaceHolder ID="ControlPlaceHolder" runat="server" />
</asp:Content>
<asp:Content ContentPlaceHolderID="testOutput" runat="server">
  <asp:PlaceHolder ID="TestOutputControlPlaceHolder" runat="server" />
</asp:Content>