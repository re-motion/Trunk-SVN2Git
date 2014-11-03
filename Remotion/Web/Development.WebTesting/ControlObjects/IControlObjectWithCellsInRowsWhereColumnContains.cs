using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing a collection of rows which themselves represent a collection of
  /// cells.
  /// </summary>
  public interface IControlObjectWithCellsInRowsWhereColumnContains<TCellControlObject>
  {
    IControlObjectWithCellsInRowsWhereColumnContains<TCellControlObject> GetCellWhere ();
    TCellControlObject GetCellWhere ([NotNull] string columnItemID, [NotNull] string containsCellText);

    TCellControlObject ColumnWithItemIDContains ([NotNull] string itemID, [NotNull] string containsCellText);
    TCellControlObject ColumnWithIndexContains (int index, [NotNull] string containsCellText);
    TCellControlObject ColumnWithTitleContains ([NotNull] string text, [NotNull] string containsCellText);
  }
}