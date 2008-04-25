<%@ Register TagPrefix="uc1" TagName="TestTabbedPersonDetailsUserControl" Src="TestTabbedPersonDetailsUserControl.ascx" %>
<%@ Page language="c#" Codebehind="RepeaterTest.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.RepeaterTest" %>
<%@ Register TagPrefix="uc1" TagName="TestTabbedPersonJobsUserControl" Src="TestTabbedPersonJobsUserControl.ascx" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>RepeaterTest</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><remotion:htmlheadcontents id=HtmlHeadContents runat="server"></remotion:htmlheadcontents>
  </head>
<body MS_POSITIONING="FlowLayout">
<form id=Form1 method=post runat="server"><remotion:webbutton id=SaveButton runat="server" Text="Save"></remotion:webbutton>
<ros:ObjectBoundRepeater id=Repeater2 runat="server" propertyidentifier="Children" datasourcecontrol="CurrentObject">
<itemtemplate>
    <div>
    <remotion:boctextvalue id="FirstNameField" runat="server" ReadOnly="true" DataSourceControl="ItemDataSourceControl" PropertyIdentifier="FirstName">
</remotion:boctextvalue>
<remotion:bindableobjectdatasourcecontrol id="ItemDataSourceControl" runat="server" Type="Remotion.ObjectBinding.Sample::Person"/>
</div>
</ItemTemplate>
</ros:ObjectBoundRepeater>
<hr>
<ros:ObjectBoundRepeater id=Repeater3 runat="server" propertyidentifier="Children" datasourcecontrol="CurrentObject">
<itemtemplate>
<table style="width:100%">
<tr>
<td>
<!--DataEditUserControl-->
<uc1:testtabbedpersondetailsusercontrol id="TestTabbedPersonDetailsUserControl1" runat="server"></uc1:testtabbedpersondetailsusercontrol>
</td>
<td>
<!--DataEditUserControl-->
<uc1:testtabbedpersonjobsusercontrol id="TestTabbedPersonJobsUserControl1" runat="server"></uc1:testtabbedpersonjobsusercontrol>
</td></tr>
</table>
</ItemTemplate>
</ros:ObjectBoundRepeater>
<hr>
<remotion:bindableobjectdatasourcecontrol id=CurrentObject runat="server" Type="Remotion.ObjectBinding.Sample::Person"/></form>
  </body>
</html>
