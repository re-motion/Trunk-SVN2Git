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
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;
using Remotion.Data.DomainObjects.Linq;

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
public abstract class ClientTransaction : IDataSource
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
    return ObjectFactory.Create<RootClientTransaction> (true, ParamList.Empty);
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
    return ObjectFactory.Create<BindingClientTransaction> (true, ParamList.Empty);
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
  public event SubTransactionCreatedEventHandler SubTransactionCreated;

  /// <summary>
  /// Occurs after the <b>ClientTransaction</b> has loaded a new object.
  /// </summary>
  public event ClientTransactionEventHandler Loaded;

  /// <summary>
  /// Occurs immediately before the <b>ClientTransaction</b> performs a <see cref="Commit"/> operation.
  /// </summary>
  public event ClientTransactionEventHandler Committing;

  /// <summary>
  /// Occurs immediately after the <b>ClientTransaction</b> has successfully performed a <see cref="Commit"/> operation.
  /// </summary>
  public event ClientTransactionEventHandler Committed;

  /// <summary>
  /// Occurs immediately before the <b>ClientTransaction</b> performs a <see cref="Rollback"/> operation.
  /// </summary>
  public event ClientTransactionEventHandler RollingBack;

  /// <summary>
  /// Occurs immediately after the <b>ClientTransaction</b> has successfully performed a <see cref="Rollback"/> operation.
  /// </summary>
  public event ClientTransactionEventHandler RolledBack;

  private readonly DataManager _dataManager;
  private readonly IObjectLoader _objectLoader;

  private readonly Dictionary<Enum, object> _applicationData;
  private readonly CompoundClientTransactionListener _listeners;
  private readonly ClientTransactionExtensionCollection _extensions;
  private readonly IEnlistedDomainObjectManager _enlistedObjectManager;

  private bool _isDiscarded;

  [NonSerialized]
  private QueryManager _queryManager;

  private readonly Guid _id = Guid.NewGuid ();

  // construction and disposing

  protected ClientTransaction (
      Dictionary<Enum, object> applicationData, 
      ClientTransactionExtensionCollection extensions, 
      ICollectionEndPointChangeDetectionStrategy collectionEndPointChangeDetectionStrategy,
      IEnlistedDomainObjectManager enlistedObjectManager)
  {
    ArgumentUtility.CheckNotNull ("applicationData", applicationData);
    ArgumentUtility.CheckNotNull ("extensions", extensions);
    ArgumentUtility.CheckNotNull ("collectionEndPointChangeDetectionStrategy", collectionEndPointChangeDetectionStrategy);
    ArgumentUtility.CheckNotNull ("enlistedObjectManager", enlistedObjectManager);

    IsReadOnly = false;
    _isDiscarded = false;
    _extensions = extensions;
   
    _listeners = new CompoundClientTransactionListener ();

    _listeners.AddListener (new LoggingClientTransactionListener ());
    _listeners.AddListener (new ReadOnlyClientTransactionListener (this));
    _listeners.AddListener (new ExtensionClientTransactionListener (this, _extensions));

    foreach (var factory in SafeServiceLocator.Current.GetAllInstances<IClientTransactionListenerFactory> ())
      _listeners.AddListener (factory.CreateClientTransactionListener (this));

    _applicationData = applicationData;
    _dataManager = new DataManager (this, collectionEndPointChangeDetectionStrategy);
    _objectLoader = new ObjectLoader (this, this, TransactionEventSink, new EagerFetcher (_dataManager));
    _enlistedObjectManager = enlistedObjectManager;

    TransactionEventSink.TransactionInitializing();
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
  /// Persists changed data in the couse of a <see cref="Commit"/> operation.
  /// </summary>
  /// <param name="changedDataContainers">The data containers for any object that was changed in this transaction.</param>
  protected abstract void PersistData (IEnumerable<DataContainer> changedDataContainers);

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
  /// This method should not set the <see cref="ClientTransaction"/> of the loaded data container, register the container in the 
  /// <see cref="DataContainerMap"/>, or set the  <see cref="DomainObject"/> of the container.
  /// All of these activities are performed by the caller. 
  /// </para>
  /// <para>
  /// The caller should also raise the <see cref="IClientTransactionListener.ObjectsLoading"/> and 
  /// <see cref="IClientTransactionListener.ObjectsLoaded"/> events.
  /// </para>
  /// </remarks>
  protected abstract DataContainer LoadDataContainer (ObjectID id);

  /// <summary>
  /// Loads a number of data containers from the underlying storage or the <see cref="ParentTransaction"/>.
  /// </summary>
  /// <param name="objectIDs">The ids of the <see cref="DataContainer"/> objects to load.</param>
  /// <returns>A <see cref="DataContainerCollection"/> with the loaded containers in the same order as in <paramref name="objectIDs"/>.</returns>
  /// <param name="throwOnNotFound">If <see langword="true" />, this method should throw a <see cref="BulkLoadException"/> if a data container 
  /// cannot be found for an <see cref="ObjectID"/>. If <see langword="false" />, the method should proceed as if the invalid ID hadn't been given.
  /// </param>
  /// <remarks>
  /// <para>
  /// This method raises the <see cref="IClientTransactionListener.ObjectsLoading"/> event on the <see cref="TransactionEventSink"/>, but not the 
  /// <see cref="IClientTransactionListener.ObjectsLoaded"/> event.
  /// </para>
  /// <para>
  /// This method should not 
  ///   set the <see cref="ClientTransaction"/> of the loaded data containers,
  ///   register the containers in the <see cref="DataContainerMap"/>,
  ///   or set the  <see cref="DomainObject"/> of the containers.
  /// All of these activities are performed by the caller. 
  /// </para>
  /// </remarks>
  protected abstract DataContainerCollection LoadDataContainers (ICollection<ObjectID> objectIDs, bool throwOnNotFound);

  /// <summary>
  /// Loads the related <see cref="DataContainer"/> for a given <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This method raises the <see cref="IClientTransactionListener.ObjectsLoading"/> event on the <see cref="TransactionEventSink"/>, but not the 
  /// <see cref="IClientTransactionListener.ObjectsLoaded"/> event.
  /// </para>
  /// <para>
  /// This method should not 
  ///   set the <see cref="ClientTransaction"/> of the loaded data container,
  ///   register the container in the <see cref="DataContainerMap"/>,
  ///   or set the  <see cref="DomainObject"/> of the container.
  /// All of these activities are performed by the caller. 
  /// </para>
  /// </remarks>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> of the end point that should be evaluated.
  /// <paramref name="relationEndPointID"/> must refer to an <see cref="ObjectEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>The related <see cref="DataContainer"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.InvalidCastException"><paramref name="relationEndPointID"/> does not refer to an 
  /// <see cref="DataManagement.ObjectEndPoint"/></exception>
  /// <exception cref="Persistence.PersistenceException">
  ///   The related object could not be loaded, but is mandatory.<br /> -or- <br />
  ///   The relation refers to non-existing object.<br /> -or- <br />
  ///   <paramref name="relationEndPointID"/> does not refer to an <see cref="DataManagement.ObjectEndPoint"/>.
  /// </exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="relationEndPointID"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  protected abstract DataContainer LoadRelatedDataContainer (RelationEndPointID relationEndPointID);

  /// <summary>
  /// Loads all related <see cref="DataContainer"/>s of a given <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> of the end point that should be evaluated.
  /// <paramref name="relationEndPointID"/> must refer to a <see cref="CollectionEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>
  /// A <see cref="DataContainerCollection"/> containing all related <see cref="DataContainer"/>s.
  /// </returns>
  /// <remarks>
  /// <para>
  /// This method does not raise any load events in this transaction. The load events are only raised when the loaded containers are merged with those
  /// that have already been loaded.
  /// </para>
  /// <para>
  /// This method should not 
  ///   set the <see cref="ClientTransaction"/> of the loaded data containers,
  ///   register the containers in the <see cref="DataContainerMap"/>,
  ///   or set the  <see cref="DomainObject"/> of the containers.
  /// All of these activities are performed by the caller. 
  /// </para>
  /// </remarks>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.PersistenceException">
  /// 	<paramref name="relationEndPointID"/> does not refer to one-to-many relation.<br/> -or- <br/>
  /// The StorageProvider for the related objects could not be initialized.
  /// </exception>
  protected abstract DataContainerCollection LoadRelatedDataContainers (RelationEndPointID relationEndPointID);

  /// <summary>
  /// Executes the given <see cref="IQuery"/> and returns its results as an array of <see cref="DataContainer"/> instances.
  /// </summary>
  /// <param name="query">The <see cref="IQuery"/> to be executed.</param>
  /// <returns>
  /// An array of <see cref="DataContainer"/> instances representing the result of the query.
  /// </returns>
  /// <remarks>
  /// <para>
  /// This method should not set the <see cref="ClientTransaction"/> of the loaded data container, register the container in a 
  /// <see cref="DataContainerMap"/>, or set the  <see cref="DomainObject"/> of the container.
  /// All of these activities are performed by the caller. 
  /// </para>
  /// <para>
  /// The caller should also raise the <see cref="IClientTransactionListener.ObjectsLoading"/> and 
  /// <see cref="IClientTransactionListener.ObjectsLoaded"/> events.
  /// </para>
  /// </remarks>
  /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="query"/> does not have a <see cref="QueryType"/> of <see cref="QueryType.Collection"/>.</exception>
  /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
  /// The <see cref="IQuery.StorageProviderID"/> of <paramref name="query"/> could not be found.
  /// </exception>
  /// <exception cref="Remotion.Data.DomainObjects.Persistence.PersistenceException">
  /// The <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/> for the given <see cref="IQuery"/> could not be instantiated.
  /// </exception>
  /// <exception cref="Remotion.Data.DomainObjects.Persistence.StorageProviderException">
  /// An error occurred while executing the query.
  /// </exception>
  protected abstract DataContainer[] LoadDataContainersForQuery (IQuery query);

  /// <summary>
  /// Executes the given <see cref="IQuery"/> and returns its result as a scalar value.
  /// </summary>
  /// <param name="query">The query to be executed.</param>
  /// <returns>The scalar query result.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="query"/> does not have a <see cref="QueryType"/> of <see cref="QueryType.Scalar"/>.
  /// </exception>
  /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
  /// The <see cref="IQuery.StorageProviderID"/> of <paramref name="query"/> could not be found.
  /// </exception>
  /// <exception cref="Remotion.Data.DomainObjects.Persistence.PersistenceException">
  /// The <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/> for the given <see cref="IQuery"/> could not be instantiated.
  /// </exception>
  /// <exception cref="Remotion.Data.DomainObjects.Persistence.StorageProviderException">
  /// An error occurred while executing the query.
  /// </exception>
  protected abstract object LoadScalarForQuery (IQuery query);

  // methods and properties

  /// <summary>
  /// Returns a <see cref="Guid"/> that uniquely identifies this <see cref="ClientTransaction"/>.
  /// </summary>
  public Guid ID
  {
    get { return _id; }
  }

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
  /// Gets the <see cref="IQueryManager"/> of the <b>ClientTransaction</b>.
  /// </summary>
  public virtual IQueryManager QueryManager
  {
    get
    {
      if (_queryManager == null)
        _queryManager = new QueryManager (this, _objectLoader, TransactionEventSink);

      return _queryManager;
    }
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
    if (!_isDiscarded)
      TransactionEventSink.TransactionDiscarding ();

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
  /// Gets the number of domain objects enlisted in this <see cref="ClientTransaction"/>.
  /// </summary>
  /// <value>The number of domain objects enlisted in this <see cref="ClientTransaction"/>.</value>
  public int EnlistedDomainObjectCount
  {
    get { return _enlistedObjectManager.EnlistedDomainObjectCount; }
  }

  /// <summary>
  /// Gets all domain objects enlisted in this <see cref="ClientTransaction"/>.
  /// </summary>
  /// <value>The domain objects enlisted in this transaction.</value>
  /// <remarks>
  /// The <see cref="DataContainer"/>s of the returned objects might not have been loaded yet. In that case, they will be loaded on first
  /// access of the respective objects' properties, and this might trigger an <see cref="ObjectNotFoundException"/> if the container cannot be loaded.
  /// </remarks>
  public IEnumerable<DomainObject> GetEnlistedDomainObjects ()
  {
    return _enlistedObjectManager.GetEnlistedDomainObjects ();
  }

  /// <summary>
  /// Returns the <see cref="DomainObject"/> enlisted for the given <paramref name="objectID"/> via <see cref="EnlistDomainObject"/>, or 
  /// <see langword="null"/> if no such object exists.
  /// </summary>
  /// <param name="objectID">The <see cref="ObjectID"/> for which to retrieve a <see cref="DomainObject"/>.</param>
  /// <returns>
  /// A <see cref="DomainObject"/> with the given <paramref name="objectID"/> previously enlisted via <see cref="EnlistDomainObject"/>,
  /// or <see langword="null"/> if no such object exists.
  /// </returns>
  /// <remarks>
  /// The <see cref="DataContainer"/> of the returned object might not have been loaded yet. In that case, it will be loaded on first
  /// access of the object's properties, and this might trigger an <see cref="ObjectNotFoundException"/> if the container cannot be loaded.
  /// </remarks>
  public DomainObject GetEnlistedDomainObject (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull ("objectID", objectID);
    return _enlistedObjectManager.GetEnlistedDomainObject (objectID);
  }

  /// <summary>
  /// Determines whether the specified <paramref name="domainObject"/> is enlisted in this transaction.
  /// </summary>
  /// <param name="domainObject">The domain object to be checked.</param>
  /// <returns>
  /// <see langword="true" /> if the specified domain object can be used in the context of this transaction; otherwise, <see langword="false" />.
  /// </returns>
  public bool IsEnlisted (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    return _enlistedObjectManager.IsEnlisted (domainObject);
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
  /// has been enlisted in transaction B before transaction's A commit, transaction B will not see the changes committed by transaction A.
  /// </para>
  /// <para>
  /// If a certain <see cref="ObjectID"/> has already been associated with a certain <see cref="DomainObject"/> in this transaction, it is not
  /// possible to register another <see cref="DomainObject"/> reference with the same <see cref="DomainObject.ID"/>.
  /// </para>
  /// <para>The data for the <see cref="DomainObject"/> is not loaded immediately by this method, but will be retrieved when the object is first
  /// used in this transaction. If the object has been deleted from the underlying database, access to such an object will result in an
  /// <see cref="ObjectNotFoundException"/>.</para>
  /// </remarks>
  /// <exception cref="InvalidOperationException">The domain object cannot be enlisted, e.g., because another <see cref="DomainObject"/> with the same
  /// <see cref="ObjectID"/> has already been associated with this transaction..</exception>
  /// <exception cref="ArgumentNullException">The <paramref name="domainObject"/> parameter is <see langword="null"/>.</exception>
  public bool EnlistDomainObject (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    
    CheckDomainObjectForEnlisting (domainObject);
    return _enlistedObjectManager.EnlistDomainObject (domainObject);
  }

  /// <summary>
  /// Checks whether the given <see cref="DomainObject"/> can be enlisted in this <see cref="ClientTransaction"/>, throwing an 
  /// <see cref="InvalidOperationException"/> if it can't.
  /// </summary>
  /// <param name="domainObject">The domain object to check.</param>
  /// <exception cref="InvalidOperationException">Thrown when the <paramref name="domainObject"/> cannot be enlisted in this transaction.</exception>
  /// <remarks>
  /// The default implementation of this method checks whether the <paramref name="domainObject"/> has already been bound to another transaction. Sub-
  /// transactions can override this method to remove this check or perform additional checks.
  /// </remarks>
  protected virtual void CheckDomainObjectForEnlisting (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    if (domainObject.HasBindingTransaction && domainObject.GetBindingTransaction() != this)
    {
      string message = String.Format ("Cannot enlist the domain object '{0}' in this transaction, because it is already bound to another transaction.",
                                      domainObject.ID);
      throw new InvalidOperationException (message);
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
  /// Ensures that the data of the <see cref="DomainObject"/> with the given <see cref="ObjectID"/> has been loaded into this 
  /// <see cref="ClientTransaction"/>. If it hasn't, this method loads the object's data.
  /// </summary>
  /// <param name="objectID">The domain object whose data must be loaded.</param>
  /// <exception cref="ArgumentNullException">The <paramref name="objectID"/> parameter is <see langword="null" />.</exception>
  /// <exception cref="ObjectDiscardedException">The given <paramref name="objectID"/> has already been discarded in this transaction.</exception>
  /// <exception cref="ObjectNotFoundException">No data could be loaded for the given <paramref name="objectID"/> because the object was not
  /// found in the underlying data source.</exception>
  public void EnsureDataAvailable (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull ("objectID", objectID);

    if (GetDataContainerWithoutLoading (objectID) == null)
      LoadObject (objectID);

    Assertion.IsTrue (DataManager.DataContainerMap[objectID] != null);
  }

  /// <summary>
  /// Ensures that the data for the <see cref="DomainObject"/>s with the given <see cref="ObjectID"/> values has been loaded into this 
  /// <see cref="ClientTransaction"/>. If it hasn't, this method loads the objects' data, performing a bulk load operation.
  /// </summary>
  /// <param name="objectIDs">The <see cref="ObjectID"/> values whose data must be loaded.</param>
  /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null" />.</exception>
  /// <exception cref="ClientTransactionsDifferException">One of the given <paramref name="objectIDs"/> cannot be used in this 
  /// <see cref="ClientTransaction"/>.</exception>
  /// <exception cref="ObjectDiscardedException">One of the given <paramref name="objectIDs"/> has already been discarded in this transaction.</exception>
  /// <exception cref="BulkLoadException">No data could be loaded for one or more of the given <paramref name="objectIDs"/> because the object 
  /// was not found in the underlying data source.</exception>
  public void EnsureDataAvailable (IEnumerable<ObjectID> objectIDs)
  {
    ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

    EnsureDataAvailable (objectIDs, true);
  }

  private void EnsureDataAvailable (IEnumerable<ObjectID> objectIDs, bool throwOnNotFound)
  {
    var idsToBeLoaded = from objectID in objectIDs
                        where GetDataContainerWithoutLoading (objectID) == null
                        select objectID;

    LoadObjects (idsToBeLoaded.ToList (), throwOnNotFound);
  }

  /// <summary>
  /// Ensures that the data of the <see cref="IEndPoint"/> with the given <see cref="RelationEndPointID"/> has been loaded into this 
  /// <see cref="ClientTransaction"/>. If it hasn't, this method loads the relation end point's data.
  /// </summary>
  /// <param name="endPointID">The <see cref="RelationEndPointID"/> of the end point whose data must be loaded. In order to force a collection-valued 
  /// relation property to be loaded, pass the <see cref="DomainObjectCollection.AssociatedEndPointID"/>.</param>
  /// <exception cref="ArgumentNullException">The <paramref name="endPointID"/> parameter is <see langword="null" />.</exception>
  public void EnsureDataAvailable (RelationEndPointID endPointID)
  {
    var endPoint = DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (endPointID);
    endPoint.EnsureDataAvailable();

    Assertion.IsTrue (endPoint.IsDataAvailable);
  }

  // TODO 2072: Move
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
        var sourceEndPoint = (ICollectionEndPoint) sourceTransaction.DataManager.RelationEndPointMap[endPointID];
        if (sourceEndPoint != null)
        {
          var sourceRelatedObjectCollection = sourceEndPoint.OppositeDomainObjects;
          var destinationRelatedObjectCollection = DataManager.RelationEndPointMap.GetRelatedObjects (endPointID);
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
    ClientTransaction subTransaction = ObjectFactory.Create<SubClientTransaction> (true, ParamList.Create (this));
    return subTransaction;
  }

  /// <summary>
  /// Returns whether at least one <see cref="DomainObject"/> in this <b>ClientTransaction</b> has been changed.
  /// </summary>
  /// <returns><see langword="true"/> if at least one <see cref="DomainObject"/> in this <b>ClientTransaction</b> has been changed; otherwise, <see langword="false"/>.</returns>
  public virtual bool HasChanged ()
  {
    return _dataManager.GetChangedData().Any();
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
      var changedButNotDeletedDomainObjects = _dataManager.GetLoadedData (StateType.Changed, StateType.New).Select (tuple => tuple.Item1).ToArray();

      var changedDataContainers = _dataManager.GetChangedDataContainersForCommit();
      PersistData (changedDataContainers);

      _dataManager.Commit ();
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

      var changedButNotNewItems = _dataManager.GetLoadedData (StateType.Changed, StateType.Deleted).Select (tuple => tuple.Item1).ToArray();

      _dataManager.Rollback ();

      EndRollback (changedButNotNewItems);
    }
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
  /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
  /// <exception cref="DataManagement.ObjectDeletedException"><paramref name="includeDeleted"/> is false and the DomainObject with <paramref name="id"/> has been deleted.</exception>
  /// <exception cref="ObjectNotFoundException">The object could not be found in the database (or it has been discarded in this transaction).</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  protected internal virtual DomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    if (DataManager.IsDiscarded (id))
      throw new ObjectNotFoundException (id);

    var objectReference = GetObjectReference (id);
    EnsureDataAvailable (id);

    var state = objectReference.TransactionContext[this].State;
    Assertion.IsFalse (state == StateType.NotLoadedYet);
    if (state == StateType.Deleted && !includeDeleted)
      throw new ObjectDeletedException (id);

    return objectReference;
  }

  /// <summary>
  /// Gets a reference to a <see cref="DomainObject"/> with the given <see cref="ObjectID"/> from this <see cref="ClientTransaction"/>. If the
  /// transaction does not currently hold an object with this <see cref="ObjectID"/>, an object reference representing that <see cref="ObjectID"/> 
  /// is created without calling a constructor and without loading the object's data from the data source. This method does not check whether an
  /// object with the given <see cref="ObjectID"/> actually exists in the data source.
  /// </summary>
  /// <param name="objectID">The <see cref="ObjectID"/> to get an object reference for.</param>
  /// <returns>An object with the given <see cref="ObjectID"/>, possibly in <see cref="StateType.NotLoadedYet"/> state.</returns>
  /// <remarks>
  /// <para>
  /// When an object with the given <paramref name="objectID"/> has already been enlisted in the transaction, that object is returned. Otherwise,
  /// an object in <see cref="StateType.NotLoadedYet"/> state is created and enlisted without loading its data from the data source. In such a case,
  /// the object's data is loaded when it's first needed; e.g., when one of its properties is accessed or when 
  /// <see cref="EnsureDataAvailable(Remotion.Data.DomainObjects.ObjectID)"/> is called for its <see cref="ObjectID"/>. At that point, an
  /// <see cref="ObjectNotFoundException"/> may be triggered when the object's data cannot be found.
  /// </para>
  /// </remarks>
  /// <exception cref="ArgumentNullException">The <paramref name="objectID"/> parameter is <see langword="null" />.</exception>
  /// <exception cref="ObjectDiscardedException">The object with the given <paramref name="objectID"/> has already been discarded.</exception>
  protected internal virtual DomainObject GetObjectReference (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull ("objectID", objectID);

    if (DataManager.IsDiscarded (objectID))
      throw new ObjectDiscardedException (objectID);

    var enlistedObject = GetEnlistedDomainObject (objectID);
    if (enlistedObject != null)
    {
      return enlistedObject;
    }
    else
    {
      var creator = objectID.ClassDefinition.GetDomainObjectCreator ();

      return creator.CreateObjectReference (objectID, this);
    }
  }
  
  protected internal virtual DomainObject NewObject (Type domainObjectType, ParamList constructorParameters)
  {
    ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);
    ArgumentUtility.CheckNotNull ("constructorParameters", constructorParameters);

    using (EnterNonDiscardingScope ())
    {
      var creator = MappingConfiguration.Current.ClassDefinitions.GetMandatory (domainObjectType).GetDomainObjectCreator ();
      var ctorInfo = creator.GetConstructorLookupInfo (domainObjectType);

      var instance = (DomainObject) constructorParameters.InvokeConstructor (ctorInfo);
      DomainObjectMixinCodeGenerationBridge.OnDomainObjectCreated (instance);

      instance.FinishReferenceInitialization ();
      return instance;
    }
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
  /// <exception cref="BulkLoadException">The data source found one or more errors when loading the objects. The exceptions can be accessed via the
  /// <see cref="BulkLoadException.Exceptions"/> property.</exception>
  public T[] GetObjects<T> (params ObjectID[] objectIDs) 
      where T : DomainObject
  {
    ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);
    return GetObjects<T> (objectIDs, true);
  }

  /// <summary>
  /// Gets a number of objects that are already loaded or attempts to load them from the data source. If an object is not be found, the result array
  /// will contain a <see langword="null" /> reference in its place.
  /// </summary>
  /// <typeparam name="T">The type of objects expected to be returned. Specify <see cref="DomainObject"/> if no specific type is expected.</typeparam>
  /// <param name="objectIDs">The IDs of the objects to be retrieved.</param>
  /// <returns>A list of objects of type <typeparamref name="T"/> corresponding to (and in the same order as) the IDs specified in <paramref name="objectIDs"/>.</returns>
  /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="ArgumentTypeException">One of the retrieved objects doesn't fit the specified type <typeparamref name="T"/>.</exception>
  /// <exception cref="ObjectDiscardedException">One of the retrieved objects has already been discarded.</exception>
  /// <exception cref="BulkLoadException">The data source found one or more errors when loading the objects. The exceptions can be accessed via the
  /// <see cref="BulkLoadException.Exceptions"/> property.</exception>
  public T[] TryGetObjects<T> (params ObjectID[] objectIDs) 
      where T : DomainObject
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
  /// <exception cref="BulkLoadException">The data source found one or more errors when loading the objects. The exceptions can be accessed via the
  /// <see cref="BulkLoadException.Exceptions"/> property.</exception>
  protected internal virtual T[] GetObjects<T> (ICollection<ObjectID> objectIDs, bool throwOnNotFound) 
      where T : DomainObject
  {
    ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

    EnsureDataAvailable (objectIDs, throwOnNotFound); // this performs a bulk load operation

    var result = from id in objectIDs
                 let maybeDataContainer = Maybe.ForValue (DataManager.DataContainerMap[id])
                 let maybeDomainObject = maybeDataContainer.Select (dc => (T) dc.DomainObject)
                 select maybeDomainObject.ValueOrDefault ();
    return result.ToArray ();
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

    DataContainer dataContainer = GetDataContainer(domainObject);
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

      TransactionEventSink.RelationReading (domainObject, relationEndPointID.Definition.PropertyName, ValueAccess.Current);
      DomainObject relatedObject = _dataManager.RelationEndPointMap.GetRelatedObject (relationEndPointID, false);
      TransactionEventSink.RelationRead (domainObject, relationEndPointID.Definition.PropertyName, relatedObject, ValueAccess.Current);

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

      TransactionEventSink.RelationReading (domainObject, relationEndPointID.Definition.PropertyName, ValueAccess.Original);
      DomainObject relatedObject = _dataManager.RelationEndPointMap.GetOriginalRelatedObject (relationEndPointID);
      TransactionEventSink.RelationRead (domainObject, relationEndPointID.Definition.PropertyName, relatedObject, ValueAccess.Original);

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

      TransactionEventSink.RelationReading (domainObject, relationEndPointID.Definition.PropertyName, ValueAccess.Current);
      var relatedObjects = _dataManager.RelationEndPointMap.GetRelatedObjects (relationEndPointID);
      var readOnlyRelatedObjects = new ReadOnlyDomainObjectCollectionAdapter<DomainObject> (relatedObjects);
      TransactionEventSink.RelationRead (domainObject, relationEndPointID.Definition.PropertyName, readOnlyRelatedObjects, ValueAccess.Current);

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

      TransactionEventSink.RelationReading (domainObject, relationEndPointID.Definition.PropertyName, ValueAccess.Original);
      DomainObjectCollection relatedObjects = _dataManager.RelationEndPointMap.GetOriginalRelatedObjects (relationEndPointID);
      var readOnlyRelatedObjects = new ReadOnlyDomainObjectCollectionAdapter<DomainObject> (relatedObjects);
      TransactionEventSink.RelationRead (domainObject, relationEndPointID.Definition.PropertyName, readOnlyRelatedObjects, ValueAccess.Original);

      return relatedObjects;
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
      var command = _dataManager.CreateDeleteCommand (domainObject);
      var fullCommand = command.ExpandToAllRelatedObjects ();
      fullCommand.NotifyAndPerform ();
    }
  }

  /// <summary>
  /// Loads an object from the data source.
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
  protected virtual DomainObject LoadObject (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);
    return _objectLoader.LoadObject (id);
  }

  /// <summary>
  /// Loads several objects from the data source in a bulk load operation.
  /// </summary>
  /// <remarks>
  /// This method raises the <see cref="Loaded"/> event.
  /// </remarks>
  /// <param name="idsToBeLoaded">An <see cref="ObjectID"/> object indicating which <see cref="DomainObject"/> instances to load. Must not be 
  /// <see langword="null"/>.</param>
  /// <param name="throwOnNotFound">If <see langword="true" />, this method should throw a <see cref="BulkLoadException"/> if a data container 
  /// cannot be found for an <see cref="ObjectID"/>. If <see langword="false" />, <see langword="null"/> is inserted in the result array for the 
  /// invalid ID.
  /// </param>
  /// <returns>The <see cref="DomainObject"/> instances that were loaded.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="idsToBeLoaded"/> is <see langword="null"/>.</exception>
  /// <exception cref="BulkLoadException">There was an error trying to load the object identified by one of the given 
  /// <paramref name="idsToBeLoaded"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for one of the given <paramref name="idsToBeLoaded"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  protected virtual DomainObject[] LoadObjects (IList<ObjectID> idsToBeLoaded, bool throwOnNotFound)
  {
    ArgumentUtility.CheckNotNull ("idsToBeLoaded", idsToBeLoaded);
    return _objectLoader.LoadObjects (idsToBeLoaded, throwOnNotFound);
  }

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
  protected internal virtual DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    return _objectLoader.LoadRelatedObject (relationEndPointID);
  }

  /// <summary>
  /// Loads all related <see cref="DomainObject"/>s of a given <see cref="DataManagement.RelationEndPointID"/>.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> of the end point that should be evaluated.
  /// <paramref name="relationEndPointID"/> must refer to a <see cref="CollectionEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>
  /// A <see cref="DomainObjectCollection"/> containing all related <see cref="DomainObject"/>s.
  /// </returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.PersistenceException">
  /// 	<paramref name="relationEndPointID"/> does not refer to one-to-many relation.<br/> -or- <br/>
  /// The StorageProvider for the related objects could not be initialized.
  /// </exception>
  protected internal virtual DomainObject[] LoadRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);
    return _objectLoader.LoadRelatedObjects (relationEndPointID);
  }

  /// <summary>
  /// Gets the <see cref="DataContainer"/> for a given <see cref="DomainObject"/> in the context of this transaction, loading
  /// it from the data source if necessary.
  /// </summary>
  /// <remarks>
  /// This method may raise the <see cref="Loaded"/> event.
  /// </remarks>
  /// <param name="domainObject">A <see cref="DomainObject"/> reference indicating the <see cref="DomainObject"/> whose <see cref="DataContainer"/>
  /// to retrieve. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
  /// <exception cref="ObjectDiscardedException">The object has been discarded in the context of this transaction.</exception>
  /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the context of this transaction.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="domainObject"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the datasource.
  /// </exception>
  protected internal DataContainer GetDataContainer (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    DomainObjectCheckUtility.CheckIfRightTransaction (domainObject, this);

    EnsureDataAvailable (domainObject.ID);
    
    var dataContainer = GetDataContainerWithoutLoading (domainObject.ID);
    
    Assertion.IsNotNull (dataContainer);
    Assertion.IsTrue (
        dataContainer.DomainObject == domainObject, 
        "Because domainObject is enlisted, LoadObject is forced to reuse the domainObject reference.");

    return dataContainer;
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
  protected internal virtual void OnLoaded (ClientTransactionEventArgs args)
  {
    ArgumentUtility.CheckNotNull ("args", args);

    if (Loaded != null)
      Loaded (this, args);
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

    var changedDomainObjects = _dataManager.GetChangedData ().Select (tuple => tuple.Item1).ToObjectList();
    var domainObjectComittingEventRaised = new DomainObjectCollection ();
    var clientTransactionCommittingEventRaised = new DomainObjectCollection ();

    List<DomainObject> clientTransactionCommittingEventNotRaised;
    do
    {
      var domainObjectCommittingEventNotRaised = changedDomainObjects.GetItemsExcept (domainObjectComittingEventRaised).ToList();
      while (domainObjectCommittingEventNotRaised.Any())
      {
        foreach (DomainObject domainObject in domainObjectCommittingEventNotRaised)
        {
          if (!domainObject.IsDiscarded)
          {
            domainObject.OnCommitting (EventArgs.Empty);

            if (!domainObject.IsDiscarded)
              domainObjectComittingEventRaised.Add (domainObject);
          }
        }

        changedDomainObjects = _dataManager.GetChangedData ().Select (tuple => tuple.Item1).ToObjectList ();
        domainObjectCommittingEventNotRaised = changedDomainObjects.GetItemsExcept (domainObjectComittingEventRaised).ToList();
      }

      clientTransactionCommittingEventNotRaised = changedDomainObjects.GetItemsExcept (clientTransactionCommittingEventRaised).ToList();
      
      OnCommitting (new ClientTransactionEventArgs (clientTransactionCommittingEventNotRaised.AsReadOnly()));
      foreach (DomainObject domainObject in clientTransactionCommittingEventNotRaised)
      {
        if (!domainObject.IsDiscarded)
          clientTransactionCommittingEventRaised.Add (domainObject);
      }

      changedDomainObjects = _dataManager.GetChangedData ().Select (tuple => tuple.Item1).ToObjectList ();
      clientTransactionCommittingEventNotRaised = changedDomainObjects.GetItemsExcept (clientTransactionCommittingEventRaised).ToList();
    } while (clientTransactionCommittingEventNotRaised.Any());
  }

  private void EndCommit (DomainObject[] changedDomainObjects)
  {
    foreach (DomainObject changedDomainObject in changedDomainObjects)
      changedDomainObject.OnCommitted (EventArgs.Empty);

    OnCommitted (new ClientTransactionEventArgs (Array.AsReadOnly (changedDomainObjects)));
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

    var changedDomainObjects = _dataManager.GetChangedData ().Select (tuple => tuple.Item1).ToObjectList ();
    var domainObjectRollingBackEventRaised = new DomainObjectCollection ();
    var clientTransactionRollingBackEventRaised = new DomainObjectCollection ();

    List<DomainObject> clientTransactionRollingBackEventNotRaised;
    do
    {
      var domainObjectRollingBackEventNotRaised = changedDomainObjects.GetItemsExcept (domainObjectRollingBackEventRaised).ToList();
      while (domainObjectRollingBackEventNotRaised.Any())
      {
        foreach (DomainObject domainObject in domainObjectRollingBackEventNotRaised)
        {
          if (!domainObject.IsDiscarded)
          {
            domainObject.OnRollingBack (EventArgs.Empty);

            if (!domainObject.IsDiscarded)
              domainObjectRollingBackEventRaised.Add (domainObject);
          }
        }

        changedDomainObjects = _dataManager.GetChangedData ().Select (tuple => tuple.Item1).ToObjectList ();
        domainObjectRollingBackEventNotRaised = changedDomainObjects.GetItemsExcept (domainObjectRollingBackEventRaised).ToList ();
      }

      clientTransactionRollingBackEventNotRaised = changedDomainObjects.GetItemsExcept (clientTransactionRollingBackEventRaised).ToList ();

      OnRollingBack (new ClientTransactionEventArgs (clientTransactionRollingBackEventNotRaised.AsReadOnly()));
      foreach (DomainObject domainObject in clientTransactionRollingBackEventNotRaised)
      {
        if (!domainObject.IsDiscarded)
          clientTransactionRollingBackEventRaised.Add (domainObject);
      }

      changedDomainObjects = _dataManager.GetChangedData ().Select (tuple => tuple.Item1).ToObjectList ();
      clientTransactionRollingBackEventNotRaised = changedDomainObjects.GetItemsExcept (clientTransactionRollingBackEventRaised).ToList ();
    } while (clientTransactionRollingBackEventNotRaised.Any());
  }

  private void EndRollback (DomainObject[] changedDomainObjects)
  {
    foreach (DomainObject changedDomainObject in changedDomainObjects)
      changedDomainObject.OnRolledBack (EventArgs.Empty);

    OnRolledBack (new ClientTransactionEventArgs (Array.AsReadOnly (changedDomainObjects)));
  }

  private DataContainer GetDataContainerWithoutLoading (ObjectID id)
  {
    if (DataManager.IsDiscarded (id))
      throw new ObjectDiscardedException (id);

    return DataManager.DataContainerMap[id];
  }

  public virtual ITransaction ToITransation ()
  {
    return new ClientTransactionWrapper (this);
  }

  [Obsolete ("This method has been removed. Please implement the desired behavior yourself, using GetEnlistedDomainObjects(), EnlistDomainObject(), "
             + "and CopyCollectionEventHandlers(). (1.13.41)", false)]
  public void EnlistSameDomainObjects (ClientTransaction sourceTransaction, bool copyCollectionEventHandlers)
  {
    throw new NotImplementedException ();
  }

  [Obsolete ("This method is now obsolete, use GetObject (ObjectID, bool) instead. (1.13.42)", true)]
  protected internal virtual DomainObject GetObject (ObjectID id)
  {
    throw new NotImplementedException ();
  }



  DataContainer IDataSource.LoadDataContainer (ObjectID id)
  {
    return LoadDataContainer (id);
  }

  DataContainerCollection IDataSource.LoadDataContainers (ICollection<ObjectID> objectIDs, bool throwOnNotFound)
  {
    return LoadDataContainers (objectIDs, throwOnNotFound);
  }

  DataContainer IDataSource.LoadRelatedDataContainer (RelationEndPointID relationEndPointID)
  {
    return LoadRelatedDataContainer (relationEndPointID);
  }

  DataContainerCollection IDataSource.LoadRelatedDataContainers (RelationEndPointID relationEndPointID)
  {
    return LoadRelatedDataContainers (relationEndPointID);
  }

  void IDataSource.PersistData (IEnumerable<DataContainer> changedDataContainers)
  {
    PersistData (changedDataContainers);
  }

  ObjectID IDataSource.CreateNewObjectID (ClassDefinition classDefinition)
  {
    return CreateNewObjectID (classDefinition);
  }

  DataContainer[] IDataSource.LoadDataContainersForQuery (IQuery query)
  {
    return LoadDataContainersForQuery (query);
  }

  object IDataSource.LoadScalarForQuery (IQuery query)
  {
    return LoadScalarForQuery (query);
  }
}
}
