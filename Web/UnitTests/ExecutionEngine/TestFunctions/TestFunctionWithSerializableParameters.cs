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

namespace Remotion.Web.UnitTests.ExecutionEngine.TestFunctions
{
  public class TestFunctionWithSerializableParameters: WxeFunction
  {
    public TestFunctionWithSerializableParameters()
    {
    }

    public TestFunctionWithSerializableParameters (params object[] args)
        : base (args)
    {
    }

    public TestFunctionWithSerializableParameters (string StringValue, int? NaInt32Value, int IntValue)
        : base (StringValue, NaInt32Value, IntValue)
    {
    }

    public TestFunctionWithSerializableParameters (string StringValue, int? NaInt32Value)
        : this (StringValue, NaInt32Value, -1)
    {
    }

    [WxeParameter (1, true, WxeParameterDirection.In)]
    public string StringValue
    {
      get { return (string) Variables["StringValue"]; }
      set { Variables["StringValue"] = value; }
    }

    [WxeParameter (2, true, WxeParameterDirection.In)]
    public int? NaInt32Value
    {
      get { return (int?) Variables["NaInt32Value"]; }
      set { Variables["NaInt32Value"] = value; }
    }

    [WxeParameter (3, false, WxeParameterDirection.In)]
    public int IntValue
    {
      get { return (int) Variables["IntValue"]; }
      set { Variables["IntValue"] = value; }
    }
  }
}