<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="LabelTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.LabelTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>Label1</h3>
  <remotion:SmartLabel ID="MySmartLabel" Text="MySmartLabelContent" runat="server"/>
  <div id="scope">
    <h3>Label2</h3>
    <remotion:SmartLabel ID="MySmartLabel2" Text="MySmartLabelContent2" runat="server"/>
  </div>
</asp:Content>