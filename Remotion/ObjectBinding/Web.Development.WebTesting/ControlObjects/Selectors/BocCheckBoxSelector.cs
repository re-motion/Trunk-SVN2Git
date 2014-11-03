using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocCheckBoxControlObject"/>.
  /// </summary>
  public class BocCheckBoxSelector : BocControlSelectorBase<BocCheckBoxControlObject>
  {
    public BocCheckBoxSelector ()
        : base ("BocCheckBox")
    {
    }

    protected override BocCheckBoxControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new BocCheckBoxControlObject (newControlObjectContext);
    }
  }
}