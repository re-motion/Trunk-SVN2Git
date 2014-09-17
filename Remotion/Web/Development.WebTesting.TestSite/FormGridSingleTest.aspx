<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormGridSingleTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.FormGridSingleTest" %>
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
    <table ID="MyFormGrid" runat="server">
      <tr>
        <td colspan="2">MyFormGrid</td>
      </tr>
      <tr>
        <td></td>
        <td>FindMe</td>
      </tr>
    </table>
    <span>DoNotFindMe</span>
  </form>
</body>
</html>
