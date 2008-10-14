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
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.UnitTests.ExecutionEngine.TestFunctions
{
  public class OtherTestFunction : WxeFunction
  {
    public static readonly string Parameter1Name = "Parameter1";
    public static readonly string ReturnUrlValue = "DefaultReturnFromOtherTestFunction.html";

    private string _lastExecutedStepID;

    public OtherTestFunction ()
      : base (new NoneTransactionMode ())
    {
      ReturnUrl = TestFunction.ReturnUrlValue;
    }

    public OtherTestFunction (params object[] args)
        : base (new NoneTransactionMode (), args)
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

    public string LastExecutedStepID
    {
      get { return _lastExecutedStepID; }
    }
  }
}