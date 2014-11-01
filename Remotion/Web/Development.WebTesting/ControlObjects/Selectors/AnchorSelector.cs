using System;
using Coypu;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="AnchorControlObject"/>.
  /// </summary>
  public class AnchorSelector
      : ControlSelectorBase<AnchorControlObject>,
          IFirstControlSelector<AnchorControlObject>,
          IPerIndexControlSelector<AnchorControlObject>,
          ISingleControlSelector<AnchorControlObject>
  {
    private const string c_htmlAnchorTag = "a";

    public AnchorControlObject SelectFirst (ControlSelectionContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var scope = context.Scope.FindCss (c_htmlAnchorTag);
      return CreateControlObject (context, scope);
    }

    public AnchorControlObject SelectSingle (ControlSelectionContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var scope = context.Scope.FindCss (c_htmlAnchorTag, Options.Single);
      return CreateControlObject (context, scope);
    }

    public AnchorControlObject SelectPerIndex (ControlSelectionContext context, int index)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      var scope = context.Scope.FindXPath (string.Format ("(.//{0})[{1}]", c_htmlAnchorTag, index));
      return CreateControlObject (context, scope);
    }
  }
}