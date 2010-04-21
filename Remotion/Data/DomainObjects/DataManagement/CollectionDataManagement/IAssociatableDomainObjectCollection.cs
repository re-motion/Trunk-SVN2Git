using System;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement
{
  /// <summary>
  /// Defines APIs used by <see cref="CollectionEndPoint"/> when it needs to transform a stand-alone <see cref="DomainObjectCollection"/> to
  /// an associated collection.
  /// </summary>
  public interface IAssociatableDomainObjectCollection
  {
    /// <summary>
    /// Creates an <see cref="IDataManagementCommand"/> instance that encapsulates all the modifications required to associate this
    /// <see cref="DomainObjectCollection"/> with the given <paramref name="endPoint"/>. This API is usually not employed by framework users,
    /// but it is automatically invoked when a collection-valued relation property is set to a new collection.
    /// </summary>
    /// <param name="endPoint">The end point to associate with. That end point's <see cref="ICollectionEndPoint.OppositeDomainObjects"/> collection
    /// must have the same type and <see cref="DomainObjectCollection.RequiredItemType"/> as this collection.</param>
    /// <exception cref="NotSupportedException">This collection is read-only.</exception>
    /// <exception cref="InvalidOperationException">This collection has another type or item type, or it is already associated with an end point.</exception>
    /// <remarks>
    /// <para>
    /// When the command is executed, it replaces the given end point's <see cref="ICollectionEndPoint.OppositeDomainObjects"/> collection with 
    /// this <see cref="DomainObjectCollection"/> instance, which is transformed into an associated collection. The previous 
    /// <see cref="ICollectionEndPoint.OppositeDomainObjects"/> collection of the end point is transformed into a stand-alone collection.
    /// </para>
    /// <para>
    /// The returned <see cref="IDataManagementCommand"/> should be executed as a bidirectional modification 
    /// (<see cref="IDataManagementCommand.ExpandToAllRelatedObjects"/>), otherwise inconsistent state might arise.
    /// </para>
    /// <para>
    /// This method does not check whether this collection is already associated with another end-point and should therefore be handled with care,
    /// otherwise an inconsistent state might result.
    /// </para>
    /// <para>
    /// This method is part of <see cref="DomainObjectCollection"/> rather than <see cref="CollectionEndPoint"/> because it is very tightly
    /// coupled to <see cref="DomainObjectCollection"/>: associating a collection will modify its inner data storage strategy, and 
    /// <see cref="CollectionEndPoint"/> has no possibility to do that.
    /// </para>
    /// </remarks>
    IDataManagementCommand CreateAssociationCommand (CollectionEndPoint endPoint);

    /// <summary>
    /// Transforms the collection to an associated collection. The collection will represent the data stored by <paramref name="endPoint"/>, and
    /// all modifications will be performed on the <paramref name="endPoint"/>. 
    /// This interface is used by  <see cref="CollectionEndPointReplaceWholeCollectionCommand"/> and should usually not be required by framework 
    /// users.
    /// </summary>
    /// <param name="endPoint">The end point to associate with.</param>
    void TransformToAssociated (ICollectionEndPoint endPoint);

    /// <summary>
    /// Transforms the collection to a stand-alone collection. The collection will get its own data store and will not be associated with an 
    /// <see cref="ICollectionEndPoint"/> any longer.
    /// This interface is used by  <see cref="CollectionEndPointReplaceWholeCollectionCommand"/> and should usually not be required by framework 
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