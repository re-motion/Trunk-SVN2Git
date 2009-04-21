<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % version 3.0 as published by the Free Software Foundation.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>

<%@ Page language="c#" Codebehind="WebForm1.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.WebForm1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>WebForm1</title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
  </HEAD>
  <body MS_POSITIONING="GridLayout">
    <form id="Form1" method="post" runat="server">
      <remotion:bindableobjectdatasourcecontrol id="CurrentObjectDataSource" runat="server" Type="Remotion.ObjectBinding.Sample::Person"/>
      <remotion:businessobjectreferencedatasourcecontrol id="PartnerDataSource" runat="server" PropertyIdentifier="Partner" ReferencedDataSource="CurrentObjectDataSource"
        DataSourceControl="CurrentObjectDataSource"></remotion:businessobjectreferencedatasourcecontrol>
      <remotion:boctextvalue id=FirstNameField style="Z-INDEX: 100; LEFT: 224px; POSITION: absolute; TOP: 56px" runat="server" PropertyIdentifier="FirstName" DataSource="<%# CurrentObjectDataSource %>">
        <textboxstyle autopostback="True" cssclass="MyCssClass"></textboxstyle>
      </remotion:boctextvalue><remotion:smartlabel id="SmartLabel2" style="Z-INDEX: 119; LEFT: 696px; POSITION: absolute; TOP: 360px"
        runat="server" ForControl="PartnerFirstNameField"></remotion:smartlabel><remotion:smartlabel id="BocPropertyLabel1" style="Z-INDEX: 101; LEFT: 16px; POSITION: absolute; TOP: 56px"
        runat="server" ForControl="FirstNameField"></remotion:smartlabel><remotion:boctextvalue id=LastNameField style="Z-INDEX: 103; LEFT: 224px; POSITION: absolute; TOP: 104px" runat="server" PropertyIdentifier="LastName" DataSource="<%# CurrentObjectDataSource %>">
      </remotion:boctextvalue><remotion:smartlabel id="BocPropertyLabel2" style="Z-INDEX: 104; LEFT: 16px; POSITION: absolute; TOP: 104px"
        runat="server" ForControl="LastNameField"></remotion:smartlabel><remotion:boctextvalue id=DateOfBirthField style="Z-INDEX: 106; LEFT: 224px; POSITION: absolute; TOP: 136px" runat="server" PropertyIdentifier="DateOfBirth" DataSource="<%# CurrentObjectDataSource %>" ValueType="Date">
      </remotion:boctextvalue><remotion:smartlabel id="BocPropertyLabel3" style="Z-INDEX: 107; LEFT: 16px; POSITION: absolute; TOP: 136px"
        runat="server" ForControl="DateOfBirthField"></remotion:smartlabel><remotion:boctextvaluevalidator id="BocTextValueValidator1" style="Z-INDEX: 108; LEFT: 480px; POSITION: absolute; TOP: 136px"
        runat="server" EnableClientScript="False" ControlToValidate="DateOfBirthField"></remotion:boctextvaluevalidator><remotion:boctextvalue id=HeightField style="Z-INDEX: 109; LEFT: 224px; POSITION: absolute; TOP: 168px" runat="server" PropertyIdentifier="Height" DataSource="<%# CurrentObjectDataSource %>">
      </remotion:boctextvalue><remotion:smartlabel id="BocPropertyLabel4" style="Z-INDEX: 110; LEFT: 16px; POSITION: absolute; TOP: 168px"
        runat="server" ForControl="HeightField"></remotion:smartlabel><remotion:boctextvaluevalidator id="BocTextValueValidator2" style="Z-INDEX: 111; LEFT: 480px; POSITION: absolute; TOP: 168px"
        runat="server" EnableClientScript="False" ControlToValidate="HeightField"></remotion:boctextvaluevalidator><asp:label id="Label1" style="Z-INDEX: 112; LEFT: 440px; POSITION: absolute; TOP: 168px" runat="server">cm</asp:label><asp:button id="SaveButton" style="Z-INDEX: 102; LEFT: 16px; POSITION: absolute; TOP: 392px"
        runat="server" Width="80px" Text="Save"></asp:button><remotion:bocenumvalue id=GenderField style="Z-INDEX: 113; LEFT: 224px; POSITION: absolute; TOP: 216px" runat="server" PropertyIdentifier="Gender" DataSource="<%# CurrentObjectDataSource %>" Width="152px" Height="24px">
        <listcontrolstyle radiobuttonlisttextalign="Right" font-bold="True" bordercolor="Red" forecolor="Green"
          radionbuttonlistrepeatlayout="Table" backcolor="#FFFF80" controltype="RadioButtonList" radiobuttonlistrepeatdirection="Vertical"></listcontrolstyle>
      </remotion:bocenumvalue><remotion:smartlabel id="BocPropertyLabel5" style="Z-INDEX: 114; LEFT: 16px; POSITION: absolute; TOP: 216px"
        runat="server" ForControl="GenderField"></remotion:smartlabel><remotion:bocenumvalue id=MarriageStatusField style="Z-INDEX: 115; LEFT: 224px; POSITION: absolute; TOP: 320px" runat="server" PropertyIdentifier="MarriageStatus" DataSource="<%# CurrentObjectDataSource %>" Width="152px">
        <listcontrolstyle radiobuttonlisttextalign="Right" radionbuttonlistrepeatlayout="Table" controltype="DropDownList"
          radiobuttonlistrepeatdirection="Vertical"></listcontrolstyle>
      </remotion:bocenumvalue><remotion:smartlabel id="SmartLabel1" style="Z-INDEX: 116; LEFT: 24px; POSITION: absolute; TOP: 328px"
        runat="server" ForControl="MarriageStatusField" Width="120px" Height="8px"></remotion:smartlabel><remotion:boctextvalue id=PartnerFirstNameField style="Z-INDEX: 117; LEFT: 904px; POSITION: absolute; TOP: 360px" runat="server" PropertyIdentifier="FirstName" DataSource="<%# PartnerDataSource %>">
      </remotion:boctextvalue><asp:label id="Label2" style="Z-INDEX: 118; LEFT: 696px; POSITION: absolute; TOP: 328px" runat="server"
        Font-Bold="True">Partner</asp:label></form>
  </body>
</HTML>
