using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing a collection of rows.
  /// </summary>
  public interface IControlObjectWithRowsWhereColumnContains<TRowControlObject>
      where TRowControlObject : ControlObject
  {
    IControlObjectWithRowsWhereColumnContains<TRowControlObject> GetRowWhere ();
    TRowControlObject GetRowWhere ([NotNull] string columnItemID, [NotNull] string containsCellText);

    TRowControlObject ColumnWithItemIDContains ([NotNull] string itemID, [NotNull] string containsCellText);
    TRowControlObject ColumnWithIndexContains (int index, [NotNull] string containsCellText);
    TRowControlObject ColumnWithTitleContains ([NotNull] string text, [NotNull] string containsCellText);
  }
}