<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="SingleViewTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.SingleViewTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>SingleView</h3>
  <remotion:SingleView ID="MySingleView" runat="server" Height="500">
    <TopControls>
      <span>TopControls</span>
    </TopControls>
    <View>
      <span>Content</span>
    </View>
    <BottomControls>
      <span>BottomControls</span>
    </BottomControls>
  </remotion:SingleView>
  <span>DoNotFindMe</span>
</asp:Content>