using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocListAsGridControlObject"/>.
  /// </summary>
  public class BocListAsGridSelector : BocControlSelectorBase<BocListAsGridControlObject>
  {
    public BocListAsGridSelector ()
        : base ("BocList")
    {
    }

    protected override BocListAsGridControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new BocListAsGridControlObject (newControlObjectContext);
    }
  }
}