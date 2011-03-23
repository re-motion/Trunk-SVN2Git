﻿using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints
{
  /// <summary>
  /// Defines an interface for classes keeping the data for an <see cref="IVirtualEndPoint"/>.
  /// </summary>
  public interface IVirtualEndPointDataKeeper : IFlattenedSerializable
  {
    RelationEndPointID EndPointID { get; }
    
    bool ContainsOriginalObjectID (ObjectID objectID);
    bool ContainsOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);

    bool ContainsOriginalItemsWithoutEndPoints ();

    void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);
    void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);

    void RegisterOriginalItemWithoutEndPoint (DomainObject domainObject);
    void UnregisterOriginalItemWithoutEndPoint (DomainObject domainObject);

    void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);
    void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);

    bool HasDataChanged();
    void Commit();
    void Rollback();
  }
}