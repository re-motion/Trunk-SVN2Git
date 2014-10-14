<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="InfrastructureTests.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.InfrastructureTests" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>Test FillWithAndWait from CoypuWaitingElementScopeExtensions</h3>
  <testsite:TestEditableTextBox ID="MyTextBox" Text="@SampleText@" AutoPostBack="true" runat="server"/>
</asp:Content>