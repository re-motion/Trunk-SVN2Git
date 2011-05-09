using System.Collections.Generic;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Provides an API to manage the <see cref="IRelationEndPoint"/> instances loaded into a <see cref="ClientTransaction"/>.
  /// </summary>
  public interface IRelationEndPointManager : IFlattenedSerializable
  {
    IRelationEndPointMapReadOnlyView RelationEndPoints { get; }

    void RegisterEndPointsForDataContainer (DataContainer dataContainer);
    IDataManagementCommand CreateUnregisterCommandForDataContainer (DataContainer dataContainer);
    IDataManagementCommand CreateUnloadVirtualEndPointsCommand (IEnumerable<RelationEndPointID> endPointIDs);

    IRelationEndPoint GetRelationEndPointWithoutLoading (RelationEndPointID endPointID);
    IRelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID);
    IRelationEndPoint GetRelationEndPointWithMinimumLoading (RelationEndPointID endPointID);

    void CommitAllEndPoints ();
    void RollbackAllEndPoints ();
    
    bool TrySetCollectionEndPointData (RelationEndPointID endPointID, DomainObject[] items);
    bool TrySetVirtualObjectEndPointData (RelationEndPointID endPointID, DomainObject item);
  }
}