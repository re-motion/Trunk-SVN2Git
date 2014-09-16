using System;
using Coypu;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting
{
  /// <summary>
  /// Control object selector base class for all business object controls.
  /// </summary>
  public abstract class BocSelectorBase<TControlObject> : ControlSelectorBase<TControlObject, BocControlSelectionParameters>
      where TControlObject : ControlObject
  {
    /// <summary>
    /// Finds the scope of the business object <see cref="ControlObject"/> within the given context.
    /// </summary>
    /// <param name="context">Search context.</param>
    /// <param name="selectionParameters">Selection parameters.</param>
    /// <param name="rootElement">Root tag of the business object control.</param>
    /// <returns></returns>
    protected ElementScope FindScope (TestObjectContext context, BocControlSelectionParameters selectionParameters, string rootElement = "span")
    {
      ElementScope scope;

      if (selectionParameters.ID != null)
        scope = context.Scope.FindId (selectionParameters.ID);
      else if (selectionParameters.LocalID != null)
        scope = context.Scope.FindIdEndingWith (selectionParameters.LocalID);
      else if (selectionParameters.BoundType != null && selectionParameters.BoundProperty != null)
      {
        scope = context.Scope.FindCss (
            string.Format (
                "{0}[{1}='{2}'][{3}='{4}']",
                rootElement,
                DiagnosticMetadataAttributes.BoundType,
                selectionParameters.BoundType,
                DiagnosticMetadataAttributes.BoundProperty,
                selectionParameters.BoundProperty));
      }
      else if (selectionParameters.BoundProperty != null)
      {
        scope = context.Scope.FindCss (
            string.Format ("{0}[{1}='{2}']", rootElement, DiagnosticMetadataAttributes.BoundProperty, selectionParameters.BoundProperty));
      }
      else if (selectionParameters.DisplayName != null)
      {
        scope = context.Scope.FindCss (
            string.Format ("{0}[{1}='{2}']", rootElement, DiagnosticMetadataAttributes.DisplayName, selectionParameters.DisplayName));
      }
      else
        throw new NotSupportedException ("Combination of selectionParameters is not supported.");

      return scope;
    }
  }
}