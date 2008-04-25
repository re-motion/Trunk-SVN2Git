<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SearchResultPersonForm.aspx.cs" Inherits="WebSample.UI.SearchResultPersonForm" %>
<%@ Register TagPrefix="obw" Namespace="Remotion.ObjectBinding.Web.UI.Controls" Assembly="Remotion.ObjectBinding.Web" %>
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" Assembly="Remotion.Web" %>
<%@ Register TagPrefix="dow" Namespace="Remotion.Data.DomainObjects.ObjectBinding.Web" Assembly="Remotion.Data.DomainObjects.ObjectBinding.Web" %>
<%@ Register TagPrefix="uc1" TagName="NavigationTabs" Src="NavigationTabs.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Search Person</title>
  <remotion:htmlheadcontents id="HtmlHeadContents" runat="server"></remotion:htmlheadcontents>
</head>

<body>
  <dow:SearchObjectDataSourceControl id="PersonSearchDataSource" runat="server" TypeName="DomainSample.Person, DomainSample" Mode="Read" />
  <remotion:FormGridManager id="FormGridManager" runat="server" />
  <uc1:NavigationTabs id="NavigationTabs" runat="server" />
  <form id="Form1" method="post" runat="server">
    <obw:BocList ID="PersonList" runat="server" DataSourceControl="PersonSearchDataSource">
			<FixedColumns>
				<obw:BocAllPropertiesPlacehoderColumnDefinition />
				<obw:BocCommandColumnDefinition Text="$res:Edit">
					<PersistedCommand>
						<obw:BocListItemCommand
								Type="WxeFunction"
								WxeFunctionCommand-Parameters="id"
								WxeFunctionCommand-TypeName="WebSample.WxeFunctions.EditPersonFunction,WebSample" />
					</PersistedCommand>
				</obw:BocCommandColumnDefinition>
			</FixedColumns>
    </obw:BocList>
  </form>
</body>

</html>
