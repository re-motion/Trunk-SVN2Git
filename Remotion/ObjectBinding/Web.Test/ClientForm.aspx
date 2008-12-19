<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>

<%@ Page language="c#" Codebehind="ClientForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.ClientForm" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>ClientForm</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
<remotion:htmlheadcontents id=HtmlHeadContents runat="server"></remotion:htmlheadcontents>

<script language="javascript">
  var _keepAliveLocation = 'WxeHandler.ashx?WxeFunctionType=OBWTest.ClientFormKeepAliveWxeFunction,OBWTest';
  var active = window.setInterval('KeepAlive()', 6000);
  
  function KeepAlive()
  {
    try 
    {
      var image = new Image();
      image.src = _keepAliveLocation;
    }
    catch (e)
    {
    }
  }
</script>
  
<script language="javascript">
  var _wxe_expiredLocation = 'WxeHandler.ashx?WxeFunctionType=OBWTest.ClientFormClosingWxeFunction,OBWTest';
  var _wxe_isSubmit = false;
  var _wxe_aspnetDoPostBack = null;
  
  function OnBeforeUnload()
  {
    if (! _wxe_isSubmit)
    {
      var activeElement = window.document.activeElement;
      var isJavaScriptAnchor = false;
      if (  activeElement != null
          && activeElement.tagName.toLowerCase() == 'a'
          && activeElement.href != null
          && activeElement.href.toLowerCase().indexOf ('javascript:') >= 0)
      {
        isJavaScriptAnchor = true;
      }
      if (! isJavaScriptAnchor)
      {
        event.returnValue = "If you leave now, forever lost your session will be.";
        event.cancelBubble = true;
      }
    }
  }

  function OnUnload()
  {
    if (! _wxe_isSubmit)
    {
      try 
      {
        var image = new Image();
        image.src = _wxe_expiredLocation;
      }
      catch (e)
      {
      }
      SmartNavigation (null);
    }
  }

  function OnLoad()
  {
    var theform;
		if (window.navigator.appName.toLowerCase().indexOf("microsoft") > -1)
			theform = document.Form;
		else 
			theform = document.forms["Form"];
	  theform.onsubmit = function() { _wxe_isSubmit = true; };
	  
	  _wxe_aspnetDoPostBack = __doPostBack;
	  __doPostBack = function (eventTarget, eventArgument)
	      {
	        _wxe_isSubmit = true;
	        SmartNavigation (document.getElementById ('eventTarget'));
	        _wxe_aspnetDoPostBack (eventTarget, eventArgument);
	      };
	  SmartNavigationRestore();
  }

  function SmartNavigationRestore()
  {
    var scrollParent = document.getElementById ('MultiView_ActiveView');
    var scrollTop = 169;
    var scrollLeft = 0;
    if (scrollParent != null)
    {
      scrollParent.scrollTop = scrollTop;
      scrollParent.scrollLeft = scrollLeft;
    }
    
    var focusElement = document.getElementById ('TestTabbedPersonJobsUserControl_MultilineTextField_Boc_TextBox');
    var offsetLeft = 417;
    var offsetTop = 605;  
    if (focusElement != null)
    {
      focusElement.focus();
    }
  }
  
  function SmartNavigation (srcElement)
  {
    var scrollParent = null;
    for (var currentNode = srcElement; currentNode != null; currentNode = currentNode.offsetParent)
    {
      if (   currentNode.style.overflow.toLowerCase() == 'auto' 
          || currentNode.style.overflow.toLowerCase() == 'scroll')
      {
        scrollParent = currentNode;
        break;
      }
    }
    if (scrollParent != null)
    {
      var scrollElement = document.getElementById ('smartNavigationScrollElement');
      var scrollTop = document.getElementById ('smartNavigationScrollTop');
      var scrollLeft = document.getElementById ('smartNavigationScrollLeft');
      scrollElement.value = scrollParent.id;
      scrollTop.value = scrollParent.scrollTop;
      scrollLeft.value = scrollParent.scrollLeft;
    }
    if (srcElement != null)
    {
      var focus = document.getElementById ('smartNavigationFocus');
      focus.value = srcElement.id;
    }
  }
</script>
</head>
<body MS_POSITIONING="FlowLayout" onLoad="OnLoad();" onBeforeUnload="OnBeforeUnload();" onUnload="OnUnload();" >
    <form id=Form method=post runat="server">
<p>
    <input type="hidden" id="smartNavigationScrollLeft">
    <input type="hidden" id="smartNavigationScrollTop">
    <input type="hidden" id="smartNavigationScrollElement">
    <input type="hidden" id="smartNavigationFocus">
      <remotion:tabbedmultiview id=MultiView runat="server" cssclass="tabbedMultiView">
      </remotion:tabbedmultiview></p>
<p>&nbsp;</p>
    </form>
  </body>
</html>
