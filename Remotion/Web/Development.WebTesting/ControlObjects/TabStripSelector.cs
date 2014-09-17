using Coypu;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object selector for <see cref="WebTabStrip"/>.
  /// </summary>
  public class TabStripSelector : ControlSelectorBase<TabStripControlObject, ControlSelectionParameters>
  {
    private const string c_cssClass = "tabStrip";

    public override TabStripControlObject FindTypedControl (TestObjectContext context, ControlSelectionParameters selectionParameters)
    {
      ElementScope scope;

      if (selectionParameters.ID != null)
        scope = context.Scope.FindId (selectionParameters.ID);
      else if (selectionParameters.Index != null)
      {
        scope = context.Scope.FindXPath (
            string.Format ("(.//div{0})[{1}]", XPathUtils.CreateContainsClassCheck (c_cssClass), selectionParameters.Index.Value));
      }
      else
      {
        // If no parameters are given, assume that the default form (= the only form) on the page has been requested
        scope = context.Scope.FindCss ("." + c_cssClass, Options.Single);
      }

      return new TabStripControlObject (scope.Id, string.Empty, context.CloneForScope (scope));
    }
  }
}