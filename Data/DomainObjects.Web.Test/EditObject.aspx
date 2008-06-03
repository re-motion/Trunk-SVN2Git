<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Page language="c#" Codebehind="EditObject.aspx.cs" AutoEventWireup="false" Inherits="Remotion.Data.DomainObjects.Web.Test.EditObjectPage" %>
<%@ Register TagPrefix="remotion" TagName="ControlWithAllDataTypes" Src="ControlWithAllDataTypes.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>Bestehendes Objekt mit allen Datentypen editieren</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <remotion:htmlheadcontents id="Htmlheadcontents1" runat="server"></remotion:htmlheadcontents>
  </HEAD>
  <body>
    <form id="DefaultForm" method="post" runat="server">
      <remotion:ControlWithAllDataTypes ID="ControlWithAllDataTypesControl" runat="server" />
    </form>
  </body>
</HTML>
