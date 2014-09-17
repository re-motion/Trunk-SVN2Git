<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormGridTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.FormGridTest" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Web.Development.WebTesting.TestSite</title>
</head>
<body>
  <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" EnablePartialRendering="true" AsyncPostBackTimeout="3600" runat="server" />
    <remotion:FormGridManager runat="server"></remotion:FormGridManager>
    <h1>FormGrid</h1>
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
  </form>
</body>
</html>
