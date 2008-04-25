<%@ Page Language="C#" AutoEventWireup="true" Codebehind="SecurableClassDefinitionListForm.aspx.cs" Inherits="Remotion.SecurityManager.Clients.Web.UI.AccessControl.SecurableClassDefinitionListForm"
  MasterPageFile="../SecurityManagerMasterPage.Master" %>

<%@ Register TagPrefix="securityManager" Src="../ErrorMessageControl.ascx" TagName="ErrorMessageControl" %>
<%@ Register TagPrefix="securityManager" Assembly="Remotion.SecurityManager.Clients.Web" Namespace="Remotion.SecurityManager.Clients.Web.Classes" %>

<asp:Content ID="ActualTopControlsPlaceHolder" runat="server" ContentPlaceHolderID="TopControlsPlaceHolder">
  <securityManager:ErrorMessageControl ID="ErrorMessageControl" runat="server" />
</asp:Content>
<asp:Content ID="ActaulMainContentPlaceHolder" runat="server" ContentPlaceHolderID="MainContentPlaceHolder">
  <remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="Remotion.SecurityManager.Domain.Metadata.SecurableClassDefinition, Remotion.SecurityManager" />
  <securityManager:SecurableClassDefinitionTreeView ID="SecurableClassDefinitionTree" runat="server" DataSourceControl="CurrentObject" EnableLookAheadEvaluation="True" EnableTopLevelExpander="False" OnClick="SecurableClassDefinitionTree_Click" PropertyIdentifier="DerivedClasses" />
</asp:Content>
