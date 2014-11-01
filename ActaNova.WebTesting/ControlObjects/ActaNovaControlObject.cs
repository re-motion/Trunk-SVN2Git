using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace ActaNova.WebTesting.ControlObjects
{
  public abstract class ActaNovaControlObject : RemotionControlObject
  {
    protected ActaNovaControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }
  }
}