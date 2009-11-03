using System;
using System.Collections.Generic;

namespace Remotion.ExtensibleEnums.Infrastructure
{
  /// <summary>
  /// Provides a common interface for the discovery of extensible enum values. This interface is used by <see cref="ExtensibleEnumDefinition{T}"/>.
  /// </summary>
  public interface IExtensibleEnumValueDiscoveryService
  {
    IEnumerable<T> GetValues<T> (ExtensibleEnumDefinition<T> definition) 
        where T: ExtensibleEnum<T>;
  }
}