<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Page language="c#" Codebehind="DesignTestTreeViewForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.Design.DesignTestTreeViewForm" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>DesignTest: TreeView Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><remotion:htmlheadcontents id=HtmlHeadContents runat="server"></remotion:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server"><remotion:webbutton id=PostBackButton runat="server" Text="PostBack"></remotion:webbutton>
<h1>DesignTest: TreeView Form</h1>
<p><ros:PersonTreeView id="PersonTreeView" runat="server" cssclass="TreeBlock" DataSourceControl="CurrentObject" enabletoplevelexpander="False" enablelookaheadevaluation="True"></ros:PersonTreeView></p>
<p>

<remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person"/></p></form>
  </body>
</html>
