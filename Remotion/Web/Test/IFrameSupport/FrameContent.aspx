<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrameContent.aspx.cs" Inherits="Remotion.Web.Test.IFrameSupport.FrameContent" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
  <script type="text/javascript">
    function UpdateMain()
    {
      window.parent.Refresh();
    }
  </script>
</head>
<body>
    <form id="form1" runat="server">
      <asp:ScriptManager runat="server" ID="ScriptManager" EnablePartialRendering="True" />
    <div>
    right
      <asp:UpdatePanel runat="server">
        <ContentTemplate>
          <asp:Button runat="server" OnClick="Button_Click"/>          
          <br/>
          <%= DateTime.Now.ToLongTimeString() %>
        </ContentTemplate>
      </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
