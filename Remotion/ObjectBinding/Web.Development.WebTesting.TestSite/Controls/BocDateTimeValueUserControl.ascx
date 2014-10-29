<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocDateTimeValueUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocDateTimeValueUserControl" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td>
      <remotion:BocDateTimeValue ID="DateOfBirthField_Normal"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="DateOfBirth"

        DateTimeTextBoxStyle-AutoPostBack="true"
        Enabled="true"
        ReadOnly="false"
        
        ShowSeconds="false"
        ValueType="DateTime"

        runat="server"/>
    </td>
    <td>(normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocDateTimeValue ID="DateOfBirthField_ReadOnly"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="DateOfBirth"

        DateTimeTextBoxStyle-AutoPostBack="true"
        Enabled="true"
        ReadOnly="true"
        
        ShowSeconds="false"
        ValueType="DateTime"

        runat="server"/>
    </td>
    <td>(read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocDateTimeValue ID="DateOfBirthField_Disabled"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="DateOfBirth"

        DateTimeTextBoxStyle-AutoPostBack="true"
        Enabled="false"
        ReadOnly="false"
        
        ShowSeconds="false"
        ValueType="DateTime"

        runat="server"/>
    </td>
    <td>(disabled)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocDateTimeValue ID="DateOfBirthField_NoAutoPostBack"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="DateOfBirth"

        DateTimeTextBoxStyle-AutoPostBack="false"
        Enabled="true"
        ReadOnly="false"
        
        ShowSeconds="false"
        ValueType="DateTime"

        runat="server"/>
    </td>
    <td>(no auto postback)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocDateTimeValue ID="DateOfBirthField_DateOnly"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="DateOfBirth"

        DateTimeTextBoxStyle-AutoPostBack="true"
        Enabled="true"
        ReadOnly="false"
        
        ShowSeconds="false"
        ValueType="Date"

        runat="server"/>
    </td>
    <td>(date-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocDateTimeValue ID="DateOfBirthField_ReadOnlyDateOnly"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="DateOfBirth"

        DateTimeTextBoxStyle-AutoPostBack="true"
        Enabled="true"
        ReadOnly="true"
        
        ShowSeconds="false"
        ValueType="Date"

        runat="server"/>
    </td>
    <td>(read-only, date-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocDateTimeValue ID="DateOfBirthField_WithSeconds"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="DateOfBirth"

        DateTimeTextBoxStyle-AutoPostBack="true"
        Enabled="true"
        ReadOnly="false"
        
        ShowSeconds="true"
        ValueType="DateTime"

        runat="server"/>
    </td>
    <td>(with seconds)</td>
  </tr>
</table>
