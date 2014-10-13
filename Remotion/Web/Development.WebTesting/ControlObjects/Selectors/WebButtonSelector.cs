using System;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="WebButtonControlObject"/>.
  /// </summary>
  public class WebButtonSelector
      : RemotionControlSelectorBase<WebButtonControlObject>,
          IPerTextControlSelector<WebButtonControlObject>,
          IPerCommandNameControlSelector<WebButtonControlObject>
  {
    public WebButtonSelector ()
        : base ("button", "webButton")
    {
    }

    public WebButtonControlObject SelectPerText (TestObjectContext context, string displayName)
    {
      var scope = context.Scope.FindDMA ("button", DiagnosticMetadataAttributes.Text, displayName);
      return CreateControlObject (context, scope);
    }

    public WebButtonControlObject SelectPerCommandName (TestObjectContext context, string commandName)
    {
      var scope = context.Scope.FindDMA ("button", DiagnosticMetadataAttributes.CommandName, commandName);
      return CreateControlObject (context, scope);
    }
  }
}