<%@ Page language="c#" Codebehind="Frameset.aspx.cs" AutoEventWireup="false" Inherits="Frameset" %>
<%@ Import namespace="OBWTest"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
  <head>
    <title>Frameset</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
  </head>
  <frameset cols="50%,50%">
  <frame name="left" src="LeftFrame.aspx"/>
  <frame name="right" src="RightFrame.aspx"/>
  </frameset>
  <body MS_POSITIONING="FlowLayout">
	
    <form id="Form1" method="post" runat="server">

     </form>
	
  </body>
</html>
