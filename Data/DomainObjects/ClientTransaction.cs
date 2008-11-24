/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Mixins;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
/// <summary>
/// Represents an in-memory transaction.
/// </summary>
/// <remarks>
/// <para>
/// When a <see cref="ClientTransaction"/> is manually instantiated, it has to be activated for the current thread by using a
/// <see cref="ClientTransactionScope"/>, e.g. via calling <see cref="EnterDiscardingScope"/> or <see cref="EnterNonDiscardingScope"/>. The current transaction
/// for a thread can be retrieved via <see cref="Current"/> or <see cref="ClientTransactionScope.ActiveScope"/>.
/// </para>
/// <para>
/// <see cref="ClientTransaction">ClientTransaction's</see> methods temporarily set the <see cref="ClientTransactionScope"/> to this instance to
/// ensure they are executed in the right context.
/// </para>
/// </remarks>
[Serializable]
public abstract class ClientTransaction
{
  // types

  // static members and constants

  /// <summary>
  /// Creates a new root <see cref="ClientTransaction"/>, specifically a <see cref="RootClientTransaction"/>.
  /// </summary>
  /// <returns>A new root <see cref="ClientTransaction"/> instance.</returns>
  /// <remarks>The object returned by this method can be extended with <b>Mixins</b> by configuring the <see cref="MixinConfiguration.ActiveConfiguration"/>
  /// to include a mixin for type <see cref="RootClientTransaction"/>. Declaratively, this can be achieved by attaching an
  /// <see cref="ExtendsAttribute"/> instance for <see cref="ClientTransaction"/> or <see cref="RootClientTransaction"/> to a mixin class.</remarks>
  public static ClientTransaction CreateRootTransaction ()
  {
    return ObjectFactory.Create<RootClientTransaction>().With();
  }

  /// <summary>
  /// Creates a new root <see cref="ClientTransaction"/> that binds all <see cref="DomainObject"/> instances that are created in its context. A bound
  /// <see cref="DomainObject"/> is always accessed in the context of its binding transaction, it never uses <see cref="Current"/>.
  /// </summary>
  /// <returns>A new binding <see cref="ClientTransaction"/> instance.</returns>
  /// <remarks>
  /// <para>
  /// The object returned by this method can be extended with <b>Mixins</b> by configuring the <see cref="MixinConfiguration.ActiveConfiguration"/>
  /// to include a mixin for type <see cref="RootClientTransaction"/>. Declaratively, this can be achieved by attaching an
  /// <see cref="ExtendsAttribute"/> instance for <see cref="ClientTransaction"/> or <see cref="RootClientTransaction"/> to a mixin class.
  /// </para>
  /// <para>
  /// Binding transactions cannot have subtransactions.
  /// </para>
  /// </remarks>
  public static ClientTransaction CreateBindingTransaction ()
  {
    return ObjectFactory.Create<BindingClientTransaction> ().With ();
  }

  /// <summary>
  /// Gets the <see cref="ClientTransaction"/> currently associated with this thread, or <see langword="null"/> if no such transaction exists.
  /// </summary>
  /// <value>The current <see cref="ClientTransaction"/> for the active thread, or <see langword="null"/> if no transaction is associated with it.</value>
  /// <remarks>This method is a shortcut for calling <see cref="ClientTransactionScope.CurrentTransaction"/>, but it doesn't throw an exception but
  /// return <see langword="null"/> if no transaction exists for the current thread.
  /// </remarks>
  public static ClientTransaction Current
  {
    get { return ClientTransactionScope.HasCurrentTransaction ? ClientTransactionScope.CurrentTransaction : null; }
  }

  // member fields

  /// <summary>
  /// Occurs when the <b>ClientTransaction</b> has created a subtransaction.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event SubTransactionCreatedEventHandler SubTransactionCreated;

  /// <summary>
  /// Occurs after the <b>ClientTransaction</b> has loaded a new object.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event ClientTransactionEventHandler Loaded;

  /// <summary>
  /// Occurs immediately before the <b>ClientTransaction</b> performs a <see cref="Commit"/> operation.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event ClientTransactionEventHandler Committing;

  /// <summary>
  /// Occurs immediately after the <b>ClientTransaction</b> has successfully performed a <see cref="Commit"/> operation.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event ClientTransactionEventHandler Committed;

  /// <summary>
  /// Occurs immediately before the <b>ClientTransaction</b> performs a <see cref="Rollback"/> operation.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event ClientTransactionEventHandler RollingBack;

  /// <summary>
  /// Occurs immediately after the <b>ClientTransaction</b> has successfully performed a <see cref="Rollback"/> operation.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event ClientTransactionEventHandler RolledBack;

  private readonly DataManager _dataManager;
  private readonly Dictionary<Enum, object> _applicationData;
  private readonly CompoundClientTransactionListener _listeners;
  private readonly ClientTransactionExtensionCollection _extensions;

  private bool _isDiscarded;
  
  // construction and disposing

  protected ClientTransaction (Dictionary<Enum, object> applicationData, ClientTransactionExtensionCollection extensions)
  {
    ArgumentUtility.CheckNotNull ("applicationData", applicationData);
    ArgumentUtility.CheckNotNull ("extensions", extensions);

    IsReadOnly = false;
    _isDiscarded = false;
    _extensions = extensions;
   
    _listeners = new CompoundClientTransactionListener ();

    _listeners.AddListener (new LoggingClientTransactionListener ());
    _listeners.AddListener (new ReadOnlyClientTransactionListener (this));
    _listeners.AddListener (new ExtensionClientTransactionListener (this, _extensions));

    _applicationData = applicationData;
    _dataManager = new DataManager (this);
  }

  // abstract methods and properties

  /// <summary>
  /// Gets the parent transaction for this <see cref="ClientTransaction"/>.
  /// </summary>
  /// <value>The parent transaction.</value>
  public abstract ClientTransaction ParentTransaction { get; }

  /// <summary>
  /// Gets the root transaction of this <see cref="ClientTransaction"/>, i.e. the top-level parent transaction in a row of subtransactions.
  /// </summary>
  /// <value>The root transaction of this <see cref="ClientTransaction"/>.</value>
  /// <remarks>When this transaction is an instance of <see cref="RootClientTransaction"/>, this property returns the transaction itself. If it
  /// is an instance of <see cref="SubClientTransaction"/>, it returns the parent's root transaction. </remarks>
  public abstract ClientTransaction RootTransaction { get; }

  /// <summary>Initializes a new instance of this transaction.</summary>
  public abstract ClientTransaction CreateEmptyTransactionOfSameType ();

  /// <summary>
  /// Enlists the given domain object in the current transaction.
  /// </summary>
  /// <param name="domainObject">The domain object to be enlisted.</param>
  /// <returns>True if the object was newly enlisted; false if it had already been enlisted before this method was called.</returns>
  /// <exception cref="InvalidOperationException">Another <see cref="DomainObject"/> instance with the same <see cref="ObjectID"/> already exists
  /// in the transaction.</exception>
  /// <exception cref="ArgumentNullException">The <paramref name="domainObject"/> parameter is <see langword="null"/>.</exception>
  /// <remarks>
  /// Implementers of this method should add the given <paramref name="domainObject"/> to the data structure holding all <see cref="DomainObject"/>
  /// instances currently registered in this transaction unless the <paramref name="domainObject"/> has already been enlisted. From within this
  /// method, the object's <see cref="DataContainer"/> must not be accessed (directly or indirectly).
  /// </remarks>
  protected internal abstract bool DoEnlistDomainObject (DomainObject domainObject);

  /// <summary>
  /// Determines whether the specified <paramref name="domainObject"/> is enlisted in this transaction.
  /// </summary>
  /// <param name="domainObject">The domain object to be checked.</param>
  /// <returns>
  /// True if the specified domain object has been enlisted via <see cref="DoEnlistDomainObject"/>; otherwise, false.
  /// </returns>
  protected internal abstract bool IsEnlisted (DomainObject domainObject);

