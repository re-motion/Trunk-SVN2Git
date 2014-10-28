using System;
using System.Collections.Generic;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="WebButtonControlObject"/>.
  /// </summary>
  public class WebButtonSelector
      : TypedControlSelectorBase<WebButtonControlObject>,
          IPerTextControlSelector<WebButtonControlObject>,
          IPerCommandNameControlSelector<WebButtonControlObject>,
          IPerItemIDControlSelector<WebButtonControlObject>
  {
    public WebButtonSelector ()
        : base ("WebButton")
    {
    }

    public WebButtonControlObject SelectPerText (TestObjectContext context, string text)
    {
      var scope = context.Scope.FindDMA (
          "*",
          new Dictionary<string, string>
          {
              { DiagnosticMetadataAttributes.ControlType, ControlType },
              { DiagnosticMetadataAttributes.Text, text }
          });
      return CreateControlObject (context, scope);
    }

    public WebButtonControlObject SelectPerCommandName (TestObjectContext context, string commandName)
    {
      var scope = context.Scope.FindDMA (
          "*",
          new Dictionary<string, string>
          {
              { DiagnosticMetadataAttributes.ControlType, ControlType },
              { DiagnosticMetadataAttributes.CommandName, commandName }
          });
      return CreateControlObject (context, scope);
    }

    public WebButtonControlObject SelectPerItemID (TestObjectContext context, string itemID)
    {
      var scope = context.Scope.FindDMA (
          "*",
          new Dictionary<string, string>
          {
              { DiagnosticMetadataAttributes.ControlType, ControlType },
              { DiagnosticMetadataAttributes.ItemID, itemID }
          });
      return CreateControlObject (context, scope);
    }
  }
}