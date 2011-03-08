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
    IDomainObjectCollectionData CollectionData { get; }
    ReadOnlyCollectionDataDecorator OriginalCollectionData { get; }
    IObjectEndPoint[] OppositeEndPoints { get; }

    bool ContainsOppositeEndPoint (IObjectEndPoint oppositeEndPoint);

    void RegisterOppositeEndPoint (IObjectEndPoint oppositeEndPoint);
    void UnregisterOppositeEndPoint (IObjectEndPoint oppositeEndPoint);

    bool HasDataChanged (ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy);

    void SortCurrentAndOriginalData ();
    void CommitOriginalContents ();
    
    void Insert (int index, IObjectEndPoint insertedEndPoint);
  }
}