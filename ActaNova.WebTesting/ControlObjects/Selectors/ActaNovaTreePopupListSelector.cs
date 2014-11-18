using System;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="ActaNovaTreePopupListControlObject"/>.
  /// </summary>
  public class ActaNovaTreePopupListSelector
      : ControlSelectorBase<ActaNovaTreePopupListControlObject>,
          ISingleControlSelector<ActaNovaTreePopupListControlObject>
  {
    public ActaNovaTreePopupListControlObject SelectSingle (ControlSelectionContext context)
    {
      var scope = context.Scope.FindCss ("div.popupTableDiv");
      scope.EnsureSingle();
      return CreateControlObject (context, scope);
    }

    /// <inheritdoc/>
    protected override ActaNovaTreePopupListControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      return new ActaNovaTreePopupListControlObject (newControlObjectContext);
    }
  }
}