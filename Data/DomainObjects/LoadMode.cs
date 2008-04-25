using System;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Indicates whether a <see cref="DomainObject"/> was loaded as a whole or if only its <see cref="DataContainer"/> was loaded.
  /// </summary>
  public enum LoadMode
  {
    /// <summary>
    /// The whole object has been loaded, e.g. as a reaction to <see cref="DomainObject.GetObject{T}"/>.
    /// </summary>
    WholeDomainObjectInitialized,
    /// <summary>
    /// Only the object's <see cref="DataContainer"/> has been loaded, e.g. as a reaction to <see cref="ClientTransaction.EnlistDomainObject"/>.
    /// </summary>
    DataContainerLoadedOnly
  }
}