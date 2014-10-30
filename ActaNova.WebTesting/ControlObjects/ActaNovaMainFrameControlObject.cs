using System;
using ActaNova.WebTesting.Infrastructure;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ControlObjects
{
  public abstract class ActaNovaMainFrameControlObject : ActaNovaControlObject
  {
    protected ActaNovaMainFrameControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    protected ICompletionDetection DetermineActualCompletionDetection ([CanBeNull] ICompletionDetection usedDefinedCompletionDetection)
    {
      if (usedDefinedCompletionDetection != null)
        return usedDefinedCompletionDetection;

      return Continue.When (WaitForActaNova.OuterInnerOuterUpdate);
    }
  }
}