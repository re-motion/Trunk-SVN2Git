<%@ Register TagPrefix="obr" Namespace="Remotion.ObjectBinding.Reflection" Assembly="Remotion.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="rwc" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Register TagPrefix="obc" Namespace="Remotion.ObjectBinding.Web.Controls" Assembly="Remotion.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="TestTabStripWithoutWxeForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestTabStripWithoutWxeForm"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head><title>Test Tabbed Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rwc:htmlheadcontents id=HtmlHeadContents runat="server"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<rwc:tabbedmultiview id=MultiView runat="server" cssclass="tabbedMultiView">
<topcontrols>
      <h1>Test Tabbed Form</h1>
      <rwc:TabStripMenu id="NavigationTabs" runat="server" width="100%">
<tabs>
<rwc:TabStripMainMenuItem ItemID="Tab1" Text="Tab 1">
<submenutabs>
<rwc:tabstripsubmenuitem ItemID="SubTab1" Text="Sub Tab 1.1"></rwc:tabstripsubmenuitem>
<rwc:tabstripsubmenuitem ItemID="SubTab2" Text="Sub Tab 1.2"></rwc:tabstripsubmenuitem>
<rwc:tabstripsubmenuitem ItemID="SubTab13" Text="Sub Tab 1.3"></rwc:tabstripsubmenuitem>
</submenutabs>
</rwc:TabStripMainMenuItem>
<rwc:TabStripMainMenuItem ItemID="Tab2" Text="Tab 2">
<submenutabs>
<rwc:TabStripSubMenuItem ItemID="SubTab1" Text="Sub Tab 2.1"></rwc:TabStripSubMenuItem>
<rwc:TabStripSubMenuItem ItemID="SubTab2" Text="Sub Tab 2.2"></rwc:TabStripSubMenuItem>
<rwc:tabstripsubmenuitem ItemID="SubTab23" Text="Sub Tab 2.3"></rwc:tabstripsubmenuitem>
</SubMenuTabs>
</rwc:TabStripMainMenuItem>
<rwc:TabStripMainMenuItem ItemID="Tab3" Text="Tab 3"></rwc:TabStripMainMenuItem>
</Tabs>
</rwc:TabStripMenu>
      <rwc:validationstateviewer id="ValidationStateViewer" runat="server" visible="true"></rwc:validationstateviewer>
</topcontrols>
<views> 
 <rwc:tabview id="first" title="First">
      <rwc:webtabstrip id="PagesTabStrip" runat="server" style="margin:3em"></rwc:webtabstrip>
 </rwc:tabview>
 <rwc:tabview id="second" title="Second">
 </rwc:tabview>
</Views>
</rwc:tabbedmultiview></form>
  </body>
</html>
