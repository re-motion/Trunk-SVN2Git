<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit$DOMAIN_CLASSNAME$Form.aspx.cs" Inherits="$PROJECT_ROOTNAMESPACE$.UI.Edit$DOMAIN_CLASSNAME$Form" %>
<%@ Register TagPrefix="app" TagName="NavigationTabs" Src="NavigationTabs.ascx" %>
<%@ Register TagPrefix="remotion" TagName="Edit$DOMAIN_CLASSNAME$Control" Src="Edit$DOMAIN_CLASSNAME$Control.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
  <title>$res:$DOMAIN_CLASSNAME$</title>
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
