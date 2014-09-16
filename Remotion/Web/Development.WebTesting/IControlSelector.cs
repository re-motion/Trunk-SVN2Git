using System;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Todo RM-6297: Docs
  /// </summary>
  public interface IControlSelector<TControlSelectionParameters>
    where TControlSelectionParameters : ControlSelectionParameters
  {
    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    ControlObject FindControl (TestObjectContext context, TControlSelectionParameters selectionParameters);
  }

  /// <summary>
  /// Todo RM-6297: Docs
  /// </summary>
  public interface IControlSelector<TControlObject, TControlSelectionParameters> : IControlSelector<TControlSelectionParameters>
      where TControlObject : ControlObject
      where TControlSelectionParameters : ControlSelectionParameters
  {
    /// <summary>
    /// Todo RM-6297: Docs
    /// </summary>
    TControlObject FindTypedControl (TestObjectContext context, TControlSelectionParameters selectionParameters);
  }
}