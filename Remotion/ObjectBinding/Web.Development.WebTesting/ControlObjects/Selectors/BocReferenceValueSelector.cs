using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocReferenceValueControlObject"/>.
  /// </summary>
  public class BocReferenceValueSelector : BocControlSelectorBase<BocReferenceValueControlObject>
  {
    public BocReferenceValueSelector ()
        : base ("BocReferenceValue")
    {
    }

    protected override BocReferenceValueControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new BocReferenceValueControlObject (newControlObjectContext);
    }
  }
}