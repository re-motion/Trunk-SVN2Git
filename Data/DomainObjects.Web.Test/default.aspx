<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Page language="c#" Codebehind="default.aspx.cs" AutoEventWireup="false" Inherits="Remotion.Data.DomainObjects.Web.Test.DefaultPage" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>default</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body >
    <form id="Form1" method="post" runat="server">
      
      <a href="WxeHandler.ashx?WxeFunctionType=Remotion.Data.DomainObjects.Web.Test.WxeFunctions.EditObjectFunction, Remotion.Data.DomainObjects.Web.Test">Bestehendes Objekt editieren</a><br>
      <a href="WxeHandler.ashx?WxeFunctionType=Remotion.Data.DomainObjects.Web.Test.WxeFunctions.NewObjectFunction, Remotion.Data.DomainObjects.Web.Test">Neues Objekt editieren</a><br>
      <a href="WxeHandler.ashx?WxeFunctionType=Remotion.Data.DomainObjects.Web.Test.WxeFunctions.SearchFunction, Remotion.Data.DomainObjects.Web.Test" target="_blank">Objekt suchen</a><br>
      <a href="WxeHandler.ashx?WxeFunctionType=Remotion.Data.DomainObjects.Web.Test.WxeFunctions.WxeTestPageFunction,Remotion.Data.DomainObjects.Web.Test&amp;ClassWithAllDataTypesID=ClassWithAllDataTypes|3f647d79-0caf-4a53-baa7-a56831f8ce2d|System.Guid" target="_blank">WxeFunctionTest</a><br>
      <a href="WxeHandler.ashx?WxeFunctionType=Remotion.Data.DomainObjects.Web.Test.WxeFunctions.UndefinedEnumTestFunction,Remotion.Data.DomainObjects.Web.Test">UndefinedEnumTest</a><br> 

    </form>
  </body>
</HTML>
