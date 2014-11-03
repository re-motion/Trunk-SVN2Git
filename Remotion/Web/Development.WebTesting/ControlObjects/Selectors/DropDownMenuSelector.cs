using System;
using System.Collections.Generic;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="DropDownMenuControlObject"/>.
  /// </summary>
  public class DropDownMenuSelector : TypedControlSelectorBase<DropDownMenuControlObject>, IPerTextControlSelector<DropDownMenuControlObject>
  {
    public DropDownMenuSelector ()
        : base ("DropDownMenu")
    {
    }

    public DropDownMenuControlObject SelectPerText (ControlSelectionContext context, string text)
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

    protected override DropDownMenuControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new DropDownMenuControlObject (newControlObjectContext);
    }
  }
}