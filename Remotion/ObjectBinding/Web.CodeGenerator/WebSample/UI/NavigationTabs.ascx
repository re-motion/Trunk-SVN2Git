<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % version 3.0 as published by the Free Software Foundation.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NavigationTabs.ascx.cs" Inherits="WebSample.UI.NavigationTabs" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>

<remotion:TabbedMenu ID="TheTabbedMenu" runat="server">
  <Tabs>
    
    <remotion:MainMenuTab ItemID="PersonTab" Text="Person">
      <PersistedCommand>
        <remotion:NavigationCommand Type="None" />
      </PersistedCommand>
      <SubMenuTabs>
        <remotion:SubMenuTab ItemID="EditPersonTab" Text="$res:New">
          <PersistedCommand>
            <remotion:NavigationCommand HrefCommand-Href="EditPerson.wxe" />
          </PersistedCommand>
        </remotion:SubMenuTab>
        <remotion:SubMenuTab ItemID="SearchPersonTab" Text="$res:List">
          <PersistedCommand>
            <remotion:NavigationCommand HrefCommand-Href="SearchPerson.wxe" />
          </PersistedCommand>
        </remotion:SubMenuTab>
      </SubMenuTabs>
    </remotion:MainMenuTab>

  </Tabs>
</remotion:TabbedMenu>
