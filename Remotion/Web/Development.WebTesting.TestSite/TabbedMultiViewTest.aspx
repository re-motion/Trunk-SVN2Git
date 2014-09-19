<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="TabbedMultiViewTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.TabbedMultiViewTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>TabbedMultiView</h3>
  <remotion:TabbedMultiView ID="MyTabbedMultiView" runat="server" Height="500">
    <TopControls>
      <span>TopControls</span>
    </TopControls>
    <Views>
      <remotion:TabView ID="Tab1" Title="Tab1Title" runat="server">
        <span>Content1</span>
      </remotion:TabView>
      <remotion:TabView ID="Tab2" Title="Tab2Title" runat="server">
        <span>Content2</span>
      </remotion:TabView>
    </Views>
    <BottomControls>
      <span>BottomControls</span>
    </BottomControls>
  </remotion:TabbedMultiView>
  <span>DoNotFindMe</span>
</asp:Content>