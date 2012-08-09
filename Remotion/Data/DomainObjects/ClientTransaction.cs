// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;

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
public class ClientTransaction
{
  /// <summary>
  /// Creates a new root <see cref="ClientTransaction"/>, a transaction which uses a <see cref="RootPersistenceStrategy"/>.
  /// </summary>
  /// <returns>A new root <see cref="ClientTransaction"/> instance.</returns>
  /// <remarks>The object returned by this method can be extended with <b>Mixins</b> by configuring the <see cref="MixinConfiguration.ActiveConfiguration"/>
  /// to include a mixin for type <see cref="RootPersistenceStrategy"/>. Declaratively, this can be achieved by attaching an
  /// <see cref="ExtendsAttribute"/> instance for <see cref="ClientTransaction"/> or <see cref="RootPersistenceStrategy"/> to a mixin class.</remarks>
  public static ClientTransaction CreateRootTransaction ()
  {
    var componentFactory = RootClientTransactionComponentFactory.Create();
    return ObjectFactory.Create<ClientTransaction> (true, ParamList.Create (componentFactory));
  }

  /// <summary>
  /// Creates a new root <see cref="ClientTransaction"/> that binds all <see cref="DomainObject"/> instances that are created in its context. A bound
  /// <see cref="DomainObject"/> is always accessed in the context of its binding transaction, it never uses <see cref="Current"/>.
  /// </summary>
  /// <returns>A new binding <see cref="ClientTransaction"/> instance.</returns>
  /// <remarks>
  /// <para>
  /// The object returned by this method can be extended with <b>Mixins</b> by configuring the <see cref="MixinConfiguration.ActiveConfiguration"/>
  /// to include a mixin for type <see cref="RootPersistenceStrategy"/>. Declaratively, this can be achieved by attaching an
  /// <see cref="ExtendsAttribute"/> instance for <see cref="ClientTransaction"/> or <see cref="RootPersistenceStrategy"/> to a mixin class.
  /// </para>
  /// <para>
  /// Binding transactions cannot have subtransactions.
  /// </para>
  /// </remarks>
  public static ClientTransaction CreateBindingTransaction ()
  {
    var componentFactory = RootClientTransactionComponentFactory.Create(); // binding transactions behave like root transactions
    return ObjectFactory.Create<BindingClientTransaction> (true, ParamList.Create (componentFactory));
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
    get
    {
      // Performance: In order to reduce SafeContext calls, we do not use HasCurrentTransaction/CurrentTransaction here
      ClientTransactionScope activeScope = ClientTransactionScope.ActiveScope;

      if (activeScope != null && activeScope.ScopedTransaction != null)
        return activeScope.ScopedTransaction;

      return null;
    }
  }

  // member fields

  /// <summary>
  /// Occurs when the <b>ClientTransaction</b> has created a subtransaction.
  /// </summary>
  public event EventHandler<SubTransactionCreatedEventArgs> SubTransactionCreated;

  /// <summary>
  /// Occurs after the <b>ClientTransaction</b> has loaded a new object.
  /// </summary>
  public event EventHandler<ClientTransactionEventArgs> Loaded;

  /// <summary>
  /// Occurs immediately before the <b>ClientTransaction</b> performs a <see cref="Commit"/> operation.
  /// </summary>
  public event EventHandler<ClientTransactionCommittingEventArgs> Committing;

  /// <summary>
  /// Occurs immediately after the <b>ClientTransaction</b> has successfully performed a <see cref="Commit"/> operation.
  /// </summary>
  public event EventHandler<ClientTransactionEventArgs> Committed;

  /// <summary>
  /// Occurs immediately before the <b>ClientTransaction</b> performs a <see cref="Rollback"/> operation.
  /// </summary>
  public event EventHandler<ClientTransactionEventArgs> RollingBack;

  /// <summary>
  /// Occurs immediately after the <b>ClientTransaction</b> has successfully performed a <see cref="Rollback"/> operation.
  /// </summary>
  public event EventHandler<ClientTransactionEventArgs> RolledBack;

  private readonly IClientTransactionComponentFactory _componentFactory;
  private readonly ClientTransaction _parentTransaction;

  private readonly Dictionary<Enum, object> _applicationData;
  private readonly ClientTransactionExtensionCollection _extensions;

  private readonly IClientTransactionEventBroker _eventBroker;

  private readonly IEnlistedDomainObjectManager _enlistedDomainObjectManager;
  private readonly IInvalidDomainObjectManager _invalidDomainObjectManager;
  private readonly IDataManager _dataManager;
  private readonly IPersistenceStrategy _persistenceStrategy;
  private readonly IQueryManager _queryManager;
  private readonly ICommitRollbackAgent _commitRollbackAgent;

  private ClientTransaction _subTransaction;

  private bool _isActive = true;
  private bool _isDiscarded;

  private readonly Guid _id = Guid.NewGuid ();

  protected ClientTransaction (IClientTransactionComponentFactory componentFactory)
  {
    ArgumentUtility.CheckNotNull ("componentFactory", componentFactory);
    
    _componentFactory = componentFactory;
    _parentTransaction = componentFactory.GetParentTransaction (this);

    _applicationData = componentFactory.CreateApplicationData (this);

    _eventBroker = componentFactory.CreateEventBroker (this);
    _enlistedDomainObjectManager = componentFactory.CreateEnlistedObjectManager (this);
    _invalidDomainObjectManager = componentFactory.CreateInvalidDomainObjectManager (this, _eventBroker);
    _persistenceStrategy = componentFactory.CreatePersistenceStrategy (this);
    _dataManager = componentFactory.CreateDataManager (this, _eventBroker, _invalidDomainObjectManager, _persistenceStrategy);
    _queryManager = componentFactory.CreateQueryManager (this, _eventBroker, _invalidDomainObjectManager, _persistenceStrategy, _dataManager);
    _commitRollbackAgent = componentFactory.CreateCommitRollbackAgent (this, _eventBroker, _persistenceStrategy, _dataManager);

    _extensions = componentFactory.CreateExtensionCollection (this);
    AddListener (new ExtensionClientTransactionListener (_extensions));

    if (_parentTransaction != null)
      _parentTransaction.RaiseListenerEvent ((tx, l) => l.SubTransactionInitialize (tx, this));
    RaiseListenerEvent ((tx, l) => l.TransactionInitialize (tx));
  }

  /// <summary>
  /// Gets the parent transaction for this <see cref="ClientTransaction"/>, or <see langword="null" /> if this transaction is a root transaction.
  /// </summary>
  /// <value>The parent transaction, or <see langword="null" /> if this transaction is a root transaction.</value>
  public ClientTransaction ParentTransaction 
  { 
    get { return _parentTransaction; }
  }

