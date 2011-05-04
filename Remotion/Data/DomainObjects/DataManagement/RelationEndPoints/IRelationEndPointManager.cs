using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Provides a read-only API to access the <see cref="RelationEndPoint"/> instances loaded into a <see cref="RelationEndPointManager"/>.
  /// </summary>
  public interface IRelationEndPointManager : IFlattenedSerializable
  {
    IRelationEndPointMapReadOnlyView RelationEndPoints { get; }

    void RegisterEndPointsForDataContainer (DataContainer dataContainer);
    IDataManagementCommand CreateUnregisterCommandForDataContainer (DataContainer dataContainer);

    IRelationEndPoint GetRelationEndPointWithoutLoading (RelationEndPointID endPointID);
    IRelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID);
    IRelationEndPoint GetRelationEndPointWithMinimumLoading (RelationEndPointID endPointID);

    DomainObject GetRelatedObject (RelationEndPointID endPointID, bool includeDeleted);
    DomainObject GetOriginalRelatedObject (RelationEndPointID endPointID);
    DomainObjectCollection GetRelatedObjects (RelationEndPointID endPointID);
    DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID endPointID);

    void CommitAllEndPoints ();
    void RollbackAllEndPoints ();
    
    void MarkCollectionEndPointComplete (RelationEndPointID endPointID, DomainObject[] items);
   
    // TODO 3634: Remove
    void RemoveEndPoint (RelationEndPointID endPointID);
  }
}