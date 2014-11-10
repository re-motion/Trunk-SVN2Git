using System;
using Coypu;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="DownLevelDmsControlObject"/>.
  /// </summary>
  public class DownLevelDmsSelector
      : ControlSelectorBase<DownLevelDmsControlObject>,
          ISingleControlSelector<DownLevelDmsControlObject>
  {
    public DownLevelDmsControlObject SelectSingle (ControlSelectionContext context)
    {
      var scope = context.Scope.FindCss ("table.dmsContentTable", new Options { Match = Match.Single });
      return CreateControlObject (context, scope);
    }

    protected override DownLevelDmsControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      return new DownLevelDmsControlObject (newControlObjectContext);
    }
  }
}