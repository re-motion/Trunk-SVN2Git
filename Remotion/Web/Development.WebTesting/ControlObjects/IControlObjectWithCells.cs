using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing a collection of cells.
  /// </summary>
  public interface IControlObjectWithCells<TCellControlObject>
      where TCellControlObject : ControlObject
  {
    IControlObjectWithCells<TCellControlObject> GetCell ();
    TCellControlObject GetCell ([NotNull] string columnItemID);

    TCellControlObject WithColumnItemID ([NotNull] string columnItemID);
    TCellControlObject WithIndex (int index);
  }
}