<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocMultilineTextValueUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocMultilineTextValueUserControl" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td>
      <remotion:BocMultilineTextValue ID="CVField_Normal"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="CV"
        ValueType="String"
        
        Enabled="true"
        ReadOnly="false"
        TextBoxStyle-AutoPostBack="true"

        runat="server"/>
    </td>
    <td>(normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocMultilineTextValue ID="CVField_ReadOnly"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="CV"
        ValueType="String"
        
        Enabled="true"
        ReadOnly="true"
        TextBoxStyle-AutoPostBack="true"
        
        runat="server"/>
    </td>
    <td>(read-only)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocMultilineTextValue ID="CVField_Disabled"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="CV"
        ValueType="String"
        
        Enabled="false"
        ReadOnly="false"
        TextBoxStyle-AutoPostBack="true"
        
        runat="server"/>
    </td>
    <td>(disabled)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocMultilineTextValue ID="CVField_NoAutoPostBack"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="CV"
        ValueType="String"
        
        Enabled="true"
        ReadOnly="false"
        TextBoxStyle-AutoPostBack="false"
        
        runat="server"/>
    </td>
    <td>(no auto postback)</td>
  </tr>
</table>
