<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="TabStripTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.TabStripTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>TabStrip1</h3>
  <remotion:WebTabStrip ID="MyTabStrip1" runat="server">
    <Tabs>
      <remotion:WebTab ItemID="Tab1" Text="Tab1Label"/>
      <remotion:WebTab ItemID="Tab2" Text="Tab2Label"/>
    </Tabs>
  </remotion:WebTabStrip>
  <div id="scope">
    <h3>TabStrip2</h3>
    <remotion:WebTabStrip ID="MyTabStrip2" runat="server">
      <Tabs>
        <remotion:WebTab ItemID="Tab1" Text="Tab1Label"/>
        <remotion:WebTab ItemID="Tab2" Text="Tab2Label"/>
      </Tabs>
    </remotion:WebTabStrip>
  </div>
</asp:Content>