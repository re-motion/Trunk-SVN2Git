<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="FormGridTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.FormGridTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <remotion:FormGridManager runat="server"></remotion:FormGridManager>
  <h3>FormGrid1</h3>
  <table ID="My1FormGrid" runat="server">
    <tr>
      <td colspan="2">MyFormGrid1</td>
    </tr>
    <tr>
      <td></td>
      <td>DoNotFindMe2</td>
    </tr>
    <tr>
      <td></td>
      <td>Content1</td>
    </tr>
  </table>
  <div id="scope">
    <h3>FormGrid2</h3>
    <table ID="My2FormGrid" runat="server">
      <tr>
        <td colspan="2">MyFormGrid2</td>
      </tr>
      <tr>
        <td></td>
        <td>DoNotFindMe1</td>
      </tr>
      <tr>
        <td></td>
        <td>Content2</td>
      </tr>
    </table>
  </div>
</asp:Content>
