using System;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="FormGridControlObject"/>.
  /// </summary>
  public class FormGridSelector
      : TypedControlSelectorBase<FormGridControlObject>, IPerTitleControlSelector<FormGridControlObject>
  {
    public FormGridSelector ()
        : base ("FormGrid")
    {
    }

    public FormGridControlObject SelectPerTitle (WebTestObjectContext context, string title)
    {
      // Todo RM-6297: Replace with CSS-based search as soon as FormGridManager is able to render the data-title attribute.
      // Note: it is not that easy, as we do not know the content of the title row on the server...FormGrid is just a design transformator...
      //var scope = context.Scope.FindCss (string.Format ("table[{0}='{1}']", DiagnosticMetadataAttributes.FormGridTitle, title));

      // Note: this implementation assumes that the title cell has the CSS class formGridTitleCell.
      var scope = context.Scope.FindXPath (
          string.Format (".//table[contains(tbody/tr/td{0},'{1}')]", XPathUtils.CreateHasClassCheck ("formGridTitleCell"), title));

      // This alterantive implementation assumes that the title cell is the very first row and column.
      // var scope = context.Scope.FindXPath (string.Format (".//table[contains(tbody/tr[1]/td[1],'{0}')]", title));

      return CreateControlObject (context, scope);
    }
  }
}