  /// <summary>
  /// Returns the <see cref="DomainObject"/> enlisted for the given <paramref name="objectID"/> in this transaction, or <see langword="null"/> if
  /// none such object exists.
  /// </summary>
  /// <param name="objectID">The <see cref="ObjectID"/> for which to retrueve a <see cref="DomainObject"/>.</param>
  /// <returns>
  /// A <see cref="DomainObject"/> with the given <paramref name="objectID"/> previously enlisted via <see cref="DoEnlistDomainObject"/>,
  /// or <see langword="null"/> if no such object exists.
  /// </returns>
  protected internal abstract DomainObject GetEnlistedDomainObject (ObjectID objectID);

  /// <summary>
  /// Gets all domain objects enlisted in this transaction.
  /// </summary>
  /// <value>The domain objects enlisted in this transaction via <see cref="DoEnlistDomainObject"/>.</value>
  protected internal abstract IEnumerable<DomainObject> EnlistedDomainObjects { get; }

  /// <summary>
  /// Gets the number of domain objects enlisted in this transaction.
  /// </summary>
  /// <value>The number of elements in <see cref="EnlistedDomainObjects"/>.</value>
  protected internal abstract int EnlistedDomainObjectCount { get; }

  /// <summary>
  /// Persists changed data in the couse of a <see cref="Commit"/> operation.
  /// </summary>
  /// <param name="changedDataContainers">The data containers for any object that was changed in this transaction.</param>
  protected abstract void PersistData (DataContainerCollection changedDataContainers);

  /// <summary>
  /// Creates a new <see cref="ObjectID"/> for the given class definition.
  /// </summary>
  /// <param name="classDefinition">The class definition to create a new <see cref="ObjectID"/> for.</param>
  /// <returns></returns>
  protected internal abstract ObjectID CreateNewObjectID (ClassDefinition classDefinition);

  /// <summary>
  /// Loads a data container from the underlying storage or the <see cref="ParentTransaction"/>.
  /// </summary>
  /// <param name="id">The id of the <see cref="DataContainer"/> to load.</param>
  /// <returns>A <see cref="DataContainer"/> with the given <paramref name="id"/>.</returns>
  /// <remarks>
  /// <para>
  /// This method raises the <see cref="IClientTransactionListener.ObjectLoading"/> event on the <see cref="TransactionEventSink"/>, sets the
  /// <see cref="ClientTransaction"/> of the loaded data container, and registers the container in the <see cref="DataContainerMap"/>. It does
  /// not raise the <see cref="IClientTransactionListener.ObjectsLoaded"/> event.
  /// </para>
  /// </remarks>
  protected abstract DataContainer LoadDataContainer (ObjectID id);

  /// <summary>
  /// Loads a number of data containers from the underlying storage or the <see cref="ParentTransaction"/>.
  /// </summary>
  /// <param name="objectIDs">The ids of the <see cref="DataContainer"/> objects to load.</param>
  /// <returns>A <see cref="DataContainerCollection"/> with the loaded containers in the same order as in <paramref name="objectIDs"/>.</returns>
  /// <param name="throwOnNotFound">If true, this method should throw an <see cref="ObjectNotFoundException"/> if a data container cannot be found
  /// for an <see cref="ObjectID"/>. If false, the method should proceed as if the invalid ID hadn't been given.</param>
  /// <remarks>
  /// <para>
  /// This method raises the <see cref="IClientTransactionListener.ObjectLoading"/> event on the <see cref="TransactionEventSink"/>, sets the
  /// <see cref="ClientTransaction"/> of the loaded data containers, and registers the containers in the <see cref="DataContainerMap"/>. It does
  /// not raise the <see cref="IClientTransactionListener.ObjectsLoaded"/> event.
  /// </para>
  /// </remarks>
  protected abstract DataContainerCollection LoadDataContainers (IEnumerable<ObjectID> objectIDs, bool throwOnNotFound);

  /// <summary>
  /// Loads a <see cref="DataContainer"/> from the datasource for an existing <see cref="DomainObject"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This method raises the <see cref="IClientTransactionListener.ObjectLoading"/> event on the <see cref="TransactionEventSink"/>, sets the
  /// <see cref="ClientTransaction"/> of the loaded data container, and registers the container in the <see cref="DataContainerMap"/>. It does
  /// not raise the <see cref="IClientTransactionListener.ObjectsLoaded"/> event.
  /// </para>
  /// </remarks>
  /// <param name="domainObject">The <see cref="DomainObject"/> to load the <see cref="DataContainer"/> for. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DataContainer"/> that was loaded.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="domainObject"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  protected internal abstract DataContainer LoadDataContainerForExistingObject (DomainObject domainObject);

  /// <summary>
  /// Loads the related <see cref="DomainObject"/> of a given <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <remarks>
  /// This method raises the <see cref="Loaded"/> event.
  /// </remarks>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> of the end point that should be evaluated.
  /// <paramref name="relationEndPointID"/> must refer to an <see cref="ObjectEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>The related <see cref="DomainObject"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.InvalidCastException"><paramref name="relationEndPointID"/> does not refer to an 
  /// <see cref="DataManagement.ObjectEndPoint"/></exception>
  /// <exception cref="DataManagement.ObjectDeletedException">The related <see cref="DomainObject"/> has been deleted.</exception>
  /// <exception cref="Persistence.PersistenceException">
  ///   The related object could not be loaded, but is mandatory.<br /> -or- <br />
  ///   The relation refers to non-existing object.<br /> -or- <br />
  ///   <paramref name="relationEndPointID"/> does not refer to an <see cref="DataManagement.ObjectEndPoint"/>.
  /// </exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="relationEndPointID"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  protected internal abstract DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID);

  /// <summary>
  /// Loads all related <see cref="DomainObject"/>s of a given <see cref="DataManagement.RelationEndPointID"/>. 
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> of the end point that should be evaluated.
  /// <paramref name="relationEndPointID"/> must refer to a <see cref="CollectionEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing all related <see cref="DomainObject"/>s.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.PersistenceException">
  ///   <paramref name="relationEndPointID"/> does not refer to one-to-many relation.<br /> -or- <br />
  ///   The StorageProvider for the related objects could not be initialized.
  /// </exception>
  protected internal abstract DomainObjectCollection LoadRelatedObjects (RelationEndPointID relationEndPointID);

  /// <summary>
  /// Determines whether a specific collection end point's data has changed with the semantics defined by this transaction.
  /// </summary>
  /// <param name="originalData">The end point to check for changes.</param>
  /// <param name="currentData">The end point to check for changes.</param>
  /// <returns>
  /// True if the collection end point data has changed; otherwise, false.
  /// </returns>
  /// <remarks>
  /// Implementations will usually just compare the <see cref="CollectionEndPoint.OppositeDomainObjects"/> collection with the
  /// <see cref="CollectionEndPoint.OriginalOppositeDomainObjects"/> collection to check for changes. However, implementations can choose whether
  /// to ignore ordering or not for such a check. Ignoring ordering means a collection end point will not be committed if only the ordering of
  /// its opposite objects has changed.
  /// </remarks>
  protected internal abstract bool HasCollectionEndPointDataChanged (DomainObjectCollection currentData, DomainObjectCollection originalData);

  /// <summary>
  /// Gets the <see cref="IQueryManager"/> of the <b>ClientTransaction</b>.
  /// </summary>
  public abstract IQueryManager QueryManager { get; }

  // methods and properties

  /// <summary>
  /// Indicates whether this transaction is set read-only.
  /// </summary>
  /// <value>True if this instance is set read-only; otherwise, false.</value>
  /// <remarks>Transactions are set read-only while there exist open subtransactions for them. A read-only transaction can only be used for
  /// operations that do not cause any change of transaction state. Most reading operations that do not require objects to be loaded
  /// from the data store are safe to be used on read-only transactions, but any method that would cause a state change will throw an exception.
  /// </remarks>
  public bool IsReadOnly { get; protected internal set; }

  /// <summary>
  /// Returns whether this <see cref="ClientTransaction"/> has been discarded. A transaction is discarded when its <see cref="Discard"/> or
  /// <see cref="ITransaction.Release"/> methods are called or when it has been used in a discarding scope.
  /// </summary>
  /// <value>True if this transaction has been discarded.</value>
  public bool IsDiscarded
  {
    get { return _isDiscarded; }
  }

