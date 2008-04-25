<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Remotion.SecurityManager.Clients.Web.Test._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">

<html >
<head runat="server">
    <title>Untitled Page</title>
    <remotion:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
    <p>
    <a href="UserList.wxe">Aufbauorganisation verwalten</a>
    </p>
    <p>
    <a href="SecurableClassDefinitionList.wxe?WxeReturnToSelf=True&TabbedMenuSelection=AccessControlTab">Berechtigungen verwalten</a>
    </p>
    <p>
      <asp:Button ID="EvaluateSecurity" runat="server" Text="Evaluate Security" OnClick="EvaluateSecurity_Click" />
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
