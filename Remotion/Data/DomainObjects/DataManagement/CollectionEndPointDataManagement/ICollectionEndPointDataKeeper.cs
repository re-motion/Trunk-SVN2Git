using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// Defines an interface for classes storing the data for a <see cref="CollectionEndPoint"/>. Used by <see cref="CollectionEndPoint"/> as
  /// its data store and by <see cref="EndPointDelegatingCollectionData"/> in order to delegate to the end-point's data store.
  /// </summary>
  public interface ICollectionEndPointDataKeeper
  {
    bool IsDataAvailable { get; }

    IDomainObjectCollectionData CollectionData { get; }
    IDomainObjectCollectionData OriginalCollectionData { get; }

    bool HasDataChanged (ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy);
    void EnsureDataAvailable ();
    void Unload ();
    void CommitOriginalContents ();
  }
}