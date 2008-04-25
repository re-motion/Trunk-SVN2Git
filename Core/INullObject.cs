using System;

namespace Remotion
{
  // TODO FS: Move to Remotion.Interfaces
  /// <summary>
  /// Represents a nullable object according to the "Null Object Pattern".
  /// </summary>
  public interface INullObject
  {
    /// <summary>
    /// Gets a value indicating whether the object is a "Null Object".
    /// </summary>
    bool IsNull {get;}
  }
}