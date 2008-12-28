<%-- This file is part of re-strict (www.re-motion.org)
 % Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SecurityManagerNavigationTabs.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.SecurityManagerNavigationTabs" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Register TagPrefix="securityManager" Src="SecurityManagerCurrentTenantControl.ascx" TagName="CurrentTenantControl" %>
<div id="CurrentTenantControl">
<securityManager:CurrentTenantControl id="SecurityManagerCurrentTenantControl" runat="server" EnableAbstractTenants="true"/>
</div>
<remotion:TabbedMenu ID="TabbedMenu" runat="server">
  <Tabs>
    <remotion:MainMenuTab ItemID="OrganizationalStructureTab" Text="$res:OrganizationalStructure">
      <PersistedCommand>
        <remotion:NavigationCommand Type="None" />
      </PersistedCommand>
      <SubMenuTabs>
        <remotion:SubMenuTab ItemID="UserTab" Text="$res:User">
          <PersistedCommand>
            <remotion:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="UserList" />
          </PersistedCommand>
        </remotion:SubMenuTab>
        <remotion:SubMenuTab ItemID="GroupTab" Text="$res:Group">
          <PersistedCommand>
            <remotion:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="GroupList" />
          </PersistedCommand>
        </remotion:SubMenuTab>
        <remotion:SubMenuTab ItemID="TenantTab" Text="$res:Tenant">
          <PersistedCommand>
            <remotion:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="TenantList" />
          </PersistedCommand>
        </remotion:SubMenuTab>
        <remotion:SubMenuTab ItemID="PositionTab" Text="$res:Position">
          <PersistedCommand>
            <remotion:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="PositionList" />
          </PersistedCommand>
        </remotion:SubMenuTab>
        <remotion:SubMenuTab ItemID="GroupTypeTab" Text="$res:GroupType">
          <PersistedCommand>
            <remotion:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="GroupTypeList" />
          </PersistedCommand>
        </remotion:SubMenuTab>
      </SubMenuTabs>
    </remotion:MainMenuTab>
    <remotion:MainMenuTab ItemID="AccessControlTab" Text="$res:AccessControl">
      <PersistedCommand>
        <remotion:NavigationCommand Type="None" />
      </PersistedCommand>
      <SubMenuTabs>
        <remotion:SubMenuTab ItemID="SecurableClassDefinitionTab" Text="$res:SecurableClassDefinition">
          <PersistedCommand>
            <remotion:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="SecurableClassDefinitionList" WxeFunctionCommand-Parameters="&quot;Tenant|00000001-0000-0000-0000-000000000001|System.Guid&quot;" />
          </PersistedCommand>
        </remotion:SubMenuTab>
      </SubMenuTabs>
    </remotion:MainMenuTab>
  </Tabs>
</remotion:TabbedMenu>
