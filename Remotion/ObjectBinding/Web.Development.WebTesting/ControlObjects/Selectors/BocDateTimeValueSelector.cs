using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocDateTimeValueControlObject"/>.
  /// </summary>
  public class BocDateTimeValueSelector : BocControlSelectorBase<BocDateTimeValueControlObject>
  {
    public BocDateTimeValueSelector ()
        : base ("BocDateTimeValue")
    {
    }

    protected override BocDateTimeValueControlObject CreateControlObject (ControlObjectContext newControlObjectContext, ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);

      return new BocDateTimeValueControlObject (newControlObjectContext);
    }
  }
}