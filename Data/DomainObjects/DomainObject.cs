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
using System.Diagnostics;
using System.Runtime.Serialization;
using Remotion.Collections;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Base class for all objects that are persisted by the framework.
  /// </summary>
  /// <remarks>
  /// If a class implementing <see cref="ISerializable"/> is derived from this base class, it must provide a deserialization constructor invoking
  /// this class' deserialization constructor, and it must call <see cref="BaseGetObjectData"/> from the <see cref="ISerializable.GetObjectData"/>
  /// implementation.
  /// </remarks>
  [Serializable]
  [DebuggerDisplay ("{GetPublicDomainObjectType().FullName}: {ID.ToString()}")]
  public class DomainObject
  {
    // types

    // static members and constants

    #region Creation and GetObject factory methods

    /// <summary>
    /// Returns an invocation object creating a new instance of a concrete domain object for the current <see cref="DomainObjects.ClientTransaction"/>.
    /// </summary>
    /// <typeparam name="T">The concrete type to be implemented by the object.</typeparam>
    /// <returns>An <see cref="IFuncInvoker{T}"/> object used to create a new domain object instance.</returns>
    /// <remarks>
    /// <para>
    /// This method's return value is an <see cref="IFuncInvoker{T}"/> object, which can be used to specify the required constructor and 
    /// pass it the necessary arguments in order to create a new domain object. Depending on the mapping being used by the object, one of two
    /// methods of object creation is used: legacy or via factory.
    /// </para>
    /// <para>
    /// Legacy objects are created by simply invoking the constructor matching the arguments passed to the <see cref="FuncInvoker{T}"/>
    /// object returned by this method.
    /// </para>
    /// <para>
    /// Objects created by the factory are not directly instantiated; instead a proxy is dynamically created for performing management tasks.
    /// </para>
    /// <para>This method should not be directly invoked by a user, but instead by static factory methods of classes derived from
    /// <see cref="DomainObject"/>.</para>
    /// <para>For more information, also see the constructor documentation (<see cref="DomainObject()"/>).</para>
    /// </remarks>
    /// <seealso cref="DomainObject()"/>
    /// <exception cref="ArgumentException">The type <typeparamref name="T"/> cannot be extended to a proxy, for example because it is sealed
    /// or abstract and non-instantiable.</exception>
    /// <exception cref="MissingMethodException">The given type <typeparamref name="T"/> does not implement the required protected
    /// constructor (see Remarks section).
    /// </exception>
    protected static IFuncInvoker<T> NewObject<T> () where T: DomainObject
    {
      return RepositoryAccessor.GetCreator (typeof (T)).GetTypesafeConstructorInvoker<T>();
    }

    /// <summary>
    /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
    /// </summary>
    /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
    /// <typeparam name="T">The expected type of the concrete <see cref="DomainObject"/></typeparam>
    /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    /// <exception cref="Persistence.StorageProviderException">
    ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
    ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
    ///   An error occurred while accessing the datasource.
    /// </exception>
    /// <exception cref="MissingMethodException">The concrete <see cref="DomainObject"/> doesn't implement the required constructor.</exception>
    /// <exception cref="InvalidCastException">The loaded <see cref="DomainObject"/> is not of the expected type <typeparamref name="T"/>.</exception>
    protected static T GetObject<T> (ObjectID id) where T: DomainObject
    {
      return GetObject<T> (id, false);
    }

    /// <summary>
    /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the datasource.
    /// </summary>
    /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
    /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
    /// <typeparam name="T">The expected type of the concrete <see cref="DomainObject"/></typeparam>
    /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    /// <exception cref="Persistence.StorageProviderException">
    ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
    ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
    ///   An error occurred while accessing the datasource.
    /// </exception>
    /// <exception cref="MissingMethodException">The concrete <see cref="DomainObject"/> doesn't implement the required constructor.</exception>
    /// <exception cref="InvalidCastException">The loaded <see cref="DomainObject"/> is not of the expected type <typeparamref name="T"/>.</exception>
    protected static T GetObject<T> (ObjectID id, bool includeDeleted) where T: DomainObject
    {
      ArgumentUtility.CheckNotNull ("id", id);
      return (T) RepositoryAccessor.GetObject (id, includeDeleted);
    }

    #endregion

    // Returns a strategy object for creating instances of the given domain object type.

    // member fields

    /// <summary>
    /// Occurs before a <see cref="PropertyValue"/> of the <see cref="DomainObject"/> is changed.
    /// </summary>
    /// <remarks>
    /// This event does not fire when a <see cref="PropertyValue"/> has been changed due to a relation change.
    /// </remarks>
    /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
    public event PropertyChangeEventHandler PropertyChanging;

    /// <summary>
    /// Occurs after a <see cref="PropertyValue"/> of the <see cref="DomainObject"/> is changed.
    /// </summary>
    /// <remarks>
    /// This event does not fire when a <see cref="PropertyValue"/> has been changed due to a relation change.
    /// </remarks>
    /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
    public event PropertyChangeEventHandler PropertyChanged;

    /// <summary>
    /// Occurs before a Relation of the <see cref="DomainObject"/> is changed.
    /// </summary>
    /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
    public event RelationChangingEventHandler RelationChanging;

    /// <summary>
    /// Occurs after a Relation of the <see cref="DomainObject"/> has been changed.
    /// </summary>
    /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
    public event RelationChangedEventHandler RelationChanged;

    /// <summary>
    /// Occurs before the <see cref="DomainObject"/> is deleted.
    /// </summary>
    /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
    public event EventHandler Deleting;

    /// <summary>
    /// Occurs after the <see cref="DomainObject"/> has been deleted.
    /// </summary>
    /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
    public event EventHandler Deleted;

    /// <summary>
    /// Occurs before the changes of a <see cref="DomainObject"/> are committed.
    /// </summary>
    /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
    public event EventHandler Committing;

    /// <summary>
    /// Occurs after the changes of a <see cref="DomainObject"/> are successfully committed.
    /// </summary>
    /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
    public event EventHandler Committed;

    /// <summary>
    /// Occurs before the changes of a <see cref="DomainObject"/> are rolled back.
    /// </summary>
    /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
    public event EventHandler RollingBack;

    /// <summary>
    /// Occurs after the changes of a <see cref="DomainObject"/> are successfully rolled back.
    /// </summary>
    /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
    public event EventHandler RolledBack;

    private ObjectID _id;
    private bool _initialConstructionEventSignalled = false;
    private ClientTransaction _bindingTransaction; // null unless this object is bound to a fixed transaction

    // construction and disposing

    /// <summary>
    /// Initializes a new <see cref="DomainObject"/> with the current <see cref="DomainObjects.ClientTransaction"/>.
    /// </summary>
    /// <remarks>Any constructors implemented on concrete domain objects should delegate to this base constructor. As domain objects generally should 
    /// not be constructed via the
    /// <c>new</c> operator, these constructors must remain protected, and the concrete domain objects should have a static "NewObject" method,
    /// which delegates to <see cref="NewObject{T}"/>, passing it the required constructor arguments.</remarks>
    protected DomainObject ()
    {
      Type publicDomainObjectType = GetPublicDomainObjectType();

      ClientTransactionScope.CurrentTransaction.TransactionEventSink.NewObjectCreating (publicDomainObjectType, this);
      DataContainer firstDataContainer = ClientTransactionScope.CurrentTransaction.CreateNewDataContainer (publicDomainObjectType);
      firstDataContainer.SetDomainObject (this);

      InitializeFromDataContainer (firstDataContainer);
      _initialConstructionEventSignalled = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainObject"/> class in the process of deserialization.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> coming from the .NET serialization infrastructure.</param>
    /// <param name="context">The <see cref="StreamingContext"/> coming from the .NET serialization infrastructure.</param>
    /// <remarks>Be sure to call this base constructor from the deserialization constructor of any concrete <see cref="DomainObject"/> type
    /// implementing the <see cref="ISerializable"/> interface.</remarks>
    protected DomainObject (SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      try
      {
        _id = (ObjectID) info.GetValue ("DomainObject.ID", typeof (ObjectID));
        _bindingTransaction = (ClientTransaction) info.GetValue ("DomainObject._bindingTransaction", typeof (ClientTransaction));
        _initialConstructionEventSignalled = true;
      }
      catch (SerializationException ex)
      {
        Type publicDomainObjectType = GetPublicDomainObjectType();
        string message = string.Format (
            "The GetObjectData method on type {0} did not call DomainObject's BaseGetObjectData method.", publicDomainObjectType.FullName);
        throw new SerializationException (message, ex);
      }
    }

    // methods and properties

    /// <summary>
    /// Serializes the base data needed to deserialize a <see cref="DomainObject"/> instance.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> coming from the .NET serialization infrastructure.</param>
    /// <param name="context">The <see cref="StreamingContext"/> coming from the .NET serialization infrastructure.</param>
    /// <remarks>Be sure to call this method from the <see cref="ISerializable.GetObjectData"/> implementation of any concrete
    /// <see cref="DomainObject"/> type implementing the <see cref="ISerializable"/> interface.</remarks>
    protected void BaseGetObjectData (SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      info.AddValue ("DomainObject.ID", ID);
      info.AddValue ("DomainObject._bindingTransaction", _bindingTransaction);
    }

    /// <summary>
    /// Sets the data container during the process creating a domain object or loading it for the first time.
    /// </summary>
    /// <param name="dataContainer">The data container to be associated with the loaded domain object.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="dataContainer"/> parameter is null.</exception>
    /// <remarks>This method is always called exactly once per <see cref="DomainObject"/> instance, and it is called regardless of
    /// whether the object has been created by <see cref="NewObject{T}"/> or loaded via <see cref="GetObject{T}(ObjectID)"/>.</remarks>
    internal void InitializeFromDataContainer (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      _id = dataContainer.ID;
      dataContainer.ClientTransaction.TransactionEventSink.ObjectInitializedFromDataContainer (_id, this);
      dataContainer.ClientTransaction.EnlistDomainObject (this);
    }

    /// <summary>
    /// GetType might return a <see cref="Type"/> object for a generated class, which is usually not what is expected.
    /// <see cref="DomainObject.GetPublicDomainObjectType"/> can be used to get the Type object of the original underlying domain object type. If
    /// the <see cref="Type"/> object for the generated class is explicitly required, this object can be cast to 'object' before calling GetType.
    /// </summary>
    [Obsolete ("GetType might return a Type object for a generated class., which is usually not what is expected. "
        + "DomainObject.GetPublicDomainObjectType can be used to get the Type object of the original underlying domain object type. If the Type object"
        + "for the generated class is explicitly required, this object can be cast to 'object' before calling GetType.", true)]
    public new Type GetType ()
    {
      throw new InvalidOperationException ("DomainObject.GetType should not be used.");
    }

    /// <summary>
    /// Returns the public type representation of this domain object, i.e. the type object visible to mappings, database, etc.
    /// </summary>
    /// <returns>The public type representation of this domain object.</returns>
    /// <remarks>A domain object should override this method if it wants to impersonate one of its base types. The framework will handle this object
    /// as if it was of the type returned by this method and ignore its actual type.</remarks>
    public virtual Type GetPublicDomainObjectType ()
    {
      return base.GetType();
    }

    public override string ToString ()
    {
      return ID.ToString();
    }

    /// <summary>
    /// Gets the <see cref="ObjectID"/> of the <see cref="DomainObject"/>.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public ObjectID ID
    {
      get { return _id; }
    }

    /// <summary>
    /// Gets the current state of the <see cref="DomainObject"/> in the <see cref="ClientTransactionScope.CurrentTransaction"/>.
    /// </summary>
    public StateType State
    {
      get { return GetStateForTransaction (GetNonNullClientTransaction()); }
    }

    /// <summary>
    /// Gets the transaction used when this <see cref="DomainObject"/> is accessed. If a <see cref="DomainObject"/> is bound to a specific
    /// <see cref="Remotion.Data.DomainObjects.ClientTransaction"/>, this property will return that transaction, otherwise it returns
    /// <see cref="DomainObjects.ClientTransaction.Current"/>.
    /// </summary>
    /// <value>The transaction used by this <see cref="DomainObject"/> when it is accessed.</value>
    /// <remarks>
    /// <para>
    /// To check whether this object is bound to the transaction returned by the <see cref="ClientTransaction"/> property, check the
    /// <see cref="IsBoundToSpecificTransaction"/> property.
    /// </para>
    /// <para>
    /// To check whether this <see cref="DomainObject"/> can actually be used in the transaction returned by this property, use the 
    /// <see cref="CanBeUsedInTransaction"/> method. To enlist the object in the transaction, call
    /// <see cref="DomainObjects.ClientTransaction.EnlistDomainObject"/>.
    /// </para>
    /// </remarks>
    public ClientTransaction ClientTransaction
    {
      get { return _bindingTransaction ?? ClientTransaction.Current; }
    }

    internal ClientTransaction GetNonNullClientTransaction ()
    {
      ClientTransaction transaction = ClientTransaction;
      if (transaction == null)
        throw new InvalidOperationException ("No ClientTransaction has been associated with the current thread or this object.");
      else
        return transaction;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is bound to specific transaction. If it is, it will always use that transaction, otherwise,
    /// it will always use <see cref="DomainObjects.ClientTransaction.Current"/> when it is accessed.
    /// </summary>
    /// <value>
    /// True if this instance is bound to specific transaction; otherwise, false.
    /// </value>
    /// <remarks>
    /// <para>
    /// To bind a <see cref="DomainObject"/> to a transaction, instantiate or load it in the scope of a transaction created via
    /// <see cref="DomainObjects.ClientTransaction.NewBindingTransaction"/>. Such a transaction will automatically bind all objects created or loaded
    /// in its scope to itself.
    /// </para>
    /// <para>
    /// To retrieve the transaction the object is bound to, use the <see cref="ClientTransaction"/> property.
    /// </para>
    /// </remarks>
    public bool IsBoundToSpecificTransaction
    {
      get { return _bindingTransaction != null; }
    }

    /// <summary>
    /// Gets the state of this object in a given <see cref="DomainObjects.ClientTransaction"/>.
    /// </summary>
    /// <param name="clientTransaction">The client transaction to retrieve the object's state from.</param>
    /// <returns>The state of this object in the given transaction.</returns>
    public StateType GetStateForTransaction (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      CheckIfRightTransaction (clientTransaction);
      if (IsDiscardedInTransaction (clientTransaction))
        return StateType.Discarded;
      else
      {
        DataContainer dataContainer = GetDataContainerForTransaction (clientTransaction);
        if (dataContainer.State == StateType.Unchanged)
        {
          if (clientTransaction.HasRelationChanged (this))
            return StateType.Changed;
          else
            return StateType.Unchanged;
        }

        return dataContainer.State;
      }
    }

    /// <summary>
    /// Marks the <see cref="DomainObject"/> as changed. If the object's previous <see cref="State"/> was <see cref="StateType.Unchanged"/>, it
    /// will be <see cref="StateType.Changed"/> after this method has been called.
    /// </summary>
    /// <exception cref="InvalidOperationException">This object is not in state <see cref="StateType.Changed"/> or <see cref="StateType.Unchanged"/>.
    /// New or deleted objects cannot be marked as changed.</exception>
    /// <exception cref="ObjectDiscardedException">The object has already been discarded.</exception>
    public void MarkAsChanged ()
    {
      ClientTransaction transaction = GetNonNullClientTransaction();
      CheckIfObjectIsDiscarded (transaction);

      DataContainer dataContainer = GetDataContainerForTransaction (transaction);
      try
      {
        dataContainer.MarkAsChanged();
      }
      catch (InvalidOperationException ex)
      {
        throw new InvalidOperationException ("Only existing DomainObjects can be marked as changed.", ex);
      }
    }

    /// <summary>
    /// Gets a value indicating the discarded status of the object in the <see cref="ClientTransaction"/>.
    /// </summary>
    /// <remarks>
    /// For more information why and when an object is discarded see <see cref="Remotion.Data.DomainObjects.DataManagement.ObjectDiscardedException"/>.
    /// </remarks>
    public bool IsDiscarded
    {
      get { return IsDiscardedInTransaction (GetNonNullClientTransaction ()); }
    }

    /// <summary>
    /// Gets a value indicating the discarded status of the object in the given <see cref="DomainObjects.ClientTransaction"/>.
    /// </summary>
    /// <param name="transaction">The transaction to check.</param>
    /// <returns>True if this object is discarded in the given <paramref name="transaction"/>; otherwise, false.</returns>
    /// <remarks>
    /// For more information why and when an object is discarded see <see cref="Remotion.Data.DomainObjects.DataManagement.ObjectDiscardedException"/>.
    /// </remarks>
    public bool IsDiscardedInTransaction (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      return transaction.DataManager.IsDiscarded (ID);
    }

    protected internal void CheckIfObjectIsDiscarded (ClientTransaction transaction)
    {
      if (IsDiscardedInTransaction (transaction))
        throw new ObjectDiscardedException (ID);
    }

    internal DataContainer GetDataContainerForTransaction (ClientTransaction transaction)
    {
      CheckIfObjectIsDiscarded (transaction);
      CheckIfRightTransaction (transaction);

      DataContainer dataContainer = transaction.DataManager.DataContainerMap[ID];
      if (dataContainer == null)
        dataContainer = transaction.LoadExistingObject (this);
      Assertion.IsNotNull (dataContainer);

      return dataContainer;
    }

    /// <summary>
    /// Deletes the <see cref="DomainObject"/> in the <see cref="ClientTransaction"/>.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    /// <remarks>To perform custom actions when a <see cref="DomainObject"/> is deleted <see cref="OnDeleting"/> and <see cref="OnDeleted"/> should be overridden.</remarks>
    protected void Delete ()
    {
      RepositoryAccessor.DeleteObject (this);
    }

    #region Transaction handling

    internal void Bind (ClientTransaction bindingTransaction)
    {
      ArgumentUtility.CheckNotNull ("bindingTransaction", bindingTransaction);
      _bindingTransaction = bindingTransaction;
    }

    /// <summary>
    /// Determines whether this instance can be used in the specified transaction.
    /// </summary>
    /// <param name="transaction">The transaction to check this object against.</param>
    /// <returns>
    /// True if this instance can be used in the specified transaction; otherwise, false.
    /// </returns>
    /// <remarks>If this method returns false, <see cref="DomainObjects.ClientTransaction.EnlistDomainObject"/> can be used to enlist this instance in another
    /// transaction.</remarks>
    public bool CanBeUsedInTransaction (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      if (transaction.IsEnlisted (this))
        return true;
      else if (ClientTransactionScope.ActiveScope != null && ClientTransactionScope.ActiveScope.AutoEnlistDomainObjects)
      {
        transaction.EnlistDomainObject (this);
        return true;
      }
      else
        return false;
    }

    internal void CheckIfRightTransaction (ClientTransaction transaction)
    {
      if (!CanBeUsedInTransaction (transaction))
      {
        string message = string.Format (
            "Domain object '{0}' cannot be used in the current transaction as it was loaded or created in another "
                + "transaction. Use a ClientTransactionScope to set the right transaction, or call EnlistInTransaction to enlist the object "
                    + "in the current transaction.",
            ID);
        throw new ClientTransactionsDifferException (message);
      }
    }

    #endregion

    #region Property access

    /// <summary>
    /// Prepares access to the <see cref="PropertyValue"/> of the given name.
    /// </summary>
    /// <param name="propertyName">The name of the <see cref="PropertyValue"/> to be accessed.</param>
    /// <remarks>This method prepares the given property for access via <see cref="CurrentProperty"/>.
    /// It is automatically invoked for virtual properties in domain objects created with interception support and thus doesn't
    /// have to be called manually for these objects. If you choose to invoke <see cref="PreparePropertyAccess"/> and
    /// <see cref="PropertyAccessFinished"/> yourself, be sure to finish the property access with exactly one call to 
    /// <see cref="PropertyAccessFinished"/> from a finally-block.</remarks>
    /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="Remotion.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
    /// <exception cref="ArgumentException">The <paramref name="propertyName"/> parameter does not denote a valid property.</exception>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    protected internal virtual void PreparePropertyAccess (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      if (!PropertyAccessor.IsValidProperty (ID.ClassDefinition, propertyName))
      {
        string message = string.Format (
            "The property identifier '{0}' is not a valid property of domain object type {1}.",
            propertyName,
            ID.ClassDefinition.ClassType.FullName);
        throw new ArgumentException (message, "propertyName");
      }

      CurrentPropertyManager.PreparePropertyAccess (propertyName);
    }

    /// <summary>
    /// Indicates that access to the <see cref="PropertyValue"/> of the given name is finished.
    /// </summary>
    /// <remarks>This method must be executed after a property previously prepared via <see cref="PreparePropertyAccess"/> has been accessed as needed.
    /// It is automatically invoked for virtual properties in domain objects created with interception suppport and thus doesn't
    /// have to be called manually for these objects. If you choose to invoke <see cref="PreparePropertyAccess"/> and
    /// <see cref="PropertyAccessFinished"/> yourself, be sure to invoke this method in a finally-block in order to guarantee its execution.</remarks>
    /// <exception cref="InvalidOperationException">There is no property to be finished. There is likely a mismatched number of calls to
    /// <see cref="PreparePropertyAccess"/> and <see cref="PropertyAccessFinished"/>.</exception>
    protected internal virtual void PropertyAccessFinished ()
    {
      CurrentPropertyManager.PropertyAccessFinished();
    }

    /// <summary>
    /// Retrieves the current property name and throws an exception if there is no current property.
    /// </summary>
    /// <returns>The current property name.</returns>
    /// <remarks>Retrieves the current property name previously initialized via <see cref="PreparePropertyAccess"/>. Domain objects created with 
    /// interception support automatically initialize their virtual properties without needing any further work.</remarks>
    /// <exception cref="InvalidOperationException">There is no current property or it hasn't been properly initialized.</exception>
    protected internal virtual string GetAndCheckCurrentPropertyName ()
    {
      string propertyName = CurrentPropertyManager.CurrentPropertyName;
      if (propertyName == null)
      {
        throw new InvalidOperationException (
            "There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?");
      }
      else
        return propertyName;
    }

    /// <summary>
    /// Provides simple, encapsulated access to the current property.
    /// </summary>
    /// <value>A <see cref="PropertyAccessor"/> object encapsulating the current property.</value>
    /// <remarks>
    /// The structure returned by this method allows simple access to the property's value and mapping definition objects regardless of
    /// whether it is a simple value property, a related object property, or a related object collection property.
    /// </remarks>
    /// <exception cref="InvalidOperationException">The current property hasn't been initialized or there is no current property. Perhaps the domain 
    /// object was created with the <c>new</c> operator instead of using the <see cref="NewObject{T}"/> method, or the property is not virtual.</exception>
    protected internal PropertyAccessor CurrentProperty
    {
      get
      {
        string propertyName = GetAndCheckCurrentPropertyName();
        return Properties[propertyName];
      }
    }

    /// <summary>
    /// Provides simple, encapsulated access to the properties of this <see cref="DomainObject"/>.
    /// </summary>
    /// <returns>A <see cref="PropertyIndexer"/> object which can be used to select a specific property of this <see cref="DomainObject"/>.</returns>
    protected internal PropertyIndexer Properties
    {
      get { return new PropertyIndexer (this); }
    }

    protected TransactionalAccessor<T> GetTransactionalAccessor<T> (PropertyAccessor property)
    {
      return new TransactionalAccessor<T> (property);
    }

    protected internal DomainObjectGraphTraverser GetGraphTraverser (IGraphTraversalStrategy strategy)
    {
      return new DomainObjectGraphTraverser (this, strategy);
    }

    #endregion

    /// <summary>
    /// Method is invoked after the loading process of the object is completed.
    /// </summary>
    /// <param name="loadMode">Specifies whether the whole domain object or only the <see cref="Remotion.Data.DomainObjects.DataContainer"/> has been
    /// newly loaded.</param>
    /// <remarks>
    /// Override this method to initialize <see cref="DomainObject"/>s that are loaded from the datasource.
    /// </remarks>
    protected virtual void OnLoaded (LoadMode loadMode)
    {
    }

    /// <summary>
    /// Raises the <see cref="Committing"/> event.
    /// </summary>
    /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
    protected virtual void OnCommitting (EventArgs args)
    {
      if (Committing != null)
        Committing (this, args);
    }

    /// <summary>
    /// Raises the <see cref="Committed"/> event.
    /// </summary>
    /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
    protected virtual void OnCommitted (EventArgs args)
    {
      if (Committed != null)
        Committed (this, args);
    }

    /// <summary>
    /// Raises the <see cref="RollingBack"/> event.
    /// </summary>
    /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
    protected virtual void OnRollingBack (EventArgs args)
    {
      if (RollingBack != null)
        RollingBack (this, args);
    }

    /// <summary>
    /// Raises the <see cref="RolledBack"/> event.
    /// </summary>
    /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
    protected virtual void OnRolledBack (EventArgs args)
    {
      if (RolledBack != null)
        RolledBack (this, args);
    }

    /// <summary>
    /// Raises the <see cref="RelationChanging"/> event.
    /// </summary>
    /// <param name="args">A <see cref="RelationChangingEventArgs"/> object that contains the event data.</param>
    protected virtual void OnRelationChanging (RelationChangingEventArgs args)
    {
      if (RelationChanging != null)
        RelationChanging (this, args);
    }

    /// <summary>
    /// Raises the <see cref="RelationChanged"/> event.
    /// </summary>
    /// <param name="args">A <see cref="RelationChangedEventArgs"/> object that contains the event data.</param>
    protected virtual void OnRelationChanged (RelationChangedEventArgs args)
    {
      if (RelationChanged != null)
        RelationChanged (this, args);
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanging"/> event.
    /// </summary>
    /// <param name="args">A <see cref="PropertyChangeEventArgs"/> object that contains the event data.</param>
    protected virtual void OnPropertyChanging (PropertyChangeEventArgs args)
    {
      if (PropertyChanging != null)
        PropertyChanging (this, args);
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="args">A <see cref="PropertyChangeEventArgs"/> object that contains the event data.</param>
    protected virtual void OnPropertyChanged (PropertyChangeEventArgs args)
    {
      if (PropertyChanged != null)
        PropertyChanged (this, args);
    }

    /// <summary>
    /// Raises the <see cref="Deleting"/> event.
    /// </summary>
    /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
    protected virtual void OnDeleting (EventArgs args)
    {
      if (Deleting != null)
        Deleting (this, args);
    }

    /// <summary>
    /// Raises the <see cref="Deleted"/> event.
    /// </summary>
    /// <param name="args">A <see cref="EventArgs"/> object that contains the event data.</param>
    protected virtual void OnDeleted (EventArgs args)
    {
      if (Deleted != null)
        Deleted (this, args);
    }

    internal void BeginRelationChange (
        string propertyName,
        DomainObject oldRelatedObject,
        DomainObject newRelatedObject)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      RelationChangingEventArgs args = new RelationChangingEventArgs (propertyName, oldRelatedObject, newRelatedObject);
      OnRelationChanging (args);
    }

    internal void EndObjectLoading ()
    {
      LoadMode loadMode;

      if (!_initialConstructionEventSignalled)
        loadMode = LoadMode.WholeDomainObjectInitialized;
      else
        loadMode = LoadMode.DataContainerLoadedOnly;

      _initialConstructionEventSignalled = true;

      DomainObjectMixinCodeGenerationBridge.OnDomainObjectLoaded (this, loadMode);
      OnLoaded (loadMode);
    }

    internal void EndRelationChange (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      OnRelationChanged (new RelationChangedEventArgs (propertyName));
    }

    internal void BeginDelete ()
    {
      OnDeleting (EventArgs.Empty);
    }

    internal void EndDelete ()
    {
      OnDeleted (EventArgs.Empty);
    }

    internal void BeginCommit ()
    {
      OnCommitting (EventArgs.Empty);
    }

    internal void EndCommit ()
    {
      OnCommitted (EventArgs.Empty);
    }

    internal void BeginRollback ()
    {
      OnRollingBack (EventArgs.Empty);
    }

    internal void EndRollback ()
    {
      OnRolledBack (EventArgs.Empty);
    }

    internal void PropertyValueChanging (object sender, PropertyChangeEventArgs args)
    {
      OnPropertyChanging (args);
    }

    internal void PropertyValueChanged (object sender, PropertyChangeEventArgs args)
    {
      OnPropertyChanged (args);
    }
  }
}
