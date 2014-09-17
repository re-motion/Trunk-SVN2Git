using System;
using Coypu;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object selector for <see cref="FormGridControlObject"/>.
  /// </summary>
  public class FormGridSelector : ControlSelectorBase<FormGridControlObject, ControlSelectionParameters>
  {
    private const string c_cssClass = "formGridTable";

    public override FormGridControlObject FindTypedControl (TestObjectContext context, ControlSelectionParameters selectionParameters)
    {
      ElementScope scope;

      if (selectionParameters.ID != null)
        scope = context.Scope.FindId (selectionParameters.ID);
      else if (selectionParameters.Index != null)
      {
        scope = context.Scope.FindXPath (
            string.Format (".//table{0}[{1}]", XPathUtils.CreateContainsClassCheck (c_cssClass), selectionParameters.Index.Value));
      }
      else if (selectionParameters.Title != null)
      {
        // Todo RM-6297 Later: Replace with CSS-based search as soon as FormGridManager is able to render the data-title attribute.
        // scope = context.Scope.FindCss (string.Format ("table[{0}='{1}']", DiagnosticMetadataAttributes.Title, selectionParameters.Title));

        // This implementation assumes that the CSS class of the title cell must be formGridTitleCell.
        scope = context.Scope.FindXPath (string.Format (".//table[contains(tbody/tr/td{0},'{1}')]", XPathUtils.CreateContainsClassCheck("formGridTitleCell"), selectionParameters.Title));

        // This alterantive implementation assumes that the formGridTitleCell is in the very first row and column.
        //scope = context.Scope.FindXPath (string.Format (".//table[contains(tbody/tr[1]/td[1],'{0}')]", selectionParameters.Title));
      }
      else
          // If no parameters are given, assume that the default form (= the only form) on the page has been requested
        scope = context.Scope.FindCss ("." + c_cssClass, Options.Single);

      return new FormGridControlObject (scope.Id, context.CloneForScope (scope));
    }
  }

  public static class XPathUtils
  {
    public static string CreateContainsClassCheck (string cssClass)
    {
      return string.Format ("[contains(concat(' ', normalize-space(@class), ' '), ' {0} ')]", cssClass);
    }
  }
}