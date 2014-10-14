<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="LabelTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.LabelTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>Label1 - re-motion SmartLabel</h3>
  <remotion:SmartLabel ID="MySmartLabel" Text="MySmartLabelContent" runat="server"/>
  <h3>Label2 - ASP.NET Label</h3>
  <asp:Label ID="MyAspLabel" Text="MyAspLabelContent" runat="server"/>
  <h3>Label3 - HTML span</h3>
  <span id="MyHtmlLabel" runat="server">MyHtmlLabelContent</span>
</asp:Content>