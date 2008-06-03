<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>

<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NavigationTabs.ascx.cs" Inherits="OBWTest.UI.NavigationTabs" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>


<remotion:TabbedMenu id="TabbedMenu" runat="server">
<tabs>
<remotion:MainMenuTab Text="Tests by Control" ItemID="IndividualControlTests">
<submenutabs>
<remotion:SubMenuTab Text="Boolean" ItemID="BocBooleanValue">
<persistedcommand>
<remotion:NavigationCommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocBooleanValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:NavigationCommand>
</PersistedCommand>
</remotion:SubMenuTab>

<remotion:submenutab Text="CheckBox" ItemID="BocCheckBox">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocCheckBoxUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="DateTime" ItemID="BocDateTimeValue">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocDateTimeValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="Enum" ItemID="BocEnumValue">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocEnumValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="List" ItemID="BocList">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocListUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="List as Grid" ItemID="BocListAsGrid">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocListAsGridUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="Literal" ItemID="BocLiteral">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocLiteralUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="MultilineText" ItemID="BocMultilineTextValue">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocMultilineTextValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="Reference" ItemID="BocReferenceValue">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocReferenceValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="Auto Complete Reference" ItemID="BocAutoCompleteReferenceValue">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocAutoCompleteReferenceValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

<remotion:submenutab Text="Text" ItemID="BocTextValue">
<persistedcommand>
<remotion:navigationcommand Type="WxeFunction" WxeFunctionCommand-Parameters="&quot;BocTextValueUserControl.ascx&quot;" WxeFunctionCommand-MappingID="IndividualControlTest"></remotion:navigationcommand>
</PersistedCommand>
</remotion:submenutab>

</SubMenuTabs>

<persistedcommand>
<remotion:NavigationCommand Type="None"></remotion:NavigationCommand>
</PersistedCommand>
</remotion:MainMenuTab>
</Tabs>
</remotion:TabbedMenu>
<div style="WIDTH: 100%;TEXT-ALIGN: right">
WAI Conformance Level: 
<remotion:BocEnumValue id="WaiConformanceLevelField" runat="server">
<listcontrolstyle autopostback="True">
</ListControlStyle></remotion:BocEnumValue>
</div>
