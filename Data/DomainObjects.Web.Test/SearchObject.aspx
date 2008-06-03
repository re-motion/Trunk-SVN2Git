<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Page language="c#" Codebehind="SearchObject.aspx.cs" AutoEventWireup="false" Inherits="Remotion.Data.DomainObjects.Web.Test.SearchObjectPage" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>SearchObject</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <remotion:htmlheadcontents id="HtmlHeadContents" runat="server"></remotion:htmlheadcontents>
  </HEAD>
  <body>
    <form id="SearchObjectForm" method="post" runat="server">
      <TABLE id="SearchFormGrid" cellSpacing="0" cellPadding="0" width="300" border="0" runat="server">
        <TR>
          <TD></TD>
          <TD><remotion:boctextvalue id="StringPropertyValue" runat="server" DataSourceControl="CurrentSearchObject"
              PropertyIdentifier="StringProperty">
              <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
            </remotion:boctextvalue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD><remotion:boctextvalue id="BytePropertyFromTextBox" runat="server" DataSourceControl="CurrentSearchObject"
              PropertyIdentifier="BytePropertyFrom">
              <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
            </remotion:boctextvalue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD><remotion:boctextvalue id="BytePropertyToTextBox" runat="server" DataSourceControl="CurrentSearchObject"
              PropertyIdentifier="BytePropertyTo">
              <TextBoxStyle TextMode="SingleLine"></TextBoxStyle>
            </remotion:boctextvalue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD><remotion:bocenumvalue id="EnumPropertyValue" runat="server" DataSourceControl="CurrentSearchObject" PropertyIdentifier="EnumProperty">
              <ListControlStyle></ListControlStyle>
            </remotion:bocenumvalue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD>
            <remotion:BocDateTimeValue id="DatePropertyFromValue" runat="server" PropertyIdentifier="DatePropertyFrom"
              DataSourceControl="CurrentSearchObject"></remotion:BocDateTimeValue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD>
            <remotion:BocDateTimeValue id="DatePropertyToValue" runat="server" PropertyIdentifier="DatePropertyTo" DataSourceControl="CurrentSearchObject"></remotion:BocDateTimeValue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD>
            <remotion:BocDateTimeValue id="DateTimeFromValue" runat="server" DataSourceControl="CurrentSearchObject" PropertyIdentifier="DateTimePropertyFrom"></remotion:BocDateTimeValue></TD>
        </TR>
        <TR>
          <TD></TD>
          <TD>
            <remotion:BocDateTimeValue id="BocDateTimeValue2" runat="server" PropertyIdentifier="DateTimePropertyTo" DataSourceControl="CurrentSearchObject"></remotion:BocDateTimeValue></TD>
        </TR>
      </TABLE>
      <asp:button id="SearchButton" runat="server" Text="Suchen"></asp:button><remotion:boclist id="ResultList" runat="server" DataSourceControl="FoundObjects">
<FixedColumns>
<remotion:BocRowEditModeColumnDefinition SaveText="Speichern" CancelText="Abbrechen" EditText="Bearbeiten" ColumnTitle="Aktion"></remotion:BocRowEditModeColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="StringProperty">
<PersistedCommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="ByteProperty">
<PersistedCommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="EnumProperty">
<PersistedCommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="DateProperty">
<PersistedCommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="DateTimeProperty">
<PersistedCommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="BooleanProperty">
<PersistedCommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
<remotion:BocSimpleColumnDefinition PropertyPathIdentifier="NaBooleanProperty">
<PersistedCommand>
<remotion:BocListItemCommand Type="None"></remotion:BocListItemCommand>
</PersistedCommand>
</remotion:BocSimpleColumnDefinition>
</FixedColumns>
      </remotion:boclist><remotion:formgridmanager id="SearchFormGridManager" runat="server"></remotion:formgridmanager><remotion:BindableObjectDataSourceControl id="FoundObjects" runat="server" Type="Remotion.Data.DomainObjects.Web.Test.Domain.ClassWithAllDataTypes, Remotion.Data.DomainObjects.Web.Test"></remotion:BindableObjectDataSourceControl>
      <remotion:BindableObjectDataSourceControl id="CurrentSearchObject" runat="server" Type="Remotion.Data.DomainObjects.Web.Test.Domain.ClassWithAllDataTypesSearch, Remotion.Data.DomainObjects.Web.Test"></remotion:BindableObjectDataSourceControl></form>
  </body>
</HTML>
