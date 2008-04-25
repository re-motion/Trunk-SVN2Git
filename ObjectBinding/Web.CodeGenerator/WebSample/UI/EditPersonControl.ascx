<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditPersonControl.ascx.cs" Inherits="WebSample.UI.EditPersonControl" %>
<%@ Register Assembly="Remotion.ObjectBinding.Web" Namespace="Remotion.ObjectBinding.Web.UI.Controls" TagPrefix="obw" %>
<%@ Register Assembly="Remotion.Data.DomainObjects.ObjectBinding.Web" Namespace="Remotion.Data.DomainObjects.ObjectBinding.Web" TagPrefix="dow" %>
<%@ Register Assembly="Remotion.Web" Namespace="Remotion.Web.UI.Controls" TagPrefix="remotion" %>

<remotion:FormGridManager ID="FormGridManager" runat="server" />
<dow:DomainObjectDataSourceControl ID="CurrentObject" runat="server" TypeName="DomainSample.Person, DomainSample" />

<div>
  <table id="PersonFormGrid" runat="server">
  <tr>
    <td><remotion:SmartLabel ID="FirstNameLabel" runat="server" ForControl="FirstNameField" /></td>
    <td><obw:BocTextValue ID="FirstNameField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="FirstName" /></td>
  </tr>
  <tr>
    <td><remotion:SmartLabel ID="LastNameLabel" runat="server" ForControl="LastNameField" /></td>
    <td><obw:BocTextValue ID="LastNameField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="LastName" /></td>
  </tr>
  <tr>
    <td><remotion:SmartLabel ID="EMailAddressLabel" runat="server" ForControl="EMailAddressField" /></td>
    <td><obw:BocTextValue ID="EMailAddressField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="EMailAddress" /></td>
  </tr>
  <tr>
    <td><remotion:SmartLabel ID="LocationLabel" runat="server" ForControl="LocationField" /></td>
    <td>
			<obw:BocReferenceValue ID="LocationField" runat="server" DataSourceControl="CurrentObject"
					PropertyIdentifier="Location" Select="AllLocations" />
		</td>
  </tr>
  <tr>
    <td><remotion:SmartLabel ID="PhoneNumbersLabel" runat="server" ForControl="PhoneNumbersField" /></td>
    <td>
			<obw:BocList ID="PhoneNumbersField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="PhoneNumbers"
					OnMenuItemClick="PhoneNumbersField_MenuItemClick" >
				<FixedColumns>
				  <obw:BocAllPropertiesPlacehoderColumnDefinition />
					<obw:BocEditDetailsColumnDefinition CancelText="$res:Cancel" EditText="$res:Edit" SaveText="$res:Save" />
				</FixedColumns>
				<ListMenuItems>
					<obw:BocMenuItem ItemID="AddMenuItem" Text="$res:Add" />
				</ListMenuItems>
			</obw:BocList>
		</td>
  </tr>
</table>

</div>
