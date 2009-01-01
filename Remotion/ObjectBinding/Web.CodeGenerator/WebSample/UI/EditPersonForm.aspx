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
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditPersonForm.aspx.cs" Inherits="WebSample.UI.EditPersonForm" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Register TagPrefix="remotion" TagName="NavigationTabs" Src="NavigationTabs.ascx" %>
<%@ Register TagPrefix="remotion" TagName="EditPersonControl" Src="EditPersonControl.ascx" %>
<%@ Register Assembly="Remotion.Data.DomainObjects.ObjectBinding.Web" Namespace="Remotion.Data.DomainObjects.ObjectBinding.Web" TagPrefix="dow" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html>

<head runat="server">
  <remotion:htmlheadcontents id="HtmlHeadContents" runat="server"></remotion:htmlheadcontents>
</head>

<body>
  <form id="TheForm" runat="server">
    <dow:DomainObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="DomainSample.Person, DomainSample" />
    <remotion:TabbedMultiView ID="MultiView" runat="server" CssClass="tabbedMultiView">
      <TopControls>
        <remotion:NavigationTabs ID="TheNavigationTabs" runat="server" />
      </TopControls>
      <Views>
        <remotion:TabView ID="EditPersonView" Title="$res:Details" runat="server">
          <remotion:EditPersonControl ID="EditPersonControl" runat="server" />
        </remotion:TabView>
      </Views>
      <BottomControls>
        <remotion:WebButton ID="SaveButton" runat="server" Text="$res:Save" OnClick="SaveButton_Click" />
        <remotion:SmartLabel runat="server" Text="&nbsp;" />
        <remotion:WebButton ID="CancelButton" runat="server" Text="$res:Cancel" OnClick="CancelButton_Click" />
      </BottomControls>
    </remotion:TabbedMultiView>
  </form>
</body>

</html>
