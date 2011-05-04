using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Provides a read-only API to access the <see cref="RelationEndPoint"/> instances loaded into a <see cref="RelationEndPointMap"/>.
  /// </summary>
  public interface IRelationEndPointMapReadOnlyView : IEnumerable<IRelationEndPoint>
  {
    int Count { get; }

    bool Contains (RelationEndPointID id);

    IRelationEndPoint GetRelationEndPointWithoutLoading (RelationEndPointID endPointID);
    IRelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID);
    IRelationEndPoint GetRelationEndPointWithMinimumLoading (RelationEndPointID endPointID);

    DomainObject GetRelatedObject (RelationEndPointID endPointID, bool includeDeleted);
    DomainObject GetOriginalRelatedObject (RelationEndPointID endPointID);
    DomainObjectCollection GetRelatedObjects (RelationEndPointID endPointID);
    DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID endPointID);
  }
}