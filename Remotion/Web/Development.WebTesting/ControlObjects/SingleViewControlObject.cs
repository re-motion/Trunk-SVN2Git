using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="SingleView"/>.
  /// </summary>
  public class SingleViewControlObject : RemotionControlObject, IControlHost
  {
    public SingleViewControlObject ([NotNull] string id, [NotNull] TestObjectContext context)
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