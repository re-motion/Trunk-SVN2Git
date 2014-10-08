<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BocTreeViewUserControl.ascx.cs" Inherits="Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls.BocTreeViewUserControl" %>
<remotion:FormGridManager ID="FormGridManager" runat="server" />
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person" />
<table id="FormGrid" runat="server">
  <tr>
    <td></td>
    <td>
      <remotion:BocTreeView
        ID="Normal"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Children"
        ShowLines="True"
        EnableTopLevelExpander="True"
        EnableLookAheadEvaluation="True"
        runat="server">
      </remotion:BocTreeView>
    </td>
    <td>(normal)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTreeView
        ID="NoTopLevelExpander"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Children"
        ShowLines="True"
        Enabled="True"
        EnableTopLevelExpander="False"
        EnableLookAheadEvaluation="True"
        runat="server"/>
    </td>
    <td>(no top level expander)</td>
  </tr>
  <tr>
    <td></td>
    <td>
      <remotion:BocTreeView
        ID="NoLookAheadEvaluation"
        DataSourceControl="CurrentObject"
        PropertyIdentifier="Children"
        ShowLines="True"
        Enabled="True"
        EnableTopLevelExpander="True"
        EnableLookAheadEvaluation="False"
        runat="server"/>
    </td>
    <td>(no look ahead evaluation)</td>
  </tr>
  <tr>
    <td>Person</td>
    <td>
      <remotion:BocTreeView
        ID="NoPropertyIdentifier"
        DataSourceControl="CurrentObject"
        ShowLines="True"
        Enabled="True"
        EnableTopLevelExpander="True"
        EnableLookAheadEvaluation="True"
        runat="server"/>
    </td>
    <td>(no property identifier)</td>
  </tr>
</table>
