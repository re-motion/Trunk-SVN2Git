using System;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="ActaNovaAutoCompleteReferenceValueControlObject"/>.
  /// </summary>
  public class ActaNovaListSelector : BocControlSelectorBase<ActaNovaListControlObject>
  {
    public ActaNovaListSelector ()
        : base ("BocList")
    {
    }

    /// <inheritdoc/>
    protected override ActaNovaListControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new ActaNovaListControlObject (newControlObjectContext);
    }
  }
}