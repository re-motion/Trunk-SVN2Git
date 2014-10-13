using System;
using ActaNova.WebTesting.Infrastructure;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ControlObjects
{
  public class ActaNovaMainFrameControlObject : ActaNovaControlObject
  {
    public ActaNovaMainFrameControlObject ([NotNull] TestObjectContext context)
        : base (context)
    {
    }

    protected IActionBehavior GetActualActionBehavior ([CanBeNull] IActionBehavior usedDefinedActionBehavior)
    {
      if (usedDefinedActionBehavior != null)
        return usedDefinedActionBehavior;

      return ActaNovaBehavior.WaitForOuterInnerOuterUpdate;
    }
  }
}