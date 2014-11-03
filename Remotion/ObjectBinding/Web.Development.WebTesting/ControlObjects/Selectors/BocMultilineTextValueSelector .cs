using System;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="BocMultilineTextValueControlObject"/>.
  /// </summary>
  public class BocMultilineTextValueSelector : BocControlSelectorBase<BocMultilineTextValueControlObject>
  {
    public BocMultilineTextValueSelector ()
        : base ("BocMultilineTextValue")
    {
    }

    protected override BocMultilineTextValueControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new BocMultilineTextValueControlObject (newControlObjectContext);
    }
  }
}