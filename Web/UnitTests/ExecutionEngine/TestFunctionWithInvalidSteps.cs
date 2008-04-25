using System;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

public class TestFunctionWithInvalidSteps: WxeFunction
{
  public TestFunctionWithInvalidSteps()
	{
	}

	public TestFunctionWithInvalidSteps (params object[] args)
    : base (args)
	{
	}

  static void InvalidStep1 ()
  {
  }

  void InvalidStep2 (object obj)
  {
  }

  void InvalidStep3 (WxeContext context, object obj)
  {
  }
}

}