  /// <summary>
  /// Gets the active sub-transaction of this <see cref="ClientTransaction"/>, or <see langword="null" /> if this transaction has no sub-transaction.
  /// </summary>
  /// <value>The active sub-transaction, or <see langword="null" /> if this transaction has no sub-transaction.</value>
  /// <remarks>When the <see cref="SubTransaction"/> is discarded, this property is automatically set to <see langword="null" />.</remarks>
  public ClientTransaction SubTransaction
  {
    get { return _subTransaction; }
  }

  /// <summary>
  /// Gets the root transaction of this <see cref="ClientTransaction"/>, that is, the top-level transaction in a row of sub-transactions.
  /// If this <see cref="ClientTransaction"/> is itself a root transaction (i.e, it has no <see cref="ParentTransaction"/>), it is returned.
  /// </summary>
  /// <value>The root transaction of this <see cref="ClientTransaction"/>.</value>
  public ClientTransaction RootTransaction 
  { 
    get
    {
      var current = this;
      while (current.ParentTransaction != null)
        current = current.ParentTransaction;

      return current;
    }
  }

  /// <summary>
  /// Gets the lowest sub-transaction of this <see cref="ClientTransaction"/>, that is, the bottom-most transaction in a row of sub-transactions.
  /// If this <see cref="ClientTransaction"/> is itself the leaf transaction (i.e, it has no <see cref="SubTransaction"/>), it itself is 
  /// returned.
  /// </summary>
  /// <value>The leaf transaction of this <see cref="ClientTransaction"/>.</value>
  public ClientTransaction LeafTransaction
  {
    get
    {
      var current = this;
      while (current.SubTransaction != null)
        current = current.SubTransaction;

      return current;
    }
  }

  /// <summary>
  /// Gets the persistence strategy associated with this <see cref="ClientTransaction"/>. The <see cref="PersistenceStrategy"/> is used to load
  /// data from the underlying data source without actually registering the data in this transaction, and it can be used to store data in the
  /// underlying data source.
  /// </summary>
  /// <value>The persistence strategy associated with this <see cref="ClientTransaction"/>.</value>
  protected IPersistenceStrategy PersistenceStrategy
  {
    get { return _persistenceStrategy; }
  }

  /// <summary>
  /// Returns a <see cref="Guid"/> that uniquely identifies this <see cref="ClientTransaction"/>.
  /// </summary>
  public Guid ID
  {
    get { return _id; }
  }

  /// <summary>
  /// Indicates whether this transaction is active, i.e., it has no <see cref="SubTransaction"/>.
  /// Inactive transactions can only be used to read and load data, not change it.
  /// </summary>
  /// <value><see langword="true" /> if this instance is active; otherwise, <see langword="false" />.</value>
  /// <remarks>
  /// <para>
  /// Transactions are made inactive while there exist open subtransactions for them. An inactive transaction can only be used for
  /// operations that do not cause any change of transaction state. Reading, loading, and querying objects is okay with inactive transactions,
  /// but any method that would cause a state change will throw an exception.
  /// </para>
  /// <para>
  /// Most of the time, this property returns <see langword="true" /> as long as <see cref="SubTransaction"/> is <see langword="null" />. However,
  /// while 
  /// <see cref="CreateSubTransaction(System.Func{Remotion.Data.DomainObjects.ClientTransaction,Remotion.Data.DomainObjects.Infrastructure.InvalidObjects.IInvalidDomainObjectManager,Remotion.Data.DomainObjects.Infrastructure.Enlistment.IEnlistedDomainObjectManager,Remotion.Data.DomainObjects.ClientTransaction})"/>
  /// is executing, there is a small time frame where this property already returns <see langword="false" /> while <see cref="SubTransaction"/> is 
  /// still <see langword="null" />.
  /// </para>
  /// </remarks>
  public bool IsActive
  {
    get { return _isActive; }
    protected internal set { _isActive = value; }
  }

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
  /// Gets the <see cref="IQueryManager"/> of the <see cref="ClientTransaction"/>.
  /// </summary>
  public IQueryManager QueryManager
  {
    get { return _queryManager; }
  }

  public override string ToString ()
  {
    string rootOrSub = ParentTransaction == null ? "root" : "sub";
    string leafOrParent = SubTransaction == null ? "leaf" : "parent";
    return string.Format ("ClientTransaction ({0}, {1}) {2}", rootOrSub, leafOrParent, ID);
  }

  /// <summary>Initializes a new instance of this transaction.</summary>
  [Obsolete (
      "This member will be removed in the near future. Use ClientTransaction.CreateRootTransaction and CreateSubTransaction instead. (1.13.138)",
      false)]
  public ClientTransaction CreateEmptyTransactionOfSameType (bool copyInvalidObjectInformation)
  {
    var transactionFactory = _componentFactory.CreateCloneFactory ();
    var emptyTransactionOfSameType = transactionFactory (this);
    if (copyInvalidObjectInformation)
    {
      var invalidObjectReferences = _invalidDomainObjectManager.InvalidObjectIDs
          .Select (id => _invalidDomainObjectManager.GetInvalidObjectReference (id));
      foreach (var obj in invalidObjectReferences)
      {
        emptyTransactionOfSameType.EnlistDomainObject (obj);
        emptyTransactionOfSameType._invalidDomainObjectManager.MarkInvalid (obj);
      }
    }
    return emptyTransactionOfSameType;
  }

  protected internal void AddListener (IClientTransactionListener listener)
  {
    ArgumentUtility.CheckNotNull ("listener", listener);
    _eventBroker.AddListener (listener);
  }

  protected internal void RemoveListener (IClientTransactionListener listener)
  {
    ArgumentUtility.CheckNotNull ("listener", listener);
    _eventBroker.RemoveListener (listener);
  }

  protected void RaiseListenerEvent (Action<ClientTransaction, IClientTransactionListener> eventRaiser)
  {
    ArgumentUtility.CheckNotNull ("eventRaiser", eventRaiser);
    _eventBroker.RaiseEvent (eventRaiser);
  }

  /// <summary>
  /// Discards this transaction (rendering it unusable) and, if this transaction is a subtransaction, returns control to the parent transaction.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When a subtransaction is created via <see cref="CreateSubTransaction()"/>, the parent transaction is made read-only and cannot be
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
  public virtual void Discard ()
  {
    if (!_isDiscarded)
    {
      RaiseListenerEvent ((tx, l) => l.TransactionDiscard (tx));

      if (ParentTransaction != null)
      {
        ParentTransaction.IsActive = true;
        ParentTransaction._subTransaction = null;
      }

      _isDiscarded = true;
      AddListener (new InvalidatedTransactionListener());
    }
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
    get { return _enlistedDomainObjectManager.EnlistedDomainObjectCount; }
  }

