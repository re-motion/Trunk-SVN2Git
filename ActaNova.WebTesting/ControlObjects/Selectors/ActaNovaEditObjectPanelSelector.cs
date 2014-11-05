using System;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="ActaNovaEditObjectPanelControlObject"/>.
  /// </summary>
  public class ActaNovaEditObjectPanelSelector
      : ControlSelectorBase<ActaNovaEditObjectPanelControlObject>,
          ISingleControlSelector<ActaNovaEditObjectPanelControlObject>
  {
    public ActaNovaEditObjectPanelControlObject SelectSingle (ControlSelectionContext context)
    {
      var scope = context.Scope.FindId ("PageEditObjectPanel_FormGridContainer");
      return CreateControlObject (context, scope);
    }

    protected override ActaNovaEditObjectPanelControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      return new ActaNovaEditObjectPanelControlObject (newControlObjectContext);
    }
  }
}