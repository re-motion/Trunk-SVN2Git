/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Web.ExecutionEngine;
using Remotion.Utilities;

namespace Remotion.Development.Web.UnitTesting.ExecutionEngine
{
  public class TestFunction : WxeFunction
  {
    public static readonly string Parameter1Name = "Parameter1";
    public static readonly string ReturnUrlValue = "DefaultReturn.html";

    private WxeContext _wxeContextStep2;
    private string _lastExecutedStepID;
    private string _executionOrder = string.Empty;

    public TestFunction ()
    {
      ReturnUrl = TestFunction.ReturnUrlValue;
    }

    public TestFunction (params object[] args)
        : base (args)
    {
      ReturnUrl = TestFunction.ReturnUrlValue;
    }

    [WxeParameter (1, false, WxeParameterDirection.In)]
    public string Parameter1
    {
      get { return (string) Variables["Parameter1"]; }
      set { Variables["Parameter1"] = value; }
    }

    private void Step1 ()
    {
      _lastExecutedStepID = "1";
    }

    private void Step2 (WxeContext context)
    {
      _wxeContextStep2 = context;
      _lastExecutedStepID = "2";
    }

    private TestStep Step3 = new TestStep();

    private void Step4 ()
    {
      _lastExecutedStepID = "4";
    }

    public void PublicStepMethod ()
    {
      Step1 ();
    }

    public void PublicStepMethodWithContext (WxeContext context)
    {
      Step2 (context);
    }

    public string LastExecutedStepID
    {
      get { return _lastExecutedStepID; }
    }

    public string ExecutionOrder
    {
      get { return _executionOrder; }
    }

    public TestStep TestStep
    {
      get { return Step3; }
    }

    public WxeContext WxeContextStep2
    {
      get { return _wxeContextStep2; }
    }
  }
}