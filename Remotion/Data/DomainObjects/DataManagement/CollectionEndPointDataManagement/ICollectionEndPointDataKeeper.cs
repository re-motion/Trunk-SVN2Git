using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// Defines an interface for classes storing the data for a <see cref="CollectionEndPoint"/>.
  /// </summary>
  public interface ICollectionEndPointDataKeeper
  {
    IDomainObjectCollectionData CollectionData { get; }
    IDomainObjectCollectionData OriginalCollectionData { get; }

    void RegisterOppositeEndPoint (IObjectEndPoint oppositeEndPoint);
    void UnregisterOppositeEndPoint (IObjectEndPoint oppositeEndPoint);

    bool HasDataChanged (ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy);

    void SortCurrentAndOriginalData ();
    void CommitOriginalContents ();
  }
}