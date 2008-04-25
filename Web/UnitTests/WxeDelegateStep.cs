using System;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests
{
  public class WxeDelegateStep : WxeStep
  {
    private readonly Proc _action;

    public WxeDelegateStep (Proc action)
    {
      ArgumentUtility.CheckNotNull ("action", action);
      _action = action;
    }

    public override void Execute (WxeContext context)
    {
      _action ();
    }
  }
}