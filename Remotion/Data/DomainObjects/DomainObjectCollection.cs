// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Represents a collection of <see cref="DomainObject"/>s.
  /// </summary>
  /// <remarks>
  /// <para>
  /// A derived collection with additional state should override at least the following methods:
  /// <list type="table">
  ///   <listheader>
  ///     <term>Method</term>
  ///     <description>Description</description>
  ///   </listheader>
  ///   <item>
  ///     <term><see cref="OnAdding"/>, <see cref="OnAdded"/></term>
  ///     <description>
  ///       These methods can be used to adjust internal state whenever a new item is added to the collection. 
  ///       The actual adjustment should be performed in the <see cref="OnAdded"/> method, 
  ///       because the operation could be cancelled after the <see cref="OnAdding"/> method has been called.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="OnRemoving"/>, <see cref="OnRemoved"/></term>
  ///     <description>
  ///       These methods can be used to adjust internal state whenever an item is removed from the collection. 
  ///       The actual adjustment should be performed in the <see cref="OnRemoved"/> method, 
  ///       because the operation could be cancelled after the <see cref="OnRemoving"/> method has been called. 
  ///       If the collection is cleared through the <see cref="Clear"/> method, <see cref="OnRemoving"/> is called for each item, then
  ///       the collection data is cleared, then <see cref="OnRemoved"/> is called for every item.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="OnDeleting"/>, <see cref="OnDeleted"/></term>
  ///     <description>
  ///       These methods can be used to clear all internal state or to unsubscribe from events whenever the <see cref="DomainObject"/> 
  ///       holding this collection is deleted. The actual adjustment should be performed in the 
  ///       <see cref="OnDeleted"/> method, because the operation could be cancelled after the <see cref="OnDeleting"/> method has been called. 
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="ReplaceItemsWithoutNotifications"/></term>
  ///     <description>
  ///       This method is automatically called on <see cref="DomainObjectCollection"/>s representing the current values 
  ///       of a one-to-many relation during the rollback operation of the associated <see cref="ClientTransaction"/>. 
  ///       A derived collection should recalculate its internal state to match the new items passed 
  ///       as an argument to this method.
  ///     </description>
  ///   </item>
  /// </list>
  /// </para>
  /// <para>
  /// When a <see cref="DomainObjectCollection"/> is used to model a bidirectional 1:n relation, the contents of the collection is lazily loaded.
  /// This means that the contents is usually not loaded together with the object owning the collection, but later, eg., when the collection's items
  /// are first accessed. In a root transaction, the collection's contents is loaded by executing a query on the underlying data source. This causes
  /// all items in the collection to be registered with the owning <see cref="ClientTransaction"/>, and the collection's contents is then
  /// derived from the items in the <see cref="ClientTransaction"/>.
  /// </para>
  /// <para>
  /// Because the collection's contents is actually derived from the items in the <see cref="ClientTransaction"/>, keep the following in mind:
  /// <list type="bullet">
  /// <item>When an item in the local <see cref="ClientTransaction"/> does not hold a foreign key to the object owning the collection, the item is 
  /// not included in the collection - even if the data source returns that item.</item>
  /// <item>When an item in the local <see cref="ClientTransaction"/> does hold a foreign key to the object owning the collection, the item is 
  /// included - even if the data source does not return that item.</item>
  /// </list>
  /// </para>
  /// <para>
  /// Due to lazy loading, a user of re-store cannot assume that a <see cref="DomainObjectCollection"/>'s contents matches exactly the respective set 
  /// of objects in the data source at any time, unless a database transaction or some similar concept is used to ensure that different read 
  /// operations on the data source always work in the scope of the same database state.
  /// </para>
  /// <para>
  /// If a calculation is made on the contents of a related object collection and it is important that the content does not change until the 
  /// calculated value is committed, then:
  /// <list type="bullet">
  ///   <item>the object owning the collection should be included in the commit set, and</item>
  ///   <item>the domain model should be programmed in such a way that all changes to the collection always cause that object to be included in the 
  ///   respective commit set as well.</item>
  /// </list>
  /// This can be achieved by calling the <see cref="DomainObject.MarkAsChanged"/> method in the respective places. That way, a 
  /// <see cref="ConcurrencyViolationException"/> will be raised on <see cref="ClientTransaction.Commit"/> if the collection changes while the value 
  /// is being calculated.
  /// </para>
  /// <para>
  ///  When, after a bidirectional related objects collection is loaded, an object is loaded into the <see cref="ClientTransaction"/> that also holds 
  ///  a foreign key to the object owning the collection, a conflict arises. Consider the following example (Order - OrderItems):
  ///  <list type="number">
  ///  <item>
  ///    Order1.OrderItems is resolved and causes OrderItem1 and OrderItem2 to be loaded into the ClientTransaction, both holding foreign keys 
  ///    back to Order1. Order1.OrderItems is determined to be the collection [OrderItem1, OrderItem2].
  ///  </item>
  ///  <item>
  ///    Object OrderItem3 is loaded (eg., by ID or via a search query), and it also contains a foreign key back to Order1.
  ///  </item>
  ///  </list>
  ///  This is a conflict because the query in step 1 was supposed to return all OrderItems that hold a foreign key back to Order1, but now an 
  ///  additional item has been found. This is a general problem with lazy loading of objects, and it can occur when the underlying data source 
  ///  changes between step 1 and step 2. It can only be technically prevented by using a database transaction that spans both steps 1 and 2.
  ///  </para>
  ///  <para> 
  ///  The behavior in this scenario is as follows: when an object with a foreign key that is part of an 1:n relationship is loaded into the 
  ///  <see cref="ClientTransaction"/>, the item is automatically added to the respective collection. When that collection has already been 
  ///  "resolved" before, the item will be added at the end of the collection without considering the 
  ///  <see cref="BidirectionalRelationAttribute.SortExpression"/> or raising an <see cref="Adding"/>/<see cref="Added"/> event (or calling the 
  ///  <see cref="OnAdding"/>/<see cref="OnAdded"/> methods) or any relation change events.
  ///  </para>
  ///  <para>
  ///  The reason for not considering the <see cref="BidirectionalRelationAttribute.SortExpression"/> is that the collection might have changed in 
  ///  the meantime, and the original sort order might no longer be valid. Considering the <see cref="BidirectionalRelationAttribute.SortExpression"/>
  ///  would not reliably lead to a state equivalent to that in the underlying data source, although it might give the impression that it would. In 
  ///  addition, transparently inserting objects into collections at arbitrary positions would automatically invalidate any stored (selection) 
  ///  indexes and similar.
  ///  </para>
  ///  <para>
  ///  The reason for not raising the <see cref="Adding"/>/<see cref="Added"/> events is that the semantics of these events (as with the relation 
  ///  change events) is that the collection has been changed, ie., the original state and the current state differ. This is not the case in this 
  ///  scenario; the collection has not changed, the loaded object is part of the original state of the collection.
  ///  </para>
  ///  <para>
  ///  When using collection-valued relations, keep in mind that the <see cref="DomainObjectCollection"/> might change when objects that could 
  ///  potentially be collection items are loaded (by query, ID, or a relation property lookup). Specifically, keep this in mind when iterating over 
  ///  a collection via foreach, and avoid performing unconstrained load operations from within such loops. Iteration using an index into the 
  ///  collection (eg, via for loops) should be able to handle the new items gracefully.
  ///  </para>
  ///  <para>
  ///  Subclasses of <see cref="DomainObjectCollection"/> that keep state additional to that managed by re-store might need to take additional 
  ///  considerations. For example, "indexed collections", where the items keep a property storing their position in the owning collection, now have 
  ///  to deal with loaded objects being registered at the end of the collection without the <see cref="Added"/> event being called. Collections are 
  ///  often implemented in such a way that they recalculate the items' indexes whenever an item is added or removed and when the item owning 
  ///  the collection is committed. The latter might cause such implementations to trigger an <see cref="ConcurrencyViolationException"/> because 
  ///  reindexing will cause the additional item's index to be overwritten with a new value.
  ///  </para>
  ///  <para> 
  ///  To avoid this exception, reindexing on commit should be handled with care. If it cannot be avoided, the algorithm might need to perform 
  ///  additional checks to determine whether a reindexing operation is actually necessary.
  /// </para>
  /// <para>
  /// If a <see cref="DomainObjectCollection"/> is used to model a bidirectional 1:n relation, consider the following about ordering:
  /// <list type="bullet">
  ///   <item>
  ///     When loading the collection from the database (via loading an object in a root transaction), the order of items is defined
  ///     by the sort order of the relation (see <see cref="BidirectionalRelationAttribute.SortExpression"/>). If there is no sort
  ///     order, the order of items is undefined.
  ///   </item>
  ///   <item>
  ///     When committing a root transaction, the order of items in the collection is ignored; the next time the object is loaded
  ///     in another root transaction, the sort order is again defined by the sort order (or undefined). Specifically, if only the
  ///     order of items changed in a 1:n relation, the respective objects will not be written to the database at all, as they will not
  ///     be considered to have changed.
  ///   </item>
  ///   <item>
  ///     When loading the collection from a parent transaction via loading an object in a subtransaction, the order of items is the same
  ///     as in the parent transaction.
  ///   </item>
  ///   <item>
  ///     When committing a subtransaction, the order of items in the collection is propagated to the parent transaction. After the commit,
  ///     the parent transaction's collection will have the items in the same order as the committed subtransaction.
  ///   </item>
  /// </list>
  /// </para>
  /// </remarks>
  [Serializable]
  public partial class DomainObjectCollection : ICloneable, IList, IAssociatableDomainObjectCollection
  {
    /// <summary>
    /// Creates an <see cref="IDomainObjectCollectionData"/> object for stand-alone collections. The returned object takes care of argument checks,
    /// required item checks, and event raising.
    /// </summary>
    /// <param name="dataStore">The data store to use for the collection.</param>
    /// <param name="requiredItemType">The required item type to use for the collection.</param>
    /// <param name="eventRaiser">The event raiser to use for raising events.</param>
    /// <returns>An instance of <see cref="IDomainObjectCollectionData"/> that can be used for stand-alone collections.</returns>
    public static IDomainObjectCollectionData CreateDataStrategyForStandAloneCollection (
        IDomainObjectCollectionData dataStore, 
        Type requiredItemType, 
        IDomainObjectCollectionEventRaiser eventRaiser)
    {
      ArgumentUtility.CheckNotNull ("dataStore", dataStore);
      ArgumentUtility.CheckNotNull ("eventRaiser", eventRaiser);

      return new ModificationCheckingCollectionDataDecorator (requiredItemType, new EventRaisingCollectionDataDecorator (eventRaiser, dataStore));
    }

    /// <summary>
    /// Occurs before an object is added to the collection.
    /// </summary>
    public event DomainObjectCollectionChangeEventHandler Adding;

    /// <summary>
    /// Occurs after an object is added to the collection.
    /// </summary>
    public event DomainObjectCollectionChangeEventHandler Added;

    /// <summary>
    /// Occurs before an object is removed to the collection.
    /// </summary>
    /// <remarks>
    /// This event is not raised if the <see cref="DomainObject"/> holding the <see cref="DomainObjectCollection"/> is being deleted. 
    /// The <see cref="Deleting"/> event is raised instead.
    /// </remarks>
    public event DomainObjectCollectionChangeEventHandler Removing;

    /// <summary>
    /// Occurs after an object is removed to the collection.
    /// </summary>
    /// <remarks>
    /// This event is not raised if the <see cref="DomainObject"/> holding the <see cref="DomainObjectCollection"/> has been deleted. 
    /// The <see cref="Deleted"/> event is raised instead.
    /// </remarks>
    public event DomainObjectCollectionChangeEventHandler Removed;

    /// <summary>
    /// Occurs before the object holding this collection is deleted if this <see cref="DomainObjectCollection"/> represents a one-to-many relation.
    /// </summary>
    public event EventHandler Deleting;

    /// <summary>
    /// Occurs after the object holding this collection is deleted if this <see cref="DomainObjectCollection"/> represents a one-to-many relation.
    /// </summary>
    public event EventHandler Deleted;

    private IDomainObjectCollectionData _dataStrategy; // holds the actual data represented by this collection
    
    /// <summary>
    /// Initializes a new <see cref="DomainObjectCollection" />.
    /// </summary>
    public DomainObjectCollection()
        : this ((Type) null)
    {
    }

    /// <summary>
    /// Initializes a new <see cref="DomainObjectCollection" /> that only takes a certain <see cref="Type"/> as members.
    /// </summary>
    /// <param name="requiredItemType">The <see cref="Type"/> that are required for members.</param>
    public DomainObjectCollection (Type requiredItemType)
    {
      _dataStrategy = CreateDataStrategyForStandAloneCollection (new DomainObjectCollectionData (), requiredItemType, this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainObjectCollection"/> class with a given <see cref="IDomainObjectCollectionData"/>
    /// data storage strategy.
    /// </summary>
    /// <param name="dataStrategy">The <see cref="IDomainObjectCollectionData"/> instance to use as the data storage strategy.</param>
    /// <remarks>
    /// <para>
    /// Derived classes must provide a constructor with the same signature. (The constructor is used for cloning as well as by relation end points.)
    /// </para>
    /// <para>
    /// Most members of <see cref="DomainObjectCollection"/> directly delegate to the given <paramref name="dataStrategy"/>, so it should 
    /// any special argument checks and event raising must be performed by the <paramref name="dataStrategy"/> itself.
    /// </para>
    /// </remarks>
    public DomainObjectCollection (IDomainObjectCollectionData dataStrategy)
    {
      ArgumentUtility.CheckNotNull ("dataStrategy", dataStrategy);

      _dataStrategy = dataStrategy;
    }

    /// <summary>
    /// Initializes a new <see cref="DomainObjectCollection"/> as a shallow copy of a given enumeration of <see cref="DomainObject"/>s.
    /// </summary>
    /// <param name="domainObjects">The <see cref="DomainObject"/>s to copy. Must not be <see langword="null"/>.</param>
    /// <param name="requiredItemType">The required item type of the new collection.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObjects"/> is <see langword="null"/>.</exception>
    public DomainObjectCollection (IEnumerable<DomainObject> domainObjects, Type requiredItemType)
    {
      var dataStore = new DomainObjectCollectionData ();
      dataStore.AddRangeAndCheckItems (domainObjects, requiredItemType);

      _dataStrategy = CreateDataStrategyForStandAloneCollection (dataStore, requiredItemType, this);
    }

    /// <summary>
    /// Gets the number of elements contained in the <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <returns>
    /// The number of elements contained in the <see cref="DomainObjectCollection"/>.
    /// </returns>
    public int Count
    {
      get { return _dataStrategy.Count; }
    }

    /// <summary>
    /// Gets a value indicating whether this collection is read-only.
    /// </summary>
    /// <returns>true if this collection is read-only; otherwise, false.
    /// </returns>
    public bool IsReadOnly
    {
      get { return _dataStrategy.IsReadOnly; }
    }

    /// <summary>
    /// Gets the required <see cref="Type"/> for all elements of the collection. If the collection is read-only, this is <see langword="null" />.
    /// </summary>
    public Type RequiredItemType
    {
      get { return _dataStrategy.RequiredItemType; }
    }

    /// <summary>
    /// Gets the <see cref="ICollectionEndPoint"/> associated with this <see cref="DomainObjectCollection"/>, or <see langword="null" /> if
    /// this is a stand-alone collection.
    /// </summary>
    /// <value>The associated end point.</value>
    public RelationEndPointID AssociatedEndPointID
    {
      get 
      {
        var associatedEndPoint = _dataStrategy.AssociatedEndPoint;
        return associatedEndPoint != null ? associatedEndPoint.ID : null; 
      }
    }

    public bool IsDataComplete
    {
      get { return _dataStrategy.IsDataComplete; }
    }

    /// <summary>
    /// Determines whether this <see cref="DomainObjectCollection"/> instance is associated to the specified <see cref="ICollectionEndPoint"/>.
    /// </summary>
    /// <param name="endPoint">The end point to check for. Pass <see langword="null" /> to check whether this collection is stand-alone.</param>
    /// <returns>
    /// 	<see langword="true"/> if this collection is associated to the specified end point; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsAssociatedWith (ICollectionEndPoint endPoint)
    {
      return _dataStrategy.AssociatedEndPoint == endPoint;
    }

    /// <summary>
    /// If this <see cref="DomainObjectCollection"/> represents a relation end point, ensures that the end point's data has been loaded, loading
    /// the data if necessary. If this <see cref="DomainObjectCollection"/> is a stand-alone collection, this method does nothing.
    /// </summary>
    public void EnsureDataComplete ()
    {
      _dataStrategy.EnsureDataComplete ();
    }

    /// <summary>
    /// Gets an enumerator for iterating over the items in this <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <returns>An enumerator for iterating over the items in this collection.</returns>
    public IEnumerator<DomainObject> GetEnumerator ()
    {
      return _dataStrategy.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }

    /// <summary>
    /// Determines whether the <see cref="DomainObjectCollection"/> contains a reference to the specified <paramref name="domainObject"/>.
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> to locate in the <see cref="DomainObjectCollection"/>. Must not be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="domainObject"/> is found in the <see cref="DomainObjectCollection"/>; otherwise, false;</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/></exception>
    /// <remarks>
    /// <para>This method only returns <see langword="true" /> if the same reference is found in the collection. It returns <see langword="false" /> 
    /// when the collection contains no matching object or another object reference (from another <see cref="ClientTransaction"/>) with the same
    /// <see cref="DomainObject.ID"/>.
    /// </para>
    /// </remarks>
    public bool ContainsObject (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      var existingObject = _dataStrategy.GetObject (domainObject.ID);
      return existingObject != null && ReferenceEquals (existingObject, domainObject);
    }

    /// <summary>
    /// Determines whether an item is in the <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> to locate in the <see cref="DomainObjectCollection"/>. Must not be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the <see cref="DomainObject"/> with the <paramref name="id"/> is found in the <see cref="DomainObjectCollection"/>; otherwise, false;</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/></exception>
    public bool Contains (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      return _dataStrategy.ContainsObjectID (id);
    }

    /// <summary>
    /// Returns the zero-based index of a given <see cref="DomainObject"/> in the collection.
    /// </summary>
    /// <param name="domainObject">The <paramref name="domainObject"/> to locate in the collection.</param>
    /// <returns>The zero-based index of the <paramref name="domainObject"/>, if found; otherwise, -1.</returns>
    /// <remarks>
    /// The method returns -1 if the <paramref name="domainObject"/> is <see langword="null" />. If the collection holds a different item with the
    /// same <see cref="DomainObject.ID"/> as <paramref name="domainObject"/>, -1 is returned as well. Use the 
    /// <see cref="IndexOf(Remotion.Data.DomainObjects.ObjectID)"/> overload taking an <see cref="ObjectID"/> to find the index in such cases.
    /// </remarks>
    public int IndexOf (DomainObject domainObject)
    {
      if (domainObject == null)
        return -1;

      var index = IndexOf (domainObject.ID);
      if (index != -1 && this[index] != domainObject)
        return -1;

      return index;
    }

    /// <summary>
    /// Returns the zero-based index of a given <see cref="ObjectID"/> in the collection.
    /// </summary>
    /// <param name="id">The <paramref name="id"/> to locate in the collection.</param>
    /// <returns>The zero-based index of the <paramref name="id"/>, if found; otherwise, -1.</returns>
    public int IndexOf (ObjectID id)
    {
      if (id != null)
        return _dataStrategy.IndexOf (id);
      else
        return -1;
    }

    /// <summary>
    /// Gets or sets the <see cref="DomainObject"/> with a given <paramref name="index"/> in the <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   <paramref name="index"/> is less than zero.<br /> -or- <br />
    ///   <paramref name="index"/> is equal to or greater than the number of items in the collection.
    /// </exception>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    /// <exception cref="System.ArgumentException">
    ///   <paramref name="value"/> is not a derived type of <see cref="DomainObject"/> and of type <see cref="RequiredItemType"/> or a derived type.
    /// </exception>
    /// <exception cref="System.InvalidOperationException"><paramref name="value"/> is already part of the collection.</exception>
    public DomainObject this [int index]
    {
      get { return _dataStrategy.GetObject (index); }
      set
      {
        this.CheckNotReadOnly ("Cannot modify a read-only collection.");

        // If new value is null: This is actually a remove operation
        if (value == null)
          RemoveAt (index);
        else
          _dataStrategy.Replace (index, value);
      }
    }

    /// <summary>
    /// Gets the <see cref="DomainObject"/> with a given <see cref="ObjectID"/> from the <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <remarks>The indexer returns <see langword="null"/> if the given <paramref name="id"/> was not found.</remarks>
    public DomainObject this [ObjectID id]
    {
      get { return _dataStrategy.GetObject (id); }
    }

    /// <summary>
    /// Adds a <see cref="DomainObject"/> to the collection.
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> to add. Must not be <see langword="null"/>.</param>
    /// <returns>The zero-based index where the <paramref name="domainObject"/> has been added.</returns>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentTypeException"><paramref name="domainObject"/> is not of type <see cref="RequiredItemType"/> or one of its derived types.</exception>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   <paramref name="domainObject"/> belongs to a <see cref="ClientTransaction"/> that is different from the <see cref="ClientTransaction"/> 
    ///   managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    public int Add (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      this.CheckNotReadOnly ("Cannot add an item to a read-only collection.");

      _dataStrategy.Insert (Count, domainObject);
      return Count - 1;
    }

    /// <summary>
    /// Adds a range of <see cref="DomainObject"/> instances to this collection, calling <see cref="Add"/> for each single item.
    /// </summary>
    /// <param name="domainObjects">The domain objects to add.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="domainObjects"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentItemNullException">The range contains a <see langword="null"/> element.</exception>
    /// <exception cref="ArgumentItemDuplicateException">The range contains a duplicate element.</exception>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    /// <exception cref="ArgumentItemTypeException">An element in the range is not of type <see cref="RequiredItemType"/> or one of its derived types.</exception>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   An element in the range belongs to a <see cref="ClientTransaction"/> that is different from the <see cref="ClientTransaction"/> 
    ///   managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    public void AddRange (IEnumerable domainObjects)
    {
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);
      this.CheckNotReadOnly ("Cannot add items to a read-only collection.");

      _dataStrategy.AddRangeAndCheckItems (domainObjects.Cast<DomainObject>(), RequiredItemType);
    }

    /// <summary>
    /// Removes a <see cref="DomainObject"/> from the collection.
    /// </summary>
    /// <param name="index">The index of the <see cref="DomainObject"/> to remove.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   <paramref name="index"/> is less than zero.<br /> -or- <br />
    ///   <paramref name="index"/> is equal to or greater than the number of items in the collection.
    /// </exception>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    public void RemoveAt (int index)
    {
      Remove (this[index]);
    }

    /// <summary>
    /// Removes a <see cref="DomainObject"/> from the collection.
    /// </summary>
    /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> to remove. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    public void Remove (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      this.CheckNotReadOnly ("Cannot remove an item from a read-only collection.");

      _dataStrategy.Remove (id);
    }

    /// <summary>
    /// Removes a <see cref="DomainObject"/> from the collection.
    /// </summary>
    /// <returns>True if the collection contained the given object when the method was called; false otherwise.
    /// </returns>
    /// <remarks>
    ///   If <see cref="Remove(Remotion.Data.DomainObjects.DomainObject)"/> is called with an object that is not in the collection, no exception is thrown, and no events are raised. 
    /// </remarks>
    /// <param name="domainObject">The <see cref="DomainObject"/> to remove. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="domainObject"/> has the same <see cref="ObjectID"/> as an object in this collection, but it is a 
    /// different object reference. You can use <see cref="Remove(ObjectID)"/> to remove an object if you only know its <see cref="ObjectID"/>.</exception>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   <paramref name="domainObject"/> belongs to a <see cref="ClientTransaction"/> that is different from the <see cref="ClientTransaction"/> 
    ///   managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    public bool Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      this.CheckNotReadOnly ("Cannot remove an item from a read-only collection.");

      return _dataStrategy.Remove (domainObject);
    }

    /// <summary>
    /// Removes all items from the <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    public void Clear()
    {
      this.CheckNotReadOnly ("Cannot clear a read-only collection.");

      _dataStrategy.Clear ();
    }

    /// <summary>
    /// Inserts a <see cref="DomainObject"/> into the collection at the specified index.
    /// </summary>
    /// <param name="index">The zero-based <paramref name="index"/> at which the item should be inserted.</param>
    /// <param name="domainObject">The <paramref name="domainObject"/> to add. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   <paramref name="index"/> is less than zero.<br /> -or- <br />
    ///   <paramref name="index"/> is greater than the number of items in the collection.
    /// </exception>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException">
    ///   The <paramref name="domainObject"/> already exists in the collection.<br /> -or- <br />
    ///   <paramref name="domainObject"/> is not of type <see cref="RequiredItemType"/> or one of its derived types.
    /// </exception>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   <paramref name="domainObject"/> belongs to a <see cref="ClientTransaction"/> that is different from the <see cref="ClientTransaction"/> 
    ///   managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    public void Insert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      this.CheckNotReadOnly ("Cannot insert an item into a read-only collection.");

      _dataStrategy.Insert (index, domainObject);
    }
   
    /// <inheritdoc />
    public void CopyTo (Array array, int index)
    {
      _dataStrategy.ToArray ().CopyTo (array, index);
    }

    /// <summary>
    /// Creates a shallow copy of this collection, i.e. a collection of the same type and with the same contents as this collection. 
    /// No <see cref="Adding"/>, <see cref="Added"/>, <see cref="Removing"/>, or <see cref="Removed"/> 
    /// events are raised during the process of cloning.
    /// </summary>
    /// <returns>The cloned collection.</returns>
    /// <remarks>
    /// <para>
    /// The clone is always a stand-alone collection, even when the source was associated with a collection end point.
    /// </para>
    /// <para>
    /// If this collection is read-only, the clone will be read-only, too. Otherwise, the clone will not be read-only.
    /// </para>
    /// <para>
    /// The <see cref="DomainObjectCollection"/> returned by this method contains the same <see cref="DomainObject"/> instances as the original
    /// collection, it does not reflect any changes made to the original collection after cloning, and changes made to it are not reflected upon
    /// the original collection.
    /// </para>
    /// </remarks>
    public DomainObjectCollection Clone ()
    {
      return Clone (IsReadOnly);
    }

    /// <summary>
    /// Creates a shallow copy of this collection, i.e. a collection of the same type and with the same contents as this collection, while allowing
    /// to specify whether the clone should be read-only or not. 
    /// No <see cref="Adding"/>, <see cref="Added"/>, <see cref="Removing"/>, or <see cref="Removed"/> 
    /// events are raised during the process of cloning.
    /// </summary>
    /// <param name="makeCloneReadOnly">Specifies whether the cloned collection should be read-only.</param>
    /// <returns>The cloned collection.</returns>
    /// <remarks>
    /// <para>
    /// The clone is always a stand-alone collection, even when the source was associated with a collection end point.
    /// </para>
    /// <para>
    /// The <see cref="DomainObjectCollection"/> returned by this method contains the same <see cref="DomainObject"/> instances as the original
    /// collection, it does not reflect any changes made to the original collection after cloning, and changes made to it are not reflected upon
    /// the original collection.
    /// </para>
    /// </remarks>
    public virtual DomainObjectCollection Clone (bool makeCloneReadOnly)
    {
      IEnumerable<DomainObject> contents = _dataStrategy;
      if (makeCloneReadOnly)
        return DomainObjectCollectionFactory.Instance.CreateReadOnlyCollection (GetType (), contents);
      else
        return DomainObjectCollectionFactory.Instance.CreateCollection (GetType (), contents, RequiredItemType);
    }

    object ICloneable.Clone ()
    {
      return Clone ();
    }

    /// <summary>
    /// Returns an implementation of <see cref="IDomainObjectCollectionData"/> that represents the data held by this collection but will
    /// not raise any notifications. This means that no events wil be raised when the data is manipulated and no bidirectional notifications will be
    /// performed. The returned object also does not check whether this collection is read-only.
    /// </summary>
    /// <returns>An implementation of <see cref="IDomainObjectCollectionData"/> that represents the data held by this collection and will
    /// not raise any notifications and manipulation.</returns>
    /// <remarks>
    /// <para>
    /// When the collection is part of a
    /// <see cref="CollectionEndPoint"/>, the manipulations performed on the data will not trigger bidirectional modifications on related objects,
    /// so manipulations must be performed with care, otherwise inconsistent state might arise. The end point will also not be marked as touched by 
    /// manipulations performed on the returned data. (The end point's <see cref="CollectionEndPoint.HasChanged"/> method might still return 
    /// <see langword="true" />, though, since it compares the original data with the collection's contents.)
    /// </para>
    /// </remarks>
    protected IDomainObjectCollectionData GetNonNotifyingData ()
    {
      // For associated collections, _dataStrategy.GetDataStore() will usually return the ChangeCachingDomainObjectCollectionData.
      return new ModificationCheckingCollectionDataDecorator (RequiredItemType, _dataStrategy.GetDataStore ());
    }

    /// <summary>
    /// Replaces the items in the collection with a given set of new items.
    /// </summary>
    /// <param name="newItems">The items to be put into the collection. Must not be <see langword="null"/>.</param>
    /// <remarks>
    ///   This method is called for collections associated to a collection end point during the rollback operation of the associated 
    ///   <see cref="ClientTransaction"/>. A derived collection should recalculate its internal state to match the <paramref name="newItems"/>.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException"><paramref name="newItems"/> is <see langword="null"/>.</exception>
    protected internal virtual void ReplaceItemsWithoutNotifications (IEnumerable<DomainObject> newItems)
    {
      ArgumentUtility.CheckNotNull ("newItems", newItems);

      var nonNotifyingData = GetNonNotifyingData ();
      nonNotifyingData.ReplaceContents (newItems);
    }

    /// <summary>
    /// Raises the <see cref="Adding"/> event.
    /// </summary>
    /// <param name="args">A <see cref="DomainObjectCollectionChangeEventArgs"/> object that contains the event data.</param>
    protected virtual void OnAdding (DomainObjectCollectionChangeEventArgs args)
    {
      if (Adding != null)
        Adding (this, args);
    }

    /// <summary>
    /// Raises the <see cref="Added"/> event.
    /// </summary>
    /// <param name="args">A <see cref="DomainObjectCollectionChangeEventArgs"/> object that contains the event data.</param>
    /// <remarks>This method can be used to adjust internal state whenever a new item is added to the collection.</remarks>
    protected virtual void OnAdded (DomainObjectCollectionChangeEventArgs args)
    {
      if (Added != null)
        Added (this, args);
    }

    /// <summary>
    /// Raises the <see cref="Removing"/> event.
    /// </summary>
    /// <param name="args">A <see cref="DomainObjectCollectionChangeEventArgs"/> object that contains the event data.</param>
    ///   If the collection is cleared through the <see cref="Clear"/> method <see cref="OnRemoving"/> 
    ///   is called for every item.
    protected virtual void OnRemoving (DomainObjectCollectionChangeEventArgs args)
    {
      if (Removing != null)
        Removing (this, args);
    }

    /// <summary>
    /// Raises the <see cref="Removed"/> event.
    /// </summary>
    /// <param name="args">A <see cref="DomainObjectCollectionChangeEventArgs"/> object that contains the event data.</param>
    /// <remarks>
    ///   This method can be used to adjust internal state whenever an item is removed from the collection.
    ///   If the collection is cleared through the <see cref="Clear"/> method <see cref="OnRemoved"/> 
    ///   is called for every item.
    /// </remarks>
    protected virtual void OnRemoved (DomainObjectCollectionChangeEventArgs args)
    {
      if (Removed != null)
        Removed (this, args);
    }

    /// <summary>
    /// The method is invoked immediately before the <see cref="DomainObject"/> holding this collection is deleted if the <see cref="DomainObjectCollection" /> represents a one-to-many relation.
    /// </summary>
    /// <remarks>
    /// During the delete process of a <see cref="DomainObject"/> all <see cref="DomainObject"/>s are removed from the <see cref="DomainObjectCollection" /> without notifying other objects.
    /// Before all <see cref="DomainObject"/>s will be removed the <see cref="OnDeleting"/> method is invoked.
    /// To clear any internal state or to unsubscribe from events whenever the <see cref="DomainObject"/> holding this collection is deleted 
    /// use the <see cref="OnDeleted"/> method, because the operation could be cancelled after the <see cref="OnDeleting"/> method has been called.<br/><br/>
    /// <note type="inotes">Inheritors overriding this method must not throw an exception from the override.</note>
    /// </remarks>
    protected virtual void OnDeleting()
    {
      if (Deleting != null)
        Deleting (this, EventArgs.Empty);
    }

    /// <summary>
    /// The method is invoked after the <see cref="DomainObject"/> holding this collection is deleted if the <see cref="DomainObjectCollection" /> represents a one-to-many relation.
    /// </summary>
    /// <remarks>
    /// During the delete process of a <see cref="DomainObject"/> all <see cref="DomainObject"/>s are removed from the <see cref="DomainObjectCollection" /> without notifying other objects.
    /// After all <see cref="DomainObject"/>s have been removed the <see cref="OnDeleted"/> method is invoked 
    /// to allow derived collections to adjust their internal state or to unsubscribe from events of contained <see cref="DomainObject"/>s.<br/><br/>
    /// <note type="inotes">Inheritors overriding this method must not throw an exception from the override.</note>
    /// </remarks>
    protected virtual void OnDeleted()
    {
      if (Deleted != null)
        Deleted (this, EventArgs.Empty);
    }

    internal void CopyEventHandlersFrom (DomainObjectCollection source)
    {
      ArgumentUtility.CheckNotNull ("source", source);

      Adding += source.Adding;
      Added += source.Added;
      Removing += source.Removing;
      Removed += source.Removed;
      Deleting += source.Deleting;
      Deleted += source.Deleted;
    }

    void IAssociatableDomainObjectCollection.TransformToAssociated (ICollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      var endPointDelegatingCollectionData = endPoint.CreateDelegatingCollectionData ();
      endPointDelegatingCollectionData.GetDataStore ().ReplaceContents (_dataStrategy.GetDataStore ()); // copy data

      _dataStrategy = endPointDelegatingCollectionData;
    }

    void IAssociatableDomainObjectCollection.TransformToStandAlone ()
    {
      Assertion.IsFalse (IsAssociatedWith (null));

      var standAloneDataStore = new DomainObjectCollectionData (_dataStrategy.GetDataStore ()); // copy data
      _dataStrategy = CreateDataStrategyForStandAloneCollection (standAloneDataStore, RequiredItemType, this);
    }
  }
}
