using System;
using Coypu;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="TextBoxControlObject"/>.
  /// </summary>
  public class TextBoxSelector
      : ControlSelectorBase<TextBoxControlObject>,
          IFirstControlSelector<TextBoxControlObject>,
          IPerIndexControlSelector<TextBoxControlObject>,
          ISingleControlSelector<TextBoxControlObject>
  {
    private const string c_htmlTextBoxTag = "input";
    private const string c_htmlTextBoxCssTypeAttributeCheck = "[type='text']";

    public TextBoxControlObject SelectFirst (ControlSelectionContext context)
    {
      var scope = context.Scope.FindCss (c_htmlTextBoxTag + c_htmlTextBoxCssTypeAttributeCheck);
      return CreateControlObject (context, scope);
    }

    public TextBoxControlObject SelectSingle (ControlSelectionContext context)
    {
      var scope = context.Scope.FindCss (c_htmlTextBoxTag + c_htmlTextBoxCssTypeAttributeCheck, Options.Single);
      return CreateControlObject (context, scope);
    }

    public TextBoxControlObject SelectPerIndex (ControlSelectionContext context, int index)
    {
      var xPathSelector = string.Format ("(.//{0}{1})[{2}]", c_htmlTextBoxTag, XPathUtils.CreateHasAttributeCheck ("type", "text"), index);
      var scope = context.Scope.FindXPath (xPathSelector);
      return CreateControlObject (context, scope);
    }
  }
}