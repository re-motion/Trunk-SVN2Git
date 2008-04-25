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
