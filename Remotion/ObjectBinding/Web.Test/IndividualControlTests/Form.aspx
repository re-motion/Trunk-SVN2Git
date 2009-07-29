<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
<%@ Page Language="c#" Codebehind="Form.aspx.cs" AutoEventWireup="True" Inherits="OBWTest.IndividualControlTests.IndividualControlTestForm" 
  Title="IndividualControlTestForm" %>
<%@ Register TagPrefix="obwt" TagName="NavigationTabs" Src="../UI/NavigationTabs.ascx" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
  <remotion:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" EnablePartialRendering="true" />
    <remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
    <remotion:SingleView ID="SingleView" runat="server">
      <TopControls>
        <obwt:NavigationTabs ID="NavigationTabs" runat="server" />
        <asp:PlaceHolder ID="ButtonPlaceHolder" runat="server">
          <div>
            <remotion:WebButton ID="PostBackButton" runat="server" Text="Post Back"/>&nbsp;
            <remotion:WebButton ID="SaveButton" runat="server" Width="10em" Text="Save" />&nbsp;
            <remotion:WebButton ID="SaveAndRestartButton" runat="server" Width="10em" Text="Save &amp; Restart" />&nbsp;
            <remotion:WebButton ID="CancelButton" runat="server" Width="10em" Text="Cancel" />
          </div>
        </asp:PlaceHolder>
      </TopControls>
      
      <View>
        <asp:UpdatePanel ID="UserControlUpdatePanel" runat="server">
          <contenttemplate>
            <asp:PlaceHolder ID="UserControlPlaceHolder" runat="server" />
          </contenttemplate>
        </asp:UpdatePanel>
      </View>
      
      <BottomControls>
         <asp:UpdatePanel ID="StackUpdatePanel" runat="server" UpdateMode="Conditional">
          <contenttemplate>
            <asp:Literal ID="Stack" runat="server" />
          </contenttemplate>
        </asp:UpdatePanel>
     </BottomControls>
    </remotion:SingleView>
</asp:Content>
