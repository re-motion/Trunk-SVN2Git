using System;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="ActaNovaTreePopupTableControlObject"/>.
  /// </summary>
  public class ActaNovaTreePopupTableSelector
      : ControlSelectorBase<ActaNovaTreePopupTableControlObject>,
          ISingleControlSelector<ActaNovaTreePopupTableControlObject>
  {
    public ActaNovaTreePopupTableControlObject SelectSingle (ControlSelectionContext context)
    {
      var scope = context.Scope.FindCss ("div.popupTableDiv");
      scope.EnsureSingle();
      return CreateControlObject (context, scope);
    }

    protected override ActaNovaTreePopupTableControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      return new ActaNovaTreePopupTableControlObject (newControlObjectContext);
    }
  }
}