  /// <summary>
  /// Gets the collection of <see cref="IClientTransactionExtension"/>s of this <see cref="ClientTransaction"/> hierarchy.
  /// </summary>
  /// <remarks>
  /// <para>
  ///   Use <see cref="ClientTransactionExtensionCollection.Add"/> and <see cref="ClientTransactionExtensionCollection.Remove"/> 
  ///   to register and unregister an extension.
  /// </para>
  /// <para>
  ///   The order of the extensions in this collection is the order in which they are notified.
  /// </para>
  /// <para>
  /// The collection of extensions is the same for a parent transactions and all of its (direct and indirect) substransactions.
  /// </para>
  /// </remarks>
  public ClientTransactionExtensionCollection Extensions
  {
    get { return _extensions; }
  }

  /// <summary>
  /// Gets the transaction event sink for this transaction.
  /// </summary>
  /// <value>The transaction event sink for this transaction.</value>
  /// <remarks>
  /// Objects such as <see cref="DataManager"/>, changes to which logically represent changes to the transaction, can use the object returned by
  /// this property in order to inform the <see cref="ClientTransaction"/> and its listeners of events.
  /// </remarks>
  internal IClientTransactionListener TransactionEventSink
  {
    get { return _listeners; }
  }

  protected internal void AddListener (IClientTransactionListener listener)
  {
    _listeners.AddListener (listener);
  }

  /// <summary>
  /// Discards this transaction (rendering it unusable) and, if this transaction is a subtransaction, returns control to the parent transaction.
  /// </summary>
  /// <returns>True if control was returned to the parent transaction, false if this transaction has no parent transaction.</returns>
  /// <remarks>
  /// <para>
  /// When a subtransaction is created via <see cref="CreateSubTransaction"/>, the parent transaction is made read-only and cannot be
  /// used in potentially modifying operations until the subtransaction returns control to the parent transaction by calling this method.
  /// </para>
  /// <para>
  /// Note that this method only affects writeability of the transactions, it does not influence the active <see cref="ClientTransactionScope"/> and
  /// <see cref="ClientTransaction.Current"/> transaction. However, by default, the scope created by <see cref="EnterDiscardingScope"/> will automatically
  /// execute this method when the scope is left (see <see cref="AutoRollbackBehavior.Discard"/>). In most cases,
  /// <see cref="Discard"/> therefore doesn't have to be called explicity; leaving the scopes suffices.
  /// </para>
  /// <para>
  /// Use <see cref="EnterNonDiscardingScope"/> instead of <see cref="EnterDiscardingScope"/> to avoid this method being called at the end of a scope.
  /// </para>
  /// </remarks>
  public virtual bool Discard ()
  {
    if (ParentTransaction != null)
      ParentTransaction.IsReadOnly = false;

    _isDiscarded = true;
    AddListener (new InvalidatedTransactionListener ());
    return ParentTransaction != null;
  }

  /// <summary>
  /// Creates a new <see cref="ClientTransactionScope"/> for this transaction and enters it, making it the
  /// <see cref="ClientTransactionScope.ActiveScope"/> for the current thread. When the scope is left, <see cref="Discard"/> is executed. This will
  /// discard this transaction and make the parent transaction (if any) writeable again.
  /// </summary>
  /// <returns>A new <see cref="ClientTransactionScope"/> for this transaction with an automatic <see cref="AutoRollbackBehavior.Discard"/>
  /// behavior.</returns>
  /// <remarks>
  /// <para>
  /// The created scope will not perform any automatic rollback, but it will return control to the parent transaction at its end if this
  /// transaction is a subtransaction.
  /// </para>
  /// <para>
  /// The new <see cref="ClientTransactionScope"/> stores the previous <see cref="ClientTransactionScope.ActiveScope"/>. When this scope's
  /// <see cref="ClientTransactionScope.Leave"/> method is called or the scope is disposed of, the previous scope is reactivated.
  /// </para>
  /// </remarks>
  public virtual ClientTransactionScope EnterDiscardingScope ()
  {
    return EnterScope (AutoRollbackBehavior.Discard);
  }

  /// <summary>
  /// Creates a new <see cref="ClientTransactionScope"/> for this transaction with the given automatic rollback behavior and enters it,
  /// making it the <see cref="ClientTransactionScope.ActiveScope"/> for the current thread.
  /// </summary>
  /// <returns>A new <see cref="ClientTransactionScope"/> for this transaction.</returns>
  /// <param name="rollbackBehavior">The automatic rollback behavior to be performed when the scope's <see cref="ClientTransactionScope.Leave"/>
  /// method is called.</param>
  /// <remarks>
  /// <para>
  /// The new <see cref="ClientTransactionScope"/> stores the previous <see cref="ClientTransactionScope.ActiveScope"/>. When this scope's
  /// <see cref="ClientTransactionScope.Leave"/> method is called or the scope is disposed of, the previous scope is reactivated.
  /// </para>
  /// </remarks>
  public virtual ClientTransactionScope EnterScope (AutoRollbackBehavior rollbackBehavior)
  {
    return new ClientTransactionScope (this, rollbackBehavior);
  }

  /// <summary>
  /// Creates a new <see cref="ClientTransactionScope"/> for this transaction and enters it, making it the
  /// <see cref="ClientTransactionScope.ActiveScope"/> for the current thread. When the scope is left, this transaction is not discarded and the
  /// parent transaction (if any) is not made writeable.
  /// </summary>
  /// <returns>A new <see cref="ClientTransactionScope"/> for this transaction with no automatic rollback behavior.</returns>
  /// <remarks>
  /// <para>
  /// The created scope will not perform any automatic rollback and it will not return control to the parent transaction at its end if this
  /// transaction is a subtransaction. You must explicitly call <see cref="Discard"/> if you want to continue working with
  /// the parent transaction. This method is useful if you want to temporarily open a scope for a transaction, then open a scope for another
  /// transaction, then open a new scope for the first transaction again. In this case, the first scope must be a non-discarding scope, otherwise the
  /// transaction will be discarded and cannot be used for a second time.
  /// </para>
  /// <para>
  /// The new <see cref="ClientTransactionScope"/> stores the previous <see cref="ClientTransactionScope.ActiveScope"/>. When this scope's
  /// <see cref="ClientTransactionScope.Leave"/> method is called or the scope is disposed of, the previous scope is reactivated.
  /// </para>
  /// </remarks>
  public virtual ClientTransactionScope EnterNonDiscardingScope ()
  {
    return EnterScope (AutoRollbackBehavior.None);
  }

  /// <summary>
  /// Allows the given <see cref="DomainObject"/> to be used in the context of this transaction without needing to explicitly reload it there.
  /// The <see cref="DomainObject"/> should be loadable into this transaction (i.e. it must be present in the underlying data store or the
  /// ParentTransaction), but this is not enforced until first access to the object.
  /// </summary>
  /// <param name="domainObject">The object to be enlisted in this transaction.</param>
  /// <returns>True if the object was newly enlisted; false if it had already been enlisted in this transaction.</returns>
  /// <remarks>
  /// <para>
  /// Unlike <see cref="DomainObject.GetObject{T}(ObjectID)"/>, this method does not create a new <see cref="DomainObject"/> reference if the object
  /// hasn't been loaded yet, but instead
  /// marks the given <see cref="DomainObject"/> for use in this transaction. After this, the same object reference can be used in both the
  /// transaction it was originally created in and the transactions it has been enlisted in.
  /// </para>
  /// <para>
  /// Using a <see cref="DomainObject"/> in two different transactions at the same time will result in its <see cref="DomainObject.Properties"/>
  /// differing depending on which transaction is currently active.
  /// For example, if a property is changed (and even committed) in transaction A and the object
  /// has been enlisted in transaction B before transaction's A commit, transaction B will never see the changes committed by transaction A.
  /// </para>
  /// <para>
  /// If a certain <see cref="ObjectID"/> has already been associated with a certain <see cref="DomainObject"/> in this transaction, it is not
  /// possible to register another <see cref="DomainObject"/> reference with the same <see cref="DomainObject.ID"/>.
  /// </para>
  /// <para>The data for the <see cref="DomainObject"/> is not loaded immediately by this method, but will be retrieved when the object is first
  /// used in this transaction. If the object has been deleted from the underlying database, access to such an object will result in an
  /// <see cref="ObjectNotFoundException"/>.</para>
  /// </remarks>
  /// <exception cref="InvalidOperationException">The domain object cannot be enlisted, because another <see cref="DomainObject"/> with the same
  /// <see cref="ObjectID"/> has already been associated with this transaction.</exception>
  /// <exception cref="ArgumentNullException">The <paramref name="domainObject"/> parameter is <see langword="null"/>.</exception>
  public bool EnlistDomainObject (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    if (domainObject.IsBoundToSpecificTransaction && domainObject.ClientTransaction != this)
    {
      string message = string.Format ("Cannot enlist the domain object '{0}' in this transaction, because it is already bound to another transaction.",
          domainObject.ID);
      throw new InvalidOperationException (message);
    }
    return DoEnlistDomainObject (domainObject);
  }

