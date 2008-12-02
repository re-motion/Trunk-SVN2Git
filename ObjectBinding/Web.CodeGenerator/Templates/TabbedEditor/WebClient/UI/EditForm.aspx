<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit$DOMAIN_CLASSNAME$Form.aspx.cs" Inherits="$PROJECT_ROOTNAMESPACE$.UI.Edit$DOMAIN_CLASSNAME$Form" %>
<%@ Register TagPrefix="app" TagName="NavigationTabs" Src="NavigationTabs.ascx" %>
<%@ Register TagPrefix="remotion" TagName="Edit$DOMAIN_CLASSNAME$Control" Src="Edit$DOMAIN_CLASSNAME$Control.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
  <title><!-- Page title set in Page_Load !--></title>
  <remotion:htmlheadcontents id="HtmlHeadContents" runat="server"></remotion:htmlheadcontents>
</head>

<body>
  <form id="ThisForm" runat="server">
    <remotion:BindableObjectDataSourceControl ID="CurrentObject" runat="server" Type="$DOMAIN_QUALIFIEDCLASSTYPENAME$" />
    <remotion:TabbedMultiView ID="MultiView" runat="server" CssClass="tabbedMultiView" >
      <TopControls>
        <app:NavigationTabs ID="TheNavigationTabs" runat="server" />
      </TopControls>
      <Views>
        <remotion:TabView ID="Edit$DOMAIN_CLASSNAME$View" Title="$res:Details" runat="server">
          <remotion:Edit$DOMAIN_CLASSNAME$Control ID="Edit$DOMAIN_CLASSNAME$Control" runat="server" />
        </remotion:TabView>
      </Views>
      <BottomControls>
        <remotion:WebButton ID="SaveButton" runat="server" Text="$res:Save" OnClick="SaveButton_Click" />
        <remotion:SmartLabel runat="server" Text="&nbsp;" />
        <remotion:WebButton ID="CancelButton" runat="server" Text="$res:Cancel" OnClick="CancelButton_Click" />
      </BottomControls>
    </remotion:TabbedMultiView>
  </form>
</body>

</html>
