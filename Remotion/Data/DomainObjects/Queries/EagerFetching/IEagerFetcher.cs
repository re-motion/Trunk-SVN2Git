using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Queries.EagerFetching
{
  /// <summary>
  /// Provides an interface for classes that can execute, correlate, and register eager fetch queries.
  /// </summary>
  public interface IEagerFetcher
  {
    void PerformEagerFetching (
        DomainObject[] originalObjects,
        IRelationEndPointDefinition relationEndPointDefinition,
        IQuery fetchQuery,
        IObjectLoader fetchQueryResultLoader,
        IDataManager dataManager);
  }
}