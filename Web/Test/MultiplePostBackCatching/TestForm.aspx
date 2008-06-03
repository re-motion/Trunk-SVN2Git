<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Page Language="C#" AutoEventWireup="true" Codebehind="TestForm.aspx.cs" Inherits="Remotion.Web.Test.MultiplePostBackCatching.TestForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
  <remotion:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
</head>
<body style="overflow: visible;">
  <form id="MyForm" runat="server">
    <asp:Table ID="TestTable" runat="server" EnableViewState="false" />
  </form>
</body>
</html>
