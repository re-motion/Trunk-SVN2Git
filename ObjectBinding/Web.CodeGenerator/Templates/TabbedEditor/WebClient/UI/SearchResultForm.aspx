<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchResult$DOMAIN_CLASSNAME$Form.aspx.cs" Inherits="$PROJECT_ROOTNAMESPACE$.UI.SearchResult$DOMAIN_CLASSNAME$Form" %>
<%@ Register TagPrefix="app" TagName="NavigationTabs" Src="NavigationTabs.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">

<head>
  <title>$res:$DOMAIN_CLASSNAME$</title>
  <remotion:htmlheadcontents id="HtmlHeadContents" runat="server"></remotion:htmlheadcontents>
</head>

<body>
  <remotion:BindableObjectDataSourceControl id="$DOMAIN_CLASSNAME$SearchDataSource" runat="server" Type="$DOMAIN_QUALIFIEDCLASSTYPENAME$" Mode="Read" />
  <remotion:FormGridManager id="FormGridManager" runat="server" />
  <app:NavigationTabs id="NavigationTabs" runat="server" />
  <form id="Form1" method="post" runat="server">
    <remotion:BocList ID="$DOMAIN_CLASSNAME$List" runat="server" DataSourceControl="$DOMAIN_CLASSNAME$SearchDataSource" OnListItemCommandClick="$DOMAIN_CLASSNAME$List_ListItemCommandClick">
      <FixedColumns>
        <remotion:BocAllPropertiesPlacehoderColumnDefinition />
        <remotion:BocCommandColumnDefinition ItemID="Edit" Text="$res:Edit">
          <PersistedCommand>
            <remotion:BocListItemCommand Type="Event" />
          </PersistedCommand>
        </remotion:BocCommandColumnDefinition>
      </FixedColumns>
    </remotion:BocList>
  </form>
</body>

</html>
