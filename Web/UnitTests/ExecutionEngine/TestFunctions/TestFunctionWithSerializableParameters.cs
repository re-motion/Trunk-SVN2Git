// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.UnitTests.ExecutionEngine.TestFunctions
{
  public class TestFunctionWithSerializableParameters: WxeFunction
  {
    public TestFunctionWithSerializableParameters()
      : base (new NoneTransactionMode ())
    {
    }

    public TestFunctionWithSerializableParameters (params object[] args)
        : base (new NoneTransactionMode (), args)
    {
    }

    public TestFunctionWithSerializableParameters (string StringValue, int? NaInt32Value, int IntValue)
        : base (new NoneTransactionMode (), StringValue, NaInt32Value, IntValue)
    {
    }

    public TestFunctionWithSerializableParameters (string StringValue, int? NaInt32Value)
        : this (new NoneTransactionMode (), StringValue, NaInt32Value, -1)
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
