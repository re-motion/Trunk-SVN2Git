<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditPermissionControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.AccessControl.EditPermissionControl" %>
<%@ Register TagPrefix="securityManager" Assembly="Remotion.SecurityManager.Clients.Web" Namespace="Remotion.SecurityManager.Clients.Web.Classes.AccessControl" %>

<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.AccessControl.Permission, Remotion.SecurityManager" />
<remotion:BusinessObjectReferenceDataSourceControl ID="AccessType" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="AccessType" />
<securityManager:PermissionBooleanValue ID="AllowedField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Allowed" ShowDescription="False" Width="16px" />
