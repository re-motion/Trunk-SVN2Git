<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
