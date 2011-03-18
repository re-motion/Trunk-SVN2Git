using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// Defines an interface for classes storing the data for a <see cref="CollectionEndPoint"/>.
  /// </summary>
  public interface ICollectionEndPointDataKeeper : IFlattenedSerializable
  {
    RelationEndPointID EndPointID { get; }
    
    IDomainObjectCollectionData CollectionData { get; }
    ReadOnlyCollectionDataDecorator OriginalCollectionData { get; }
    IObjectEndPoint[] OriginalOppositeEndPoints { get; }
    DomainObject[] OriginalItemsWithoutEndPoints { get; }

    bool ContainsOriginalOppositeEndPoint (IObjectEndPoint oppositeEndPoint);

    void RegisterOriginalOppositeEndPoint (IObjectEndPoint oppositeEndPoint);
    void UnregisterOriginalOppositeEndPoint (IObjectEndPoint oppositeEndPoint);

    bool ContainsCurrentOppositeEndPoint (IObjectEndPoint oppositeEndPoint);

    void RegisterCurrentOppositeEndPoint (IObjectEndPoint oppositeEndPoint);
    void UnregisterCurrentOppositeEndPoint (IObjectEndPoint oppositeEndPoint);

    bool ContainsOriginalItemWithoutEndPoint (DomainObject domainObject);

    void RegisterOriginalItemWithoutEndPoint (DomainObject domainObject);
    void UnregisterOriginalItemWithoutEndPoint (DomainObject domainObject);
    
    bool HasDataChanged ();

    void SortCurrentAndOriginalData (IComparer<DomainObject> comparer);
    void Commit ();
  }
}