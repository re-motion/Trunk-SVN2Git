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
  public class TestFunctionWithNesting: WxeFunction
  {
    public static readonly string Parameter1Name = "Parameter1";
    public static readonly string ReturnUrlValue = "DefaultReturn.html";

    private WxeContext _wxeContext;
    private string _lastExecutedStepID;

    public TestFunctionWithNesting()
      : base (new NoneTransactionMode ())
    {
      ReturnUrl = TestFunction.ReturnUrlValue;
    }

    public TestFunctionWithNesting (params object[] args)
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

    void Step1()
    {
      _lastExecutedStepID = "1";
    }
  
    void Step2()
    {
      _lastExecutedStepID = "2";
    }

    class Step3: WxeFunction
    {
      public Step3()
        : base (new NoneTransactionMode ())
      {
      }

      void Step1()
      {
        TestFunctionWithNesting._lastExecutedStepID = "3.1";
      }
    
      void Step2()
      {
        TestFunctionWithNesting._lastExecutedStepID = "3.2";
      }
    
      void Step3_()
      {
        TestFunctionWithNesting._lastExecutedStepID = "3.3";
      }

      private TestFunctionWithNesting TestFunctionWithNesting
      {
        get { return (TestFunctionWithNesting) ParentStep; }
      }

    }
  
    void Step4()
    {
      _lastExecutedStepID = "4";
    }

    public WxeContext WxeContext
    {
      get { return _wxeContext; }
    }

    public string LastExecutedStepID
    {
      get { return _lastExecutedStepID; }
    }

    public override void Execute (WxeContext context)
    {
      _wxeContext = context;
      base.Execute (context);
    }

  }
}