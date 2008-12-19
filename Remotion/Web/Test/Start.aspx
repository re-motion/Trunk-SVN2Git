<%-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 %
 % This program is free software: you can redistribute it and/or modify it under 
 % the terms of the re:motion license agreement in license.txt. If you did not 
 % receive it, please visit http://www.re-motion.org/licensing.
 % 
 % Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 % WITHOUT WARRANTY OF ANY KIND, either express or implied. 
--%>
<%@ Page language="c#" Codebehind="Start.aspx.cs" AutoEventWireup="false" Inherits="Remotion.Web.Test.Start" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Start</title>
  </head>
  <body>
    <form id="Form1" method="post" runat="server">
      <p>
        <a href="WxeHandler.ashx?WxeFunctionType=Remotion.Web.Test::ExecutionEngine.SampleWxeFunction">
          WxeHandler.ashx?WxeFunctionType=Remotion.Web.Test::ExecutionEngine.SampleWxeFunction
        </a>
      </p>
      <p>
        <a href="WxeHandler.ashx?WxeFunctionType=Remotion.Web.Test::ExecutionEngine.SessionWxeFunction&amp;ReadOnly=True">
          WxeHandler.ashx?WxeFunctionType=Remotion.Web.Test::ExecutionEngine.SessionWxeFunction&amp;ReadOnly=True
        </a>
      </p>
      <p>
        <a href="session.wxe?ReadOnly=True">session.wxe?ReadOnly=True</a>
      </p>
      <p>
        <a href="MultiplePostbackCatching/SutForm.aspx">Multiple Postback Catcher Tests</a>
      </p>
      <p>
        <a href="MultiplePostbackCatching/UpdatePanelSutForm.aspx">Multiple Postback Catcher Tests for Update Panel</a>
      </p>
      <p>
        <a href="UpdatePanelTests/Sut.wxe">UpdatePanel SUT</a>
      </p>
      <p>
        <a href="../../../../tools/Selenium/trunk/Core/TestRunner.html?test=../../../../Remotion/trunk/Remotion/Web/Test/MultiplePostbackCatching/TestSuiteForm.aspx">Remotion.Web.Tests.MultiplePostBackCatching</a>
      </p>
      <p>
        <a href="../../../../tools/Selenium/trunk/Core/TestRunner.html?test=../../../../Remotion/trunk/Remotion/Web/Test/MultiplePostbackCatching/UpdatePanelTestSuiteForm.aspx">Remotion.Web.Tests.MultiplePostBackCatching for UpdatePanel</a>
      </p>
      <p>
        <a href="redirected.wxe">redirected.wxe</a>
      </p>
      <p>
        <a href="ShowUserControl.wxe">ShowUserControl.wxe</a>
      </p>      
      <asp:Button id="ResetSessionButton" runat="server" Text="Reset Session"></asp:Button>
    </form>
  </body>
</html>
