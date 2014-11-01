using System;
using System.Collections.Generic;
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

    public DropDownMenuControlObject SelectPerText (WebTestObjectContext context, string text)
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
  }
}