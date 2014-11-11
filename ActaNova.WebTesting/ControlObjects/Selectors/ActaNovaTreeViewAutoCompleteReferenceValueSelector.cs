using System;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="ActaNovaTreeViewAutoCompleteReferenceValueControlObject"/>.
  /// </summary>
  public class ActaNovaTreeViewAutoCompleteReferenceValueSelector : BocControlSelectorBase<ActaNovaTreeViewAutoCompleteReferenceValueControlObject>
  {
    public ActaNovaTreeViewAutoCompleteReferenceValueSelector ()
        : base ("BocAutoCompleteReferenceValue")
    {
    }

    protected override ActaNovaTreeViewAutoCompleteReferenceValueControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new ActaNovaTreeViewAutoCompleteReferenceValueControlObject (newControlObjectContext);
    }
  }
}