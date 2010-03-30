<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="OBWTest.ControlLayoutTests.Form" %>

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
        <table id=FormGrid runat="server">
          <tr>
            <td>Line 1</td>
            <td>
              <remotion:BocCheckBox ID="Line01CheckBox01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em"/> 
              <remotion:BocBooleanValue ID="Line01BooleanValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="Deceased" Width="3em" /> 
              <remotion:BocTextValue ID="Line01TextValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="LastName" Width="8em"/> 
              <remotion:BocEnumValue ID="Line01EnumValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="MarriageStatus" Width="8em"/> 
              <remotion:BocDateTimeValue ID="Line01DateTimeValue01" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="DateOfBirth" Width="15em"/> 
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
            <td>Line 1</td>
          </tr>

          <tr>
            <td>Line 2</td>
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
            <td>Line 2</td>
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

      </View>
      
      <BottomControls>
     </BottomControls>
    </remotion:SingleView>
</asp:content>
