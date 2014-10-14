<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="TextBoxTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.TextBoxTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>TextBox1 - re-motion IEditableTextBox</h3>
  <testsite:TestEditableTextBox ID="MyEditableTextBox" Value="MyEditableTextBoxValue" AutoPostBack="true" runat="server"/>
  <h3>TextBox2 - ASP.NET TextBox</h3>
  <asp:TextBox ID="MyAspTextBox" Text="MyAspTextBoxValue" AutoPostBack="true" runat="server"/>
  <h3>TextBox3 - HTML input[type=text]</h3>
  <input id="MyHtmlTextBox" type="Text" value="MyHtmlTextBoxValue" runat="server"/>
  <div id="scope">
    <h3>TextBox4 - ASP.NET TextBox (no auto postback)</h3>
    <asp:TextBox ID="MyAspTextBoxNoAutoPostBack" Text="MyAspTextBoxNoAutoPostBackValue" AutoPostBack="false" runat="server"/>
  </div>
</asp:Content>