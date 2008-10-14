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
  public class TestFunctionWithNonSerializableParameters: WxeFunction
  {
    public TestFunctionWithNonSerializableParameters()
      : base (new NoneTransactionMode ())
    {
    }

    public TestFunctionWithNonSerializableParameters (params object[] args)
        : base (new NoneTransactionMode (), args)
    {
    }

    public TestFunctionWithNonSerializableParameters (string StringValue, object ObjectValue)
        : base (new NoneTransactionMode (), StringValue, ObjectValue)
    {
    }

    [WxeParameter (1, true, WxeParameterDirection.In)]
    public string StringValue
    {
      get { return (string) Variables["StringValue"]; }
      set { Variables["StringValue"] = value; }
    }

    [WxeParameter (2, true, WxeParameterDirection.In)]
    public object ObjectValue
    {
      get { return Variables["ObjectValue"]; }
      set { Variables["ObjectValue"] = value; }
    }
  }
}