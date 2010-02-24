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
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Represents a collection of <see cref="DomainObject"/>s.
  /// </summary>
  /// <remarks>
  /// <para>
  /// If a <see cref="DomainObjectCollection"/> is used to model a bidirectional 1:n relation, consider the following about ordering:
  /// <list type="bullet">
  ///   <item>
  ///     When loading the collection from the database (via loading an object in a root transaction), the order of items is defined
  ///     by the sort order of the relation (see <see cref="DBBidirectionalRelationAttribute.SortExpression"/>). If there is no sort
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
  ///     <term><see cref="Commit"/></term>
  ///     <description>
  ///       This method is only called on <see cref="DomainObjectCollection"/>s representing the values 
  ///       of a one-to-many relation during the commit operation of the associated <see cref="ClientTransaction"/>. 
  ///       A derived collection should replace its internal state with the state of the provided collection passed 
  ///       as an argument to this method.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="Rollback"/></term>
  ///     <description>
  ///       This method is only called on <see cref="DomainObjectCollection"/>s representing the current values 
  ///       of a one-to-many relation during the rollback operation of the associated <see cref="ClientTransaction"/>. 
  ///       A derived collection should replace its internal state with the state of the provided collection passed 
  ///       as an argument to this method.
  ///     </description>
  ///   </item>
  /// </list>
  /// </para>
  /// </remarks>
  [Serializable]
  public partial class DomainObjectCollection : ICloneable, IList
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

      return new ArgumentCheckingCollectionDataDecorator (requiredItemType, new EventRaisingCollectionDataDecorator (eventRaiser, dataStore));
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
    private bool _isReadOnly;
    
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
      get { return _isReadOnly; }
    }

    /// <summary>
    /// Gets the required <see cref="Type"/> for all members of the collection.
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
    public ICollectionEndPoint AssociatedEndPoint
    {
      get { return _dataStrategy.AssociatedEndPoint; }
    }

    public bool IsDataAvailable
    {
      get { return _dataStrategy.IsDataAvailable; }
    }

    /// <summary>
    /// If this <see cref="DomainObjectCollection"/> represents a relation end point, ensures that the end point's data has been loaded, loading
    /// the data if necessary. If this <see cref="DomainObjectCollection"/> is a stand-alone collection, this method does nothing.
    /// </summary>
    public void EnsureDataAvailable ()
    {
      _dataStrategy.EnsureDataAvailable ();
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
      return new ArgumentCheckingCollectionDataDecorator (RequiredItemType, _dataStrategy.GetDataStore());
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
      var clone = DomainObjectCollectionFactory.Instance.CreateCollection (GetType (), contents, RequiredItemType);

      Assertion.IsTrue (clone._dataStrategy != _dataStrategy);
      Assertion.IsTrue (clone.RequiredItemType == RequiredItemType);

      clone.SetIsReadOnly (makeCloneReadOnly);
      return clone;
    }

    object ICloneable.Clone ()
    {
      return Clone ();
    }

    /// <summary>
    /// Creates an <see cref="IDataManagementCommand"/> instance that encapsulates all the modifications required to associate this
    /// <see cref="DomainObjectCollection"/> with the given <paramref name="endPoint"/>. This API is usually not employed by framework users,
    /// but it is invoked when a collection-valued relation property is set to a new collection.
    /// </summary>
    /// <param name="endPoint">The end point to associate with. That end point's <see cref="ICollectionEndPoint.OppositeDomainObjects"/> collection
    /// must have the same type and <see cref="RequiredItemType"/> as this collection.</param>
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
    public IDataManagementCommand CreateAssociationCommand (CollectionEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      if (RequiredItemType != endPoint.OppositeDomainObjects.RequiredItemType)
        throw new InvalidOperationException ("This collection has a different item type than the end point's current opposite collection.");

      if (GetType () != endPoint.OppositeDomainObjects.GetType ())
      {
        var message = string.Format (
            "This collection ('{0}') is not of the same type as the end point's current opposite collection ('{1}').",
            GetType (),
            endPoint.OppositeDomainObjects.GetType ());
        throw new InvalidOperationException (message);
      }

      return new CollectionEndPointReplaceWholeCollectionCommand (
          endPoint,
          this,
          new Transformer (endPoint.OppositeDomainObjects),
          new Transformer (this),
          endPoint.OppositeDomainObjects._dataStrategy.GetDataStore());
    }

    /// <summary>
    /// Returns a read-only <see cref="DomainObjectCollection"/> of the same type as this collection and holding the same data as this 
    /// <see cref="DomainObjectCollection"/>.
    /// The data is not copied; instead, the returned collection holds the same data store as the original collection and will therefore reflect
    /// any changes made to the original.
    /// </summary>
    /// <returns>A read-only <see cref="DomainObjectCollection"/> that holds the same data as this <see cref="DomainObjectCollection"/>.</returns>
    public DomainObjectCollection AsReadOnly ()
    {
      var newCollection = DomainObjectCollectionFactory.Instance.CreateCollection (GetType(), new ReadOnlyCollectionDataDecorator (_dataStrategy));
      newCollection.SetIsReadOnly (true);
      return newCollection;
    }

    /// <summary>
    /// Changes the <see cref="IsReadOnly"/> flag which controls whether this collection is read-only.
    /// </summary>
    /// <param name="isReadOnly">If set to <see langword="true"/>, the collection is made read-only. If set to <see langword="false" />,
    /// the collection is made modifiable.</param>
    protected void SetIsReadOnly (bool isReadOnly)
    {
      _isReadOnly = isReadOnly;
    }

    /// <summary>
    /// Performs a rollback of the collection by replacing the items in the collection with the items of a given <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <param name="originalDomainObjects">A <see cref="DomainObjectCollection"/> containing the original items of the collection. Must not be <see langword="null"/>.</param>
    /// <remarks>
    ///   This method is called for collections associated to a collection end point during the rollback operation of the associated 
    ///   <see cref="ClientTransaction"/>. 
    ///   A derived collection should replace its internal state with the state of <paramref name="originalDomainObjects"/>.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException"><paramref name="originalDomainObjects"/> is <see langword="null"/>.</exception>
    protected internal virtual void Rollback (DomainObjectCollection originalDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("originalDomainObjects", originalDomainObjects);

      var nonNotifyingData = GetNonNotifyingData ();
      nonNotifyingData.ReplaceContents (originalDomainObjects.Cast<DomainObject> ());
    }

    /// <summary>
    /// Performs a commit of the collection by replacing the items in the collection with the items of a given <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <param name="domainObjects">A <see cref="DomainObjectCollection"/> containing the new items for the collection. Must not be <see langword="null"/>.</param>
    /// <remarks>
    ///   This method is called for those collections representing the original values of a collection-valued relation during the commit operation
    ///   of the associated <see cref="ClientTransaction"/>. 
    ///   A derived collection should replace its internal state accordingly.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObjects"/> is <see langword="null"/>.</exception>
    protected internal virtual void Commit (IEnumerable<DomainObject> domainObjects)
    {
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      var nonNotifyingData = GetNonNotifyingData ();
      nonNotifyingData.ReplaceContents (domainObjects.Cast<DomainObject> ());
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
  }
}
