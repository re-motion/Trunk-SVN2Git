// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

[Serializable]
public class WxeFunctionStateMock: WxeFunctionState
{
  public WxeFunctionStateMock (WxeFunction function, bool enableCleanUp)
    : base (function, enableCleanUp)
  {
  }
  
  public WxeFunctionStateMock (WxeFunction function, int lifetime, bool enableCleanUp)
    : base (function, lifetime, enableCleanUp)
  {
  }
  
  public WxeFunctionStateMock (
      WxeFunction function, int lifetime, bool enableCleanUp, string functionToken)
    : base (function, lifetime, enableCleanUp)
  {
    FunctionToken = functionToken;
  }

  public new WxeFunction Function
  {
    get { return base.Function; }
    set {PrivateInvoke.SetNonPublicField (this, "_function", value); }
  }

  public new string FunctionToken
  {
    get { return base.FunctionToken; }
    set {PrivateInvoke.SetNonPublicField (this, "_functionToken", value); }
  }

  public new void Abort()
  {
    base.Abort();
  }
}

}
