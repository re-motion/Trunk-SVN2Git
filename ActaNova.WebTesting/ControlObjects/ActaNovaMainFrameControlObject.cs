using System;
using ActaNova.WebTesting.Infrastructure;
using Coypu;
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

    protected override ICompletionDetection GetDefaultCompletionDetection (ElementScope scope)
    {
      return Continue.When (WaitForActaNova.OuterInnerOuterUpdate);
    }
  }
}