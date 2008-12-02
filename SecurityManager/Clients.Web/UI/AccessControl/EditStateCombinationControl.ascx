<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditStateCombinationControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.AccessControl.EditStateCombinationControl" %>
<%@ Register TagPrefix="securityManager" Assembly="Remotion.SecurityManager.Clients.Web" Namespace="Remotion.SecurityManager.Clients.Web.Classes" %>
<div>
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.AccessControl.StateCombination, Remotion.SecurityManager" />
<div id="StateDefinitionContainer" runat="server" style="white-space:nowrap;">
<remotion:BocReferenceValue id="StateDefinitionField" runat="server" Required="True" Width="10em" />
<remotion:WebButton ID="DeleteStateDefinitionButton" runat="server" OnClick="DeleteStateDefinitionButton_Click"  CssClass="imageButton" style="padding-bottom:0.5em"/>
</div>
<asp:CustomValidator ID="RequiredStateCombinationValidator" runat="server" ErrorMessage="###" OnServerValidate="RequiredStateCombinationValidator_ServerValidate" style="display:block"/>
</div>
