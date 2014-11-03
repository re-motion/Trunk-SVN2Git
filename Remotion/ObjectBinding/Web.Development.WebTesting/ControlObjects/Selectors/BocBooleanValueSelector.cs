using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocBooleanValueControlObject"/>.
  /// </summary>
  public class BocBooleanValueSelector : BocControlSelectorBase<BocBooleanValueControlObject>
  {
    public BocBooleanValueSelector ()
        : base ("BocBooleanValue")
    {
    }

    protected override BocBooleanValueControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new BocBooleanValueControlObject (newControlObjectContext);
    }
  }
}