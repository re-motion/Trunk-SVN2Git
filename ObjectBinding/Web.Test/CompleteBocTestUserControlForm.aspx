<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Register TagPrefix="iuc" TagName="CompleteBocTestUserControl" Src="CompleteBocTestUserControl.ascx" %>
<%@ Page language="c#" Codebehind="CompleteBocTestUserControlForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.CompleteBocUserControlForm" %>



<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>CompleteBocTest: UserControl Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
<remotion:htmlheadcontents runat="server" id="HtmlHeadContents"></remotion:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<h1>CompleteBocTest: UserControl Form</h1>
<p><iuc:CompleteBocTestUserControl id="CompleteBocTestUserControl" runat="server"></iuc:CompleteBocTestUserControl></p>
<p><remotion:ValidationStateViewer id="ValidationStateViewer1" runat="server" noticetext="Es sind fehlerhafte Eingaben gefunden worden." visible="true"></remotion:ValidationStateViewer></p></form>
  </body>
</html>
