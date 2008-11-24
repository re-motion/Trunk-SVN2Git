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
using System.Runtime.Serialization;
using Remotion.Collections;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Represents a container for the persisted properties of a DomainObject.
  /// </summary>
  public sealed class DataContainer : IFlattenedSerializable
  {
    // types

    private enum DataContainerStateType
    {
      Existing = 0,
      New = 1,
      Deleted = 2
    }

    // static members and constants

    /// <summary>
    /// Creates an empty <see cref="DataContainer"/> for a new <see cref="Remotion.Data.DomainObjects.DomainObject"/>. The <see cref="DataContainer"/>
    /// contains a new <see cref="PropertyValue"/> object for every <see cref="PropertyDefinition"/> in the respective <see cref="ClassDefinition"/>.
    /// </summary>
    /// <remarks>
    /// The new <see cref="DataContainer"/> has a <see cref="State"/> of <see cref="StateType.New"/>. All <see cref="PropertyValue"/>s for the class specified by <see cref="ObjectID.ClassID"/> are created.
    /// </remarks>
    /// <param name="id">The <see cref="ObjectID"/> of the new <see cref="DataContainer"/> to create. Must not be <see langword="null"/>.</param>
    /// <returns>The new <see cref="DataContainer"/>.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    public static DataContainer CreateNew (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      DataContainer newDataContainer = new DataContainer (id);
      newDataContainer._state = DataContainerStateType.New;

      InitializeDefaultPropertyValues (newDataContainer);
      return newDataContainer;
    }

    /// <summary>
    /// Creates an empty <see cref="DataContainer"/> for an existing <see cref="Remotion.Data.DomainObjects.DomainObject"/>. The <see cref="DataContainer"/>
    /// contain all <see cref="PropertyValue"/> objects, just as if it had been created with <see cref="CreateNew"/>, but the values for persistent 
    /// properties are set as returned by a lookup method.
    /// </summary>
    /// <remarks>
    /// The new <see cref="DataContainer"/> has a <see cref="State"/> of <see cref="StateType.Unchanged"/>. All <see cref="PropertyValue"/>s for the class specified by <see cref="ObjectID.ClassID"/> are created.
    /// </remarks>
    /// <param name="id">The <see cref="ObjectID"/> of the new <see cref="DataContainer"/> to create. Must not be <see langword="null"/>.</param>
    /// <param name="timestamp">The timestamp value of the existing object in the datasource.</param>
    /// <param name="persistentValueLookup">A function object returning the value of a given persistent property for the existing object.</param>
    /// <returns>The new <see cref="DataContainer"/>.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    /// <exception cref="Mapping.MappingException">ClassDefinition of <paramref name="id"/> does not exist in mapping.</exception>
    public static DataContainer CreateForExisting (ObjectID id, object timestamp, Func<PropertyDefinition, object> persistentValueLookup)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      DataContainer dataContainer = new DataContainer (id, timestamp);
      dataContainer._state = DataContainerStateType.Existing;

      foreach (PropertyDefinition propertyDefinition in dataContainer.ClassDefinition.GetPropertyDefinitions ())
      {
        if (propertyDefinition.StorageClass == StorageClass.Persistent)
        {
          object value = persistentValueLookup (propertyDefinition);
          dataContainer.PropertyValues.Add (new PropertyValue (propertyDefinition, value));
        }
        else
          dataContainer.PropertyValues.Add (new PropertyValue (propertyDefinition));
      }

      return dataContainer;
    }

    private static void InitializeDefaultPropertyValues (DataContainer newDataContainer)
    {
      foreach (PropertyDefinition propertyDefinition in newDataContainer.ClassDefinition.GetPropertyDefinitions ())
        newDataContainer.PropertyValues.Add (new PropertyValue (propertyDefinition));
    }

    /// <summary>
    /// Creates a <see cref="DataContainer"/> for the given <paramref name="id"/>, assuming the same state as another <see cref="DataContainer"/>.
    /// </summary>
    /// <param name="id">The <see cref="ObjectID"/> of the new <see cref="DataContainer"/>. Must not be <see langword="null"/>.</param>
    /// <param name="stateSource">The <see cref="DataContainer"/> whose state to copy to the new container. Must not be <see langword="null"/> and must
    /// match the <see cref="ClassDefinition"/> of <paramref name="id"/>.</param>
    /// <returns>A <see cref="DataContainer"/> with exactly the same state as <paramref name="stateSource"/> and the given <paramref name="id"/>.</returns>
    /// <exception cref="ArgumentNullException">One of the arguments passed to this method is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The <see cref="ClassDefinition"/> specified in the given <paramref name="id"/> does not match the
    /// <see cref="ClassDefinition"/> of <paramref name="stateSource"/>.</exception>
    /// <remarks>
    /// <para>
    /// This is identical to a <see cref="Clone"/> operation on the object passed as <paramref name="stateSource"/>, but it allows the
    /// new <see cref="DataContainer"/> to assume a different <see cref="ObjectID"/> than <paramref name="stateSource"/>. It is meant mainly for
    /// <see cref="StorageProvider"/> implementations.
    /// </para>
    /// </remarks>
    public static DataContainer CreateAndCopyState (ObjectID id, DataContainer stateSource)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      ArgumentUtility.CheckNotNull ("stateSource", stateSource);

      if (!id.ClassDefinition.Equals (stateSource.ClassDefinition))
      {
        string message = string.Format (
            "The ID parameter specifies class '{0}', but the state source is of class '{1}'.",
            id.ClassDefinition,
            stateSource.ClassDefinition);
        throw new ArgumentException (message, "stateSource");
      }

      DataContainer dataContainer = CreateNew (id);
      dataContainer.AssumeSameState (stateSource);
      return dataContainer;
    }

    // member fields

    /// <summary>
    /// Occurs before a <see cref="PropertyValue"/> is changed.
    /// </summary>
    /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
    public event PropertyChangeEventHandler PropertyChanging;

    /// <summary>
    /// Occurs after a <see cref="PropertyValue"/> is changed.
    /// </summary>
    /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
    public event PropertyChangeEventHandler PropertyChanged;

    private readonly ObjectID _id;
    private readonly PropertyValueCollection _propertyValues;

    private ClientTransaction _clientTransaction;
    private object _timestamp;
    private DataContainerStateType _state;
    private DomainObject _domainObject;
    private RelationEndPointID[] _relationEndPointIDs = null;
    private bool _isDiscarded = false;
    private bool _hasBeenMarkedChanged = false;

    // construction and disposing

    private DataContainer (ObjectID id)
        : this (id, null)
    {
    }

    private DataContainer (ObjectID id, object timestamp)
        : this (id, timestamp, new PropertyValueCollection())
    {
    }

    private DataContainer (ObjectID id, object timestamp, PropertyValueCollection propertyValues)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      ArgumentUtility.CheckNotNull ("propertyValues", propertyValues);
      if (id.ClassDefinition != MappingConfiguration.Current.ClassDefinitions.GetMandatory (id.ClassID))
      {
        string message = string.Format ("The ClassDefinition '{0}' of the ObjectID '{1}' is not part of the current mapping.", id.ClassDefinition, id);
        throw new ArgumentException (message, "id");
      }

      _id = id;
      _timestamp = timestamp;

      _propertyValues = propertyValues;
      _propertyValues.RegisterForChangeNotification (this);
    }

    // methods and properties

    /// <summary>
    /// Gets the value of the <see cref="PropertyValue"/> specified by <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="propertyName">The name of the property. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="Remotion.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
    /// <exception cref="System.ArgumentException">The given <paramref name="propertyName"/> does not exist in the data container.</exception>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public object this [string propertyName]
    {
      get
      {
        ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
        CheckDiscarded();

        return _propertyValues[propertyName].Value;
      }
      set
      {
        ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
        CheckDiscarded();

        _propertyValues[propertyName].Value = value;
      }
    }

    /// <summary>
    /// Gets the value of the <see cref="PropertyValue"/> specified by <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="propertyName">The name of the <see cref="PropertyValue"/>. Must not be <see langword="null"/>.</param>
    /// <returns>The value of the <see cref="PropertyValue"/>.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="Remotion.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
    /// <exception cref="System.ArgumentException">The given <paramref name="propertyName"/> does not exist in the data container.</exception>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public object GetValue (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      CheckDiscarded();

      return this[propertyName];
    }

    /// <summary>
    /// Sets the value of the <see cref="PropertyValue"/> specified by <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="propertyName">The name of the <see cref="PropertyValue"/>. Must not be <see langword="null"/>.</param>
    /// <param name="value">The value the <see cref="PropertyValue"/> is set to.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="Remotion.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
    /// <exception cref="System.ArgumentException">The given <paramref name="propertyName"/> does not exist in the data container.</exception>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public void SetValue (string propertyName, object value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      CheckDiscarded();

      this[propertyName] = value;
    }


    /// <summary>
    /// Gets the <see cref="Remotion.Data.DomainObjects.ClientTransaction"/> which the <see cref="DataContainer"/> is part of.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public ClientTransaction ClientTransaction
    {
      get
      {
        CheckDiscarded();

        if (_clientTransaction == null)
          throw new DomainObjectException ("Internal error: ClientTransaction of DataContainer is not set.");

        return _clientTransaction;
      }
    }

    /// <summary>
    /// Gets the <see cref="Remotion.Data.DomainObjects.DomainObject"/> associated with the <see cref="DataContainer"/>.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public DomainObject DomainObject
    {
      get
      {
        if (_domainObject == null)
        {
          Assertion.IsFalse (IsDiscarded, "DataContainers cannot be discarded when they don't have a DomainObject referende");
          _domainObject = ClientTransaction.GetObjectForDataContainer (this);
        }

        return _domainObject;
      }
    }

    /// <summary>
    /// Gets the <see cref="ObjectID"/> of the <see cref="DataContainer"/>.
    /// </summary>
    /// <remarks>
    /// This property can also be used when the object is already discarded.
    /// </remarks>
    public ObjectID ID
    {
      get { return _id; }
    }

    /// <summary>
    /// Gets the <see cref="Mapping.ClassDefinition"/> of the <see cref="DataContainer"/>.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public ClassDefinition ClassDefinition
    {
      get
      {
        CheckDiscarded();
        return _id.ClassDefinition;
      }
    }

    /// <summary>
    /// Gets the <see cref="Type"/> of the <see cref="Remotion.Data.DomainObjects.DomainObject"/> of the <see cref="DataContainer"/>.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public Type DomainObjectType
    {
      get
      {
        CheckDiscarded();
        return _id.ClassDefinition.ClassType;
      }
    }


    /// <summary>
    /// Gets the <see cref="PropertyValueCollection"/> of all <see cref="PropertyValue"/>s that are part of the <see cref="DataContainer"/>.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public PropertyValueCollection PropertyValues
    {
      get
      {
        CheckDiscarded();
        return _propertyValues;
      }
    }


    /// <summary>
    /// Gets the state of the <see cref="DataContainer"/>.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public StateType State
    {
      get
      {
        if (_isDiscarded)
          return StateType.Discarded;
        else if (_state == DataContainerStateType.Existing)
          return GetStateForExistingDataContainer();
        else if (_state == DataContainerStateType.New)
          return StateType.New;
        else
          return StateType.Deleted;
      }
    }

    /// <summary>
    /// Marks an existing <see cref="DataContainer"/> as changed. <see cref="State"/> will return <see cref="StateType.Changed"/> after this method
    /// has been called.
    /// </summary>
    /// <exception cref="InvalidOperationException">This <see cref="DataContainer"/> is not in state <see cref="DataContainerStateType.Existing"/>.
    /// New or deleted objects cannot be marked as changed.</exception>
    /// <exception cref="ObjectDiscardedException">The object has already been discarded.</exception>
    public void MarkAsChanged ()
    {
      CheckDiscarded();
      if (_state != DataContainerStateType.Existing)
        throw new InvalidOperationException ("Only existing DataContainers can be marked as changed.");
      _hasBeenMarkedChanged = true;
    }

    /// <summary>
    /// Gets the timestamp of the last committed change of the data in the <see cref="DataContainer"/>.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public object Timestamp
    {
      get
      {
        CheckDiscarded();
        return _timestamp;
      }
    }

    /// <summary>
    /// Gets a value indicating the discarded status of the <see cref="DataContainer"/>.
    /// </summary>
    /// <remarks>
    /// For more information why and when a <see cref="DataContainer"/> is discarded see <see cref="Remotion.Data.DomainObjects.DataManagement.ObjectDiscardedException"/>.
    /// </remarks>
    public bool IsDiscarded
    {
      get { return _isDiscarded; }
    }

    internal RelationEndPointID[] RelationEndPointIDs
    {
      get
      {
        if (_relationEndPointIDs == null)
          _relationEndPointIDs = RelationEndPointID.GetAllRelationEndPointIDs (this);

        return _relationEndPointIDs;
      }
    }

    internal void SetClientTransaction (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      _clientTransaction = clientTransaction;
    }


    /// <summary>
    /// Raises the <see cref="PropertyChanging"/> event.
    /// </summary>
    /// <param name="args">A <see cref="PropertyChangeEventArgs"/> object that contains the event data.</param>
    private void OnPropertyChanging (PropertyChangeEventArgs args)
    {
      if (PropertyChanging != null)
        PropertyChanging (this, args);
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="args">A <see cref="PropertyChangeEventArgs"/> object that contains the event data.</param>
    private void OnPropertyChanged (PropertyChangeEventArgs args)
    {
      if (PropertyChanged != null)
        PropertyChanged (this, args);
    }


    internal void Delete ()
    {
      CheckDiscarded();

      if (_state == DataContainerStateType.New)
        Discard();

      _state = DataContainerStateType.Deleted;
    }

    internal void SetTimestamp (object timestamp)
    {
      ArgumentUtility.CheckNotNull ("timestamp", timestamp);

      _timestamp = timestamp;
    }

    internal void Commit ()
    {
      CheckDiscarded();

      _hasBeenMarkedChanged = false;

      if (_state == DataContainerStateType.Deleted)
        Discard();
      else
      {
        foreach (PropertyValue propertyValue in _propertyValues)
          propertyValue.Commit();

        _state = DataContainerStateType.Existing;
      }
    }

    internal void Rollback ()
    {
      CheckDiscarded ();

      _hasBeenMarkedChanged = false;

      if (_state == DataContainerStateType.New)
        Discard();
      else
      {
        foreach (PropertyValue propertyValue in _propertyValues)
          propertyValue.Rollback();

        _state = DataContainerStateType.Existing;
      }
    }

    internal void SetDomainObject (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      Assertion.IsTrue (_domainObject == null || _domainObject == domainObject, "a DataContainer can only be associated with one DomainObject");

      _domainObject = domainObject;
    }

    internal object GetFieldValue (string propertyName, ValueAccess valueAccess)
    {
      return _propertyValues[propertyName].GetFieldValue (valueAccess);
    }

    internal void PropertyValueChanging (PropertyValueCollection propertyValueCollection, PropertyChangeEventArgs args)
    {
      if (_state == DataContainerStateType.Deleted)
        throw new ObjectDeletedException (_id);

      if (_clientTransaction != null)
        _clientTransaction.TransactionEventSink.PropertyValueChanging (this, args.PropertyValue, args.OldValue, args.NewValue);

      if (args.PropertyValue.PropertyType != typeof (ObjectID))
      {
        // Note: .NET 1.1 will not deserialize delegates to non-public (that means internal, protected, private) methods. 
        // Therefore notification of DomainObject when changing property values is not organized through events.
        DomainObject.EventManager.BeginPropertyValueChange (args.PropertyValue, args.OldValue, args.NewValue);

        OnPropertyChanging (args);
      }
    }

    internal void PropertyValueChanged (PropertyValueCollection propertyValueCollection, PropertyChangeEventArgs args)
    {
      if (args.PropertyValue.PropertyType != typeof (ObjectID))
      {
        OnPropertyChanged (args);

        // Note: .NET 1.1 will not deserialize delegates to non-public (that means internal, protected, private) methods. 
        // Therefore notification of DomainObject when changing property values is not organized through events.
        DomainObject.EventManager.EndPropertyValueChange (args.PropertyValue, args.OldValue, args.NewValue);
      }

      if (_clientTransaction != null)
        _clientTransaction.TransactionEventSink.PropertyValueChanged (this, args.PropertyValue, args.OldValue, args.NewValue);
    }

    internal void PropertyValueReading (PropertyValue propertyValue, ValueAccess valueAccess)
    {
      if (_clientTransaction != null)
        _clientTransaction.TransactionEventSink.PropertyValueReading (this, propertyValue, valueAccess);
    }

    internal void PropertyValueRead (PropertyValue propertyValue, object value, ValueAccess valueAccess)
    {
      if (_clientTransaction != null)
        _clientTransaction.TransactionEventSink.PropertyValueRead (this, propertyValue, value, valueAccess);
    }

    private StateType GetStateForExistingDataContainer ()
    {
      Assertion.IsFalse (IsDiscarded);
      Assertion.IsTrue (_state == DataContainerStateType.Existing);

      if (_hasBeenMarkedChanged)
        return StateType.Changed;
      else
      {
        foreach (PropertyValue propertyValue in _propertyValues)
        {
          if (propertyValue.HasChanged)
            return StateType.Changed;
        }

        return StateType.Unchanged;
      }
    }

    private void Discard ()
    {
      CheckDiscarded();

      if (_domainObject == null)
        throw new InvalidOperationException ("A DataContainer cannot be discarded while it doesn't have an associated DomainObject.");

      _clientTransaction.DataManager.MarkDiscarded (this);

      _propertyValues.Discard();
      _clientTransaction = null;

      _isDiscarded = true;
    }

    private void CheckDiscarded ()
    {
      if (_isDiscarded)
        throw new ObjectDiscardedException (_id);
    }

    /// <summary>
    /// Creates a copy of this data container and its state.
    /// </summary>
    /// <returns>A copy of this data container with the same <see cref="ObjectID"/> and the same property values. The copy's
    /// <see cref="ClientTransaction"/> member is not set, so the returned <see cref="DataContainer"/> cannot be used until it is registered with a
    /// <see cref="ClientTransaction"/>.</returns>
    public DataContainer Clone ()
    {
      CheckDiscarded();

      DataContainer clone = CreateAndCopyState (_id, this);
      Assertion.IsNull (clone._clientTransaction);
      return clone;
    }

    private void AssumeSameState (DataContainer sourceContainer)
    {
      Assertion.IsTrue (sourceContainer.ClassDefinition == ClassDefinition);

      _state = sourceContainer._state;
      _timestamp = sourceContainer._timestamp;
      _isDiscarded = sourceContainer._isDiscarded;
      _hasBeenMarkedChanged = sourceContainer._hasBeenMarkedChanged;

      Assertion.IsTrue (
          _domainObject == null || sourceContainer._domainObject == null || _domainObject == sourceContainer._domainObject,
          "State should only be copied between DataContainers referring to the same DomainObjects");
      _domainObject = sourceContainer._domainObject;

      AssumeSamePropertyValues(sourceContainer);
    }

    internal void AssumeSamePropertyValues (DataContainer sourceContainer)
    {
      _relationEndPointIDs = null; // reinitialize on next use

      for (int i = 0; i < _propertyValues.Count; ++i)
        _propertyValues[i].AssumeSameState (sourceContainer._propertyValues[i]);
    }

    internal void TakeOverCommittedData (DataContainer sourceContainer)
    {
      Assertion.IsTrue (sourceContainer.ClassDefinition == ClassDefinition);
      Assertion.IsTrue (sourceContainer._domainObject == _domainObject || _domainObject == null);

      _isDiscarded = sourceContainer._isDiscarded;
      _timestamp = sourceContainer._timestamp;
      _hasBeenMarkedChanged |= sourceContainer._hasBeenMarkedChanged;
      _domainObject = sourceContainer._domainObject;
      _relationEndPointIDs = null; // reinitialize on next use

      for (int i = 0; i < _propertyValues.Count; ++i)
        _propertyValues[i].TakeOverCommittedData (sourceContainer._propertyValues[i]);
    }

    #region Serialization

    private DataContainer (FlattenedDeserializationInfo info)
        : this (info.GetValueForHandle<ObjectID> (), info.GetValue<object> (), new PropertyValueCollection())
    {
      InitializeDefaultPropertyValues (this);

      _isDiscarded = info.GetBoolValue ();
      if (!_isDiscarded)
        RestorePropertyValuesFromData (info);

      PropertyChanging += info.GetValue<PropertyChangeEventHandler>();
      PropertyChanged += info.GetValue<PropertyChangeEventHandler>();
      _clientTransaction = info.GetValueForHandle<ClientTransaction> ();
      _state = info.GetValue<DataContainerStateType> ();
      _domainObject = info.GetValueForHandle<DomainObject> ();
      _hasBeenMarkedChanged = info.GetBoolValue ();
    }

    private void RestorePropertyValuesFromData (FlattenedDeserializationInfo info)
    {
      // TODO: wrap exceptions
      int numberOfProperties = _propertyValues.Count;
      for (int i = 0; i < numberOfProperties; ++i)
      {
        string propertyName = info.GetValueForHandle<string>();
        _propertyValues[propertyName].DeserializeFromFlatStructure (info);
      }
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_id);
      info.AddValue (_timestamp);
      info.AddBoolValue (_isDiscarded);
      if (!_isDiscarded)
      {
        foreach (PropertyValue propertyValue in _propertyValues)
        {
          info.AddHandle (propertyValue.Name);
          propertyValue.SerializeIntoFlatStructure (info);
        }
      }

      info.AddValue (PropertyChanging);
      info.AddValue (PropertyChanged);
      info.AddHandle (_clientTransaction);
      info.AddValue (_state);
      info.AddHandle (_domainObject);
      info.AddBoolValue (_hasBeenMarkedChanged);
    }

    #endregion Serialization
  }
}
