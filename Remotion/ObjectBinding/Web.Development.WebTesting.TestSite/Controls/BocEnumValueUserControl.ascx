<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocEnumValueUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocEnumValueUserControl" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_DropDownListNormal"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="MarriageStatus"
        UndefinedItemText="Is_So_Undefined"
        
        Enabled="true"
        ListControlStyle-AutoPostBack="true"
        ListControlStyle-ControlType="DropDownList"
        ReadOnly="false"
        Required="false"
        
        runat="server"/>
    </td>
    <td>(DropDownList, normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_DropDownListReadOnly"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="MarriageStatus"
        UndefinedItemText="Is_So_Undefined"
        
        Enabled="true"
        ListControlStyle-AutoPostBack="true"
        ListControlStyle-ControlType="DropDownList"
        ReadOnly="true"
        Required="true"
        
        runat="server"/>
    </td>
    <td>(DropDownList, read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_DropDownListDisabled"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="MarriageStatus"
        UndefinedItemText="Is_So_Undefined"
        
        Enabled="false"
        ListControlStyle-AutoPostBack="true"
        ListControlStyle-ControlType="DropDownList"
        ReadOnly="false"
        Required="true"
        
        runat="server"/>
    </td>
    <td>(DropDownList, disabled)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_DropDownListNoAutoPostBack"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="MarriageStatus"
        UndefinedItemText="Is_So_Undefined"
        
        Enabled="true"
        ListControlStyle-AutoPostBack="false"
        ListControlStyle-ControlType="DropDownList"
        ReadOnly="false"
        Required="true"
        
        runat="server"/>
    </td>
    <td>(DropDownList, no auto postback)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_ListBoxNormal"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="MarriageStatus"
        UndefinedItemText="Is_So_Undefined"
        
        Enabled="true"
        ListControlStyle-AutoPostBack="true"
        ListControlStyle-ControlType="ListBox"
        ReadOnly="false"
        Required="false"
        
        runat="server"/>
    </td>
    <td>(ListBox, normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_ListBoxReadOnly"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="MarriageStatus"
        UndefinedItemText="Is_So_Undefined"
        
        Enabled="true"
        ListControlStyle-AutoPostBack="true"
        ListControlStyle-ControlType="ListBox"
        ReadOnly="true"
        Required="true"
        
        runat="server"/>
    </td>
    <td>(ListBox, read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_ListBoxDisabled"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="MarriageStatus"
        UndefinedItemText="Is_So_Undefined"
        
        Enabled="false"
        ListControlStyle-AutoPostBack="true"
        ListControlStyle-ControlType="ListBox"
        ReadOnly="false"
        Required="true"
        
        runat="server"/>
    </td>
    <td>(ListBox, disabled)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_ListBoxNoAutoPostBack"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="MarriageStatus"
        UndefinedItemText="Is_So_Undefined"
        
        Enabled="true"
        ListControlStyle-AutoPostBack="false"
        ListControlStyle-ControlType="ListBox"
        ReadOnly="false"
        Required="true"
        
        runat="server"/>
    </td>
    <td>(ListBox, no auto postback)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_RadioButtonListNormal"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="MarriageStatus"
        UndefinedItemText="Is_So_Undefined"
        
        Enabled="true"
        ListControlStyle-AutoPostBack="true"
        ListControlStyle-ControlType="RadioButtonList"
        ReadOnly="false"
        Required="false"
        
        runat="server"/>
    </td>
    <td>(RadioButtonList, normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_RadioButtonListReadOnly"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="MarriageStatus"
        UndefinedItemText="Is_So_Undefined"
        
        Enabled="true"
        ListControlStyle-AutoPostBack="true"
        ListControlStyle-ControlType="RadioButtonList"
        ReadOnly="true"
        Required="true"
        
        runat="server"/>
    </td>
    <td>(RadioButtonList, read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_RadioButtonListDisabled"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="MarriageStatus"
        UndefinedItemText="Is_So_Undefined"
        
        Enabled="false"
        ListControlStyle-AutoPostBack="true"
        ListControlStyle-ControlType="RadioButtonList"
        ReadOnly="false"
        Required="true"
        
        runat="server"/>
    </td>
    <td>(RadioButtonList, disabled)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocEnumValue ID="MarriageStatusField_RadioButtonListNoAutoPostBack"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="MarriageStatus"
        UndefinedItemText="Is_So_Undefined"
        
        Enabled="true"
        ListControlStyle-AutoPostBack="false"
        ListControlStyle-ControlType="RadioButtonList"
        ReadOnly="false"
        Required="true"
        
        runat="server"/>
    </td>
    <td>(RadioButtonList, no auto postback)</td>
  </tr>
</table>
