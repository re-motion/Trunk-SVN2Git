using System;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Base class for all <see cref="IControlSelector{TControlObject}"/> implementations. Provides a default implementation for
  /// <see cref="FindControl"/> based on <see cref="FindTypedControl"/>.
  /// </summary>
  /// <typeparam name="TControlObject">The control object type to be found.</typeparam>
  /// <typeparam name="TControlSelectionParameters">The control selection parameters type required for this control selector.</typeparam>
  public abstract class ControlSelectorBase<TControlObject, TControlSelectionParameters>
      : IControlSelector<TControlObject, TControlSelectionParameters>
      where TControlObject : ControlObject
      where TControlSelectionParameters : ControlSelectionParameters
  {
    public ControlObject FindControl (TestObjectContext context, TControlSelectionParameters selectionParameters)
    {
      return FindTypedControl (context, selectionParameters);
    }

    public abstract TControlObject FindTypedControl (TestObjectContext context, TControlSelectionParameters selectionParameters);
  }
}