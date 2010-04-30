using System;
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public interface IRelationEndPointMapReadOnlyView : IEnumerable<RelationEndPoint>
  {
    int Count { get; }
    RelationEndPoint this[RelationEndPointID endPointID] { get; }

    bool Contains (RelationEndPointID id);

    RelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID);

    DomainObject GetRelatedObject (RelationEndPointID endPointID, bool includeDeleted);
    DomainObject GetOriginalRelatedObject (RelationEndPointID endPointID);
    DomainObjectCollection GetRelatedObjects (RelationEndPointID endPointID);
    DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID endPointID);
  }
}