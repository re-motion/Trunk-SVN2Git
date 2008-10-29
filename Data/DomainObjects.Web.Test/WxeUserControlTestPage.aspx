<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WxeUserControlTestPage.aspx.cs" Inherits="Remotion.Data.DomainObjects.Web.Test.WxeUserControlTestPage" %>
<%@ Register TagPrefix="webTest" TagName="FirstControl" Src="FirstControl.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <label>This is the page.</label><BR />
      <label>ClientTransaction: </label><asp:Label id="ClientTransactionLabel" runat="server" /><BR />
      <webTest:FirstControl ID="FirstControl" runat="server"/>
    </div>
    </form>
</body>
</html>
