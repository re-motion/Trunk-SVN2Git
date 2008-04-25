<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditStateCombinationControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.AccessControl.EditStateCombinationControl" %>
<%@ Register TagPrefix="securityManager" Assembly="Remotion.SecurityManager.Clients.Web" Namespace="Remotion.SecurityManager.Clients.Web.Classes" %>
<div style="border: solid 1px black">
<remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.AccessControl.StateCombination, Remotion.SecurityManager" />
<remotion:BocReferenceValue id="StateDefinitionField" runat="server" />
</div>