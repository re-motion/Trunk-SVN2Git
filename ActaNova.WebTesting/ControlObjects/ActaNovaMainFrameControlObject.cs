using System;
using ActaNova.WebTesting.Infrastructure;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ControlObjects
{
  public abstract class ActaNovaMainFrameControlObject : ActaNovaControlObject
  {
    protected ActaNovaMainFrameControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    protected ICompletionDetector DetermineActualCompletionDetection ([CanBeNull] ICompletionDetection usedDefinedCompletionDetection)
    {
      if (usedDefinedCompletionDetection != null)
        return usedDefinedCompletionDetection.Build();

      return Continue.When (WaitForActaNova.OuterInnerOuterUpdate).Build();
    }
  }
}