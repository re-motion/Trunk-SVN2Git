<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Page language="c#" AutoEventWireup="false" Inherits="Remotion.Web.UI.Controls.DatePickerPage" EnableSessionState="False"%>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head runat="server">
    <title>Date Picker</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
<remotion:htmlheadcontents id=HtmlHeadContents runat="server"></remotion:htmlheadcontents>
  </head>
<body >
<form id=Form method=post runat="server"><asp:calendar id=Calendar runat="server" Height="100%" Width="100%" BackColor="White" DayNameFormat="FirstLetter" ForeColor="Black" Font-Size="8pt" Font-Names="Verdana" BorderColor="#999999" CellPadding="4">
<todaydaystyle forecolor="Black" backcolor="#CCCCCC">
</TodayDayStyle>

<selectorstyle backcolor="#CCCCCC">
</SelectorStyle>

<nextprevstyle verticalalign="Bottom">
</NextPrevStyle>

<dayheaderstyle font-size="7pt" font-bold="True" backcolor="#CCCCCC">
</DayHeaderStyle>

<selecteddaystyle font-bold="True" forecolor="White" backcolor="#666666">
</SelectedDayStyle>

<titlestyle font-bold="True" bordercolor="Black" backcolor="#999999">
</TitleStyle>

<weekenddaystyle backcolor="#FFFFCC">
</WeekendDayStyle>

<othermonthdaystyle forecolor="#808080">
</OtherMonthDayStyle>
</asp:Calendar></FORM></body>
</html>
