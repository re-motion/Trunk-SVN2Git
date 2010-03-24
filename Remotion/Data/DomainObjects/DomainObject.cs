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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Base class for all objects that are persisted by the framework.
  /// </summary>
  /// <remarks>
  /// <para>
  /// If a class implementing <see cref="ISerializable"/> is derived from this base class, it must provide a deserialization constructor invoking
  /// this class' deserialization constructor, and it must call <see cref="BaseGetObjectData"/> from the <see cref="ISerializable.GetObjectData"/>
  /// implementation.
  /// </para>
  /// </remarks>
  [Serializable]
  [DebuggerDisplay ("{GetPublicDomainObjectType().FullName}: {ID.ToString()}")]
  public class DomainObject
  {
    // types

    // static members and constants

    #region Creation and GetObject factory methods

    /// <summary>
    /// Returns a new instance of a concrete domain object for the current <see cref="DomainObjects.ClientTransaction"/>. The object is constructed
    /// using the default constructor in the <see cref="DomainObjects.ClientTransaction.Current"/> <see cref="DomainObjects.ClientTransaction"/>.
    /// </summary>
    /// <typeparam name="T">The concrete type to be implemented by the object.</typeparam>
    /// <returns>A new domain object instance.</returns>
    /// <remarks>
    /// <para>
    /// Objects created by this factory method are not directly instantiated; instead a proxy is dynamically created, which will assist in 
    /// management tasks at runtime.
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
    protected static T NewObject<T> () where T : DomainObject
    {
      return NewObject<T> (ParamList.Empty);
    }

    /// <summary>
    /// Returns a new instance of a concrete domain object for the current <see cref="DomainObjects.ClientTransaction"/>. The object is constructed
    /// using the supplied constructor arguments in the <see cref="DomainObjects.ClientTransaction.Current"/> <see cref="DomainObjects.ClientTransaction"/>.
    /// </summary>
    /// <typeparam name="T">The concrete type to be implemented by the object.</typeparam>
    /// <param name="constructorParameters">A <see cref="ParamList"/> encapsulating the parameters to be passed to the constructor. Instantiate this
    /// by using one of the <see cref="ParamList.Create{A1,A2}"/> methods.</param>
    /// <returns>A new domain object instance.</returns>
    /// <remarks>
    /// <para>
    /// Objects created by this factory method are not directly instantiated; instead a proxy is dynamically created, which will assist in 
    /// management tasks at runtime.
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
    protected static T NewObject<T> (ParamList constructorParameters) where T : DomainObject
    {
      ArgumentUtility.CheckNotNull ("constructorParameters", constructorParameters);

      return (T) LifetimeService.NewObject (ClientTransaction.Current, typeof (T), constructorParameters);
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
      return (T) LifetimeService.GetObject (ClientTransaction.Current, id, includeDeleted);
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
    public event PropertyChangeEventHandler PropertyChanging;

    /// <summary>
    /// Occurs after a <see cref="PropertyValue"/> of the <see cref="DomainObject"/> is changed.
    /// </summary>
    /// <remarks>
    /// This event does not fire when a <see cref="PropertyValue"/> has been changed due to a relation change.
    /// </remarks>
    public event PropertyChangeEventHandler PropertyChanged;

    /// <summary>
    /// Occurs before a Relation of the <see cref="DomainObject"/> is changed.
    /// This event might be raised more than once for a given relation change operation. For example, when a whole related object collection is 
    /// replaced in one go, this event is raised once for each old object that is not in the new collection and once for each new object not in the 
    /// old collection.
    /// </summary>
    public event RelationChangingEventHandler RelationChanging;

    /// <summary>
    /// Occurs after a Relation of the <see cref="DomainObject"/> has been changed.
    /// This event might be raised more than once for a given relation change operation. For example, when a whole related object collection is 
    /// replaced in one go, this event is raised once for each old object that is not in the new collection and once for each new object not in the 
    /// old collection.
    /// </summary>
    public event RelationChangedEventHandler RelationChanged;

    /// <summary>
    /// Occurs before the <see cref="DomainObject"/> is deleted.
    /// </summary>
    public event EventHandler Deleting;

    /// <summary>
    /// Occurs after the <see cref="DomainObject"/> has been deleted.
    /// </summary>
    public event EventHandler Deleted;

    /// <summary>
    /// Occurs before the changes of a <see cref="DomainObject"/> are committed.
    /// </summary>
    public event EventHandler Committing;

    /// <summary>
    /// Occurs after the changes of a <see cref="DomainObject"/> are successfully committed.
    /// </summary>
    public event EventHandler Committed;

    /// <summary>
    /// Occurs before the changes of a <see cref="DomainObject"/> are rolled back.
    /// </summary>
    public event EventHandler RollingBack;

    /// <summary>
    /// Occurs after the changes of a <see cref="DomainObject"/> are successfully rolled back.
    /// </summary>
    public event EventHandler RolledBack;

    private ObjectID _id;
    private ClientTransaction _bindingTransaction; // null unless this object is bound to a fixed transaction
    private bool _needsLoadModeDataContainerOnly; // true if the object was created by a constructor call or OnLoaded has already been called once

    [NonSerialized] // required when ISerializable is not implemented by subclass
    private PropertyIndexer _properties; // lazily initialized

    // construction and disposing

    /// <summary>
    /// Initializes a new <see cref="DomainObject"/> with the current <see cref="DomainObjects.ClientTransaction"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Any constructors implemented on concrete domain objects should delegate to this base constructor. As domain objects generally should 
    /// not be constructed via the
    /// <c>new</c> operator, these constructors must remain protected, and the concrete domain objects should have a static "NewObject" method,
    /// which delegates to <see cref="NewObject{T}(ParamList)"/>, passing it the required constructor arguments.
    /// </para>
    /// <para>
    /// It is safe to access virtual properties that are automatically implemented by the framework from constructors because this base constructor
    /// prepares everything necessary for them to work.
    /// </para>
    /// </remarks>
    protected DomainObject ()
    {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
      PerformConstructorCheck ();
// ReSharper restore DoNotCallOverridableMethodsInConstructor

      var publicDomainObjectType = GetPublicDomainObjectType ();

      var clientTransaction = ClientTransaction.Current;
      clientTransaction.TransactionEventSink.NewObjectCreating (publicDomainObjectType, this);

      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (publicDomainObjectType);
      var objectID = clientTransaction.CreateNewObjectID (classDefinition);

      Initialize (objectID, clientTransaction as BindingClientTransaction);

      var newDataContainer = DataContainer.CreateNew (objectID);
      newDataContainer.SetDomainObject (this);
      newDataContainer.RegisterWithTransaction (clientTransaction);
      clientTransaction.EnlistDomainObject (this);

      _needsLoadModeDataContainerOnly = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainObject"/> class in the process of deserialization.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> coming from the .NET serialization infrastructure.</param>
    /// <param name="context">The <see cref="StreamingContext"/> coming from the .NET serialization infrastructure.</param>
    /// <remarks>Be sure to call this base constructor from the deserialization constructor of any concrete <see cref="DomainObject"/> type
    /// implementing the <see cref="ISerializable"/> interface.</remarks>
#pragma warning disable 168
// ReSharper disable UnusedParameter.Local
    protected DomainObject (SerializationInfo info, StreamingContext context)
// ReSharper restore UnusedParameter.Local
#pragma warning restore 168
    {
      ArgumentUtility.CheckNotNull ("info", info);

      try
      {
        _id = (ObjectID) info.GetValue ("DomainObject.ID", typeof (ObjectID));
        _bindingTransaction = (ClientTransaction) info.GetValue ("DomainObject._bindingTransaction", typeof (ClientTransaction));
        _needsLoadModeDataContainerOnly = info.GetBoolean ("DomainObject._needsLoadModeDataContainerOnly");

        PropertyChanging = (PropertyChangeEventHandler) info.GetValue ("DomainObject.PropertyChanging", typeof (PropertyChangeEventHandler));
        PropertyChanged = (PropertyChangeEventHandler) info.GetValue ("DomainObject.PropertyChanged", typeof (PropertyChangeEventHandler));
        RelationChanging = (RelationChangingEventHandler) info.GetValue ("DomainObject.RelationChanging", typeof (RelationChangingEventHandler));
        RelationChanged = (RelationChangedEventHandler) info.GetValue ("DomainObject.RelationChanged", typeof (RelationChangedEventHandler));
        Deleting = (EventHandler) info.GetValue ("DomainObject.Deleting", typeof (EventHandler));
        Deleted = (EventHandler) info.GetValue ("DomainObject.Deleted", typeof (EventHandler));
        Committing = (EventHandler) info.GetValue ("DomainObject.Committing", typeof (EventHandler));
        Committed = (EventHandler) info.GetValue ("DomainObject.Committed", typeof (EventHandler));
        RollingBack = (EventHandler) info.GetValue ("DomainObject.RollingBack", typeof (EventHandler));
        RolledBack = (EventHandler) info.GetValue ("DomainObject.RolledBack", typeof (EventHandler));
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
    /// Gets the <see cref="ObjectID"/> of the <see cref="DomainObject"/>.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public ObjectID ID
    {
      get { return _id; }
    }

    /// <summary>
    /// Gets the <see cref="DomainObjects.ClientTransaction"/> this <see cref="DomainObject"/> instance was bound to. If the object has not been 
    /// bound, this method throws an exception. Use <see cref="HasBindingTransaction"/> to check whether the object has been boung to a 
    /// <see cref="BindingClientTransaction"/> or not.
    /// </summary>
    /// <value>The <see cref="DomainObjects.ClientTransaction"/> this object was bound to, or <see langword="null"/>.</value>
    /// <exception cref="InvalidOperationException">The object has not been bound.</exception>
    /// <remarks>
    /// If a <see cref="DomainObject"/> has been bound to a <see cref="BindingClientTransaction"/>, its properties are always accessed in the context of that
    /// <see cref="BindingClientTransaction"/> instead of the <see cref="DomainObjects.ClientTransaction.Current"/> transaction. See 
    /// <see cref="DomainObjects.ClientTransaction.CreateBindingTransaction"/> for more information.
    /// </remarks>
    /// <seealso cref="DomainObjects.ClientTransaction.CreateBindingTransaction"/>
    public ClientTransaction GetBindingTransaction ()
    {
      if (!HasBindingTransaction)
      {
        throw new InvalidOperationException (
            "This object has not been bound to a specific transaction, it uses the current transaction when it is "
            + "accessed. Use HasBindingTransaction to check whether an object has been bound or not.");
      }
      return _bindingTransaction;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is bound to specific transaction. If it is, it will always use that transaction, otherwise,
    /// it will always use <see cref="DomainObjects.ClientTransaction.Current"/> when it is accessed.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this instance has binding transaction; otherwise, <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// <para>
    /// To bind a <see cref="DomainObject"/> to a transaction, instantiate or load it in the scope of a transaction created via
    /// <see cref="DomainObjects.ClientTransaction.CreateBindingTransaction"/>. Such a transaction will automatically bind all objects created or loaded
    /// in its scope to itself.
    /// </para>
    /// <para>
    /// To retrieve the transaction the object is bound to, use the <see cref="GetBindingTransaction"/> method.
    /// </para>
    /// <seealso cref="DomainObjects.ClientTransaction.CreateBindingTransaction"/>
    /// </remarks>
    public bool HasBindingTransaction
    {
      get { return _bindingTransaction != null; }
    }

    /// <summary>
    /// Gets a <see cref="DomainObjectTransactionContextIndexer"/> object that can be used to select an <see cref="IDomainObjectTransactionContext"/>
    /// for a specific <see cref="DomainObjects.ClientTransaction"/>. To obtain the default context, use <see cref="DefaultTransactionContext"/>.
    /// </summary>
    /// <value>The transaction context.</value>
    public DomainObjectTransactionContextIndexer TransactionContext
    {
      get { return new DomainObjectTransactionContextIndexer (this); }
    }

    /// <summary>
    /// Gets the default <see cref="IDomainObjectTransactionContext"/>, i.e. the transaction context that is used when this 
    /// <see cref="DomainObject"/>'s properties are accessed without specifying a <see cref="DomainObjects.ClientTransaction"/>.
    /// </summary>
    /// <value>The default transaction context.</value>
    /// <remarks>
    /// When an object is bound to a <see cref="BindingClientTransaction"/>, the <see cref="DefaultTransactionContext"/> represents that transaction.
    /// Otherwise, it represents the <see cref="DomainObjects.ClientTransaction.Current"/> transaction.
    /// </remarks>
    /// <exception cref="InvalidOperationException">No <see cref="DomainObjects.ClientTransaction"/> has been associated with the current thread or 
    /// this <see cref="DomainObject"/>.</exception>
    public IDomainObjectTransactionContext DefaultTransactionContext
    {
      get
      {
        var clientTransaction = HasBindingTransaction ? GetBindingTransaction() : ClientTransaction.Current;
        if (clientTransaction == null)
          throw new InvalidOperationException ("No ClientTransaction has been associated with the current thread or this object.");

        return TransactionContext[clientTransaction];
      }
    }

    /// <summary>
    /// Gets the current state of the <see cref="DomainObject"/> in the <see cref="ClientTransactionScope.CurrentTransaction"/>.
    /// </summary>
    public StateType State
    {
      get { return DefaultTransactionContext.State; }
    }

    /// <summary>
    /// Gets a value indicating the discarded status of the object in the default transaction, ie. in its binding transaction or - if
    /// none - <see cref="DomainObjects.ClientTransaction.Current"/>.
    /// </summary>
    /// <remarks>
    /// For more information why and when an object is discarded see <see cref="Remotion.Data.DomainObjects.DataManagement.ObjectDiscardedException"/>.
    /// </remarks>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    public bool IsDiscarded
    {
      get { return DefaultTransactionContext.IsDiscarded; }
    }

    /// <summary>
    /// Gets the timestamp used for optimistic locking when the object is committed to the database in the default transaction, ie. in 
    /// its binding transaction or - if none - <see cref="DomainObjects.ClientTransaction.Current"/>.
    /// </summary>
    /// <value>The timestamp of the object.</value>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the current transaction.</exception>
    /// <exception cref="ObjectDiscardedException">The object has already been discarded.</exception>
    public object Timestamp
    {
      get { return DefaultTransactionContext.Timestamp; }
    }

    /// <summary>
    /// Ensures that <see cref="DomainObject"/> instances are not created via constructor checks.
    /// </summary>
    /// <remarks>
    /// The default implementation of this method throws an exception. When the runtime code generation invoked via <see cref="NewObject{T}()"/>
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
      info.AddValue ("DomainObject._needsLoadModeDataContainerOnly", _needsLoadModeDataContainerOnly);

      info.AddValue ("DomainObject.PropertyChanging", PropertyChanging);
      info.AddValue ("DomainObject.PropertyChanged", PropertyChanged);
      info.AddValue ("DomainObject.RelationChanging", RelationChanging);
      info.AddValue ("DomainObject.RelationChanged", RelationChanged);
      info.AddValue ("DomainObject.Deleted", Deleted);
      info.AddValue ("DomainObject.Deleting", Deleting);
      info.AddValue ("DomainObject.Committing", Committing);
      info.AddValue ("DomainObject.Committed", Committed);
      info.AddValue ("DomainObject.RollingBack", RollingBack);
      info.AddValue ("DomainObject.RolledBack", RolledBack);
    }

    /// <summary>
    /// Initializes a new <see cref="DomainObject"/> during a call to <see cref="NewObject{T}()"/> or <see cref="GetObject{T}(ObjectID)"/>. This method
    /// is automatically called by the framework and should not normally be invoked by user code.
    /// </summary>
    /// <param name="id">The <see cref="ObjectID"/> to associate the new <see cref="DomainObject"/> with.</param>
    /// <param name="bindingTransaction">The <see cref="DomainObjects.ClientTransaction"/> to bind the new <see cref="DomainObject"/> to, or 
    /// <see langword="null" /> if the object should not be bound.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="id"/> or <paramref name="bindingTransaction"/> parameter is null.</exception>
    /// <exception cref="InvalidOperationException">This <see cref="DomainObject"/> has already been initialized.</exception>
    /// <remarks>This method is always called exactly once per <see cref="DomainObject"/> instance by the framework. It sets the object's 
    /// <see cref="ID"/> and enlists it with the given <see cref="DomainObjects.ClientTransaction"/>.</remarks>
    public void Initialize (ObjectID id, ClientTransaction bindingTransaction)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      if (_id != null)
        throw new InvalidOperationException ("The object cannot be initialized, it already has an ID.");

      _id = id;
      _bindingTransaction = bindingTransaction;
    }

    /// <summary>
    /// GetType might return a <see cref="Type"/> object for a generated class, which is usually not what is expected.
    /// <see cref="DomainObject.GetPublicDomainObjectType"/> can be used to get the Type object of the original underlying domain object type. If
    /// the <see cref="Type"/> object for the generated class is explicitly required, this object can be cast to 'object' before calling GetType.
    /// </summary>
    [Obsolete ("GetType might return a Type object for a generated class, which is usually not what is expected. "
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
    public Type GetPublicDomainObjectType ()
    {
      return GetPublicDomainObjectTypeImplementation ();
    }

    /// <summary>
    /// Implements the functionality required by <see cref="GetPublicDomainObjectType"/>. This is a separate method to avoid having to make the 
    /// virtual call in the constructor. The implementation of this class must expect calls from the constructor of a base class.
    /// </summary>
    /// <returns>The public type representation of this domain object.</returns>
    /// <remarks>A domain object should override this method if it wants to impersonate one of its base types. The framework will handle this object
    /// as if it was of the type returned by this method and ignore its actual type.</remarks>
    protected virtual Type GetPublicDomainObjectTypeImplementation ()
    {
      return base.GetType ();
    }

    /// <summary>
    /// Returns a textual representation of this object's <see cref="ID"/>.
    /// </summary>
    /// <returns>
    /// A textual representation of <see cref="ID"/>.
    /// </returns>
    public override string ToString ()
    {
      return ID.ToString ();
    }

    /// <summary>
    /// Marks the <see cref="DomainObject"/> as changed in the default transaction, ie. in its binding transaction or - if
    /// none - <see cref="DomainObjects.ClientTransaction.Current"/>. If the object's previous <see cref="State"/> was <see cref="StateType.Unchanged"/>, it
    /// will be <see cref="StateType.Changed"/> after this method has been called.
    /// </summary>
    /// <exception cref="InvalidOperationException">This object is not in state <see cref="StateType.Changed"/> or <see cref="StateType.Unchanged"/>.
    /// New or deleted objects cannot be marked as changed.</exception>
    /// <exception cref="ObjectDiscardedException">The object has already been discarded.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the current transaction.</exception>
    public void MarkAsChanged ()
    {
      DefaultTransactionContext.MarkAsChanged ();
    }

    /// <summary>
    /// Ensures that this <see cref="DomainObject"/>'s data has been loaded into the default transaction, ie. in its binding transaction or - if
    /// none - <see cref="DomainObjects.ClientTransaction.Current"/>. If it hasn't, this method causes the object's data to be loaded.
    /// </summary>
    /// <exception cref="ObjectDiscardedException">The object has already been discarded.</exception>
    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the current transaction.</exception>
    /// <exception cref="ObjectNotFoundException">No data could be loaded for this <see cref="DomainObject"/> because the object was not
    /// found in the data source.</exception>
    public void EnsureDataAvailable ()
    {
      DefaultTransactionContext.EnsureDataAvailable ();
    }

    /// <summary>
    /// Deletes the <see cref="DomainObject"/> in the default transaction, ie. in its binding transaction or - if
    /// none - <see cref="DomainObjects.ClientTransaction.Current"/>.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    /// <remarks>To perform custom actions when a <see cref="DomainObject"/> is deleted <see cref="OnDeleting"/> and <see cref="OnDeleted"/> should be overridden.</remarks>
    protected void Delete ()
    {
      LifetimeService.DeleteObject (DefaultTransactionContext.ClientTransaction, this);
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
    /// object was created with the <c>new</c> operator instead of using the <see cref="NewObject{T}()"/> method, or the property is not virtual.</exception>
    protected PropertyAccessor CurrentProperty
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
    /// Calls the <see cref="OnReferenceInitialized"/> method, setting a flag indicating that no mapped properties must be used.
    /// </summary>
    internal void FinishReferenceInitialization ()
    {
      OnReferenceInitialized ();
      DomainObjectMixinCodeGenerationBridge.OnDomainObjectReferenceInitialized (this);
    }

    /// <summary>
    /// Calls the <see cref="OnLoaded(LoadMode)"/> method with the right <see cref="LoadMode"/> parameter.
    /// </summary>
    internal void OnLoaded ()
    {
      LoadMode loadMode = _needsLoadModeDataContainerOnly ? LoadMode.DataContainerLoadedOnly : LoadMode.WholeDomainObjectInitialized;
      _needsLoadModeDataContainerOnly = true;

      DomainObjectMixinCodeGenerationBridge.OnDomainObjectLoaded (this, loadMode);
      OnLoaded (loadMode);
    }

    /// <summary>
    /// This method is invoked after this <see cref="DomainObject"/> reference has been initialized. This occurs whenever a <see cref="DomainObject"/> 
    /// is initialized, no matter whether the object is created, loaded, transported, cloned, or somehow else instantiated, and it occurs at a point 
    /// of time where it is safe to access the <see cref="ID"/> of the object. The <see cref="OnReferenceInitialized"/> notification occurs exactly 
    /// once per DomainObject, and its purpose is the initialization of DomainObject fields that do not depend on the object's mapped data properties. 
    /// See restrictions in the Remarks section.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Override this method to initialize fields and properties of a <see cref="DomainObject"/> that do not depend on the object's mapped data
    /// properties, no matter how the object is created. Object deserialization is not regarded as the initialization of a reference. Use the 
    /// deserialization hooks provided by the .NET framework (deserialization constructor, <see cref="IDeserializationCallback"/>) to react on the 
    /// deserialization of an object, or simply include the fields in the serialization process. 
    /// </para>
    /// <para>
    /// While this method is being executed, it is not possible to access any properties or methods of the DomainObject that read or modify the state 
    /// or data of the object in a <see cref="ClientTransaction"/>. All automatically implemented properties, <see cref="CurrentProperty"/>, 
    /// <see cref="Properties"/>, <see cref="State"/>, <see cref="Timestamp"/>, <see cref="MarkAsChanged"/>, <see cref="EnsureDataAvailable"/>, etc. 
    /// will throw <see cref="InvalidOperationException"/>. It is possible to call <see cref="GetBindingTransaction"/> on the object (if the object 
    /// is bound), and the object is guaranteed to be enlisted in the <see cref="ClientTransaction.Current"/> transaction.
    /// </para>
    /// <para>The reason why it is explicitly disallowed to access mapped properties from the notification method is that 
    /// <see cref="OnReferenceInitialized"/> is usually called when no data has yet been loaded for the object. Accessing a property would cause the 
    /// data to be loaded, defeating lazy loading via object references.
    /// </para>
    /// <para>
    /// To initialize an object based on its data, use the constructor, <see cref="OnLoaded(Remotion.Data.DomainObjects.LoadMode)"/>, or the facility 
    /// callbacks. <see cref="OnLoaded(Remotion.Data.DomainObjects.LoadMode)"/> might be called more than once per object.
    /// </para>
    /// </remarks>
    protected virtual void OnReferenceInitialized ()
    {
    }

    /// <summary>
    /// This method is invoked after the loading process of the object is completed.
    /// </summary>
    /// <param name="loadMode">Specifies whether the whole domain object or only the <see cref="Remotion.Data.DomainObjects.DataManagement.DataContainer"/> has been
    /// newly loaded.</param>
    /// <remarks>
    /// <para>
    /// Override this method to initialize <see cref="DomainObject"/>s that are loaded from the underlying storage.
    /// </para>
    /// <para>
    /// When a <see cref="DomainObject"/> is loaded for the first time, a new <see cref="DomainObject"/> reference will be created for it. In this
    /// case, the <see cref="OnLoaded(LoadMode)"/> method will be called with <see cref="LoadMode.WholeDomainObjectInitialized"/> being passed to the
    /// method. When, however, an additional <see cref="DataContainer"/> is loaded for an existing <see cref="DomainObject"/> reference - 
    /// in reaction to an existing <see cref="DomainObject"/> being loaded into another transaction (eg. a subtransaction), 
    /// <see cref="LoadMode.DataContainerLoadedOnly"/> is passed to the method.
    /// </para>
    /// <para>
    /// Even when an object is first loaded in a subtransaction, this method is called once with <see cref="LoadMode.WholeDomainObjectInitialized"/>,
    /// and then once with <see cref="LoadMode.DataContainerLoadedOnly"/>. <see cref="LoadMode.WholeDomainObjectInitialized"/> can thus be used to
    /// identify when the object was actually loaded from the underlying storage.
    /// </para>
    /// </remarks>
    protected virtual void OnLoaded (LoadMode loadMode)
    {
    }

    /// <summary>
    /// This method is invoked before an object's data is unloaded from the <see cref="ClientTransaction.Current"/> transaction.
    /// </summary>
    /// <remarks>
    /// <note type="inotes">Overrides of this method can throw an exception in order to stop the operation.</note>
    /// </remarks>
    protected internal virtual void OnUnloading ()
    {
    }

    /// <summary>
    /// This method is invoked after an object's data has been unloaded from the <see cref="ClientTransaction.Current"/> transaction.
    /// </summary>
    /// <remarks>
    /// <note type="inotes">Overrides of this method must not throw an exception; the operation has already been performed here.</note>
    /// </remarks>
    protected internal virtual void OnUnloaded ()
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
    /// This method is invoked once per involved operation and thus might be raised more often than <see cref="OnRelationChanged"/>. For example,
    /// when a whole related object collection is replaced in one go, this method is invoked once for each old object that is not in the new collection
    /// and once for each new object not in the old collection.
    /// </summary>
    /// <param name="args">A <see cref="RelationChangingEventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnRelationChanging (RelationChangingEventArgs args)
    {
      if (RelationChanging != null)
        RelationChanging (this, args);
    }

    /// <summary>
    /// Raises the <see cref="RelationChanged"/> event.
    /// This method is only invoked once per relation change and thus might be invoked less often than <see cref="OnRelationChanging"/>.
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
  }
}
