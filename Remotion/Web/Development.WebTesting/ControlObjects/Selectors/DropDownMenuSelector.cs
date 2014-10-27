using System;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="DropDownMenuControlObject"/>.
  /// </summary>
  public class DropDownMenuSelector : RemotionControlSelectorBase<DropDownMenuControlObject>, IPerTextControlSelector<DropDownMenuControlObject>
  {
    public DropDownMenuSelector ()
        : base ("span", "DropDownMenuContainer")
    {
    }

    public DropDownMenuControlObject SelectPerText (TestObjectContext context, string text)
    {
      var scope = context.Scope.FindDMA ("span", DiagnosticMetadataAttributes.Text, text);
      return CreateControlObject (context, scope);
    }
  }
}