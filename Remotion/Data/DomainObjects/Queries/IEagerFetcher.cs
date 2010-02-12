using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// Provides an interface for classes that can correlate and register the results of an eager fetch query.
  /// </summary>
  public interface IEagerFetcher
  {
    void CorrelateAndRegisterFetchResults (
        IEnumerable<DomainObject> originalObjects, 
        IEnumerable<DomainObject> fetchedObjects, 
        IRelationEndPointDefinition relationEndPointDefinition);
  }
}