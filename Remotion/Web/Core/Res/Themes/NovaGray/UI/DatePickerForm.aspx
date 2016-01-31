<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (c) rubicon IT GmbH, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % as published by the Free Software Foundation; either version 2.1 of the 
 % License, or (at your option) any later version.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Page Language="c#" AutoEventWireup="false" Inherits="Remotion.Web.UI.Controls.DatePickerPage"
  EnableSessionState="False" %>

<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
  <title>Date Picker</title>
  <remotion:htmlheadcontents id="HtmlHeadContents" runat="server" />
</head>
<body>
  <form id="Form" method="post" runat="server">
  <asp:Calendar ID="Calendar" runat="server" Height="16em" Width="14em" Font-Size="100%" BackColor="White"
    DayNameFormat="FirstLetter" ForeColor="Black" BorderColor="Black" CellPadding="4">
    <TodayDayStyle ForeColor="Black" BackColor="#C5E0FC"></TodayDayStyle>
    <SelectorStyle BackColor="#C5E0FC"></SelectorStyle>
    <NextPrevStyle ForeColor="Black" VerticalAlign="Bottom"></NextPrevStyle>
    <DayHeaderStyle Font-Bold="True" BackColor="#F0F0F0"></DayHeaderStyle>
    <SelectedDayStyle Font-Bold="True" Font-Underline="True" ForeColor="Black" BackColor="#C5E0FC"></SelectedDayStyle>
    <TitleStyle ForeColor="Black" Font-Bold="True" BorderColor="Black" BackColor="#C5E0FC"></TitleStyle>
    <WeekendDayStyle BackColor="#F0F0F0"></WeekendDayStyle>
    <OtherMonthDayStyle ForeColor="Gray"></OtherMonthDayStyle>
  </asp:Calendar>
  </form>
</body>
</html>
