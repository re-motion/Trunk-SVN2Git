using System;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Accessor methods for <see cref="BocListRowControlObject"/> and <see cref="BocListEditableRowControlObject"/>.
  /// </summary>
  public interface IBocListRowControlObjectHostAccessor
  {
    /// <summary>
    /// Returns the one-based column index of the given <paramref name="columnItemID"/> within the parent control.
    /// </summary>
    int GetColumnIndex (string columnItemID);
  }
}