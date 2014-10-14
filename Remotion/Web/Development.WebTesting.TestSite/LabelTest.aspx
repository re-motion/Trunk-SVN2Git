<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="LabelTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.LabelTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>Label1</h3>
  <remotion:SmartLabel ID="MySmartLabel" Text="MySmartLabelContent" runat="server"/>
  <h3>Label2</h3>
  <asp:Label ID="MyAspLabel" Text="MyAspLabelContent" runat="server"/>
  <h3>Label3</h3>
  <span id="MyHtmlLabel" runat="server">MyHtmlLabelContent</span>
</asp:Content>