using System;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

public class TestStep: WxeStep
{
  public static new WxeStep GetStepByType (WxeStep step, Type type)
  {
    return WxeStep.GetStepByType (step, type);
  }

  private bool _isExecuteCalled;
  private bool _isAbortRecursiveCalled;
  private WxeContext _wxeContext;

  public TestStep()
	{
	}

  public override void Execute(WxeContext context)
  {
    _isExecuteCalled = true;
    _wxeContext = context;
  }

  protected override void AbortRecursive()
  {
    base.AbortRecursive ();
    _isAbortRecursiveCalled = true;
  }

  public bool IsExecuteCalled
  {
    get { return _isExecuteCalled; }
  }

  public bool IsAbortRecursiveCalled
  {
    get { return _isAbortRecursiveCalled; }
  }

  public WxeContext WxeContext
  {
    get { return _wxeContext; }
  }
}

}
