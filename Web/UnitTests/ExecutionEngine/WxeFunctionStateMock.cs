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
