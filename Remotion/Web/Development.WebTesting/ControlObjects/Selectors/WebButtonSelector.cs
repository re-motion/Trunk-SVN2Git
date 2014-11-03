using System;
using System.Collections.Generic;
using Remotion.Utilities;
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

    public WebButtonControlObject SelectPerText (ControlSelectionContext context, string text)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("text", text);

      var scope = context.Scope.FindDMA (
          "*",
          new Dictionary<string, string>
          {
              { DiagnosticMetadataAttributes.ControlType, ControlType },
              { DiagnosticMetadataAttributes.Text, text }
          });
      return CreateControlObject (context, scope);
    }

    public WebButtonControlObject SelectPerCommandName (ControlSelectionContext context, string commandName)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("commandName", commandName);

      var scope = context.Scope.FindDMA (
          "*",
          new Dictionary<string, string>
          {
              { DiagnosticMetadataAttributes.ControlType, ControlType },
              { DiagnosticMetadataAttributes.CommandName, commandName }
          });
      return CreateControlObject (context, scope);
    }

    public WebButtonControlObject SelectPerItemID (ControlSelectionContext context, string itemID)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("itemID", itemID);

      var scope = context.Scope.FindDMA (
          "*",
          new Dictionary<string, string>
          {
              { DiagnosticMetadataAttributes.ControlType, ControlType },
              { DiagnosticMetadataAttributes.ItemID, itemID }
          });
      return CreateControlObject (context, scope);
    }

    protected override WebButtonControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new WebButtonControlObject (newControlObjectContext);
    }
  }
}