<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MainForm.aspx.cs" Inherits="Remotion.Web.Test.IFrameSupport.MainForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%">
<head runat="server">
  <title></title>
  <script type="text/javascript">
    function Refresh() {
      window.__doPostBack("RefreshButton", "");
    }
  </script>
</head>
<body style="height: 95%">
  <form id="form1" runat="server" style="height: 100%">
    <asp:ScriptManager runat="server" ID="ScriptManager" EnablePartialRendering="True" />
    <div style="float: left; width: 30%; height: 100%; border-right: 1px solid black">
      left<br />
      <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
          <asp:Button runat="server" ID="RefreshButton" />
          <br/>
          <%= DateTime.Now.ToLongTimeString() %>
        </ContentTemplate>
      </asp:UpdatePanel>
    </div>
    <div style="margin-left: 31%; height: 100%;">
      <iframe src="FrameContent.aspx" style="height: 100%; width: 100%"></iframe>
    </div>
  </form>
</body>
</html>
