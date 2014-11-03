using System;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Interface for all <see cref="ControlObject"/> implementations representing a collection of cells.
  /// </summary>
  public interface IControlObjectWithRows<TRowControlObject>
      where TRowControlObject : ControlObject
  {
    IControlObjectWithRows<TRowControlObject> GetRow ();
    TRowControlObject GetRow ([NotNull] string columnItemID);

    TRowControlObject WithColumnItemID ([NotNull] string columnItemID);
    TRowControlObject WithIndex (int index);
  }
}