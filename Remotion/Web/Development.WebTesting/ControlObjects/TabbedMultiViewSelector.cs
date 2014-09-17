using System;
using Coypu;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object selector for <see cref="SingleViewControlObject"/>.
  /// </summary>
  public class TabbedMultiViewSelector : ControlSelectorBase<TabbedMultiViewControlObject, ControlSelectionParameters>
  {
    private const string c_cssClass = "tabbedMultiView";

    public override TabbedMultiViewControlObject FindTypedControl (TestObjectContext context, ControlSelectionParameters selectionParameters)
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

      return new TabbedMultiViewControlObject (scope.Id, context.CloneForScope (scope));
    }
  }
}