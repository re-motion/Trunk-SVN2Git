using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocTextValueControlObject"/>.
  /// </summary>
  public class BocTextValueSelector : BocControlSelectorBase<BocTextValueControlObject>
  {
    public BocTextValueSelector ()
        : base ("BocTextValue")
    {
    }

    protected override BocTextValueControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new BocTextValueControlObject (newControlObjectContext);
    }
  }
}