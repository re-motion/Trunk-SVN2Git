using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  /// <summary>
  /// Defines APIs used by <see cref="CollectionEndPoint"/> when it needs to transform a stand-alone <see cref="DomainObjectCollection"/> to
  /// an associated collection.
  /// </summary>
  public interface IAssociatableDomainObjectCollection
  {
    /// <summary>
    /// Transforms the collection to an associated collection. The collection will represent the data stored by the <see cref="ICollectionEndPoint"/>
    /// represented by <paramref name="endPointID"/>, and all modifications will be performed on that <see cref="ICollectionEndPoint"/>.
    /// This interface is used by <see cref="CollectionEndPointSetCollectionCommand"/> and should usually not be called by framework
    /// users.
    /// </summary>
    /// <param name="endPointID">The <see cref="RelationEndPointID"/> of the <see cref="ICollectionEndPoint"/> to associate with.</param>
    /// <param name="associatedCollectionDataStrategyFactory">
    ///   The <see cref="IAssociatedCollectionDataStrategyFactory"/> to get the new data strategy from.
    /// </param>
    void TransformToAssociated (RelationEndPointID endPointID, IAssociatedCollectionDataStrategyFactory associatedCollectionDataStrategyFactory);

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