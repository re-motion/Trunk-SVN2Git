using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="SingleViewControlObject"/>.
  /// </summary>
  public class SingleViewSelector : TypedControlSelectorBase<SingleViewControlObject>
  {
    public SingleViewSelector ()
        : base ("SingleView")
    {
    }

    protected override SingleViewControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new SingleViewControlObject (newControlObjectContext);
    }
  }
}