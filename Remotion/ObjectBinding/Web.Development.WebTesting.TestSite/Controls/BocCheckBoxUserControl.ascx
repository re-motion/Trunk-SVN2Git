<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocCheckBoxUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocCheckBoxUserControl" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td>
      <remotion:BocCheckBox ID="DeceasedField_Normal"
        DataSourceControl="CurrentObject"
        FalseDescription="Is_So_False"
        PropertyIdentifier="Deceased"
        TrueDescription="Is_So_True"

        AutoPostBack="true"
        Enabled="true"
        ReadOnly="false"
        
        runat="server"/>
    </td>
    <td>(normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocCheckBox ID="DeceasedField_ReadOnly"
        DataSourceControl="CurrentObject"
        FalseDescription="Is_So_False"
        PropertyIdentifier="Deceased"
        TrueDescription="Is_So_True"

        AutoPostBack="true"
        Enabled="true"
        ReadOnly="true"
        
        runat="server"/>
    </td>
    <td>(read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocCheckBox ID="DeceasedField_Disabled"
        DataSourceControl="CurrentObject"
        FalseDescription="Is_So_False"
        PropertyIdentifier="Deceased"
        TrueDescription="Is_So_True"

        AutoPostBack="true"
        Enabled="false"
        ReadOnly="false"
        
        runat="server"/>
    </td>
    <td>(disabled)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocCheckBox ID="DeceasedField_NoAutoPostBack"
        DataSourceControl="CurrentObject"
        FalseDescription="Is_So_False"
        PropertyIdentifier="Deceased"
        TrueDescription="Is_So_True"

        AutoPostBack="false"
        Enabled="true"
        ReadOnly="false"
        
        runat="server"/>
    </td>
    <td>(no auto postback)</td>
  </tr> 
</table>
