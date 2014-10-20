using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ControlObjects
{
  public class ActaNovaTreeControlObject : ActaNovaControlObject
  {
    // TODO RM-6297: Implement using BocTreeView. Use a different waiting strategy - wait for second increase of main wxePostSequenceCounter!

    public ActaNovaTreeControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }
  }
}