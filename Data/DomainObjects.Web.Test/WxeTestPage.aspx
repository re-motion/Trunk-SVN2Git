<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Page language="c#" Codebehind="WxeTestPage.aspx.cs" AutoEventWireup="false" Inherits="Remotion.Data.DomainObjects.Web.Test.WxeTestPage" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>FirstPage</title>
    <remotion:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
  </HEAD>
<body>
<form id=Form1 method=post runat="server">
<P>
<asp:Label id="ResultLabel" runat="server">ResultLabel</asp:Label></P>
<h2>WXE-TransactionMode</h2>
<TABLE id="Table1" cellSpacing="1" cellPadding="10" border="1">
  <TR>
    <TD>WxeTransactionMode = CreateNew:</TD>
    <TD>
<asp:Button id="WxeTransactedFunctionCreateNewButton" runat="server" Text="Run Test"></asp:Button></TD></TR>
</TABLE><BR>

<h2>Write and read data with different values for TransactionMode and AutoCommit</h2>
<TABLE id="Table2" cellSpacing="1" cellPadding="10" border="1">
  <TR>
    <TD>
      <P align=right>TransactionMode:</P></TD>
    <TD>CreateNew</TD>
    <TD>None</TD></TR>
  <TR>
    <TD>AutoCommit = true</TD>
    <TD>
<asp:Button id="WxeTransactedFunctionCreateNewAutoCommitButton" runat="server" Text="Run Test"></asp:Button></TD>
    <TD></TD></TR>
  <TR>
    <TD>
      <P>AutoCommit = false</P></TD>
    <TD>
<asp:Button id="WxeTransactedFunctionCreateNewNoAutoCommitButton" runat="server" Text="Run Test"></asp:Button></TD>
    <TD>
<asp:Button id="WxeTransactedFunctionCreateNoneButton" runat="server" Text="Run Test"></asp:Button></TD></TR></TABLE>
<h2>Page step in nested transacted functions</h2>
<p><asp:Button id="WxeTransactedFunctionWithPageStepButton" runat="server" Text="Run Test"></asp:Button></p>
<P></P></form>
	
  </body>
</HTML>
