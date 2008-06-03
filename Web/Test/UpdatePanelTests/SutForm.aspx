<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Page Language="C#" AutoEventWireup="true" Codebehind="SutForm.aspx.cs" Inherits="Remotion.Web.Test.UpdatePanelTests.SutForm" %>

<%@ Register Src="SutUserControl.ascx" TagName="SutUserControl" TagPrefix="rwt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Update Panel SUT</title>
</head>
<body>
  <form id="MyForm" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" />
    <table style="width: 100%; height: 100%;">
      <tr>
        <td style="vertical-align: top;">
          <rwt:SutUserControl ID="SutUserControl" runat="server" />
        </td>
      </tr>
    </table>
  </form>
</body>
</html>
