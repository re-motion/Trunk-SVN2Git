using System;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="WebButtonControlObject"/>.
  /// </summary>
  public class WebButtonSelector
      : RemotionControlSelectorBase<WebButtonControlObject>,
          //IPerDisplayNameControlSelector<WebButtonControlObject>,
          IPerCommandNameControlSelector<WebButtonControlObject>
  {
    public WebButtonSelector ()
        : base ("button", "webButton")
    {
    }

    public WebButtonControlObject SelectPerDisplayName (TestObjectContext context, string displayName)
    {
      var scope = context.Scope.FindCss (string.Format ("button[{0}='{1}']", DiagnosticMetadataAttributes.Text, displayName));
      return CreateControlObject (context, scope);
    }

    public WebButtonControlObject SelectPerCommandName (TestObjectContext context, string commandName)
    {
      var scope = context.Scope.FindCss (string.Format ("button[{0}='{1}']", DiagnosticMetadataAttributes.CommandName, commandName));
      return CreateControlObject (context, scope);
    }
  }
}