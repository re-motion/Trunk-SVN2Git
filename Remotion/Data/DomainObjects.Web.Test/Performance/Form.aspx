<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form.aspx.cs" Inherits="Remotion.Data.DomainObjects.Web.Test.Performance.Form" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Performance Test</title>
  <remotion:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
  <style type="text/css">
    html
    {
      height: 100%;
      margin: 0;
      padding: 0;
      overflow: hidden;
    }
    body
    {
      height: 100%;
      margin: 0;
      padding: 0;
      overflow: hidden;
    }
    form
    {
      height: 100%;
      width: 100%;
    }
    #UpdatePanel
    {
      height: 100%;
    }
  </style>
</head>
<body>
  <div style="height: 100%">
    <form id="TheForm" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" EnablePartialRendering="true" />
    <asp:UpdatePanel ID="UpdatePanel" runat="server">
      <contenttemplate>
        <remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.Data.DomainObjects.Web.Test::Domain.ClassForRelationTest" />
        <remotion:FormGridManager ID="FormGridManager" runat="server" />    
        <remotion:SingleView ID="TheView" runat="server">
          <TopControls>
            <asp:Literal ID="Top" runat="server">Top Controls</asp:Literal>
          </TopControls>
          <View>
            <table id="FormGrid" runat="server">
              <tr>
                <td>
                  <remotion:SmartLabel ID="ItemsLabel" runat="server" ForControl="ItemList" Text="Items" />
                </td>
                <td>
                  <remotion:BocList ID="ItemList" runat="server" DataSourceControl="CurrentObject" PageSize="100" Width="100%" Height="15em">
                    <FixedColumns>
                      <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="Name" ColumnTitle="Name" />
                      <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="ClassWithAllDataTypesMandatory.StringProperty" ColumnTitle="String" />
                      <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="ClassWithAllDataTypesMandatory.DateProperty" ColumnTitle="Date" />
                      <remotion:BocSimpleColumnDefinition PropertyPathIdentifier="ClassWithAllDataTypesMandatory.Int32Property" ColumnTitle="Int32" />
                    </FixedColumns>
                  </remotion:BocList>
                </td>
              </tr>
            </table>
          </View>
          <BottomControls>
            <remotion:WebButton ID="SynchPostBackButton" runat="server" Text="Synchronous PostBack" RequiresSynchronousPostBack="true" />
            <remotion:WebButton ID="AsynchPostBackButton" runat="server" Text="Asynchronous PostBack" RequiresSynchronousPostBack="false" />
          </BottomControls>
        </remotion:SingleView>
      </contenttemplate>
    </asp:UpdatePanel>
    </form>
  </div>
</body>
</html>
