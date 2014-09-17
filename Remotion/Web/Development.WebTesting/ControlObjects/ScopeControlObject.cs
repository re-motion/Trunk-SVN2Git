using System;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// This control object represents controls (or areas within a control) which do not have any additional features than hosting other
  /// controls (<see cref="IControlHost"/>). Typcially this control object is returned by other control objects in order to scope into a specific
  /// area (e.g. top controls or bottom controls in <see cref="TabbedMultiViewControlObject"/>.
  /// </summary>
  public class ScopeControlObject : RemotionControlObject, IControlHost
  {
    public ScopeControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
        : base (id, context)
    {
    }

    // Todo RM-6297: ControlHostingRemotionControlObject?
    public ControlObject GetControl<TControlSelectionParameters> (
        IControlSelector<TControlSelectionParameters> selector,
        TControlSelectionParameters selectionParameters) where TControlSelectionParameters : ControlSelectionParameters
    {
      ArgumentUtility.CheckNotNull ("selector", selector);
      ArgumentUtility.CheckNotNull ("selectionParameters", selectionParameters);

      return selector.FindControl (Context, selectionParameters);
    }

    public TControlObject GetControl<TControlObject, TControlSelectionParameters> (
        IControlSelector<TControlObject, TControlSelectionParameters> selector,
        TControlSelectionParameters selectionParameters) where TControlObject : ControlObject
        where TControlSelectionParameters : ControlSelectionParameters
    {
      ArgumentUtility.CheckNotNull ("selector", selector);
      ArgumentUtility.CheckNotNull ("selectionParameters", selectionParameters);

      return selector.FindTypedControl (Context, selectionParameters);
    }
  }
}