  /// <summary>
  /// Gets all domain objects enlisted in this <see cref="ClientTransaction"/>.
  /// </summary>
  /// <value>The domain objects enlisted in this transaction.</value>
  /// <remarks>
  /// The <see cref="DataContainer"/>s of the returned objects might not have been loaded yet. In that case, they will be loaded on first
  /// access of the respective objects' properties, and this might trigger an <see cref="ObjectsNotFoundException"/> if the container cannot be loaded.
  /// </remarks>
  public IEnumerable<DomainObject> GetEnlistedDomainObjects ()
  {
    return _enlistedDomainObjectManager.GetEnlistedDomainObjects ();
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
  /// access of the object's properties, and this might trigger an <see cref="ObjectsNotFoundException"/> if the container cannot be loaded.
  /// </remarks>
  public DomainObject GetEnlistedDomainObject (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull ("objectID", objectID);
    return _enlistedDomainObjectManager.GetEnlistedDomainObject (objectID);
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
    return _enlistedDomainObjectManager.IsEnlisted (domainObject);
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
  /// <see cref="ObjectsNotFoundException"/>.</para>
  /// </remarks>
  /// <exception cref="InvalidOperationException">The domain object cannot be enlisted, e.g., because another <see cref="DomainObject"/> with the same
  /// <see cref="ObjectID"/> has already been associated with this transaction..</exception>
  /// <exception cref="ArgumentNullException">The <paramref name="domainObject"/> parameter is <see langword="null"/>.</exception>
  public bool EnlistDomainObject (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    
    CheckDomainObjectForEnlisting (domainObject);
    return _enlistedDomainObjectManager.EnlistDomainObject (domainObject);
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
  /// result in an <see cref="ObjectsNotFoundException"/>.</remarks>
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
  /// result in an <see cref="ObjectsNotFoundException"/>.</remarks>
  public void EnlistDomainObjects (params DomainObject[] domainObjects)
  {
    ArgumentUtility.CheckNotNull ("domainObjects", domainObjects);

    EnlistDomainObjects ((IEnumerable<DomainObject>) domainObjects);
  }

  /// <summary>
  /// Ensures that the data of the <see cref="DomainObject"/> with the given <see cref="ObjectID"/> has been loaded into this 
  /// <see cref="ClientTransaction"/>. If it hasn't, this method loads the object's data. If the object's data can't be found, an exception is thrown
  /// and the object is marked <see cref="StateType.Invalid"/> in the <see cref="ClientTransaction"/>.
  /// </summary>
  /// <param name="objectID">The domain object whose data must be loaded.</param>
  /// <exception cref="ArgumentNullException">The <paramref name="objectID"/> parameter is <see langword="null" />.</exception>
  /// <exception cref="ObjectInvalidException">The given <paramref name="objectID"/> is invalid in this transaction.</exception>
  /// <exception cref="ObjectsNotFoundException">
  /// The object could not be found in the data source. Note that the <see cref="ClientTransaction"/> marks
  /// not found objects as <see cref="StateType.Invalid"/>, so calling this API again witht he same <see cref="ObjectID"/> results in a 
  /// <see cref="ObjectInvalidException"/> being thrown.
  /// </exception>
  public void EnsureDataAvailable (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull ("objectID", objectID);

    _dataManager.GetDataContainerWithLazyLoad (objectID, throwOnNotFound: true);
  }

  /// <summary>
  /// Ensures that the data for the <see cref="DomainObject"/>s with the given <see cref="ObjectID"/> values has been loaded into this 
  /// <see cref="ClientTransaction"/>. If it hasn't, this method loads the objects' data, performing a bulk load operation.
  /// If an object's data can't be found, an exception is thrown, and the object is marked <see cref="StateType.Invalid"/> in the 
  /// <see cref="ClientTransaction"/>.
  /// </summary>
  /// <param name="objectIDs">The <see cref="ObjectID"/> values whose data must be loaded.</param>
  /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null" />.</exception>
  /// <exception cref="ClientTransactionsDifferException">One of the given <paramref name="objectIDs"/> cannot be used in this 
  /// <see cref="ClientTransaction"/>.</exception>
  /// <exception cref="ObjectInvalidException">One of the given <paramref name="objectIDs"/> is invalid in this transaction.</exception>
  /// <exception cref="ObjectsNotFoundException">
  /// One or more objects could not be found in the data source. Note that the <see cref="ClientTransaction"/> marks
  /// not found objects as <see cref="StateType.Invalid"/>, so calling this API again witht he same <see cref="ObjectID"/> results in a 
  /// <see cref="ObjectInvalidException"/> being thrown.
  /// </exception>
  public void EnsureDataAvailable (IEnumerable<ObjectID> objectIDs)
  {
    ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

    DataManager.GetDataContainersWithLazyLoad (objectIDs, throwOnNotFound: true);
  }

  /// <summary>
  /// Ensures that the data of the <see cref="DomainObject"/> with the given <see cref="ObjectID"/> has been loaded into this
  /// <see cref="ClientTransaction"/>. If it hasn't, this method loads the object's data. The method returns a value indicating whether the
  /// object's data was found. If an object's data can't be found, the object is marked <see cref="StateType.Invalid"/> in the 
  /// <see cref="ClientTransaction"/>.
  /// </summary>
  /// <param name="objectID">The domain object whose data must be loaded.</param>
  /// <returns><see langword="true" /> if the object's data is now available in the <see cref="ClientTransaction"/>, <see langword="false" /> if the 
  /// data couldn't be found.</returns>
  /// <exception cref="ArgumentNullException">The <paramref name="objectID"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="ObjectInvalidException">The given <paramref name="objectID"/> is invalid in this transaction.</exception>
  public bool TryEnsureDataAvailable (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull ("objectID", objectID);

    var dataContainer = DataManager.GetDataContainerWithLazyLoad (objectID, throwOnNotFound: false);
    return dataContainer != null;
  }

  /// <summary>
  /// Ensures that the data for the <see cref="DomainObject"/>s with the given <see cref="ObjectID"/> values has been loaded into this 
  /// <see cref="ClientTransaction"/>. If it hasn't, this method loads the objects' data, performing a bulk load operation.
  /// The method returns a value indicating whether the data of all the objects was found.
  /// If an object's data can't be found, the object is marked <see cref="StateType.Invalid"/> in the <see cref="ClientTransaction"/>.
  /// </summary>
  /// <param name="objectIDs">The <see cref="ObjectID"/> values whose data must be loaded.</param>
  /// <returns><see langword="true" /> if the data is now available in the <see cref="ClientTransaction"/> for all objects, <see langword="false" /> 
  /// if the data couldn't be found for one or more objects.</returns>
  /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null" />.</exception>
  /// <exception cref="ClientTransactionsDifferException">One of the given <paramref name="objectIDs"/> cannot be used in this 
  /// <see cref="ClientTransaction"/>.</exception>
  /// <exception cref="ObjectInvalidException">One of the given <paramref name="objectIDs"/> is invalid in this transaction.</exception>
  public bool TryEnsureDataAvailable (IEnumerable<ObjectID> objectIDs)
  {
    ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

    var dataContainers = DataManager.GetDataContainersWithLazyLoad (objectIDs, false);
    return dataContainers.All (dc => dc != null);
  }

  /// <summary>
  /// Creates a new <see cref="ObjectID"/> for the given class definition.
  /// </summary>
  /// <param name="classDefinition">The class definition to create a new <see cref="ObjectID"/> for.</param>
  /// <returns></returns>
  protected internal ObjectID CreateNewObjectID (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    return _persistenceStrategy.CreateNewObjectID (classDefinition);
  }

  /// <summary>
  /// Ensures that the data of the <see cref="IRelationEndPoint"/> with the given <see cref="RelationEndPointID"/> has been loaded into this 
  /// <see cref="ClientTransaction"/>. If it hasn't, this method loads the relation end point's data.
  /// </summary>
  /// <param name="endPointID">The <see cref="RelationEndPointID"/> of the end point whose data must be loaded. In order to force a collection-valued 
  /// relation property to be loaded, pass the <see cref="DomainObjectCollection.AssociatedEndPointID"/>.</param>
  /// <exception cref="ArgumentNullException">The <paramref name="endPointID"/> parameter is <see langword="null" />.</exception>
  public void EnsureDataComplete (RelationEndPointID endPointID)
  {
    var endPoint = DataManager.GetRelationEndPointWithLazyLoad (endPointID);
    endPoint.EnsureDataComplete();

    Assertion.IsTrue (endPoint.IsDataComplete);
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
  /// <exception cref="ObjectsNotFoundException">The <paramref name="domainObject"/> could not be found in either the current transaction or the
  /// <paramref name="sourceTransaction"/>.</exception>
  /// <exception cref="ObjectInvalidException">The <paramref name="domainObject"/> is invalid in either the current transaction or the
  /// <paramref name="sourceTransaction"/>.</exception>
  public void CopyCollectionEventHandlers (DomainObject domainObject, ClientTransaction sourceTransaction)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);
    ArgumentUtility.CheckNotNull ("sourceTransaction", sourceTransaction);

    foreach (PropertyAccessor property in domainObject.Properties.AsEnumerable (sourceTransaction))
    {
      if (property.PropertyData.Kind == PropertyKind.RelatedObjectCollection)
      {
        // access source property via RelationEndPointManager, we don't want to load any objects and we don't want to raise any events
        var endPointID = RelationEndPointID.Create(domainObject.ID, property.PropertyData.RelationEndPointDefinition);
        var sourceEndPoint = (ICollectionEndPoint) sourceTransaction.DataManager.GetRelationEndPointWithoutLoading (endPointID);
        if (sourceEndPoint != null)
        {
          var sourceRelatedObjectCollection = sourceEndPoint.Collection;
          
          var destinationCollectionEndPoint = (ICollectionEndPoint) DataManager.GetRelationEndPointWithLazyLoad (endPointID);
          var destinationRelatedObjectCollection = destinationCollectionEndPoint.Collection;
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
    return CreateSubTransaction ((parentTx, invalidDomainObjectManager, enlistedDomainObjectManager) =>
    {
      var componentFactory = SubClientTransactionComponentFactory.Create (parentTx, invalidDomainObjectManager, enlistedDomainObjectManager);
      return ObjectFactory.Create<ClientTransaction> (true, ParamList.Create (componentFactory));
    });
  }

  /// <summary>
  /// Initializes a new subtransaction with this <see cref="ClientTransaction"/> as its <see cref="ParentTransaction"/>. A custom transaction
  /// factory is used to instantiate the subtransaction. This allows subtransactions of types derived from <see cref="ClientTransaction"/>
  /// to be created. The factory must create a subtransaction whose <see cref="ParentTransaction"/> is this <see cref="ClientTransaction"/>, otherwise
  /// this method throws an exception.
  /// </summary>
  /// <param name="subTransactionFactory">A custom implementation of <see cref="IClientTransactionComponentFactory"/> to use when instantiating
  /// the subtransaction.</param>
  /// <remarks>
  /// <para>
  /// When a subtransaction is created, the parent transaction is automatically made read-only and cannot be modified until the subtransaction
  /// returns control to it via <see cref="Discard"/>. <see cref="Discard"/> is automatically called when a
  /// scope created by <see cref="EnterDiscardingScope"/> is left.
  /// </para>
  /// </remarks>
  public virtual ClientTransaction CreateSubTransaction (
      Func<ClientTransaction, IInvalidDomainObjectManager, IEnlistedDomainObjectManager, ClientTransaction> subTransactionFactory)
  {
    ArgumentUtility.CheckNotNull ("subTransactionFactory", subTransactionFactory);

    RaiseListenerEvent ((tx, l) => l.SubTransactionCreating (tx));

    IsActive = false;

    ClientTransaction subTransaction;
    try
    {
      subTransaction = subTransactionFactory (this, _invalidDomainObjectManager, _enlistedDomainObjectManager);
      if (subTransaction.ParentTransaction != this)
        throw new InvalidOperationException ("The given component factory did not create a sub-transaction for this transaction.");
    }
    catch
    {
      IsActive = true;
      throw;
    }

    _subTransaction = subTransaction;

    RaiseListenerEvent ((tx, l) => l.SubTransactionCreated (tx, subTransaction));
    return subTransaction;
  }

  /// <summary>
  /// Returns whether at least one <see cref="DomainObject"/> in this <b>ClientTransaction</b> has been changed.
  /// </summary>
  /// <returns><see langword="true"/> if at least one <see cref="DomainObject"/> in this <b>ClientTransaction</b> has been changed; otherwise, <see langword="false"/>.</returns>
  public virtual bool HasChanged ()
  {
    return _commitRollbackAgent.HasDataChanged();
  }

  /// <summary>
  /// Commits all changes within the <b>ClientTransaction</b> to the underlying data source.
  /// </summary>
  /// <exception cref="Persistence.PersistenceException">Changes to objects from multiple storage providers were made.</exception>
  /// <exception cref="Persistence.StorageProviderException">An error occurred while committing the changes to the data source.</exception>
  /// <remarks>
  /// <para>
  /// Committing a <see cref="ClientTransaction"/> raises a number of events:
  /// <list type="number">
  /// <item><description>
  /// First, a chain of Commtting events is raised. Each Committing event can cancel the <see cref="Commit"/> operation by throwing an exception 
  /// (which, after canceling the operation, will be propagated to the caller). Committing event handlers can also modify each 
  /// <see cref="DomainObject"/> being committed, and they can add or remove objects to or from the commit set. For example, if a Committing event
  /// handler modifies a changed object so that it becomes <see cref="StateType.Unchanged"/>, that object will be removed from the commit set.
  /// Or, if a handler modifies an unchanged object so that it becomes <see cref="StateType.Changed"/>, it will become part of the commit set.
  /// When a set of objects (a, b) is committed, the Committing event chain consists of the following events, raised in order:
  /// <list type="number">
  /// <item><description>
  /// <see cref="IClientTransactionListener"/>.<see cref="IClientTransactionListener.TransactionCommitting"/> and 
  /// <see cref="IClientTransactionExtension"/>.<see cref="IClientTransactionExtension.Committing"/> for (a, b)</description></item>
  /// <item><description><see cref="ClientTransaction"/>.<see cref="ClientTransaction.Committing"/> for (a, b)</description></item>
  /// <item><description>a.<see cref="DomainObject.Committing"/>, b.<see cref="DomainObject.Committing"/> (order undefined)</description></item>
  /// </list>
  /// Usually, every event handler in the Committing event chain receives each object in the commit set exactly once. (See 
  /// <see cref="ICommittingEventRegistrar.RegisterForAdditionalCommittingEvents"/> in order to repeat the Committing events for an object.)
  /// If any event handler adds an object c to the commit set (e.g., by changing or creating it), the whole chain is repeated, but only for c.
  /// </description></item>
  /// <item><description>
  /// Then, <see cref="IClientTransactionListener"/>.<see cref="IClientTransactionListener.TransactionCommitValidate"/> and 
  /// <see cref="IClientTransactionExtension"/>.<see cref="IClientTransactionExtension.CommitValidate"/> are raised for the commit set.
  /// The event handlers for those events get the commit set exactly as it is saved to the underlying data store (or parent transaction) and are 
  /// allowed to cancel the operation by throwing an exception, e.g., if a validation rule fails. The event handlers must not modify the 
  /// <see cref="ClientTransaction"/>'s state (including that of any <see cref="DomainObject"/> in the transaction) in any way.
  /// </description></item>
  /// <item><description>
  /// Then, the data is saved to the underlying data store or parent transaction. The data store or parent transaction may cancel the operation
  /// by throwing an exception (e.g., a <see cref="ConcurrencyViolationException"/> or a database-level exception).
  /// </description></item>
  /// <item><description>
  /// Finally, if the <see cref="Commit"/> operation was completed successfully, a chain of Committed events is raised. Committed event handlers must
  /// not throw an exception. When a set of objects (a, b) was committed, the Committed event chain consists of the following events, raised in order:
  /// <list type="number">
  /// <item><description>a.<see cref="DomainObject.Committed"/>, b.<see cref="DomainObject.Committed"/> (order undefined)</description></item>
  /// <item><description><see cref="ClientTransaction"/>.<see cref="ClientTransaction.Committed"/> for (a, b)</description></item>
  /// <item><description>
  /// <see cref="IClientTransactionListener"/>.<see cref="IClientTransactionListener.TransactionCommitted"/> and 
  /// <see cref="IClientTransactionExtension"/>.<see cref="IClientTransactionExtension.Committed"/> for (a, b)
  /// </description></item>
  /// </list>
  /// </description></item>
  /// </list>
  /// </para>
  /// </remarks>
  public virtual void Commit ()
  {
    using (EnterNonDiscardingScope ())
    {
      _commitRollbackAgent.CommitData();
    }
  }

  /// <summary>
  /// Performs a rollback of all changes within the <b>ClientTransaction</b>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Rolling back a <see cref="ClientTransaction"/> raises a number of events:
  /// <list type="number">
  /// <item><description>
  /// First, a chain of RollingBack events is raised. Each RollingBack event can cancel the <see cref="Rollback"/> operation by throwing an exception 
  /// (which, after canceling the operation, will be propagated to the caller). RollingBack event handlers can also modify each 
  /// <see cref="DomainObject"/> being rolled back, and they can add or remove objects to or from the rollback set. For example, if a RollingBack event
  /// handler modifies a changed object so that it becomes <see cref="StateType.Unchanged"/>, that object will no longer need to be rolled back.
  /// Or, if a handler modifies an unchanged object so that it becomes <see cref="StateType.Changed"/>, it will become part of the rollback set.
  /// When a set of objects (a, b) is rolled back, the RollingBack event chain consists of the following events, raised in order:
  /// <list type="number">
  /// <item><description>
  /// <see cref="IClientTransactionListener"/>.<see cref="IClientTransactionListener.TransactionRollingBack"/> and 
  /// <see cref="IClientTransactionExtension"/>.<see cref="IClientTransactionExtension.RollingBack"/> for (a, b)</description></item>
  /// <item><description><see cref="ClientTransaction"/>.<see cref="ClientTransaction.RollingBack"/> for (a, b)</description></item>
  /// <item><description>a.<see cref="DomainObject.RollingBack"/>, b.<see cref="DomainObject.RollingBack"/> (order undefined)</description></item>
  /// </list>
  /// Every event handler in the RollingBack event chain receives each object in the rollback set exactly once.
  /// If any event handler adds an object c to the rollback set (e.g., by changing or creating it), the whole chain is repeated, but only for c.
  /// </description></item>
  /// <item><description>
  /// Then, the data is rolled back.
  /// </description></item>
  /// <item><description>
  /// Finally, if the <see cref="Rollback"/> operation was completed successfully, a chain of RolledBack events is raised. RolledBack event handlers must
  /// not throw an exception. When a set of objects (a, b) was rolled back, the RolledBack event chain consists of the following events, raised in order:
  /// <list type="number">
  /// <item><description>a.<see cref="DomainObject.RolledBack"/>, b.<see cref="DomainObject.RolledBack"/> (order undefined)</description></item>
  /// <item><description><see cref="ClientTransaction"/>.<see cref="ClientTransaction.RolledBack"/> for (a, b)</description></item>
  /// <item><description>
  /// <see cref="IClientTransactionListener"/>.<see cref="IClientTransactionListener.TransactionRolledBack"/> and 
  /// <see cref="IClientTransactionExtension"/>.<see cref="IClientTransactionExtension.RolledBack"/> for (a, b)
  /// </description></item>
  /// </list>
  /// </description></item>
  /// </list>
  /// </para>
  /// </remarks>
  public virtual void Rollback ()
  {
    using (EnterNonDiscardingScope ())
    {
      _commitRollbackAgent.RollbackData();
    }
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the data source. If the object's data can't be found, an 
  /// exception is thrown, and the object is marked <see cref="StateType.Invalid"/> in the <see cref="ClientTransaction"/>.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
  /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
  /// <exception cref="ObjectsNotFoundException">
  /// The object could not be found in the data source. Note that the <see cref="ClientTransaction"/> marks
  /// not found objects as <see cref="StateType.Invalid"/>, so calling this API again witht he same <see cref="ObjectID"/> results in a 
  /// <see cref="ObjectInvalidException"/> being thrown.
  /// </exception>
  /// <exception cref="ObjectInvalidException">The object is invalid in this transaction.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the data source.
  /// </exception>
  /// <exception cref="ObjectDeletedException">The object has already been deleted and the <paramref name="includeDeleted"/> flag is 
  /// <see langword="false" />.</exception>
  protected internal virtual DomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    if (IsInvalid (id))
      throw new ObjectInvalidException (id);

    var dataContainer = _dataManager.GetDataContainerWithLazyLoad (id, throwOnNotFound: true);
    
    if (dataContainer.State == StateType.Deleted && !includeDeleted)
      throw new ObjectDeletedException (id);

    return dataContainer.DomainObject;
  }

  /// <summary>
  /// Gets an object that is already loaded (even if its marked <see cref="StateType.Invalid"/>) or attempts to load them from the data source. 
  /// If an object cannot be found, it will be marked <see cref="StateType.Invalid"/> in the <see cref="ClientTransaction"/>, and the method will
  /// return a <see langword="null" /> reference in its place.
  /// </summary>
  /// <param name="objectID">The ID of the object to be retrieved.</param>
  /// <returns>
  /// The <see cref="DomainObject"/> with the specified <paramref name="objectID"/>, or <see langword="null" /> if it couldn't be found.
  /// </returns>
  /// <exception cref="ArgumentNullException">The <paramref name="objectID"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="objectID"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the data source.
  /// </exception>
  protected internal virtual DomainObject TryGetObject (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull ("objectID", objectID);

    if (IsInvalid (objectID))
      return GetInvalidObjectReference (objectID);

    var dataContainer = _dataManager.GetDataContainerWithLazyLoad (objectID, throwOnNotFound: false);
    if (dataContainer == null)
      return null;

    return dataContainer.DomainObject;
  }

  /// <summary>
  /// Gets a reference to a <see cref="DomainObject"/> with the given <see cref="ObjectID"/> from this <see cref="ClientTransaction"/>. If the
  /// transaction does not currently hold an object with this <see cref="ObjectID"/>, an object reference representing that <see cref="ObjectID"/> 
  /// is created without calling a constructor and without loading the object's data from the data source. This method does not check whether an
  /// object with the given <see cref="ObjectID"/> actually exists in the data source, and it will also return invalid or deleted objects.
  /// </summary>
  /// <param name="objectID">The <see cref="ObjectID"/> to get an object reference for.</param>
  /// <returns>An object with the given <see cref="ObjectID"/>, possibly in <see cref="StateType.NotLoadedYet"/>, <see cref="StateType.Deleted"/>,
  /// or <see cref="StateType.Invalid"/> state.</returns>
  /// <remarks>
  /// <para>
  /// When an object with the given <paramref name="objectID"/> has already been enlisted in the transaction, that object is returned. Otherwise,
  /// an object in <see cref="StateType.NotLoadedYet"/> state is created and enlisted without loading its data from the data source. In such a case,
  /// the object's data is loaded when it's first needed; e.g., when one of its properties is accessed or when 
  /// <see cref="EnsureDataAvailable(Remotion.Data.DomainObjects.ObjectID)"/> is called for its <see cref="ObjectID"/>. At that point, an
  /// <see cref="ObjectsNotFoundException"/> may be triggered when the object's data cannot be found.
  /// </para>
  /// </remarks>
  /// <exception cref="ArgumentNullException">The <paramref name="objectID"/> parameter is <see langword="null" />.</exception>
  protected internal virtual DomainObject GetObjectReference (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull ("objectID", objectID);

    if (IsInvalid (objectID))
      return GetInvalidObjectReference (objectID);

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

  /// <summary>
  /// Gets a reference to a <see cref="DomainObject"/> that is currently in <see cref="StateType.Invalid"/> state. If the object is not actually
  /// invalid (check with <see cref="IsInvalid"/>), an exception is throws.
  /// </summary>
  /// <param name="objectID">The object ID to get the <see cref="DomainObject"/> reference for.</param>
  /// <returns>An object with the given <see cref="ObjectID"/> in <see cref="StateType.Invalid"/> state.</returns>
  /// <exception cref="InvalidOperationException">The object is not currently in <see cref="StateType.Invalid"/> state.</exception>
  protected internal virtual DomainObject GetInvalidObjectReference (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull ("objectID", objectID);
    return _invalidDomainObjectManager.GetInvalidObjectReference (objectID);
  }

  /// <summary>
  /// Determines whether the specified <see cref="ObjectID"/> has been marked invalid in the scope of this <see cref="ClientTransaction"/>.
  /// </summary>
  /// <param name="objectID">The <see cref="ObjectID"/> to check.</param>
  /// <returns>
  /// 	<see langword="true"/> if the specified <paramref name="objectID"/> is invalid; otherwise, <see langword="false"/>.
  /// </returns>
  public bool IsInvalid (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull ("objectID", objectID);
    return _invalidDomainObjectManager.IsInvalid (objectID);
  }

  protected internal virtual DomainObject NewObject (Type domainObjectType, ParamList constructorParameters)
  {
    ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);
    ArgumentUtility.CheckNotNull ("constructorParameters", constructorParameters);

    using (EnterNonDiscardingScope ())
    {
      RaiseListenerEvent ((tx, l) => l.NewObjectCreating (tx, domainObjectType));

      var creator = MappingConfiguration.Current.GetTypeDefinition (domainObjectType).GetDomainObjectCreator ();
      var ctorInfo = creator.GetConstructorLookupInfo (domainObjectType);

      var instance = (DomainObject) constructorParameters.InvokeConstructor (ctorInfo);
      DomainObjectMixinCodeGenerationBridge.OnDomainObjectCreated (instance);
      return instance;
    }
  }

  /// <summary>
  /// Gets a number of objects that are already loaded or attempts to load them from the data source.
  /// If an object's data can't be found, an exception is thrown, and the object is marked <see cref="StateType.Invalid"/> in the 
  /// <see cref="ClientTransaction"/>.
  /// </summary>
  /// <typeparam name="T">The type of objects expected to be returned. Specify <see cref="DomainObject"/> if no specific type is expected.</typeparam>
  /// <param name="objectIDs">The IDs of the objects to be retrieved.</param>
  /// <returns>A list of objects of type <typeparamref name="T"/> corresponding to (and in the same order as) the IDs specified in 
  /// <paramref name="objectIDs"/>. This list might include deleted objects.</returns>
  /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="InvalidCastException">One of the retrieved objects doesn't fit the expected type <typeparamref name="T"/>.</exception>
  /// <exception cref="ObjectInvalidException">One of the retrieved objects is invalid in this transaction.</exception>
  /// <exception cref="ObjectsNotFoundException">
  /// One or more objects could not be found in the data source. Note that the <see cref="ClientTransaction"/> marks
  /// not found objects as <see cref="StateType.Invalid"/>, so calling this API again witht he same <see cref="ObjectID"/> results in a 
  /// <see cref="ObjectInvalidException"/> being thrown.
  /// </exception>
  public T[] GetObjects<T> (params ObjectID[] objectIDs) 
      where T : DomainObject
  {
    return GetObjects<T> ((IEnumerable<ObjectID>) objectIDs);
  }

  /// <summary>
  /// Gets a number of objects that are already loaded or attempts to load them from the data source.
  /// If an object's data can't be found, an exception is thrown, and the object is marked <see cref="StateType.Invalid"/> in the 
  /// <see cref="ClientTransaction"/>.
  /// </summary>
  /// <typeparam name="T">The type of objects expected to be returned. Specify <see cref="DomainObject"/> if no specific type is expected.</typeparam>
  /// <param name="objectIDs">The IDs of the objects to be retrieved.</param>
  /// <returns>A list of objects of type <typeparamref name="T"/> corresponding to (and in the same order as) the IDs specified in 
  /// <paramref name="objectIDs"/>. This list might include deleted objects.</returns>
  /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="InvalidCastException">One of the retrieved objects doesn't fit the expected type <typeparamref name="T"/>.</exception>
  /// <exception cref="ObjectInvalidException">One of the retrieved objects is invalid in this transaction.</exception>
  /// <exception cref="ObjectsNotFoundException">
  /// One or more objects could not be found in the data source. Note that the <see cref="ClientTransaction"/> marks
  /// not found objects as <see cref="StateType.Invalid"/>, so calling this API again witht he same <see cref="ObjectID"/> results in a 
  /// <see cref="ObjectInvalidException"/> being thrown.
  /// </exception>

  public T[] GetObjects<T> (IEnumerable<ObjectID> objectIDs)
      where T : DomainObject
  {
    ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

    // this performs a bulk load operation, throwing on invalid IDs and unknown objects
    return DataManager.GetDataContainersWithLazyLoad (objectIDs, throwOnNotFound: true)
        .Select (dc => dc.DomainObject)
        .Cast<T> ()
        .ToArray ();
  }

  /// <summary>
  /// Gets a number of objects that are already loaded (including invalid objects) or attempts to load them from the data source. 
  /// If an object cannot be found, it will be marked <see cref="StateType.Invalid"/> in the <see cref="ClientTransaction"/>, and the result array will 
  /// contain a <see langword="null" /> reference in its place.
  /// </summary>
  /// <typeparam name="T">The type of objects expected to be returned. Specify <see cref="DomainObject"/> if no specific type is expected.</typeparam>
  /// <param name="objectIDs">The IDs of the objects to be retrieved.</param>
  /// <returns>A list of objects of type <typeparamref name="T"/> corresponding to (and in the same order as) the IDs specified in 
  /// <paramref name="objectIDs"/>. This list can contain invalid and <see langword="null" /> <see cref="DomainObject"/> references.</returns>
  /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="InvalidCastException">One of the retrieved objects doesn't fit the specified type <typeparamref name="T"/>.</exception>
  public T[] TryGetObjects<T> (params ObjectID[] objectIDs) 
      where T : DomainObject
  {
    return TryGetObjects<T> ((IEnumerable<ObjectID>) objectIDs);
  }

  /// <summary>
  /// Gets a number of objects that are already loaded (including invalid objects) or attempts to load them from the data source. 
  /// If an object cannot be found, it will be marked <see cref="StateType.Invalid"/> in the <see cref="ClientTransaction"/>, and the result array will 
  /// contain a <see langword="null" /> reference in its place.
  /// </summary>
  /// <typeparam name="T">The type of objects expected to be returned. Specify <see cref="DomainObject"/> if no specific type is expected.</typeparam>
  /// <param name="objectIDs">The IDs of the objects to be retrieved.</param>
  /// <returns>A list of objects of type <typeparamref name="T"/> corresponding to (and in the same order as) the IDs specified in 
  /// <paramref name="objectIDs"/>. This list can contain invalid and <see langword="null" /> <see cref="DomainObject"/> references.</returns>
  /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="InvalidCastException">One of the retrieved objects doesn't fit the specified type <typeparamref name="T"/>.</exception>
  public T[] TryGetObjects<T> (IEnumerable<ObjectID> objectIDs)
      where T : DomainObject
  {
    ArgumentUtility.CheckNotNull ("objectIDs", objectIDs);

    var objectIDsAsCollection = objectIDs.ConvertToCollection();
    
    var validObjectIDs = objectIDsAsCollection.Where (id => !IsInvalid (id)).ConvertToCollection ();
    
    // this performs a bulk load operation
    var dataContainersByID = validObjectIDs.Zip (DataManager.GetDataContainersWithLazyLoad (validObjectIDs, false)).ToDictionary (t => t.Item1, t => t.Item2);

    var result = objectIDsAsCollection.Select (
        id =>
        {
          DataContainer loadResult;
          if (dataContainersByID.TryGetValue (id, out loadResult))
            return loadResult == null ? null : (T) loadResult.DomainObject;
          else
          {
            Assertion.IsTrue (
                IsInvalid (id), "All valid IDs have been passed to EnsureDataAvailable, so if its not in the loadResult, it must be invalid.");
            return (T) GetInvalidObjectReference (id);
          }
        });
    return result.ToArray ();
  }

  /// <summary>
  /// Gets the related object of a given <see cref="RelationEndPointID"/>.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="RelationEndPointID"/> to evaluate. It must refer to a <see cref="ObjectEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the current related object.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="relationEndPointID"/> does not refer to an <see cref="ObjectEndPoint"/></exception>
  protected internal virtual DomainObject GetRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    if (relationEndPointID.Definition.Cardinality != CardinalityType.One)
      throw new ArgumentException ("The given end-point ID does not denote a related object (cardinality one).", "relationEndPointID");

    using (EnterNonDiscardingScope ())
    {
      DomainObject domainObject = GetObject (relationEndPointID.ObjectID, true);

      RaiseListenerEvent ((tx, l) => l.RelationReading (tx, domainObject, relationEndPointID.Definition, ValueAccess.Current));

      var objectEndPoint = (IObjectEndPoint) DataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
      DomainObject relatedObject = objectEndPoint.GetOppositeObject ();
      RaiseListenerEvent ((tx, l) => l.RelationRead (tx, domainObject, relationEndPointID.Definition, relatedObject, ValueAccess.Current));

      return relatedObject;
    }
  }

  /// <summary>
  /// Gets the original related object of a given <see cref="RelationEndPointID"/> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="RelationEndPointID"/> to evaluate. It must refer to a <see cref="ObjectEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the original related object.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="relationEndPointID"/> does not refer to an <see cref="ObjectEndPoint"/></exception>
  protected internal virtual DomainObject GetOriginalRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    if (relationEndPointID.Definition.Cardinality != CardinalityType.One)
      throw new ArgumentException ("The given end-point ID does not denote a related object (cardinality one).", "relationEndPointID");

    using (EnterNonDiscardingScope ())
    {
      DomainObject domainObject = GetObject (relationEndPointID.ObjectID, true);

      RaiseListenerEvent ((tx, l) => l.RelationReading (tx, domainObject, relationEndPointID.Definition, ValueAccess.Original));
      var objectEndPoint = (IObjectEndPoint) _dataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
      DomainObject relatedObject = objectEndPoint.GetOriginalOppositeObject ();
      RaiseListenerEvent ((tx, l) => l.RelationRead (tx, domainObject, relationEndPointID.Definition, relatedObject, ValueAccess.Original));

      return relatedObject;
    }
  }

  /// <summary>
  /// Gets the related objects of a given <see cref="RelationEndPointID"/>.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="RelationEndPointID"/> to evaluate. It must refer to a <see cref="CollectionEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing the current related objects.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="relationEndPointID"/> does not refer to a <see cref="CollectionEndPoint"/></exception>
  protected internal virtual DomainObjectCollection GetRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    if (relationEndPointID.Definition.Cardinality != CardinalityType.Many)
      throw new ArgumentException ("The given end-point ID does not denote a related object collection (cardinality many).", "relationEndPointID");

    using (EnterNonDiscardingScope ())
    {
      DomainObject domainObject = GetObject (relationEndPointID.ObjectID, true);

      RaiseListenerEvent ((tx, l) => l.RelationReading (tx, domainObject, relationEndPointID.Definition, ValueAccess.Current));

      var collectionEndPoint = (ICollectionEndPoint) _dataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
      var relatedObjects = collectionEndPoint.Collection;
      var readOnlyRelatedObjects = new ReadOnlyDomainObjectCollectionAdapter<DomainObject> (relatedObjects);
      RaiseListenerEvent ((tx, l) => l.RelationRead (tx, domainObject, relationEndPointID.Definition, readOnlyRelatedObjects, ValueAccess.Current));

      return relatedObjects;
    }
  }

  /// <summary>
  /// Gets the original related objects of a given <see cref="RelationEndPointID"/> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="RelationEndPointID"/> to evaluate. It must refer to a <see cref="CollectionEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>A <see cref="DomainObjectCollection"/> containing the original related objects.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="relationEndPointID"/> does not refer to a <see cref="CollectionEndPoint"/></exception>
  protected internal virtual DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull ("relationEndPointID", relationEndPointID);

    if (relationEndPointID.Definition.Cardinality != CardinalityType.Many)
      throw new ArgumentException ("The given end-point ID does not denote a related object collection (cardinality many).", "relationEndPointID");

    using (EnterNonDiscardingScope ())
    {
      DomainObject domainObject = GetObject (relationEndPointID.ObjectID, true);

      RaiseListenerEvent ((tx, l) => l.RelationReading (tx, domainObject, relationEndPointID.Definition, ValueAccess.Original));
      var collectionEndPoint = (ICollectionEndPoint) _dataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
      var relatedObjects = collectionEndPoint.GetCollectionWithOriginalData();
      var readOnlyRelatedObjects = new ReadOnlyDomainObjectCollectionAdapter<DomainObject> (relatedObjects);
      RaiseListenerEvent ((tx, l) => l.RelationRead (tx, domainObject, relationEndPointID.Definition, readOnlyRelatedObjects, ValueAccess.Original));

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

    var command = _dataManager.CreateDeleteCommand (domainObject);
    var fullCommand = command.ExpandToAllRelatedObjects ();
    fullCommand.NotifyAndPerform ();
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
  protected internal virtual void OnCommitting (ClientTransactionCommittingEventArgs args)
  {
    ArgumentUtility.CheckNotNull ("args", args);

    if (Committing != null)
      Committing (this, args);
  }


  /// <summary>
  /// Raises the <see cref="Committed"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected internal virtual void OnCommitted (ClientTransactionEventArgs args)
  {
    ArgumentUtility.CheckNotNull ("args", args);

    if (Committed != null)
      Committed (this, args);
  }

  /// <summary>
  /// Raises the <see cref="RollingBack"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected internal virtual void OnRollingBack (ClientTransactionEventArgs args)
  {
    ArgumentUtility.CheckNotNull ("args", args);

    if (RollingBack != null)
      RollingBack (this, args);
  }

  /// <summary>
  /// Raises the <see cref="RolledBack"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected internal virtual void OnRolledBack (ClientTransactionEventArgs args)
  {
    ArgumentUtility.CheckNotNull ("args", args);

    if (RolledBack != null)
      RolledBack (this, args);
  }

  /// <summary>
  /// Raises the <see cref="SubTransactionCreated"/> event.
  /// </summary>
  /// <param name="eventArgs">A <see cref="Remotion.Data.DomainObjects.SubTransactionCreatedEventArgs"/> instance containing the event data.</param>
  protected internal virtual void OnSubTransactionCreated (SubTransactionCreatedEventArgs eventArgs)
  {
    ArgumentUtility.CheckNotNull ("eventArgs", eventArgs);
    
    if (SubTransactionCreated != null)
      SubTransactionCreated (this, eventArgs);
  }

  /// <summary>
  /// Gets the <see cref="IDataManager"/> of this <see cref="ClientTransaction"/>.
  /// </summary>
  protected internal IDataManager DataManager
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

  public virtual ITransaction ToITransation ()
  {
    return new ClientTransactionWrapper (this);
  }

  // ReSharper disable UnusedParameter.Global
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

  [Obsolete ("This method is now obsolete, use DataManager.HasRelationChanged instead. (1.13.62)", true)]
  protected internal virtual bool HasRelationChanged (DomainObject domainObject)
  {
    throw new NotImplementedException ();
  }

  [Obsolete ("This method has been moved, use DataManager.GetDataContainerWitLazyLoad instead. (1.13.62)", true)]
  protected internal DataContainer GetDataContainer (DomainObject domainObject)
  {
    throw new NotImplementedException();
  }

  [Obsolete ("This method has been obsoleted. To intercept the loading of objects, replace the IObjectLoader of the transaction when its created.", true)]
  protected virtual DomainObject LoadObject (ObjectID id)
  {
    throw new NotImplementedException ();
  }

  [Obsolete ("This method has been obsoleted. To intercept the loading of objects, replace the IObjectLoader of the transaction when its created.", true)]
  protected virtual DomainObject[] LoadObjects (IList<ObjectID> idsToBeLoaded, bool throwOnNotFound)
  {
    throw new NotImplementedException ();
  }

  [Obsolete ("This method has been obsoleted. To intercept the loading of objects, replace the IObjectLoader of the transaction when its created.", true)]
  protected internal virtual DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID)
  {
    throw new NotImplementedException ();
  }

  [Obsolete ("This method has been obsoleted. To intercept the loading of objects, replace the IObjectLoader of the transaction when its created.", true)]
  protected internal virtual DomainObject[] LoadRelatedObjects (RelationEndPointID relationEndPointID)
  {
    throw new NotImplementedException ();
  }
  // ReSharper restore UnusedParameter.Global
}
}
