using System;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Base class for all <see cref="IControlSelector{TControlObject}"/> implementations. Provides a default implementation for
  /// <see cref="FindControl"/> based on <see cref="FindTypedControl"/>.
  /// </summary>
  /// <typeparam name="TControlObject">The control object type to be found.</typeparam>
  public abstract class ControlSelectorBase<TControlObject> : IControlSelector<TControlObject>
      where TControlObject : ControlObject
  {
    public ControlObject FindControl (TestObjectContext context, ControlSelectionParameters selectionParameters)
    {
      return FindTypedControl (context, selectionParameters);
    }

    public abstract TControlObject FindTypedControl (TestObjectContext context, ControlSelectionParameters selectionParameters);
  }
}