  /// <summary>
  /// Calls <see cref="EnlistDomainObject"/> for each <see cref="DomainObject"/> reference currently enlisted with the given
  /// <paramref name="sourceTransaction"/>.
  /// </summary>
  /// <param name="sourceTransaction">The source transaction.</param>
  /// <param name="copyCollectionEventHandlers">If true, <see cref="CopyCollectionEventHandlers"/> will be used to copy any event handlers registered
  /// with <see cref="DomainObjectCollection"/> properties of the objects being enlisted. Events are only copied for objects
  /// that are newly enlisted by this method. Note that setting this parameter to true causes the enlisted objects to be loaded (if they exist);
  /// otherwise they will only be loaded on first access.</param>
  /// <exception cref="ArgumentNullException">The <paramref name="sourceTransaction"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="InvalidOperationException">A domain object cannot be enlisted, because another <see cref="DomainObject"/> with the same
  /// <see cref="ObjectID"/> has already been associated with this transaction.</exception>
  /// <remarks>
  /// This method also enlists objects that do not exist in the database; accessing such an object in the context of this transaction will
  /// result in an <see cref="ObjectNotFoundException"/>.</remarks>
  public void EnlistSameDomainObjects (ClientTransaction sourceTransaction, bool copyCollectionEventHandlers)
  {
    ArgumentUtility.CheckNotNull ("sourceTransaction", sourceTransaction);
    var enlistedObjects = new Set<DomainObject> ();

    foreach (DomainObject domainObject in sourceTransaction.EnlistedDomainObjects)
    {
      bool enlisted = EnlistDomainObject (domainObject);
      if (enlisted || IsEnlisted (domainObject))
        enlistedObjects.Add (domainObject);
    }

    if (copyCollectionEventHandlers)
    {
      foreach (DomainObject domainObject in enlistedObjects)
      {
        try
        {
          CopyCollectionEventHandlers (domainObject, sourceTransaction);
        }
        catch (ObjectNotFoundException)
        {
          // ignore
        }
        catch (ObjectDiscardedException)
        {
          // ignore
        }
      }
    }
  }

  /// <summary>
  /// Calls <see cref="EnlistDomainObject"/> for each <see cref="DomainObject"/> reference in the given collection.
  /// </summary>
  /// <param name="domainObjects">The domain objects to enlist.</param>
  /// <exception cref="ArgumentNullException">The <paramref name="domainObjects"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="InvalidOperationException">A domain object cannot be enlisted, because another <see cref="DomainObject"/> with the same
  /// <see cref="ObjectID"/> has already been associated with this transaction.</exception>
  /// <remarks>This method also enlists objects that do not exist in the database; accessing such an object in the context of this transaction will
  /// result in an <see cref="ObjectNotFoundException"/>.</remarks>
  public void EnlistDomainObjects (IEnumerable<DomainObject> domainObjects)
  {
    ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

    foreach (DomainObject domainObject in domainObjects)
      EnlistDomainObject (domainObject);
  }

  /// <summary>
  /// Calls <see cref="EnlistDomainObject"/> for each <see cref="DomainObject"/> reference in the given collection.
  /// </summary>
  /// <param name="domainObjects">The domain objects to enlist.</param>
  /// <exception cref="ArgumentNullException">The <paramref name="domainObjects"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="InvalidOperationException">A domain object cannot be enlisted, because another <see cref="DomainObject"/> with the same
  /// <see cref="ObjectID"/> has already been associated with this transaction.</exception>
  /// <remarks>This method also enlists objects that do not exist in the database; accessing such an object in the context of this transaction will
  /// result in an <see cref="ObjectNotFoundException"/>.</remarks>
  public void EnlistDomainObjects (params DomainObject[] domainObjects)
  {
    ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

    EnlistDomainObjects ((IEnumerable<DomainObject>) domainObjects);
  }

  /// <summary>
  /// Copies the event handlers defined on the given <see cref="DomainObject"/>'s collection properties from another transaction to this
  /// transaction.
  /// </summary>
  /// <param name="domainObject">The domain object the event handlers of whose collection properties are to be copied.</param>
  /// <param name="sourceTransaction">The transaction to copy the event handlers from.</param>
  /// <remarks>
  /// When a <see cref="DomainObject"/> instance is used in multiple transactions at the same time, its event handlers are shared across transactions,
  /// because they are registered on the instance itself, not in the context of a transaction. However, the event handlers defined on
  /// <see cref="DomainObjectCollection"/> properties of the <see cref="DomainObject"/> are not shared, because each collection instance is unique
  /// to one transaction. To avoid having to manually re-register all such event handlers in all transactions after calling
  /// <see cref="EnlistDomainObject"/>, this method copies all collection event handlers from a source transaction to this transaction.
  /// </remarks>
  /// <exception cref="ObjectNotFoundException">The <paramref name="domainObject"/> could not be found in either the current transaction or the
  /// <paramref name="sourceTransaction"/>.</exception>
  /// <exception cref="ObjectDiscardedException">The <paramref name="domainObject"/> was discarded in either the current transaction or the
  /// <paramref name="sourceTransaction"/>.</exception>
  public void CopyCollectionEventHandlers (DomainObject domainObject, ClientTransaction sourceTransaction)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    ArgumentUtility.CheckNotNull ("sourceTransaction", sourceTransaction);

