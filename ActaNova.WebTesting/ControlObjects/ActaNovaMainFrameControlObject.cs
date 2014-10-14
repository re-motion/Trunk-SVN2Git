using System;
using ActaNova.WebTesting.Infrastructure;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ControlObjects
{
  public class ActaNovaMainFrameControlObject : ActaNovaControlObject
  {
    public ActaNovaMainFrameControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    protected IActionBehavior GetActualActionBehavior ([CanBeNull] IActionBehavior usedDefinedActionBehavior)
    {
      if (usedDefinedActionBehavior != null)
        return usedDefinedActionBehavior;

      return Behavior.WaitFor (WaitForActaNova.OuterInnerOuterUpdate);
    }
  }
}