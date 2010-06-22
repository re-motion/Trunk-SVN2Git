using System;
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Provides a read-only API to access the <see cref="RelationEndPoint"/> instances loaded into a <see cref="RelationEndPointMap"/>.
  /// </summary>
  public interface IRelationEndPointMapReadOnlyView : IEnumerable<RelationEndPoint>
  {
    int Count { get; }
    RelationEndPoint this[RelationEndPointID endPointID] { get; }

    ICollectionEndPointChangeDetectionStrategy CollectionEndPointChangeDetectionStrategy { get; }

    bool Contains (RelationEndPointID id);

    RelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID);

    DomainObject GetRelatedObject (RelationEndPointID endPointID, bool includeDeleted);
    DomainObject GetOriginalRelatedObject (RelationEndPointID endPointID);
    DomainObjectCollection GetRelatedObjects (RelationEndPointID endPointID);
    DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID endPointID);
  }
}