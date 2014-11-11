using System;
using Coypu;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="ActaNovaDownLevelDmsControlObject"/>.
  /// </summary>
  public class ActaNovaDownLevelDmsSelector
      : ControlSelectorBase<ActaNovaDownLevelDmsControlObject>,
          ISingleControlSelector<ActaNovaDownLevelDmsControlObject>
  {
    public ActaNovaDownLevelDmsControlObject SelectSingle (ControlSelectionContext context)
    {
      var scope = context.Scope.FindCss ("table.dmsContentTable", new Options { Match = Match.Single });
      return CreateControlObject (context, scope);
    }

    protected override ActaNovaDownLevelDmsControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      return new ActaNovaDownLevelDmsControlObject (newControlObjectContext);
    }
  }
}