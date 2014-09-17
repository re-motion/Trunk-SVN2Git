using System;
using Coypu;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object selector for <see cref="FormGridControlObject"/>.
  /// </summary>
  public class FormGridSelector : ControlSelectorBase<FormGridControlObject, ControlSelectionParameters>
  {
    public override FormGridControlObject FindTypedControl (TestObjectContext context, ControlSelectionParameters selectionParameters)
    {
      ElementScope scope;

      if (selectionParameters.ID != null)
        scope = context.Scope.FindId (selectionParameters.ID);
      else if (selectionParameters.Index != null)
      {
        var xpath = ".//table[contains(concat(' ', normalize-space(@class), ' '), ' formGridTable ')]";
        scope = context.Scope.FindXPath (string.Format ("{0}[{1}]", xpath, selectionParameters.Index.Value));
      }
      else if (selectionParameters.Title != null)
        scope = context.Scope.FindCss (string.Format ("table[{0}='{1}']", DiagnosticMetadataAttributes.Title, selectionParameters.Title));
      else
          // If no parameters are given, assume that the default form (= the only form) on the page has been requested
        scope = context.Scope.FindCss (".formGridTable", Options.Single);

      return new FormGridControlObject (scope.Id, context.CloneForScope (scope));
    }
  }
}