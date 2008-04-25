using System.Configuration;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.Configuration
{
  /// <summary>
  /// The <see cref="IDomainObjectsConfiguration"/> interface is an abstraction for the <see cref="ConfigurationSectionGroup"/> and the fake 
  /// implementation of the domain objects configuration.
  /// </summary>
  public interface IDomainObjectsConfiguration
  {
    MappingLoaderConfiguration MappingLoader { get; }

    PersistenceConfiguration Storage { get; }

    QueryConfiguration Query { get; }
  }
}