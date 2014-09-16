using System;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Todo RM-6297: Docs
  /// </summary>
  public interface IControlSelector
  {
    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    ControlObject FindControl (TestObjectContext context, ControlSelectionParameters selectionParameters);
  }

  /// <summary>
  /// Todo RM-6297: Docs
  /// </summary>
  public interface IControlSelector<TControlObject> : IControlSelector
      where TControlObject : ControlObject
  {
    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    TControlObject FindTypedControl (TestObjectContext context, ControlSelectionParameters selectionParameters);
  }
}