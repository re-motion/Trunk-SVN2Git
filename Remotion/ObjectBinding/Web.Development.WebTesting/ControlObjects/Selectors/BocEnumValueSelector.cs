using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocEnumValueControlObject"/>.
  /// </summary>
  public class BocEnumValueSelector : BocControlSelectorBase<BocEnumValueControlObject>
  {
    public BocEnumValueSelector ()
        : base ("BocEnumValue")
    {
    }

    protected override BocEnumValueControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new BocEnumValueControlObject (newControlObjectContext);
    }
  }
}