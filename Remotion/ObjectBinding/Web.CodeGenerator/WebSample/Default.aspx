<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="WebSample.DefaultPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >

<head runat="server">
	<title>Sample</title>
	<link type="text/css" rel="stylesheet" href="./res/Remotion.Web/HTML/Style.css">
	<script language="javascript" type="text/javascript">
	function StartApp(url)
	{
		// Set height so that IE window fits for 1024x768 with windows taskbar
		var windowMaxWidth = 1010;
		var windowMaxHeight = 700;
		var windowLeft = (screen.width - windowMaxWidth) / 2;

		var windowTop;
		if (screen.height > 768)
		{
			// Center window vertical for resolutions greater than 1024x768
			windowTop = (screen.height - windowMaxHeight) / 2;
		}
		else
		{
			// Do not center window because of window taskbar
			windowTop = 5;
		}

    if (! false)
      window.open(url, "_self");
    else
      window.open(url, "_blank",
          "width=" + windowMaxWidth + ",height=" + windowMaxHeight + ",top=" + windowTop + ",left=" + windowLeft + "," +
          "resizable=yes,location=yes,menubar=no,status=$USER_STATUSBAR$,toolbar=no,scrollbars=no" );
	}
	</script>
</head>


<body>
	<form id="defaultForm" method="post" runat="server">
		<div id="PageHeader">
			<img src="Images/rublogo.bmp" alt="rubicon-it">
		</div>
		<h1>Sample für UIGen-Prototyp</h1>
		<p>
			Verwenden Sie einen der folgenden Links, um irgendwas zu starten:
			<br>
			<img class="pfeil" src="Images/arrow.gif">&nbsp;<a href="javascript:StartApp('EditPerson.wxe');">Sample</a>&nbsp;
			</p>
	</form>
</body>

</html>
