using System;
using ActaNova.WebTesting.Infrastructure;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
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

    /// <inheritdoc/>
    protected override ICompletionDetection GetDefaultCompletionDetection (ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      return Continue.When (ActaNovaCompletion.OuterInnerOuterUpdated);
    }
  }
}