    foreach (PropertyAccessor property in domainObject.Properties.AsEnumerable (sourceTransaction))
    {
      if (property.PropertyData.Kind == PropertyKind.RelatedObjectCollection)
      {
        // access source property via RelationEndPointMap, we don't want to load any objects and we don't want to raise any events
        var endPointID = new RelationEndPointID (domainObject.ID, property.PropertyData.RelationEndPointDefinition);
        var sourceEndPoint = (CollectionEndPoint) sourceTransaction.DataManager.RelationEndPointMap[endPointID];
        if (sourceEndPoint != null)
        {
          DomainObjectCollection sourceRelatedObjectCollection = sourceEndPoint.OppositeDomainObjects;
          DomainObjectCollection destinationRelatedObjectCollection = DataManager.RelationEndPointMap.GetRelatedObjects (endPointID);
          destinationRelatedObjectCollection.CopyEventHandlersFrom (sourceRelatedObjectCollection);
        }
      }
    }
  }

  /// <summary>
  /// Copies the event handlers defined on the given <see cref="ClientTransaction"/> to this transaction.
  /// </summary>
  /// <param name="sourceTransaction">The transaction to copy the event handlers from.</param>
  public void CopyTransactionEventHandlers (ClientTransaction sourceTransaction)
  {
    ArgumentUtility.CheckNotNull ("sourceTransaction", sourceTransaction);

    Committed += sourceTransaction.Committed;
    Committing += sourceTransaction.Committing;
    Loaded += sourceTransaction.Loaded;
    RolledBack += sourceTransaction.RolledBack;
    RollingBack += sourceTransaction.RollingBack;
    SubTransactionCreated += sourceTransaction.SubTransactionCreated;
  }

  /// <summary>
  /// Initializes a new subtransaction with this <see cref="ClientTransaction"/> as its <see cref="ParentTransaction"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When a subtransaction is created, the parent transaction is automatically made read-only and cannot be modified until the subtransaction
  /// returns control to it via <see cref="Discard"/>. <see cref="Discard"/> is automatically called when a
  /// scope created by <see cref="EnterDiscardingScope"/> is left.
  /// </para>
  /// </remarks>
  public virtual ClientTransaction CreateSubTransaction ()
  {
    ClientTransaction subTransaction = ObjectFactory.Create<SubClientTransaction> ().With (this);
    return subTransaction;
  }

  /// <summary>
  /// Returns whether at least one <see cref="DomainObject"/> in this <b>ClientTransaction</b> has been changed.
  /// </summary>
  /// <returns><see langword="true"/> if at least one <see cref="DomainObject"/> in this <b>ClientTransaction</b> has been changed; otherwise, <see langword="false"/>.</returns>
  public virtual bool HasChanged ()
  {
    using (EnterNonDiscardingScope ())
    {
      DomainObjectCollection changedDomainObjects = _dataManager.GetChangedDomainObjects();
      return changedDomainObjects.Count > 0;
    }
  }

  /// <summary>
  /// Commits all changes within the <b>ClientTransaction</b> to the persistent datasources.
  /// </summary>
  /// <exception cref="Persistence.PersistenceException">Changes to objects from multiple storage providers were made.</exception>
  /// <exception cref="Persistence.StorageProviderException">An error occurred while committing the changes to the datasource.</exception>
  public virtual void Commit ()
  {
    using (EnterNonDiscardingScope ())
    {
      BeginCommit();
      DomainObjectCollection changedButNotDeletedDomainObjects = _dataManager.GetDomainObjects (new[] {StateType.Changed, StateType.New});

      DataContainerCollection changedDataContainers = _dataManager.GetChangedDataContainersForCommit();
      PersistData (changedDataContainers);

      _dataManager.Commit();
      EndCommit (changedButNotDeletedDomainObjects);
    }
  }

  /// <summary>
  /// Performs a rollback of all changes within the <b>ClientTransaction</b>.
  /// </summary>
  public virtual void Rollback ()
  {
    using (EnterNonDiscardingScope ())
    {
      BeginRollback();
      DomainObjectCollection changedButNotNewDomainObjects = _dataManager.GetDomainObjects (new[] {StateType.Changed, StateType.Deleted});

      _dataManager.Rollback();

      EndRollback (changedButNotNewDomainObjects);
    }
  }

  /// <summary>
  /// Retrieves a <see cref="DomainObject"/> to be used with the given <paramref name="dataContainer"/>.
  /// </summary>
  /// <param name="dataContainer">The data container for which to retrieve a <see cref="DomainObject"/>.</param>
  /// <returns>The <see cref="DomainObject"/> enlisted with the given <paramref name="dataContainer"/>'s <see cref="ObjectID"/>, or
  /// a newly loaded <see cref="DomainObject"/> if none has been enlisted.</returns>
  protected internal virtual DomainObject GetObjectForDataContainer (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
    DomainObject enlistedObject = GetEnlistedDomainObject (dataContainer.ID);
    if (enlistedObject != null)
      return enlistedObject;
    else
      return RepositoryAccessor.NewObjectFromDataContainer (dataContainer);
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  protected internal virtual DomainObject GetObject (ObjectID id)
  {
    return GetObject (id, false);
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
  /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
  /// <exception cref="DataManagement.ObjectDeletedException"><paramref name="includeDeleted"/> is false and the DomainObject with <paramref name="id"/> has been deleted.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  protected internal virtual DomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    return _dataManager.DataContainerMap.GetObject (id, includeDeleted);
  }

  /// <summary>
  /// Gets a number of objects that are already loaded or attempts to load them from the data source. If an object cannot be found, an exception is
  /// thrown.
  /// </summary>
  /// <typeparam name="T">The type of objects expected to be returned. Specify <see cref="DomainObject"/> if no specific type is expected.</typeparam>
  /// <param name="objectIDs">The IDs of the objects to be retrieved.</param>
  /// <returns>A list of objects of type <typeparamref name="T"/> corresponding to (and in the same order as) the IDs specified in <paramref name="objectIDs"/>.</returns>
  /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="ArgumentTypeException">One of the retrieved objects doesn't fit the specified type <typeparamref name="T"/>.</exception>
  /// <exception cref="ObjectDiscardedException">One of the retrieved objects has already been discarded.</exception>
  /// <exception cref="ObjectDeletedException">One of the retrieved objects has already been deleted.</exception>
  /// <exception cref="BulkLoadException">The data source found one or more errors when loading the objects. The exceptions can be accessed via the
  /// <see cref="BulkLoadException.Exceptions"/> property.</exception>
  public ObjectList<T> GetObjects<T> (params ObjectID[] objectIDs) where T : DomainObject
  {
    ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);
    return GetObjects<T> (objectIDs, true);
  }

  /// <summary>
  /// Gets a number of objects that are already loaded or attempts to load them from the data source. If an object cannot be found, it is simply not
  /// included in the results list.
  /// </summary>
  /// <typeparam name="T">The type of objects expected to be returned. Specify <see cref="DomainObject"/> if no specific type is expected.</typeparam>
  /// <param name="objectIDs">The IDs of the objects to be retrieved.</param>
  /// <returns>A list of objects of type <typeparamref name="T"/> corresponding to (and in the same order as) the IDs specified in <paramref name="objectIDs"/>.</returns>
  /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="ArgumentTypeException">One of the retrieved objects doesn't fit the specified type <typeparamref name="T"/>.</exception>
  /// <exception cref="ObjectDiscardedException">One of the retrieved objects has already been discarded.</exception>
  /// <exception cref="ObjectDeletedException">One of the retrieved objects has already been deleted.</exception>
  /// <exception cref="BulkLoadException">The data source found one or more errors when loading the objects. The exceptions can be accessed via the
  /// <see cref="BulkLoadException.Exceptions"/> property.</exception>
  public ObjectList<T> TryGetObjects<T> (params ObjectID[] objectIDs) where T : DomainObject
  {
    ArgumentUtility.CheckNotNullOrEmpty ("objectIDs", objectIDs);
    return GetObjects<T> (objectIDs, false);
  }

  /// <summary>
  /// Gets a number of objects that are already loaded or attempts to load them from the data source.
  /// </summary>
  /// <typeparam name="T">The type of objects expected to be returned. Specify <see cref="DomainObject"/> if no specific type is expected.</typeparam>
  /// <param name="objectIDs">The IDs of the objects to be retrieved.</param>
  /// <param name="throwOnNotFound">Specifies whether an <see cref="ObjectNotFoundException"/> is raised (and encapsulated in a
  /// <see cref="BulkLoadException"/>) when an object cannot be found in the data source. If this parameter is set to false, such objects are
  /// ignored.</param>
  /// <returns>A list of objects of type <typeparamref name="T"/> corresponding to (and in the same order as) the IDs specified in <paramref name="objectIDs"/>.</returns>
  /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="ArgumentTypeException">One of the retrieved objects doesn't fit the specified type <typeparamref name="T"/>.</exception>
  /// <exception cref="ObjectDiscardedException">One of the retrieved objects has already been discarded.</exception>
  /// <exception cref="ObjectDeletedException">One of the retrieved objects has already been deleted.</exception>
  /// <exception cref="BulkLoadException">The data source found one or more errors when loading the objects. The exceptions can be accessed via the
  /// <see cref="BulkLoadException.Exceptions"/> property.</exception>
  protected internal virtual ObjectList<T> GetObjects<T> (ObjectID[] objectIDs, bool throwOnNotFound) where T : DomainObject
  {
    ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

    using (EnterNonDiscardingScope ())
    {
      var loadedObjects = new DomainObject[objectIDs.Length];
      var idsToBeLoaded = new List<ObjectID> ();

      for (int i = 0; i < objectIDs.Length; i++)
      {
        DomainObject alreadyLoadedObject = DataManager.DataContainerMap.GetObjectWithoutLoading (objectIDs[i], false);
        if (alreadyLoadedObject != null)
          loadedObjects[i] = alreadyLoadedObject;
        else
          idsToBeLoaded.Add (objectIDs[i]);
      }

      if (idsToBeLoaded.Count > 0)
      {
        DataContainerCollection additionalDataContainers = LoadDataContainers (idsToBeLoaded, throwOnNotFound);
        var loadedDomainObjects = new DomainObjectCollection (additionalDataContainers, true);
        OnLoaded (new ClientTransactionEventArgs (loadedDomainObjects));

        for (int i = 0; i < objectIDs.Length; i++)
        {
          if (loadedObjects[i] == null)
          {
            DataContainer dataContainer = additionalDataContainers[objectIDs[i]];
            if (dataContainer != null)
              loadedObjects[i] = dataContainer.DomainObject;
          }
        }
      }

      ObjectList<T> objectList = MakeObjectList<T> (loadedObjects);
      return objectList;
    }
  }

  private ObjectList<T> MakeObjectList<T> (DomainObject[] loadedObjects) where T : DomainObject
  {
    var objectList = new ObjectList<T> ();
    foreach (DomainObject domainObject in loadedObjects)
    {
      if (domainObject != null)
        objectList.Add (domainObject);
    }
    return objectList;
  }

  internal DataContainer CreateNewDataContainer (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (type);

    ObjectID newObjectID = CreateNewObjectID (classDefinition);
    return CreateNewDataContainer (newObjectID);
  }

  internal DataContainer CreateNewDataContainer (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    using (EnterNonDiscardingScope ())
    {
      DataContainer newDataContainer = DataContainer.CreateNew (id);
      RegisterNewDataContainer(newDataContainer);
      return newDataContainer;
    }
  }

  protected internal void RegisterNewDataContainer (DataContainer newDataContainer)
  {
    SetClientTransaction (newDataContainer);
    _dataManager.RegisterNewDataContainer (newDataContainer);
  }

  /// <summary>
  /// Evaluates if any relations of the given <see cref="DomainObject"/> have changed since instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="domainObject">The <see cref="DomainObject"/> to evaluate. Must not be <see langword="null"/>.</param>
  /// <returns><see langword="true"/> if any relations have changed; otherwise, <see langword="false"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
  protected internal virtual bool HasRelationChanged (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    DataContainer dataContainer = domainObject.GetDataContainerForTransaction (this);
    return _dataManager.RelationEndPointMap.HasRelationChanged (dataContainer);
  }

  /// <summary>
  /// Gets the related object of a given <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.ObjectEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the current related object.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="relationEndPointID"/> does not refer to an <see cref="DataManagement.ObjectEndPoint"/></exception>
  protected internal virtual DomainObject GetRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    using (EnterNonDiscardingScope ())
    {
      DomainObject domainObject = GetObject (relationEndPointID.ObjectID, true);

      TransactionEventSink.RelationReading (domainObject, relationEndPointID.PropertyName, ValueAccess.Current);
      DomainObject relatedObject = _dataManager.RelationEndPointMap.GetRelatedObject (relationEndPointID, false);
      TransactionEventSink.RelationRead (domainObject, relationEndPointID.PropertyName, relatedObject, ValueAccess.Current);

      return relatedObject;
    }
  }

  /// <summary>
  /// Gets the original related object of a given <see cref="DataManagement.RelationEndPointID"/> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.ObjectEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the original related object.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="relationEndPointID"/> does not refer to an <see cref="DataManagement.ObjectEndPoint"/></exception>
  protected internal virtual DomainObject GetOriginalRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    using (EnterNonDiscardingScope ())
    {
      DomainObject domainObject = GetObject (relationEndPointID.ObjectID, true);

      TransactionEventSink.RelationReading (domainObject, relationEndPointID.PropertyName, ValueAccess.Original);
      DomainObject relatedObject = _dataManager.RelationEndPointMap.GetOriginalRelatedObject (relationEndPointID);
      TransactionEventSink.RelationRead (domainObject, relationEndPointID.PropertyName, relatedObject, ValueAccess.Original);

      return relatedObject;
    }
  }

  /// <summary>
  /// Gets the related objects of a given <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.CollectionEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing the current related objects.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="relationEndPointID"/> does not refer to a <see cref="DataManagement.CollectionEndPoint"/></exception>
  protected internal virtual DomainObjectCollection GetRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    using (EnterNonDiscardingScope ())
    {
      DomainObject domainObject = GetObject (relationEndPointID.ObjectID, true);

      TransactionEventSink.RelationReading (domainObject, relationEndPointID.PropertyName, ValueAccess.Current);
      DomainObjectCollection relatedObjects = _dataManager.RelationEndPointMap.GetRelatedObjects (relationEndPointID);
      TransactionEventSink.RelationRead (domainObject, relationEndPointID.PropertyName, relatedObjects.Clone (true), ValueAccess.Current);

      return relatedObjects;
    }
  }

  /// <summary>
  /// Gets the original related objects of a given <see cref="DataManagement.RelationEndPointID"/> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> to evaluate. It must refer to a <see cref="DataManagement.CollectionEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing the original related objects.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="relationEndPointID"/> does not refer to a <see cref="DataManagement.CollectionEndPoint"/></exception>
  protected internal virtual DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    using (EnterNonDiscardingScope ())
    {
      DomainObject domainObject = GetObject (relationEndPointID.ObjectID, true);

      TransactionEventSink.RelationReading (domainObject, relationEndPointID.PropertyName, ValueAccess.Original);
      DomainObjectCollection relatedObjects = _dataManager.RelationEndPointMap.GetOriginalRelatedObjects (relationEndPointID);
      TransactionEventSink.RelationRead (domainObject, relationEndPointID.PropertyName, relatedObjects, ValueAccess.Original);

      return relatedObjects;
    }
  }  

  /// <summary>
  /// Sets a relation between two relationEndPoints.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> referring the <see cref="DataManagement.RelationEndPoint"/> that should relate to <paramref name="newRelatedObject"/>. Must not be <see langword="null"/>.</param>
  /// <param name="newRelatedObject">The new <see cref="DomainObject"/> that should be related; <see langword="null"/> indicates that no object should be referenced.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException">
  ///   <paramref name="relationEndPointID"/> does not refer to an <see cref="DataManagement.ObjectEndPoint"/><br /> -or- <br />
  ///   <paramref name="relationEndPointID"/> belongs to a <see cref="DomainObject"/> that has been deleted.<br /> -or- <br />
  ///   <paramref name="newRelatedObject"/> has been deleted.
  /// </exception>
  /// <exception cref="DataManagement.ClientTransactionsDifferException">
  ///   <paramref name="newRelatedObject"/> does belongs to a different <b>ClientTransaction</b>.
  /// </exception>
  protected internal virtual void SetRelatedObject (RelationEndPointID relationEndPointID, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    using (EnterNonDiscardingScope ())
    {
      GetObject (relationEndPointID.ObjectID, true); // ensure that the object is actually loaded before accessing its related objects
      _dataManager.RelationEndPointMap.SetRelatedObject (relationEndPointID, newRelatedObject);
    }
  }
  
  /// <summary>
  /// Deletes a <see cref="DomainObject"/>.
  /// </summary>
  /// <param name="domainObject">The <see cref="DomainObject"/> to delete. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
  /// <exception cref="DataManagement.ClientTransactionsDifferException">
  ///   <paramref name="domainObject"/> belongs to a different <see cref="ClientTransaction"/>. 
  /// </exception>
  protected internal virtual void Delete (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    using (EnterNonDiscardingScope ())
    {
      _dataManager.Delete (domainObject);
    }
  }

  /// <summary>
  /// Loads an object from the datasource.
  /// </summary>
  /// <remarks>
  /// This method raises the <see cref="Loaded"/> event.
  /// </remarks>
  /// <param name="id">An <see cref="ObjectID"/> object indicating which <see cref="DomainObject"/> to load. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> object that was loaded.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  protected internal virtual DomainObject LoadObject (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    using (EnterNonDiscardingScope ())
    {
      DataContainer dataContainer = LoadDataContainer (id);

      var loadedDomainObjects = new DomainObjectCollection (new[] {dataContainer.DomainObject}, true);
      OnLoaded (new ClientTransactionEventArgs (loadedDomainObjects));

      return dataContainer.DomainObject;
    }
  }

  /// <summary>
  /// Loads the data of an existing object from the datasource.
  /// </summary>
  /// <remarks>
  /// This method raises the <see cref="Loaded"/> event.
  /// </remarks>
  /// <param name="domainObject">A <see cref="DomainObject"/> reference indicating the <see cref="DomainObject"/> whose <see cref="DataContainer"/>
  /// to load. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="domainObject"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  protected internal virtual DataContainer LoadExistingObject (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    using (EnterNonDiscardingScope ())
    {
      DataContainer dataContainer = LoadDataContainerForExistingObject (domainObject);

      var loadedDomainObjects = new DomainObjectCollection (new[] { dataContainer.DomainObject }, true);
      OnLoaded (new ClientTransactionEventArgs (loadedDomainObjects));

      return dataContainer;
    }
  }

  /// <summary>
  /// Creates a new <see cref="DomainObjectCollection"/>, registers the <see cref="DataContainer"/>s with this <b>ClientTransaction</b>, discards already loaded <see cref="DataContainer"/>s, raises the <see cref="Loaded"/> event and optionally registers the relation with the specified <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="dataContainers">The newly loaded <see cref="DataContainer"/>s.</param>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> that should be evaluated. <paramref name="relationEndPointID"/> must refer to a <see cref="CollectionEndPoint"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.InvalidCastException">The relation property's type cannot be cast to <see cref="DomainObjectCollection"/>.</exception>
  internal DomainObjectCollection MergeLoadedDomainObjects (
      DataContainerCollection dataContainers, 
      RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    return MergeLoadedDomainObjects (
        dataContainers,
        relationEndPointID.Definition.PropertyType,
        relationEndPointID.OppositeEndPointDefinition.ClassDefinition.ClassType,
        relationEndPointID);
  }

  /// <summary>
  /// Creates a new <see cref="DomainObjectCollection"/> with the specified <paramref name="collectionType"/>, registers the <see cref="DataContainer"/>s with this <b>ClientTransaction</b>, discards already loaded <see cref="DataContainer"/>s and raises the <see cref="Loaded"/> event.
  /// </summary>
  /// <param name="dataContainers">The newly loaded <see cref="DataContainer"/>s.</param>
  /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated.</param>
  /// <param name="requiredItemType">If not <see langword="null"/>, the created collection will only accept items of this type.</param>
  /// <returns>A <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.InvalidCastException"><paramref name="collectionType"/> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
  internal DomainObjectCollection MergeLoadedDomainObjects (
      DataContainerCollection dataContainers, 
      Type collectionType,
      Type requiredItemType)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);
    ArgumentUtility.CheckNotNull ("collectionType", collectionType);
    return MergeLoadedDomainObjects (dataContainers, collectionType, requiredItemType, null);
  }

  /// <summary>
  /// Creates a new <see cref="DomainObjectCollection"/> with the specified <paramref name="collectionType"/>, registers the <see cref="DataContainer"/>s with this <b>ClientTransaction</b>, discards already loaded <see cref="DataContainer"/>s, raises the <see cref="Loaded"/> event and optionally registers the relation with the specified <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="dataContainers">The newly loaded <see cref="DataContainer"/>s.</param>
  /// <param name="collectionType">The <see cref="Type"/> of the new collection that should be instantiated.</param>
  /// <param name="requiredItemType">The permitted <see cref="Type"/> of an item in the <see cref="DomainObjectCollection"/>. If specified only this type or derived types can be added to the <b>DomainObjectCollection</b>.</param>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> that should be evaluated.</param>
  /// <returns>A <see cref="DomainObjectCollection"/>.</returns>
  /// <exception cref="System.InvalidCastException"><paramref name="collectionType"/> cannot be casted to <see cref="DomainObjectCollection"/>.</exception>
  internal DomainObjectCollection MergeLoadedDomainObjects (
      DataContainerCollection dataContainers, 
      Type collectionType,
      Type requiredItemType,
      RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);
    ArgumentUtility.CheckNotNull ("collectionType", collectionType);

    using (EnterNonDiscardingScope ())
    {
      DataContainerCollection newLoadedDataContainers = _dataManager.DataContainerMap.GetNotRegisteredDataContainers (dataContainers);
      NotifyOfLoading (newLoadedDataContainers);
      SetClientTransaction (newLoadedDataContainers);
      _dataManager.RegisterExistingDataContainers (newLoadedDataContainers);

      DomainObjectCollection domainObjects = DomainObjectCollection.Create (
          collectionType, _dataManager.DataContainerMap.MergeWithRegisteredDataContainers (dataContainers), requiredItemType);

      if (relationEndPointID != null)
        _dataManager.RelationEndPointMap.RegisterCollectionEndPoint (relationEndPointID, domainObjects);

      var newLoadedDomainObjects = new DomainObjectCollection (newLoadedDataContainers, true);
      OnLoaded (new ClientTransactionEventArgs (newLoadedDomainObjects));

      return domainObjects;
    }
  }
      
  /// <summary>
  /// Sets the ClientTransaction property of all <see cref="DataContainer"/>s of a given <see cref="DataManagement.DataContainerCollection"/>.
  /// </summary>
  /// <param name="dataContainers">The <see cref="DataContainerCollection"/> with all the <see cref="DataContainer"/> objects that should be set to the <b>ClientTransaction</b>. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="dataContainers"/> is <see langword="null"/>.</exception>
  protected void SetClientTransaction (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);
    foreach (DataContainer dataContainer in dataContainers)
      SetClientTransaction (dataContainer);
  }

  protected void NotifyOfLoading (DataContainerCollection loadedDataContainers)
  {
    foreach (DataContainer dataContainer in loadedDataContainers)
      TransactionEventSink.ObjectLoading (dataContainer.ID);
  }

  /// <summary>
  /// Sets the ClientTransaction property of a given <see cref="DataContainer"/>
  /// </summary>
  /// <param name="dataContainer">The <see cref="DataContainer"/> that should be set to the <b>ClientTransaction</b>. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="dataContainer"/> is <see langword="null"/>.</exception>
  protected void SetClientTransaction (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
    dataContainer.SetClientTransaction (this);
  }

  protected internal void NotifyOfSubTransactionCreating ()
  {
    OnSubTransactionCreating ();
    IsReadOnly = true;
  }

  private void OnSubTransactionCreating ()
  {
    using (EnterNonDiscardingScope ())
    {
      TransactionEventSink.SubTransactionCreating();
    }
  }

  protected internal void NotifyOfSubTransactionCreated (SubClientTransaction subTransaction)
  {
    OnSubTransactionCreated (new SubTransactionCreatedEventArgs (subTransaction));
  }

  protected virtual void OnSubTransactionCreated (SubTransactionCreatedEventArgs args)
  {
    using (EnterNonDiscardingScope ())
    {
      TransactionEventSink.SubTransactionCreated (args.SubTransaction);

      if (SubTransactionCreated != null)
        SubTransactionCreated (this, args);
    }
  }

  /// <summary>
  /// Raises the <see cref="Loaded"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected virtual void OnLoaded (ClientTransactionEventArgs args)
  {
    using (EnterNonDiscardingScope ())
    {
      foreach (DomainObject loadedDomainObject in args.DomainObjects)
        loadedDomainObject.EventManager.EndObjectLoading();

      if (args.DomainObjects.Count != 0)
      {
        TransactionEventSink.ObjectsLoaded (args.DomainObjects);

        if (Loaded != null)
          Loaded (this, args);
      }
    }
  }

  /// <summary>
  /// Raises the <see cref="Committing"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected virtual void OnCommitting (ClientTransactionEventArgs args)
  {
    using (EnterNonDiscardingScope ())
    {
      TransactionEventSink.TransactionCommitting (args.DomainObjects);

      if (Committing != null)
        Committing (this, args);
    }
  }


  /// <summary>
  /// Raises the <see cref="Committed"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected virtual void OnCommitted (ClientTransactionEventArgs args)
  {
    using (EnterNonDiscardingScope ())
    {
      if (Committed != null)
        Committed (this, args);

      TransactionEventSink.TransactionCommitted (args.DomainObjects);
    }
  }

  /// <summary>
  /// Raises the <see cref="RollingBack"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected virtual void OnRollingBack (ClientTransactionEventArgs args)
  {
    using (EnterNonDiscardingScope ())
    {
      TransactionEventSink.TransactionRollingBack (args.DomainObjects);

      if (RollingBack != null)
        RollingBack (this, args);
    }
  }

  /// <summary>
  /// Raises the <see cref="RolledBack"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected virtual void OnRolledBack (ClientTransactionEventArgs args)
  {
    using (EnterNonDiscardingScope ())
    {
      if (RolledBack != null)
        RolledBack (this, args);

      TransactionEventSink.TransactionRolledBack (args.DomainObjects);
    }
  }

  /// <summary>
  /// Gets the <see cref="DataManager"/> of the <b>ClientTransaction</b>.
  /// </summary>
  protected internal DataManager DataManager
  {
    get { return _dataManager; }
  }

  /// <summary>
  /// Gets a <see cref="System.Collections.Generic.Dictionary {TKey, TValue}"/> to store application specific objects 
  /// within the <see cref="ClientTransaction"/> hierarchy.
  /// </summary>
  /// <remarks>
  /// <para>
  /// To store and access values create project specific <see cref="System.Enum"/>(s) which ensure namespace separation of keys in the dictionary.
  /// </para>
  /// <para>
  /// Note that the application data collection is not managed in a transactional way. Also, it is the same for a parent transactions and all of
  /// its (direct and indirect) substransactions.
  /// </para>
  /// </remarks>
  public Dictionary<Enum, object> ApplicationData
  {
    get { return _applicationData; }
  }

  private void BeginCommit ()
  {
    // TODO Doc: ES
    
    // Note regarding to Committing: 
    // Every object raises a Committing event even if another object's Committing event changes the first object's state back to original 
    // during its own Committing event. Because the event order of .NET is not deterministic, this behavior is desired to ensure consistency: 
    // Every object changed during a ClientTransaction raises a Committing event regardless of the Committing event order of specific objects.  
    // But: The same object is not included in the ClientTransaction's Committing event, because this order (DomainObject Committing events are raised
    // before the ClientTransaction Committing events) IS deterministic.
    
    // Note regarding to Committed: 
    // If an object is changed back to its original state during the Committing phase, no Committed event will be raised,
    // because in this case the object won't be committed to the underlying backend (e.g. database).

    var changedDomainObjects = _dataManager.GetChangedDomainObjects ();
    var domainObjectComittingEventRaised = new DomainObjectCollection ();
    var clientTransactionCommittingEventRaised = new DomainObjectCollection ();

    DomainObjectCollection clientTransactionCommittingEventNotRaised;
    do
    {
      DomainObjectCollection domainObjectCommittingEventNotRaised = domainObjectComittingEventRaised.GetItemsNotInCollection (changedDomainObjects);
      while (domainObjectCommittingEventNotRaised.Count > 0)
      {
        foreach (DomainObject domainObject in domainObjectCommittingEventNotRaised)
        {
          if (!domainObject.IsDiscarded)
          {
            domainObject.EventManager.BeginCommit ();

            if (!domainObject.IsDiscarded)
              domainObjectComittingEventRaised.Add (domainObject);
          }
        }

        changedDomainObjects = _dataManager.GetChangedDomainObjects ();
        domainObjectCommittingEventNotRaised = domainObjectComittingEventRaised.GetItemsNotInCollection (changedDomainObjects);
      }

      clientTransactionCommittingEventNotRaised = clientTransactionCommittingEventRaised.GetItemsNotInCollection (changedDomainObjects);
      
      OnCommitting (new ClientTransactionEventArgs (clientTransactionCommittingEventNotRaised.Clone (true)));
      foreach (DomainObject domainObject in clientTransactionCommittingEventNotRaised)
      {
        if (!domainObject.IsDiscarded)
          clientTransactionCommittingEventRaised.Add (domainObject);
      }

      changedDomainObjects = _dataManager.GetChangedDomainObjects ();
      clientTransactionCommittingEventNotRaised = clientTransactionCommittingEventRaised.GetItemsNotInCollection (changedDomainObjects);
    } while (clientTransactionCommittingEventNotRaised.Count > 0);
  }

  private void EndCommit (DomainObjectCollection changedDomainObjects)
  {
    foreach (DomainObject changedDomainObject in changedDomainObjects)
      changedDomainObject.EventManager.EndCommit ();
    
    OnCommitted (new ClientTransactionEventArgs (changedDomainObjects.Clone (true)));
  }

  private void BeginRollback ()
  {
    // TODO Doc: ES

    // Note regarding to RollingBack: 
    // Every object raises a RollingBack event even if another object's RollingBack event changes the first object's state back to original 
    // during its own RollingBack event. Because the event order of .NET is not deterministic, this behavior is desired to ensure consistency: 
    // Every object changed during a ClientTransaction raises a RollingBack event regardless of the RollingBack event order of specific objects.  
    // But: The same object is not included in the ClientTransaction's RollingBack event, because this order (DomainObject RollingBack events are raised
    // before the ClientTransaction RollingBack events) IS deterministic.

    // Note regarding to RolledBack: 
    // If an object is changed back to its original state during the RollingBack phase, no RolledBack event will be raised,
    // because the object actually has never been changed from a ClientTransaction's perspective.

    var changedDomainObjects = _dataManager.GetChangedDomainObjects ();
    var domainObjectRollingBackEventRaised = new DomainObjectCollection ();
    var clientTransactionRollingBackEventRaised = new DomainObjectCollection ();

    DomainObjectCollection clientTransactionRollingBackEventNotRaised;
    do
    {
      DomainObjectCollection domainObjectRollingBackEventNotRaised = domainObjectRollingBackEventRaised.GetItemsNotInCollection (changedDomainObjects);
      while (domainObjectRollingBackEventNotRaised.Count > 0)
      {
        foreach (DomainObject domainObject in domainObjectRollingBackEventNotRaised)
        {
          if (!domainObject.IsDiscarded)
          {
            domainObject.EventManager.BeginRollback ();

            if (!domainObject.IsDiscarded)
              domainObjectRollingBackEventRaised.Add (domainObject);
          }
        }

        changedDomainObjects = _dataManager.GetChangedDomainObjects ();
        domainObjectRollingBackEventNotRaised = domainObjectRollingBackEventRaised.GetItemsNotInCollection (changedDomainObjects);
      }

      clientTransactionRollingBackEventNotRaised = clientTransactionRollingBackEventRaised.GetItemsNotInCollection (changedDomainObjects);

      OnRollingBack (new ClientTransactionEventArgs (clientTransactionRollingBackEventNotRaised.Clone (true)));
      foreach (DomainObject domainObject in clientTransactionRollingBackEventNotRaised)
      {
        if (!domainObject.IsDiscarded)
          clientTransactionRollingBackEventRaised.Add (domainObject);
      }

      changedDomainObjects = _dataManager.GetChangedDomainObjects ();
      clientTransactionRollingBackEventNotRaised = clientTransactionRollingBackEventRaised.GetItemsNotInCollection (changedDomainObjects);
    } while (clientTransactionRollingBackEventNotRaised.Count > 0);
  }

  private void EndRollback (DomainObjectCollection changedDomainObjects)
  {
    foreach (DomainObject changedDomainObject in changedDomainObjects)
      changedDomainObject.EventManager.EndRollback ();

    OnRolledBack (new ClientTransactionEventArgs (changedDomainObjects.Clone (true)));
  }

  public virtual ITransaction ToITransation ()
  {
    return new ClientTransactionWrapper (this);
  }
}
}
