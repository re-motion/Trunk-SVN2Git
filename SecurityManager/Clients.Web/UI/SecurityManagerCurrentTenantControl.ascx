<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SecurityManagerCurrentTenantControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.CurrentTenantControl" %>
 <div>
  <remotion:BocReferenceValue ID="CurrentUserField" runat="server" ReadOnly="True">
    <PersistedCommand>
      <remotion:BocCommand />
    </PersistedCommand>
  </remotion:BocReferenceValue>
  &nbsp;
  <remotion:BocReferenceValue ID="CurrentSubstitutionField" runat="server" Required="False" OnSelectionChanged="CurrentSubstitutionField_SelectionChanged" OnCommandClick="CurrentSubstitutionField_CommandClick" >
    <PersistedCommand>
      <remotion:BocCommand Show="ReadOnly" Type="Event" />
    </PersistedCommand>
    <DropDownListStyle AutoPostBack="True" />
  </remotion:BocReferenceValue>
</div>
<div>
  <remotion:BocReferenceValue ID="CurrentTenantField" runat="server" Required="True" OnSelectionChanged="CurrentTenantField_SelectionChanged" OnCommandClick="CurrentTenantField_CommandClick">
    <PersistedCommand>
      <remotion:BocCommand Show="ReadOnly" Type="Event" />
    </PersistedCommand>
    <DropDownListStyle AutoPostBack="True" />
  </remotion:BocReferenceValue>
</div>
