<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationTabs.ascx.cs" Inherits="$PROJECT_ROOTNAMESPACE$.UI.NavigationTabs" %>

<remotion:TabbedMenu ID="TheTabbedMenu" runat="server">
  <Tabs>
    $REPEAT_FOREACHCLASS_BEGIN$
    <remotion:MainMenuTab ItemID="$DOMAIN_CLASSNAME$Tab" Text="$res:$DOMAIN_CLASSNAME$">
      <PersistedCommand>
        <remotion:NavigationCommand type="None" />
      </PersistedCommand>
      <SubMenuTabs>
        <remotion:SubMenuTab ItemID="Edit$DOMAIN_CLASSNAME$Tab" Text="$res:New">
          <PersistedCommand>
            <remotion:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="Edit$DOMAIN_CLASSNAME$" />
          </PersistedCommand>
        </remotion:SubMenuTab>
        <remotion:SubMenuTab ItemID="Search$DOMAIN_CLASSNAME$Tab" Text="$res:List">
          <PersistedCommand>
            <remotion:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="Search$DOMAIN_CLASSNAME$" />
          </PersistedCommand>
        </remotion:SubMenuTab>
      </SubMenuTabs>
    </remotion:MainMenuTab>
    $REPEAT_FOREACHCLASS_END$
  </Tabs>
</remotion:TabbedMenu>
