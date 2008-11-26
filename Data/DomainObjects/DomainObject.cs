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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
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
  public class DomainObject : IDomainObjectTransactionContext
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
    private ClientTransaction _bindingTransaction; // null unless this object is bound to a fixed transaction
    
    [NonSerialized] // lazily initialized
    private PropertyIndexer _properties;

    private DomainObjectEventManager _eventManager;

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
// ReSharper disable DoNotCallOverridableMethodsInConstructor
      PerformConstructorCheck ();
      Type publicDomainObjectType = GetPublicDomainObjectType();
// ReSharper restore DoNotCallOverridableMethodsInConstructor

      ClientTransactionScope.CurrentTransaction.TransactionEventSink.NewObjectCreating (publicDomainObjectType, this);
      DataContainer firstDataContainer = ClientTransactionScope.CurrentTransaction.CreateNewDataContainer (publicDomainObjectType);
      firstDataContainer.SetDomainObject (this);

      InitializeFromDataContainer (firstDataContainer);
      _eventManager = new DomainObjectEventManager (this, true);
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
        _eventManager = (DomainObjectEventManager) info.GetValue ("DomainObject._eventManager", typeof (DomainObjectEventManager));
      }
      catch (SerializationException ex)
      {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
        Type publicDomainObjectType = GetPublicDomainObjectType();
// ReSharper restore DoNotCallOverridableMethodsInConstructor
        string message = string.Format (
            "The GetObjectData method on type {0} did not call DomainObject's BaseGetObjectData method.", publicDomainObjectType.FullName);
        throw new SerializationException (message, ex);
      }
    }

    // methods and properties

    /// <summary>
    /// Ensures that <see cref="DomainObject"/> instances are not created via constructor checks.
    /// </summary>
    /// <remarks>
    /// The default implementation of this method throws an exception. When the runtime code generation invoked via <see cref="NewObject{T}"/>
    /// generates a concrete <see cref="DomainObject"/> type, it overrides this method to disable the exception. This ensures that 
    /// <see cref="DomainObject"/> instances cannot be created simply by calling the <see cref="DomainObject"/>'s constructor.
    /// </remarks>
    [EditorBrowsable (EditorBrowsableState.Never)]
    protected virtual void PerformConstructorCheck ()
    {
      throw new InvalidOperationException ("DomainObject constructors must not be called directly. Use DomainObject.NewObject to create DomainObject "
          + "instances.");
    }

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
      info.AddValue ("DomainObject._eventManager", _eventManager);
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

    /// <summary>
    /// Returns a textual representation of this object's <see cref="ID"/>.
    /// </summary>
    /// <returns>
    /// A textual representation of <see cref="ID"/>.
    /// </returns>
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

    /// <summary>
    /// Gets the <see cref="ClientTransaction"/> this <see cref="DomainObject"/> instance was bound to, or <see langword="null"/>
    /// if the object has not been bound.
    /// </summary>
    /// <value>The <see cref="ClientTransaction"/> this object was bound to, or <see langword="null"/>.</value>
    public ClientTransaction BindingTransaction
    {
      get { return _bindingTransaction; }
    }

    public DomainObjectTransactionContextIndexer TransactionContext
    {
      get { return new DomainObjectTransactionContextIndexer (this); }
    }

    /// <summary>
    /// Gets the current state of the <see cref="DomainObject"/> in the <see cref="ClientTransactionScope.CurrentTransaction"/>.
    /// </summary>
    public StateType State
    {
      get { return TransactionContext[DomainObjectUtility.GetNonNullClientTransaction(this)].State; }
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
    /// <see cref="DomainObjects.ClientTransaction.CreateBindingTransaction"/>. Such a transaction will automatically bind all objects created or loaded
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
    /// Gets the event manager responsible for raising this object's events.
    /// </summary>
    /// <value>The event manager for this <see cref="DomainObject"/>.</value>
    internal DomainObjectEventManager EventManager
    {
      get
      {
        if (_eventManager == null)
          _eventManager = new DomainObjectEventManager (this, false);

        return _eventManager;
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
      get { return TransactionContext[DomainObjectUtility.GetNonNullClientTransaction (this)].IsDiscarded; }
    }

    /// <summary>
    /// Gets the timestamp used for optimistic locking when the object is committed to the database.
    /// </summary>
    /// <value>The timestamp of the object.</value>
    public object Timestamp
    {
      get { return TransactionContext[DomainObjectUtility.GetNonNullClientTransaction (this)].Timestamp; }
    }

    /// <summary>
    /// Determines whether this instance can be used in the <see cref="ClientTransaction"/>.
    /// </summary>
    /// <value></value>
    /// <remarks>If this property returns false, <see cref="DomainObjects.ClientTransaction.EnlistDomainObject"/> can be used to enlist the object
    /// in the transaction.</remarks>
    public bool CanBeUsedInTransaction
    {
      get { return TransactionContext[DomainObjectUtility.GetNonNullClientTransaction(this)].CanBeUsedInTransaction; }
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
      TransactionContext[DomainObjectUtility.GetNonNullClientTransaction (this)].MarkAsChanged ();
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
        string propertyName = CurrentPropertyManager.GetAndCheckCurrentPropertyName();
        return Properties[propertyName];
      }
    }

    /// <summary>
    /// Provides simple, encapsulated access to the properties of this <see cref="DomainObject"/>.
    /// </summary>
    /// <returns>A <see cref="PropertyIndexer"/> object which can be used to select a specific property of this <see cref="DomainObject"/>.</returns>
    protected internal PropertyIndexer Properties
    {
      get
      { 
        if (_properties == null)
          _properties = new PropertyIndexer (this);
        return _properties;
      }
    }
    
    /// <summary>
    /// This method is invoked after the loading process of the object is completed.
    /// </summary>
    /// <param name="loadMode">Specifies whether the whole domain object or only the <see cref="Remotion.Data.DomainObjects.DataContainer"/> has been
    /// newly loaded.</param>
    /// <remarks>
    /// Override this method to initialize <see cref="DomainObject"/>s that are loaded from the datasource.
    /// </remarks>
    protected internal virtual void OnLoaded (LoadMode loadMode)
    {
    }

    /// <summary>
    /// Raises the <see cref="Committing"/> event.
    /// </summary>
    /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnCommitting (EventArgs args)
    {
      if (Committing != null)
        Committing (this, args);
    }

    /// <summary>
    /// Raises the <see cref="Committed"/> event.
    /// </summary>
    /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnCommitted (EventArgs args)
    {
      if (Committed != null)
        Committed (this, args);
    }

    /// <summary>
    /// Raises the <see cref="RollingBack"/> event.
    /// </summary>
    /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnRollingBack (EventArgs args)
    {
      if (RollingBack != null)
        RollingBack (this, args);
    }

    /// <summary>
    /// Raises the <see cref="RolledBack"/> event.
    /// </summary>
    /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnRolledBack (EventArgs args)
    {
      if (RolledBack != null)
        RolledBack (this, args);
    }

    /// <summary>
    /// Raises the <see cref="RelationChanging"/> event.
    /// </summary>
    /// <param name="args">A <see cref="RelationChangingEventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnRelationChanging (RelationChangingEventArgs args)
    {
      if (RelationChanging != null)
        RelationChanging (this, args);
    }

    /// <summary>
    /// Raises the <see cref="RelationChanged"/> event.
    /// </summary>
    /// <param name="args">A <see cref="RelationChangedEventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnRelationChanged (RelationChangedEventArgs args)
    {
      if (RelationChanged != null)
        RelationChanged (this, args);
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanging"/> event.
    /// </summary>
    /// <param name="args">A <see cref="PropertyChangeEventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnPropertyChanging (PropertyChangeEventArgs args)
    {
      if (PropertyChanging != null)
        PropertyChanging (this, args);
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="args">A <see cref="PropertyChangeEventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnPropertyChanged (PropertyChangeEventArgs args)
    {
      if (PropertyChanged != null)
        PropertyChanged (this, args);
    }

    /// <summary>
    /// Raises the <see cref="Deleting"/> event.
    /// </summary>
    /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnDeleting (EventArgs args)
    {
      if (Deleting != null)
        Deleting (this, args);
    }

    /// <summary>
    /// Raises the <see cref="Deleted"/> event.
    /// </summary>
    /// <param name="args">A <see cref="EventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnDeleted (EventArgs args)
    {
      if (Deleted != null)
        Deleted (this, args);
    }

    internal void BindToTransaction (ClientTransaction bindingTransaction)
    {
      ArgumentUtility.CheckNotNull ("bindingTransaction", bindingTransaction);
      _bindingTransaction = bindingTransaction;
    }
  }
}
