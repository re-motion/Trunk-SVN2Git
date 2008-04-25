<%@ Control Language="C#" AutoEventWireup="true" Codebehind="SecurityManagerCurrentTenantControl.ascx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.CurrentTenantControl" %>
 <div>
  <remotion:BocReferenceValue ID="CurrentUserField" runat="server" ReadOnly="True">
    <PersistedCommand>
      <remotion:BocCommand />
    </PersistedCommand>
  </remotion:BocReferenceValue>
</div>
<div>
  <remotion:BocReferenceValue ID="CurrentTenantField" runat="server" Required="True" OnSelectionChanged="CurrentTenantField_SelectionChanged" OnCommandClick="CurrentTenantField_CommandClick">
    <PersistedCommand>
      <remotion:BocCommand Show="ReadOnly" Type="Event" />
    </PersistedCommand>
    <DropDownListStyle AutoPostBack="True" />
  </remotion:BocReferenceValue>
</div>