using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ControlObjects
{
  public class ActaNovaTree : ActaNovaControlObject
  {
    // TODO RM-62978: Implement using BocTreeView. Use a different waiting strategy - wait for second increase of main wxePostSequenceCounter!

    public ActaNovaTree ([NotNull] TestObjectContext context)
        : base(context)
    {
    }
  }
}