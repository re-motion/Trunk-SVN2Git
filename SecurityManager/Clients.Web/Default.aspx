<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Default.aspx.cs" Inherits="Remotion.SecurityManager.Clients.Web.DefaultPage" %>

<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head runat="server">
  <title>Security Manager</title>
  <remotion:HtmlHeadContents ID="HtmlHeadContents" runat="server">
  </remotion:HtmlHeadContents>
</head>
<body>
  <form id="ThisForm" runat="server">
    <p>
      <a href="UserList.wxe">Aufbauorganisation verwalten</a>
    </p>
    <p>
      <a href="SecurableClassDefinitionList.wxe?WxeReturnToSelf=True&TabbedMenuSelection=AccessControlTab">Berechtigungen verwalten</a>
    </p>
    <p>
      <remotion:BocReferenceValue runat="server" ID="UsersField" OnSelectionChanged="UsersField_SelectionChanged">
        <PersistedCommand>
          <remotion:BocCommand />
        </PersistedCommand>
        <DropDownListStyle AutoPostBack="True" />
      </remotion:BocReferenceValue>
    </p>
  </form>
</body>
</html>
