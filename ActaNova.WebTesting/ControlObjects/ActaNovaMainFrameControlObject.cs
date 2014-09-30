using System;
using ActaNova.WebTesting.Infrastructure;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.WaitingStrategies;

namespace ActaNova.WebTesting.ControlObjects
{
  public class ActaNovaMainFrameControlObject : ActaNovaControlObject
  {
    public ActaNovaMainFrameControlObject ([NotNull] TestObjectContext context)
        : base (context)
    {
    }

    protected IWaitingStrategy GetActualWaitingStrategy ([CanBeNull] IWaitingStrategy waitingStrategy)
    {
      if (waitingStrategy != null)
        return waitingStrategy;

      return WaitForActaNova.OuterInnerOuterUpdate;
    }
  }
}