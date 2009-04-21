<%-- This file is part of re-strict (www.re-motion.org)
 % Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
 % 
 % This program is free software; you can redistribute it and/or modify
 % it under the terms of the GNU Affero General Public License version 3.0 
 % as published by the Free Software Foundation.
 % 
 % This program is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Affero General Public License for more details.
 % 
 % You should have received a copy of the GNU Affero General Public License
 % along with this program; if not, see http://www.gnu.org/licenses.
 % 
 % Additional permissions are listed in the file re-motion_exceptions.txt.
--%>
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
