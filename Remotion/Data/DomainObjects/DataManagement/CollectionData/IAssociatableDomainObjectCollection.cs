using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  /// <summary>
  /// Defines APIs used by <see cref="CollectionEndPoint"/> when it needs to transform a stand-alone <see cref="DomainObjectCollection"/> to
  /// an associated collection.
  /// </summary>
  public interface IAssociatableDomainObjectCollection
  {
    /// <summary>
    /// Transforms the collection to an associated collection. The collection will represent the data stored by <paramref name="endPoint"/>, and
    /// all modifications will be performed on the <paramref name="endPoint"/>. 
    /// This interface is used by  <see cref="CollectionEndPointSetCollectionCommand"/> and should usually not be required by framework 
    /// users.
    /// </summary>
    /// <param name="endPoint">The end point to associate with.</param>
    void TransformToAssociated (ICollectionEndPoint endPoint);

    /// <summary>
    /// Transforms the collection to a stand-alone collection. The collection will get its own data store and will not be associated with an 
    /// <see cref="ICollectionEndPoint"/> any longer.
    /// This interface is used by  <see cref="CollectionEndPointSetCollectionCommand"/> and should usually not be required by framework 
    /// users.
    /// </summary>
    void TransformToStandAlone ();

    /// <summary>
    /// Gets the <see cref="ICollectionEndPoint"/> associated with this <see cref="DomainObjectCollection"/>, or <see langword="null" /> if
    /// this is a stand-alone collection.
    /// </summary>
    /// <value>The associated end point.</value>
    RelationEndPointID AssociatedEndPointID { get; }

    /// <summary>
    /// Determines whether this <see cref="DomainObjectCollection"/> instance is associated to the specified <see cref="ICollectionEndPoint"/>.
    /// </summary>
    /// <param name="endPoint">The end point to check for. Pass <see langword="null" /> to check whether this collection is stand-alone.</param>
    /// <returns>
    /// 	<see langword="true"/> if this collection is associated to the specified end point; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsAssociatedWith (ICollectionEndPoint endPoint);
  }
}