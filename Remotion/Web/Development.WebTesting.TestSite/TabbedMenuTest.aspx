<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="TabbedMenuTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.TabbedMenuTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>TabbedMenu</h3>
  <remotion:TabbedMenu ID="MyTabbedMenu" StatusText=" MyStatusText " runat="server">
    <Tabs>
      <remotion:MainMenuTab ItemID="EventCommandTab" Text="EventCommandTabTitle">
        <PersistedCommand>
          <remotion:NavigationCommand Type="Event"/>
        </PersistedCommand>
      </remotion:MainMenuTab>
      <remotion:MainMenuTab ItemID="HrefCommandTab" Text="HrefCommandTabTitle">
        <PersistedCommand>
          <remotion:NavigationCommand Type="Href" HrefCommand-Href="TabbedMenuTest.wxe"/>
        </PersistedCommand>
      </remotion:MainMenuTab>
      <remotion:MainMenuTab ItemID="WxeFunctionCommandTab" Text="WxeFunctionCommandTabTitle">
        <PersistedCommand>
          <remotion:NavigationCommand Type="WxeFunction" WxeFunctionCommand-MappingID="TabbedMenuTest"/>
        </PersistedCommand>
      </remotion:MainMenuTab>
      <remotion:MainMenuTab ItemID="NoneCommandTab" Text="NoneCommandTabTitle">
        <PersistedCommand>
          <remotion:NavigationCommand Type="None"/>
        </PersistedCommand>
      </remotion:MainMenuTab>
      <remotion:MainMenuTab ItemID="TabWithSubMenu" Text="TabWithSubMenuTitle">
        <SubMenuTabs>
          <remotion:SubMenuTab ItemID="SubMenuTab1" Text="SubMenuTab1Title">
            <PersistedCommand>
              <remotion:NavigationCommand Type="Event"/>
            </PersistedCommand>
          </remotion:SubMenuTab>
          <remotion:SubMenuTab ItemID="SubMenuTab2" Text="SubMenuTab2Title">
            <PersistedCommand>
              <remotion:NavigationCommand Type="Event"/>
            </PersistedCommand>
          </remotion:SubMenuTab>
          <remotion:SubMenuTab ItemID="SubMenuTab3" Text="SubMenuTab3Title"/>
        </SubMenuTabs>
      </remotion:MainMenuTab>
    </Tabs>
  </remotion:TabbedMenu>
  <span>DoNotFindMe</span>
</asp:Content>