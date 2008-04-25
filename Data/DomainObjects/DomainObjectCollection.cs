using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

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
  ///       Note: If the collection is cleared through the <see cref="Clear"/> method <see cref="OnRemoving"/> 
  ///       and <see cref="OnRemoved"/> are called for every item.
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
  ///       This method is only called on <see cref="DomainObjectCollection"/>s representing the original values 
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
  public class DomainObjectCollection : CommonCollection, ICloneable, IList, ICollection<DomainObject>
  {
    // types

    // static members and constants

    /// <summary>
    /// Creates an empty <see cref="DomainObjectCollection"/> of a given <see cref="Type"/>.
    /// </summary>
    /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated. Must not be <see langword="null"/>.</param>
    /// <returns>The new <see cref="DomainObjectCollection"/>.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="collectionType"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.InvalidCastException"><paramref name="collectionType"/> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
    public static DomainObjectCollection Create (Type collectionType)
    {
      return Create (collectionType, new DataContainerCollection(), null);
    }

    /// <summary>
    /// Creates a <see cref="DomainObjectCollection"/> of a given <see cref="System.Type"/> and sets the <see cref="RequiredItemType"/>.
    /// </summary>
    /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated. Must not be <see langword="null"/>.</param>
    /// <param name="requiredItemType">The permitted <see cref="Type"/> of an item in the <see cref="DomainObjectCollection"/>. If specified only this type or derived types can be added to the <b>DomainObjectCollection</b>.</param>
    /// <returns>The new <see cref="DomainObjectCollection"/>.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="collectionType"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.InvalidCastException"><paramref name="collectionType"/> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
    public static DomainObjectCollection Create (Type collectionType, Type requiredItemType)
    {
      return Create (collectionType, new DataContainerCollection(), requiredItemType);
    }

    /// <summary>
    /// Creates a <see cref="DomainObjectCollection"/> of a given <see cref="System.Type"/> and adds the <see cref="DomainObject"/>s of the given <see cref="DataContainerCollection"/>.
    /// </summary>
    /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated. Must not be <see langword="null"/>.</param>
    /// <param name="dataContainers">The <see cref="DataContainer"/>s of the <see cref="DomainObject"/>s that are added to the collection. Must not be <see langword="null"/>.</param>
    /// <returns>The new <see cref="DomainObjectCollection"/>.</returns>
    /// <exception cref="System.ArgumentNullException">
    ///   <paramref name="collectionType"/> is <see langword="null"/>.<br /> -or- <br />
    ///   <paramref name="dataContainers"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="System.InvalidCastException"><paramref name="collectionType"/> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
    public static DomainObjectCollection Create (Type collectionType, DataContainerCollection dataContainers)
    {
      return Create (collectionType, dataContainers, null);
    }

    /// <summary>
    /// Creates a <see cref="DomainObjectCollection"/> of a given <see cref="System.Type"/> and adds the <see cref="DomainObject"/>s of the given <see cref="DataContainerCollection"/>.
    /// </summary>
    /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated. Must not be <see langword="null"/>.</param>
    /// <param name="dataContainers">The <see cref="DataContainer"/>s of the <see cref="DomainObject"/>s that are added to the collection. Must not be <see langword="null"/>.</param>
    /// <param name="requiredItemType">The permitted <see cref="Type"/> of an item in the <see cref="DomainObjectCollection"/>. If specified only this type or derived types can be added to the <b>DomainObjectCollection</b>.</param>
    /// <returns>The new <see cref="DomainObjectCollection"/>.</returns>
    /// <exception cref="System.ArgumentNullException">
    ///   <paramref name="collectionType"/> is <see langword="null"/>.<br /> -or- <br />
    ///   <paramref name="dataContainers"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="System.InvalidCastException"><paramref name="collectionType"/> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
    public static DomainObjectCollection Create (
        Type collectionType,
        DataContainerCollection dataContainers,
        Type requiredItemType)
    {
      ArgumentUtility.CheckNotNull ("collectionType", collectionType);
      ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

      DomainObjectCollection domainObjects = (DomainObjectCollection) ReflectionUtility.CreateObject (collectionType);
      domainObjects._requiredItemType = requiredItemType;

      foreach (DataContainer dataContainer in dataContainers)
        domainObjects.Add (dataContainer.DomainObject);

      return domainObjects;
    }

    /// <summary>
    /// Compares two instances of <see cref="DomainObjectCollection"/> for equality.
    /// </summary>
    /// <param name="collection1">The first <see cref="DomainObjectCollection"/>.</param>
    /// <param name="collection2">The second <see cref="DomainObjectCollection"/>.</param>
    /// <returns><see langword="true"/> if the collections are equal; otherwise, <see langword="false"/>.</returns>
    public static bool Compare (DomainObjectCollection collection1, DomainObjectCollection collection2)
    {
      if (collection1 == null && collection2 == null)
        return true;
      if (collection1 == null)
        return false;
      if (collection2 == null)
        return false;
      if (collection1.Count != collection2.Count)
        return false;

      for (int i = 0; i < collection1.Count; i++)
      {
        if (collection1[i] != (collection2[i]))
          return false;
      }

      return true;
    }

    /// <summary>
    /// Compares two instances of <see cref="DomainObjectCollection"/> for equality.
    /// </summary>
    /// <param name="collection1">The first <see cref="DomainObjectCollection"/>.</param>
    /// <param name="collection2">The second <see cref="DomainObjectCollection"/>.</param>
    /// <param name="ignoreItemOrder">Indicates whether the compare should ignore the order of the items in the collections for the compare operation.</param>
    /// <returns><see langword="true"/> if the collections are equal; otherwise, <see langword="false"/>.</returns>
    public static bool Compare (DomainObjectCollection collection1, DomainObjectCollection collection2, bool ignoreItemOrder)
    {
      if (!ignoreItemOrder)
        return (Compare (collection1, collection2));

      if (collection1 == null && collection2 == null)
        return true;
      if (collection1 == null)
        return false;
      if (collection2 == null)
        return false;
      if (collection1.Count != collection2.Count)
        return false;

      foreach (DomainObject domainObject in collection1)
      {
        if (!collection2.ContainsObject (domainObject))
          return false;
      }

      return true;
    }

    // member fields

    /// <summary>
    /// Occurs before an object is added to the collection.
    /// </summary>
    /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
    public event DomainObjectCollectionChangeEventHandler Adding;

    /// <summary>
    /// Occurs after an object is added to the collection.
    /// </summary>
    /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
    public event DomainObjectCollectionChangeEventHandler Added;

    /// <summary>
    /// Occurs before an object is removed to the collection.
    /// </summary>
    /// <remarks>
    /// This event is not raised if the <see cref="DomainObject"/> holding the <see cref="DomainObjectCollection"/> is being deleted. 
    /// Either subscribe to the <see cref="DomainObject.Deleting"/> event or override the <see cref="OnDeleting"/> method to implement 
    /// business logic handling this situation.
    /// </remarks>
    /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
    public event DomainObjectCollectionChangeEventHandler Removing;

    /// <summary>
    /// Occurs after an object is removed to the collection.
    /// </summary>
    /// <remarks>
    /// This event is not raised if the <see cref="DomainObject"/> holding the <see cref="DomainObjectCollection"/> has been deleted. 
    /// Either subscribe to the <see cref="DomainObject.Deleted"/> event or override the <see cref="OnDeleted"/> method to implement 
    /// business logic handling this situation.
    /// </remarks>
    /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
    public event DomainObjectCollectionChangeEventHandler Removed;

    private Type _requiredItemType;
    
    // this field will not be serialized via IFlattenedSerializable.SerializeIntoFlatStructure
    [NonSerialized]
    private ICollectionChangeDelegate _changeDelegate = null;

    // construction and disposing

    /// <summary>
    /// Initializes a new <b>DomainObjectCollection</b>.
    /// </summary>
    /// <remarks>A derived class must support this constructor.</remarks>
    public DomainObjectCollection()
        : this ((Type) null)
    {
    }

    /// <summary>
    /// Initializes a new <b>DomainObjectCollection</b> that only takes a certain <see cref="Type"/> as members.
    /// </summary>
    /// <param name="requiredItemType">The <see cref="Type"/> that are required for members.</param>
    public DomainObjectCollection (Type requiredItemType)
    {
      _requiredItemType = requiredItemType;
    }

    // standard constructor for collections
    /// <summary>
    /// Initializes a new <b>DomainObjectCollection</b> as a shallow copy of a given <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <remarks>
    /// The new <b>DomainObjectCollection</b> has the same <see cref="RequiredItemType"/> and the same items as the 
    /// given <paramref name="collection"/>.
    /// </remarks>
    /// <param name="collection">The <see cref="DomainObjectCollection"/> to copy. Must not be <see langword="null"/>.</param>
    /// <param name="makeCollectionReadOnly">Indicates whether the new collection should be read-only.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
    public DomainObjectCollection (DomainObjectCollection collection, bool makeCollectionReadOnly)
        : this ((IEnumerable<DomainObject>) ArgumentUtility.CheckNotNull ("collection", collection), makeCollectionReadOnly)
    {
      Assertion.IsTrue (base.Count == collection.Count);
      _requiredItemType = collection.RequiredItemType;
    }

    /// <summary>
    /// Initializes a new <b>DomainObjectCollection</b> as a shallow copy of a given enumeration of <see cref="DomainObject"/>s.
    /// </summary>
    /// <param name="domainObjects">The <see cref="DomainObject"/>s to copy. Must not be <see langword="null"/>.</param>
    /// <param name="makeCollectionReadOnly">Indicates whether the new collection should be read-only.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObjects"/> is <see langword="null"/>.</exception>
    public DomainObjectCollection (IEnumerable<DomainObject> domainObjects, bool makeCollectionReadOnly)
    {
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      foreach (DomainObject domainObject in domainObjects)
        Add (domainObject);

      this.SetIsReadOnly (makeCollectionReadOnly);
    }

    /// <summary>
    /// Initializes a new <b>DomainObjectCollection</b> as a shallow copy of a <see cref="DataManagement.DataContainerCollection"/>s.
    /// </summary>
    /// <param name="dataContainers">The <see cref="DataManagement.DataContainerCollection"/> to copy. Must not be <see langword="null"/>.</param>
    /// <param name="makeCollectionReadOnly">Indicates whether the new collection should be read-only.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="dataContainers"/> is <see langword="null"/>.</exception>
    public DomainObjectCollection (DataContainerCollection dataContainers, bool makeCollectionReadOnly)
        : this (GetDomainObjectsFromDataContainers (ArgumentUtility.CheckNotNull ("dataContainers", dataContainers)), makeCollectionReadOnly)
    {
      Assertion.IsTrue (base.Count == dataContainers.Count);
    }

    private static IEnumerable<DomainObject> GetDomainObjectsFromDataContainers (DataContainerCollection dataContainers)
    {
      foreach (DataContainer dataContainer in dataContainers)
        yield return dataContainer.DomainObject;
    }

    // methods and properties

    /// <summary>
    /// Adds all items of the given <see cref="DomainObjectCollection"/> to the <b>DomainObjectCollection</b>, that are not already part of it.
    /// </summary>
    /// <remarks>
    /// <para>The method does not modify the given <see cref="DomainObjectCollection"/>.</para>
    /// <para>To check if an item is already part of the <b>DomainObjectCollection</b> its <see cref="DomainObject.ID"/> is used. 
    /// <b>Combine</b> does not check if the item references are identical.
    /// The two <see cref="DomainObjectCollection"/>s could contain items with the same ID, but reference different <see cref="DomainObject"/>s, 
    /// if the collections contain items of different <see cref="ClientTransaction"/>s.</para>
    /// </remarks>
    /// <param name="domainObjects">The <see cref="DomainObjectCollection"/> to add items from. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObjects"/> is <see langword="null"/>.</exception>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   <paramref name="domainObjects"/> contains a <see cref="DomainObject"/> that belongs to a <see cref="ClientTransaction"/> that is different from 
    ///   the <see cref="ClientTransaction"/> managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    public void Combine (DomainObjectCollection domainObjects)
    {
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);
      if (IsReadOnly)
        throw new NotSupportedException ("A read-only collection cannot be combined with another collection.");

      foreach (DomainObject domainObject in domainObjects)
      {
        if (!Contains (domainObject.ID))
          Add (domainObject);
      }

      Touch ();
    }

    /// <summary>
    /// Returns all items of a given <see cref="DomainObjectCollection"/> that are not part of the <b>DomainObjectCollection</b>.
    /// </summary>
    /// <remarks>
    /// <para>The method does not modify the given <see cref="DomainObjectCollection"/> nor the <b>DomainObjectCollection</b> itself.</para>
    /// <para>To check if an item is already part of the <b>DomainObjectCollection</b> its <see cref="DomainObject.ID"/> 
    /// and the item reference are considered. 
    /// Therefore <b>GetItemsNotInCollection</b> does return items with the same ID but are from different <see cref="ClientTransaction"/>s.</para>
    /// </remarks>
    /// <param name="domainObjects">The collection to evaluate. Must not be <see langword="null"/>.</param>
    /// <returns>A <see cref="DomainObjectCollection"/> with all items of <paramref name="domainObjects"/> that are not part of the collection.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObjects"/> is <see langword="null"/>.</exception>
    public DomainObjectCollection GetItemsNotInCollection (DomainObjectCollection domainObjects)
    {
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      DomainObjectCollection itemsNotInCollection = new DomainObjectCollection();

      foreach (DomainObject domainObject in domainObjects)
      {
        if (!ContainsObject (domainObject))
          itemsNotInCollection.Add (domainObject);
      }

      return itemsNotInCollection;
    }

    /// <summary>
    /// Gets the required <see cref="Type"/> for all members of the collection.
    /// </summary>
    public Type RequiredItemType
    {
      get { return _requiredItemType; }
    }

    internal ICollectionChangeDelegate ChangeDelegate
    {
      set { _changeDelegate = value; }
    }

    /// <summary>
    /// Determines whether the <see cref="DomainObjectCollection"/> contains a reference to the specified <paramref name="domainObject"/>.
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> to locate in the <see cref="DomainObjectCollection"/>. Must not be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="domainObject"/> is found in the <see cref="DomainObjectCollection"/>; otherwise, false;</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/></exception>
    /// <remarks>This method only returns true, if the same reference is found in the collection.</remarks>
    [Obsolete ("Use Contains (ObjectID) to determine if the DomainObjectCollection contains a DomainObject with the specified ID or use"
        + " ContainsObject (DomainObject) to determine if the DomainObjectCollection contains a reference to the specified DomainObject."
            + " Note: In most scenarios Contains (ObjectID) should be used.", true)]
    // TODO: Remove this method after 01.06.2006
    public bool Contains (DomainObject domainObject)
    {
      return ContainsObject (domainObject);
    }

    /// <summary>
    /// Determines whether the <see cref="DomainObjectCollection"/> contains a reference to the specified <paramref name="domainObject"/>.
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> to locate in the <see cref="DomainObjectCollection"/>. Must not be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="domainObject"/> is found in the <see cref="DomainObjectCollection"/>; otherwise, false;</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/></exception>
    /// <remarks>
    /// <para>This method only returns true, if the same reference is found in the collection.</para>
    /// <para>Note: The <b>DomainObjectCollection</b> could contain an item with the same ID, 
    /// but reference a different <see cref="DomainObject"/>, 
    /// if the given <paramref name="domainObject" /> is part of a different <see cref="ClientTransaction"/>.</para></remarks>
    public bool ContainsObject (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      return BaseContains (domainObject.ID, domainObject);
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

      return BaseContainsKey (id);
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
      get { return (DomainObject) BaseGetObject (index); }
      set
      {
        CheckIndexForIndexer ("index", index);
        if (IsReadOnly)
          throw new NotSupportedException ("Cannot modify a read-only collection.");

        // If new value is null: This is actually a remove operation
        if (value == null)
        {
          RemoveAt (index);
          return;
        }

        CheckItemType (value, "value");

        // If old and new objects are the same: Perform no operation
        if (ReferenceEquals (this[index], value))
        {
          if (_changeDelegate != null)
            _changeDelegate.PerformSelfReplace (this, value, index);
          else
            Touch ();

          return;
        }
        
        if (Contains (value.ID))
          throw CreateInvalidOperationException ("The object '{0}' is already part of this collection.", value.ID);

        if (_changeDelegate != null)
          _changeDelegate.PerformReplace (this, value, index);
        else
        {
          DomainObject oldObject = this[index];

          BeginRemove (oldObject);
          BeginAdd (value);

          PerformRemove (oldObject);
          PerformInsert (index, value);

          EndRemove (oldObject);
          EndAdd (value);
        }
      }
    }

    /// <summary>
    /// Gets the <see cref="DomainObject"/> with a given <see cref="ObjectID"/> from the <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <remarks>The indexer returns <see langword="null"/> if the given <paramref name="id"/> was not found.</remarks>
    public DomainObject this [ObjectID id]
    {
      get { return (DomainObject) BaseGetObject (id); }
    }

    /// <summary>
    /// Adds a <see cref="DomainObject"/> to the collection.
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> to add. Must not be <see langword="null"/>.</param>
    /// <returns>The zero-based index where the <paramref name="domainObject"/> has been added.</returns>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="domainObject"/> is not of type <see cref="RequiredItemType"/> or one of its derived types.</exception>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   <paramref name="domainObject"/> belongs to a <see cref="ClientTransaction"/> that is different from the <see cref="ClientTransaction"/> 
    ///   managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    public int Add (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      CheckItemType (domainObject, "domainObject");
      if (IsReadOnly)
        throw new NotSupportedException ("Cannot add an item to a read-only collection.");

      if (Contains (domainObject.ID))
      {
        throw CreateArgumentException (
            "domainObject", "Cannot add object '{0}' already part of this collection.", domainObject.ID);
      }

      if (_changeDelegate != null)
        _changeDelegate.PerformInsert (this, domainObject, Count);
      else
      {
        BeginAdd (domainObject);
        PerformAdd (domainObject);
        EndAdd (domainObject);
      }

      return Count - 1;
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
      if (IsReadOnly)
        throw new NotSupportedException ("Cannot remove an item from a read-only collection.");

      DomainObject domainObject = this[id];
      if (domainObject != null)
        Remove (domainObject);
      else
        Touch ();
    }

    /// <summary>
    /// Removes a <see cref="DomainObject"/> from the collection.
    /// </summary>
    /// <returns>True if the collection contained the given object when the method was called; false otherwise.
    /// </returns>
    /// <remarks>
    ///   If <b>Remove</b> is called with an object that is not in the collection, no exception is thrown, and no events are raised. 
    /// </remarks>
    /// <param name="domainObject">The <see cref="DomainObject"/> to remove. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   <paramref name="domainObject"/> belongs to a <see cref="ClientTransaction"/> that is different from the <see cref="ClientTransaction"/> 
    ///   managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    public bool Remove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      if (IsReadOnly)
        throw new NotSupportedException ("Cannot remove an item from a read-only collection.");

      // Do not perform remove if domain object is not part of this collection     
      if (this[domainObject.ID] == null)
      {
        Touch();
        return false;
      }

      if (_changeDelegate != null)
        _changeDelegate.PerformRemove (this, domainObject);
      else
      {
        BeginRemove (domainObject);
        PerformRemove (domainObject);
        EndRemove (domainObject);
      }
      return true;
    }

    /// <summary>
    /// Removes all items from the <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    public void Clear()
    {
      if (IsReadOnly)
        throw new NotSupportedException ("Cannot clear a read-only collection.");

      for (int i = Count - 1; i >= 0; i--)
        Remove (this[i].ID);

      Touch ();
    }

    /// <summary>
    /// Returns the zero-based index of a given <see cref="DomainObject"/> in the collection.
    /// </summary>
    /// <param name="domainObject">The <paramref name="domainObject"/> to locate in the collection.</param>
    /// <returns>The zero-based index of the <paramref name="domainObject"/>, if found; otherwise, -1.</returns>
    public int IndexOf (DomainObject domainObject)
    {
      ObjectID id = null;

      if (domainObject != null)
        id = domainObject.ID;

      return IndexOf (id);
    }

    /// <summary>
    /// Returns the zero-based index of a given <see cref="ObjectID"/> in the collection.
    /// </summary>
    /// <param name="id">The <paramref name="id"/> to locate in the collection.</param>
    /// <returns>The zero-based index of the <paramref name="id"/>, if found; otherwise, -1.</returns>
    public int IndexOf (ObjectID id)
    {
      return BaseIndexOfKey (id);
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
      CheckIndexForInsert ("index", index);
      if (IsReadOnly)
        throw new NotSupportedException ("Cannot insert an item into a read-only collection.");
      CheckItemType (domainObject, "domainObject");

      if (Contains (domainObject.ID))
      {
        throw CreateArgumentException (
            "domainObject", "Cannot insert object '{0}' already part of this collection.", domainObject.ID);
      }

      if (_changeDelegate != null)
        _changeDelegate.PerformInsert (this, domainObject, index);
      else
      {
        BeginAdd (domainObject);
        PerformInsert (index, domainObject);
        EndAdd (domainObject);
      }
    }

    /// <summary>
    /// Gets a value indicating if the collection has a fixed size. Always returns false.
    /// </summary>
    public bool IsFixedSize
    {
      get { return false; }
    }

    #region Explicitly implemeted IList Members

    /// <summary>
    /// Gets or sets the element at the specified index. 
    /// </summary>
    /// <exception cref="Remotion.Utilities.ArgumentTypeException"><paramref name="value"/> is not of type <see cref="DomainObject"/> or a derived type.</exception>
    object IList.this [int index]
    {
      get { return this[index]; }
      set
      {
        ArgumentUtility.CheckType<DomainObject> ("value", value);

        this[index] = (DomainObject) value;
      }
    }

    /// <summary>
    /// Inserts an item to the IList at the specified position
    /// </summary>
    /// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted.</param>
    /// <param name="value">The <see cref="DomainObject"/> to insert into the <see cref="IList"/>. Must not be <see langword="null"/>.</param>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   <paramref name="value"/> is a <see cref="DomainObject"/> that belongs to a <see cref="ClientTransaction"/> that is different 
    ///   from the <see cref="ClientTransaction"/> managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    /// <exception cref="System.ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="Remotion.Utilities.ArgumentTypeException"><paramref name="value"/> is not of type <see cref="DomainObject"/> or a derived type.</exception>
    void IList.Insert (int index, object value)
    {
      ArgumentUtility.CheckNotNullAndType<DomainObject> ("value", value);

      Insert (index, (DomainObject) value);
    }

    /// <summary>
    /// Removes a specific object from the <see cref="IList"/>.
    /// </summary>
    /// <param name="value">The <see cref="Object"/> to remove from the <see cref="IList"/>.</param>
    void IList.Remove (object value)
    {
      if (value is DomainObject)
        Remove ((DomainObject) value);

      if (value is ObjectID)
        Remove ((ObjectID) value);
    }

    /// <summary>
    /// Determines whether the <see cref="IList"/> contains a specific <see cref="DomainObject"/> or <see cref="ObjectID"/>.
    /// </summary>
    /// <param name="value">The <see cref="DomainObject"/> or <see cref="ObjectID"/> to locate in the <see cref="IList"/>.</param>
    /// <returns><see langword="true"/> if the <see cref="DomainObject"/> or <see cref="ObjectID"/> is found in the <see cref="IList"/>; otherwise, <see langword="false"/></returns>
    bool IList.Contains (object value)
    {
      if (value is DomainObject)
        return ContainsObject ((DomainObject) value);

      if (value is ObjectID)
        return Contains ((ObjectID) value);

      return false;
    }

    /// <summary>
    /// Determines the index of a specific item in the <see cref="IList"/>.
    /// </summary>
    /// <param name="value">The <see cref="Object"/> to locate in the <see cref="IList"/>.</param>
    /// <returns>The index of <paramref name="value"/> if found in the list; otherwise, -1.</returns>
    int IList.IndexOf (object value)
    {
      if (value is DomainObject)
        return IndexOf ((DomainObject) value);

      if (value is ObjectID)
        return IndexOf ((ObjectID) value);

      return -1;
    }

    /// <summary>
    /// Adds an item to the <see cref="IList"/>.
    /// </summary>
    /// <param name="value">The <see cref="DomainObject"/> to add to the <see cref="IList"/>. Must not be <see langword="null"/>.</param>
    /// <returns>The position into which the new element was inserted.</returns>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   <paramref name="value"/> is a <see cref="DomainObject"/> that belongs to a <see cref="ClientTransaction"/> that is different 
    ///   from the <see cref="ClientTransaction"/> managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    /// <exception cref="System.ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="Remotion.Utilities.ArgumentTypeException"><paramref name="value"/> is not of type <see cref="DomainObject"/> or a derived type.</exception>
    int IList.Add (object value)
    {
      ArgumentUtility.CheckNotNullAndType<DomainObject> ("value", value);

      return Add ((DomainObject) value);
    }

    #endregion

    #region IEnumerable<DomainObject> members
    public new IEnumerator<DomainObject> GetEnumerator ()
    {
      foreach (DomainObject domainObject in (CommonCollection) this)
        yield return domainObject;
    }

    #endregion

    #region ICloneable Members

    /// <summary>
    /// Creates a shallow copy of this collection.
    /// </summary>
    /// <returns>The cloned collection.</returns>
    /// <remarks>
    /// If this collection is read-only, the clone will be read-only too. 
    /// If this collection is not read-only, the clone will not be read-only too.<br/><br/>
    /// A shallow copy creates a new <see cref="DomainObjectCollection"/> instance
    /// which can be independently modified without affecting the original collection.
    /// Thus meaning the references to the domain objects are copied, not the domain objects themselves.
    /// </remarks>
    object ICloneable.Clone()
    {
      return Clone();
    }

    /// <summary>
    /// Creates a shallow copy of this collection.
    /// </summary>
    /// <returns>The cloned collection.</returns>
    /// <remarks>
    /// If this collection is read-only, the clone will be read-only too. 
    /// If this collection is not read-only, the clone will not be read-only too.<br/><br/>
    /// A shallow copy creates a new <see cref="DomainObjectCollection"/> instance
    /// which can be independently modified without affecting the original collection.
    /// Thus meaning the references to the domain objects are copied, not the domain objects themselves.
    /// </remarks>
    public DomainObjectCollection Clone()
    {
      return Clone (this.IsReadOnly);
    }

    /// <summary>
    /// Creates a shallow copy of this collection. Can be overridden in derived classes.
    /// </summary>
    /// <param name="makeCloneReadOnly">Specifies whether the cloned collection should be read-only.</param>
    /// <returns>The cloned collection.</returns>
    /// <remarks>
    /// A shallow copy creates a new <see cref="DomainObjectCollection"/> instance
    /// which can be independently modified without affecting the original collection.
    /// Thus meaning the references to the domain objects are copied, not the domain objects themselves.<br/><br/>
    /// The <see cref="System.Type"/> of the cloned collection is equal to the <see cref="System.Type"/> of the <b>DomainObjectCollection</b>.
    /// </remarks>
    public virtual DomainObjectCollection Clone (bool makeCloneReadOnly)
    {
      DomainObjectCollection clone = Create (this.GetType());

      clone._requiredItemType = this.RequiredItemType;
      clone.Combine (this);
      clone.SetIsReadOnly (makeCloneReadOnly);

      return clone;
    }

    #endregion

    internal void AssumeSameState (DomainObjectCollection source)
    {
      Assertion.IsTrue (_requiredItemType == source._requiredItemType);

      SetIsReadOnly (false);
      BaseClear ();
      foreach (DomainObject domainObject in source)
        BaseAdd (domainObject.ID, domainObject);
      SetIsReadOnly (source.IsReadOnly);
    }

    internal void TakeOverCommittedData (DomainObjectCollection source)
    {
      Assertion.IsTrue (_requiredItemType == source._requiredItemType);

      SetIsReadOnly (false);
      BaseClear ();
      foreach (DomainObject domainObject in source)
        BaseAdd (domainObject.ID, domainObject);
      SetIsReadOnly (source.IsReadOnly);
    }

    internal void BeginAdd (DomainObject domainObject)
    {
      OnAdding (new DomainObjectCollectionChangeEventArgs (domainObject));
    }

    /// <summary>
    /// Performs a rollback of the collection by replacing the items in the collection with the items of a given <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <param name="originalDomainObjects">A <see cref="DomainObjectCollection"/> containing the original items of the collection. Must not be <see langword="null"/>.</param>
    /// <remarks>
    ///   This method is only called on <see cref="DomainObjectCollection"/>s representing the current values 
    ///   of a one-to-many relation during the rollback operation of the associated <see cref="ClientTransaction"/>. 
    ///   A derived collection should replace its internal state with the state of <paramref name="originalDomainObjects"/>.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException"><paramref name="originalDomainObjects"/> is <see langword="null"/>.</exception>
    protected internal virtual void Rollback (DomainObjectCollection originalDomainObjects)
    {
      ArgumentUtility.CheckNotNull ("originalDomainObjects", originalDomainObjects);

      ReplaceItems (originalDomainObjects);
    }

    /// <summary>
    /// Performs a commit of the collection by replacing the items in the collection with the items of a given <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <param name="domainObjects">A <see cref="DomainObjectCollection"/> containing the new items for the collection. Must not be <see langword="null"/>.</param>
    /// <remarks>
    ///   This method is only called on <see cref="DomainObjectCollection"/>s representing the original values 
    ///   of a one-to-many relation during the commit operation of the associated <see cref="ClientTransaction"/>. 
    ///   A derived collection should replace its internal state with the state of <paramref name="domainObjects"/>.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObjects"/> is <see langword="null"/>.</exception>
    protected internal virtual void Commit (DomainObjectCollection domainObjects)
    {
      ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

      ReplaceItems (domainObjects);
    }

    /// <summary>
    /// Replaces the items in the collection with the items of a given <see cref="DomainObjectCollection"/>.
    /// </summary>
    /// <param name="domainObjects">A <see cref="DomainObjectCollection"/> containing the new items for the collection.</param>
    /// <remarks>
    ///   This method actually performs the replace operation for <see cref="Commit"/> and <see cref="Rollback"/>.
    ///   Note: The replacement raises no events.
    /// </remarks>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   <paramref name="domainObjects"/> contains a <see cref="DomainObject"/> that belongs to a <see cref="ClientTransaction"/> that is different from 
    ///   the <see cref="ClientTransaction"/> managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    protected virtual void ReplaceItems (DomainObjectCollection domainObjects)
    {
      bool isReadOnly = IsReadOnly;

      SetIsReadOnly (false);

      BaseClear();
      foreach (DomainObject domainObject in domainObjects)
        PerformAdd (domainObject);

      SetIsReadOnly (isReadOnly);
      Touch ();
    }

    /// <summary>
    /// Adds a <see cref="DomainObject"/> to the collection without raising the <see cref="Adding"/> and <see cref="Added"/> events.
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> to add to the collection. Must not be <see langword="null"/>.</param>
    /// <returns>The position into which the new <see cref="DomainObject"/> was inserted.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="domainObject"/> is not of type <see cref="RequiredItemType"/> or one of its derived types.</exception>
    /// <exception cref="DataManagement.ClientTransactionsDifferException">
    ///   <paramref name="domainObject"/> belongs to a <see cref="ClientTransaction"/> that is different from the <see cref="ClientTransaction"/> managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    protected internal int PerformAdd (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      if (IsReadOnly)
        throw new NotSupportedException ("Cannot add an item to a read-only collection.");
      CheckItemType (domainObject, "domainObject");

      int index = BaseAdd (domainObject.ID, domainObject);
      Touch ();
      return index;
    }

    /// <summary>
    /// Inserts a <see cref="DomainObject"/> at a given index to the collection without raising the <see cref="Adding"/> and <see cref="Added"/> events.
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
    ///   <paramref name="domainObject"/> belongs to a <see cref="ClientTransaction"/> that is different from the <see cref="ClientTransaction"/> managing this collection. 
    ///   This applies only to <see cref="DomainObjectCollection"/>s that represent a relation.
    /// </exception>
    protected internal void PerformInsert (int index, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      if (IsReadOnly)
        throw new NotSupportedException ("Cannot insert an item into a read-only collection.");
      CheckItemType (domainObject, "domainObject");

      BaseInsert (index, domainObject.ID, domainObject);
      Touch ();
    }

    internal void EndAdd (DomainObject domainObject)
    {
      OnAdded (new DomainObjectCollectionChangeEventArgs (domainObject));
    }

    internal void BeginRemove (DomainObject domainObject)
    {
      OnRemoving (new DomainObjectCollectionChangeEventArgs (domainObject));
    }

    /// <summary>
    /// Removes a <see cref="DomainObject"/> from the collection without raising the <see cref="Removing"/> and <see cref="Removed"/> events.
    /// </summary>
    /// <param name="domainObject">The <see cref="DomainObject"/> to remove from the collection. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    protected internal void PerformRemove (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      if (IsReadOnly)
        throw new NotSupportedException ("Cannot remove an item from a read-only collection.");

      BaseRemove (domainObject.ID);
      Touch ();
    }

    internal void EndRemove (DomainObject domainObject)
    {
      OnRemoved (new DomainObjectCollectionChangeEventArgs (domainObject));
    }

    /// <summary>
    /// Clears the <see cref="DomainObjectCollection"/> without raising the <see cref="Removing"/> and <see cref="Removed"/> events.
    /// </summary>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    internal void PerformDelete()
    {
      if (IsReadOnly)
        throw new NotSupportedException ("Cannot clear a read-only collection.");

      OnDeleting();
      BaseClear();
      Touch ();
      OnDeleted ();
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
    /// The method is invoked immediately before the <see cref="DomainObject"/> holding this collection is deleted if the <b>DomainObjectCollection</b> represents a one-to-many relation.
    /// </summary>
    /// <remarks>
    /// During the delete process of a <see cref="DomainObject"/> all <see cref="DomainObject"/>s are removed from the <b>DomainObjectCollection</b> without notifying other objects.
    /// Immediately before all <see cref="DomainObject"/>s will be removed the <b>OnDeleting</b> method is invoked.
    /// To clear any internal state or to unsubscribe from events whenever the <see cref="DomainObject"/> holding this collection is deleted 
    /// use the <see cref="OnDeleted"/> method, because the operation could be cancelled after the <see cref="OnDeleting"/> method has been called.<br/><br/>
    /// <b>Note:</b> A derived collection overriding this method must not raise an exception.
    /// </remarks>
    protected virtual void OnDeleting()
    {
    }

    /// <summary>
    /// The method is invoked after the <see cref="DomainObject"/> holding this collection is deleted if the <b>DomainObjectCollection</b> represents a one-to-many relation.
    /// </summary>
    /// <remarks>
    /// During the delete process of a <see cref="DomainObject"/> all <see cref="DomainObject"/>s are removed from the <b>DomainObjectCollection</b> without notifying other objects.
    /// After all <see cref="DomainObject"/>s have been removed the <b>OnDeleted</b> method is invoked 
    /// to allow derived collections to adjust their internal state or to unsubscribe from events of contained <see cref="DomainObject"/>s.<br/><br/>
    /// <b>Note:</b> A derived collection overriding this method must not raise an exception.
    /// </remarks>
    protected virtual void OnDeleted()
    {
    }

    private void CheckItemType (DomainObject domainObject, string argumentName)
    {
      if (_requiredItemType != null && !_requiredItemType.IsInstanceOfType (domainObject))
      {
        string message = string.Format ("Values of type '{0}' cannot be added to this collection. Values must be of type '{1}' or derived from '{1}'.",
            domainObject.GetPublicDomainObjectType (), _requiredItemType);
        throw new ArgumentTypeException (message, argumentName, _requiredItemType, domainObject.GetPublicDomainObjectType());
      }
    }

    private InvalidOperationException CreateInvalidOperationException (string message, params object[] args)
    {
      return new InvalidOperationException (string.Format (message, args));
    }

    private ArgumentException CreateArgumentException (string argumentName, string message, params object[] args)
    {
      return new ArgumentException (string.Format (message, args), argumentName);
    }

    internal void CopyEventHandlersFrom (DomainObjectCollection source)
    {
      ArgumentUtility.CheckNotNull ("source", source);
      Added += source.Added;
      Adding += source.Adding;
      Removed += source.Removed;
      Removing += source.Removing;
    }

    private void Touch ()
    {
      if (_changeDelegate != null)
        _changeDelegate.MarkAsTouched ();
    }

    void ICollection<DomainObject>.Add (DomainObject item)
    {
      ArgumentUtility.CheckNotNull ("item", item);
      Add (item);
    }

    void ICollection<DomainObject>.CopyTo (DomainObject[] array, int arrayIndex)
    {
      ArgumentUtility.CheckNotNull ("array", array);
      CopyTo (array, arrayIndex);
    }

    #region Serialization

    public void DeserializeFromFlatStructure (FlattenedDeserializationInfo info)
    {
      _requiredItemType = info.GetValueForHandle<Type>();
      Added += info.GetValue<DomainObjectCollectionChangeEventHandler> ();
      Adding += info.GetValue<DomainObjectCollectionChangeEventHandler> ();
      Removed += info.GetValue<DomainObjectCollectionChangeEventHandler> ();
      Removing += info.GetValue<DomainObjectCollectionChangeEventHandler> ();

      int count = info.GetIntValue();
      for (int i = 0; i < count; ++i)
      {
        ObjectID id = info.GetValueForHandle<ObjectID>();
        DomainObject domainObject = info.GetValueForHandle<DomainObject>();
        BaseAdd (id, domainObject);
      }

      SetIsReadOnly (info.GetBoolValue ());
    }

    public void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_requiredItemType);
      info.AddValue (Added);
      info.AddValue (Adding);
      info.AddValue (Removed);
      info.AddValue (Removing);

      info.AddIntValue (Count);
      for (int i = 0; i < Count; ++i)
      {
        DomainObject domainObject = this[i];
        info.AddHandle (domainObject.ID);
        info.AddHandle (domainObject);
      }

      info.AddBoolValue (IsReadOnly);
    }

    #endregion
  }
}
