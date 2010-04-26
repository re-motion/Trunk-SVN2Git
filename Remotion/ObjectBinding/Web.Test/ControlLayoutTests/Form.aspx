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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="OBWTest.ControlLayoutTests.Form" MasterPageFile="~/StandardMode.Master" %>

<%@ Register TagPrefix="obwt" TagName="NavigationTabs" Src="../UI/NavigationTabs.ascx" %>
<asp:content contentplaceholderid="head" runat="server">
</asp:content>
<asp:content contentplaceholderid="body" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" EnablePartialRendering="true" AsyncPostBackTimeout="3600" />
    <remotion:BindableObjectDataSourceControl id=CurrentObject runat="server" Type="Remotion.ObjectBinding.Sample::Person"/>
    <remotion:FormGridManager ID="FormGridManager" runat="server" />    
        
    <remotion:SingleView ID="OuterSingleView" runat="server">
      <TopControls>
        <obwt:NavigationTabs ID="NavigationTabs" runat="server" />
      </TopControls>
      
      <View>
      <asp:PlaceHolder runat="server">
        <table ID=FormGrid runat="server">
          <tr>
            <td>Line&nbsp;1</td>
            <td>
              <remotion:BocCheckBox ID="Line01CheckBox01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em"/> 
              <remotion:BocBooleanValue ID="Line01BooleanValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em" /> 
              <remotion:BocTextValue ID="Line01TextValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="LastName" Width="8em"/> 
              <remotion:BocEnumValue ID="Line01EnumValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="MarriageStatus" Width="8em"/> 
              <remotion:BocDateTimeValue ID="Line01DateTimeValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfBirth" Width="15em"/> 
              <remotion:BocDateTimeValue ID="Line01DateTimeValue02" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfDeath" Width="10em"/> 
              <remotion:BocReferenceValue ID="Line01ReferenceValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner" Width="15em">
                <OptionsMenuItems>
                  <remotion:WebMenuItem ItemID="Item1" Text="Item 1"/>
                </OptionsMenuItems>
              </remotion:BocReferenceValue> 
              <remotion:BocAutoCompleteReferenceValue ID="Line01AutoCompleteReferenceValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner" Width="15em">
                <OptionsMenuItems>
                  <remotion:WebMenuItem ItemID="Item1" Text="Item 1"/>
                </OptionsMenuItems>
              </remotion:BocAutoCompleteReferenceValue> 
            </td>
            <td>Line&nbsp;1</td>
          </tr>

          <tr>
            <td>Line&nbsp;2</td>
            <td>
              M
              <remotion:BocCheckBox ID="Line02CheckBox01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em"/> 
              M
              <remotion:BocBooleanValue ID="Line02BooleanValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em" /> 
              M
              <remotion:BocTextValue ID="Line02TextValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="LastName" Width="8em"/> 
              M
              <remotion:BocEnumValue ID="Line02EnumValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="MarriageStatus" Width="8em"/> 
              M
              <remotion:BocDateTimeValue ID="Line02DateTimeValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfBirth" Width="15em"/> 
              M
              <remotion:BocDateTimeValue ID="Line02DateTimeValue02" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfDeath" Width="10em"/> 
              M
              <remotion:BocReferenceValue ID="Line02ReferenceValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner" Width="15em">
                <OptionsMenuItems>
                  <remotion:WebMenuItem ItemID="Item1" Text="Item 1"/>
                </OptionsMenuItems>
              </remotion:BocReferenceValue> 
              M
              <remotion:BocAutoCompleteReferenceValue ID="Line02AutoCompleteReferenceValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner" Width="15em">
                <OptionsMenuItems>
                  <remotion:WebMenuItem ItemID="Item1" Text="Item 1"/>
                </OptionsMenuItems>
              </remotion:BocAutoCompleteReferenceValue> 
              M
            </td>
            <td>Line&nbsp;2</td>
          </tr>

          <tr>
            <td></td>
            <td></td>
            <td></td>
          </tr>

          <tr>
            <td></td>
            <td></td>
            <td></td>
          </tr>

          <tr>
            <td></td>
            <td></td>
            <td></td>
          </tr>


        </table>
        
        <div>
          Line 11
          M
          <remotion:BocCheckBox ID="Line11CheckBox01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em"/> 
          M
          <remotion:BocBooleanValue ID="Line11BooleanValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em" /> 
          M
          <remotion:BocTextValue ID="Line11TextValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="LastName" Width="8em"/> 
          M
          <remotion:BocEnumValue ID="Line11EnumValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="MarriageStatus" Width="8em"/> 
          M
          <remotion:BocDateTimeValue ID="Line11DateTimeValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfBirth" Width="15em"/> 
          M
          <remotion:BocDateTimeValue ID="Line11DateTimeValue02" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfDeath" Width="10em"/> 
          M
          <remotion:BocReferenceValue ID="Line11ReferenceValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner" Width="15em">
            <OptionsMenuItems>
              <remotion:WebMenuItem ItemID="Item1" Text="Item 1"/>
            </OptionsMenuItems>
          </remotion:BocReferenceValue> 
          M
          <remotion:BocAutoCompleteReferenceValue ID="Line11AutoCompleteReferenceValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Partner" Width="15em">
            <OptionsMenuItems>
              <remotion:WebMenuItem ItemID="Item1" Text="Item 1"/>
            </OptionsMenuItems>
          </remotion:BocAutoCompleteReferenceValue> 
          M
        </div>
      </asp:PlaceHolder>        
      </View>
      
      <BottomControls>
     </BottomControls>
    </remotion:SingleView>
</asp:content>
