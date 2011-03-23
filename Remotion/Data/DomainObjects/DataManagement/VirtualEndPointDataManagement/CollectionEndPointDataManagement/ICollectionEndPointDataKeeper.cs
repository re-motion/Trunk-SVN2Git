using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.DomainObjects.DataManagement.VirtualEndPointDataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// Defines an interface for classes storing the data for a <see cref="CollectionEndPoint"/>.
  /// </summary>
  public interface ICollectionEndPointDataKeeper : IFlattenedSerializable
  {
    RelationEndPointID EndPointID { get; }
    
    IDomainObjectCollectionData CollectionData { get; }
    ReadOnlyCollectionDataDecorator OriginalCollectionData { get; }

    IRealObjectEndPoint[] OriginalOppositeEndPoints { get; }
    DomainObject[] OriginalItemsWithoutEndPoints { get; }
    IRealObjectEndPoint[] CurrentOppositeEndPoints { get; }

    bool ContainsOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);

    void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);
    void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);

    bool ContainsCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);

    void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);
    void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint);

    bool ContainsOriginalItemWithoutEndPoint (DomainObject domainObject);

    void RegisterOriginalItemWithoutEndPoint (DomainObject domainObject);
    void UnregisterOriginalItemWithoutEndPoint (DomainObject domainObject);
    
    bool HasDataChanged ();

    void SortCurrentAndOriginalData (IComparer<DomainObject> comparer);

    void Commit ();
    void Rollback ();
  }
}