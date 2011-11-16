using System;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Defines an interface for classes storing the data for a <see cref="CollectionEndPoint"/>.
  /// </summary>
  public interface ICollectionEndPointDataKeeper : IVirtualEndPointDataKeeper
  {
    IDomainObjectCollectionData CollectionData { get; }
    ReadOnlyCollectionDataDecorator OriginalCollectionData { get; }

    IRealObjectEndPoint[] OriginalOppositeEndPoints { get; }
    DomainObject[] OriginalItemsWithoutEndPoints { get; }
    IRealObjectEndPoint[] CurrentOppositeEndPoints { get; }

    bool ContainsOriginalItemWithoutEndPoint (DomainObject domainObject);

    void SortCurrentAndOriginalData (Comparison<DomainObject> comparison);
    void SetDataFromSubTransaction (ICollectionEndPointDataKeeper sourceDataKeeper, IRelationEndPointProvider endPointProvider);
  }
}