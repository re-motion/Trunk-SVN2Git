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
