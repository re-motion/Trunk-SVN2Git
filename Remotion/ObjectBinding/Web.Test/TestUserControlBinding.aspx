<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % version 3.0 as published by the Free Software Foundation.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Page language="c#" Codebehind="TestUserControlBinding.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestUserControlBinding" %>


<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <meta name="vs_showGrid" content="False">
    <title>TestUserControlBinding</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <remotion:HtmlHeadContents id="HtmlHeadContents" runat="server"></remotion:HtmlHeadContents>
  </HEAD>
  <body>
    <form id="Form1" method="post" runat="server">
      <div visible="false" runat="server" style="BORDER-RIGHT: black 1px solid; BORDER-TOP: black 1px solid; FONT-SIZE: x-small; BORDER-LEFT: black 1px solid; BORDER-BOTTOM: black 1px solid; FONT-FAMILY: Verdana, Arial, Sans-Serif; BACKGROUND-COLOR: #ffffe0">
        <remotion:FormGridManager id="FormGridManager" runat="server"></remotion:FormGridManager>
        <remotion:BindableObjectDataSourceControl id="DataSource" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
      </div>
      <TABLE id="NameFormGrid" runat="server" style="WIDTH: 100%">
        <TR>
          <TD>
            <asp:Label id="Label1" runat="server">Personendaten</asp:Label></TD>
          <TD></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD>
            <remotion:BocTextValue id="BocTextValue1" runat="server"></remotion:BocTextValue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD></TD>
        </TR>
      </TABLE>
      <P>
        <remotion:UserControlBinding id="UserControlBinding1" runat="server" UserControlPath="address.ascx" DataSourceControl="DataSource"
          PropertyIdentifier="Partner"></remotion:UserControlBinding></P>
    </form>
  </body>
</HTML>
