<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditPermissionControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.AccessControl.EditPermissionControl" %>
<%@ Register TagPrefix="securityManager" Assembly="Remotion.SecurityManager.Clients.Web" Namespace="Remotion.SecurityManager.Clients.Web.Classes" %>

<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.AccessControl.Permission, Remotion.SecurityManager" />
<remotion:BusinessObjectReferenceDataSourceControl ID="AccessType" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="AccessType" />
<remotion:BocBooleanValue ID="AllowedField" runat="server" DataSourceControl="CurrentObject" PropertyIdentifier="BinaryAllowed" ShowDescription="False" Width="1em" />
<remotion:BocTextValue ID="NameField" runat="server" DataSourceControl="AccessType" PropertyIdentifier="DisplayName" ReadOnly="True" />
