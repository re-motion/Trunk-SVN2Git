using System;
using Coypu;
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

    public AnchorControlObject SelectFirst (TestObjectContext context)
    {
      var scope = context.Scope.FindCss (c_htmlAnchorTag);
      return CreateControlObject (context, scope);
    }

    public AnchorControlObject SelectSingle (TestObjectContext context)
    {
      var scope = context.Scope.FindCss (c_htmlAnchorTag, Options.Single);
      return CreateControlObject (context, scope);
    }

    public AnchorControlObject SelectPerIndex (TestObjectContext context, int index)
    {
      var scope = context.Scope.FindXPath (string.Format ("(.//{0})[{1}]", c_htmlAnchorTag, index));
      return CreateControlObject (context, scope);
    }
  }
}