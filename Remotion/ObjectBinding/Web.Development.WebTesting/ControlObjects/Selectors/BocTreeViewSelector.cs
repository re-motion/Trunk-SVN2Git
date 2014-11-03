using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocTreeViewControlObject"/>.
  /// </summary>
  public class BocTreeViewSelector : BocControlSelectorBase<BocTreeViewControlObject>
  {
    public BocTreeViewSelector ()
        : base ("BocTreeView")
    {
    }

    protected override BocTreeViewControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new BocTreeViewControlObject (newControlObjectContext);
    }
  }
}