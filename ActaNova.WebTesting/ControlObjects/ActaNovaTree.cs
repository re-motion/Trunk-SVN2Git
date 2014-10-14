using System;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ControlObjects
{
  public class ActaNovaTree : ActaNovaControlObject
  {
    // TODO RM-6297: Implement using BocTreeView. Use a different waiting strategy - wait for second increase of main wxePostSequenceCounter!

    public ActaNovaTree ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }
  }
}