using System;
using ActaNova.WebTesting.Infrastructure;
using Coypu;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace ActaNova.WebTesting.ControlObjects
{
  public abstract class ActaNovaMainFrameControlObject : WebFormsControlObject
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