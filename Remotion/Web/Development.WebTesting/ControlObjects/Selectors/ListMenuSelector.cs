using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="ListMenuControlObject"/>.
  /// </summary>
  public class ListMenuSelector : TypedControlSelectorBase<ListMenuControlObject>
  {
    public ListMenuSelector ()
        : base ("ListMenu")
    {
    }

    protected override ListMenuControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new ListMenuControlObject (newControlObjectContext);
    }
  }
}