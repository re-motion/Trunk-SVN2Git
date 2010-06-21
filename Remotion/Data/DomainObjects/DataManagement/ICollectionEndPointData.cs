using System;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Defines an interface for classes storing the data for a <see cref="CollectionEndPoint"/>. Used by <see cref="CollectionEndPoint"/> as
  /// its data store and by <see cref="EndPointDelegatingCollectionData"/> in order to delegate to the end-point's data store.
  /// </summary>
  public interface ICollectionEndPointData
  {
    bool IsDataAvailable { get; }

    IDomainObjectCollectionData CollectionData { get; }
    IDomainObjectCollectionData OriginalCollectionData { get; }

    bool HasDataChanged (ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy);
    void EnsureDataAvailable ();
    void Unload ();
  }
}