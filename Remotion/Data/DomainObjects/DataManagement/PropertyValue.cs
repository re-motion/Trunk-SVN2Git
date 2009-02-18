// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Represents a property of a domain object that is persisted by the framework.
  /// </summary>
  public class PropertyValue
  {
    // types

    // static members and constants

    private static bool AreValuesDifferent (object value1, object value2)
    {
      return !object.Equals (value1, value2);
    }

    // member fields

    private readonly PropertyDefinition _definition;
    private readonly ArrayList _accessObservers;
  
    // actual property data
    private object _value;
    private object _originalValue;
    private bool _hasBeenTouched;
    private bool _isDiscarded;

    // construction and disposing

    /// <summary>
    /// Initializes a new <b>PropertyValue</b> with a given <see cref="PropertyDefinition"/>.
    /// </summary>
    /// <param name="definition">The <see cref="PropertyDefinition"/> to use for initializing the <b>PropertyValue</b>. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="definition"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidEnumDefinitionException"><paramref name="definition"/> is a reference to an invalid enum.</exception>
    public PropertyValue (PropertyDefinition definition) : this (definition, definition.DefaultValue)
    {
    }

    /// <summary>
    /// Initializes a new <b>PropertyValue</b> with a given <see cref="PropertyDefinition"/> and an initial <see cref="Value"/>.
    /// </summary>
    /// <param name="definition">The <see cref="PropertyDefinition"/> to use for initializing the <b>PropertyValue</b>. Must not be <see langword="null"/>.</param>
    /// <param name="value">The initial <see cref="Value"/> for the <b>PropertyValue</b>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="definition"/> is <see langword="null"/>.</exception>
    /// <exception cref="Remotion.Data.DomainObjects.InvalidTypeException"><paramref name="value"/> does not match the required type specified in <paramref name="definition"/>.</exception>
    /// <exception cref="Remotion.Data.DomainObjects.ValueTooLongException"><paramref name="value"/> is longer than the maximum length specified in <paramref name="definition"/>.</exception>
    public PropertyValue (PropertyDefinition definition, object value)
        : this (definition, value, value)
    {
    }

    private PropertyValue (PropertyDefinition definition, object value, object originalValue)
    {
      ArgumentUtility.CheckNotNull ("definition", definition);

      if (!definition.PropertyType.IsValueType
          && definition.PropertyType != typeof (string)
          && definition.PropertyType != typeof (byte[]) 
          && definition.PropertyType != typeof (Type)
          && definition.PropertyType != typeof (ObjectID))
      {
        var message = string.Format ("The property '{0}' (declared on class '{1}') is invalid because its values cannot be copied. Only value types, "
            + "strings, the Type type, byte arrays, and ObjectIDs are currently supported, but the property's type is '{2}'.",
            definition.PropertyName, definition.ClassDefinition.ID, definition.PropertyType.FullName);
        throw new NotSupportedException (message);
      }

      CheckValue (value, definition);
      CheckValue (originalValue, definition);

      _definition = definition;
      _value = value;
      _originalValue = originalValue;
      _isDiscarded = false;
      _hasBeenTouched = false;
      _accessObservers = new ArrayList ();
    }

    // methods and properties

    /// <summary>
    /// Gets the <see cref="PropertyDefinition"/> of the <see cref="PropertyValue"/>.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public PropertyDefinition Definition
    {
      get 
      {
        CheckDiscarded ();
        return _definition; 
      }
    }

    /// <summary>
    /// Gets the name of the <see cref="PropertyValue"/>.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public string Name
    {
      get 
      {
        CheckDiscarded ();
        return _definition.PropertyName; 
      }
    }

    /// <summary>
    /// Gets the <see cref="Type"/> of the <see cref="Value"/> of a <see cref="PropertyValue"/>.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public Type PropertyType
    {
      get 
      {
        CheckDiscarded ();
        return _definition.PropertyType; 
      }
    }

    /// <summary>
    /// Gets or sets the value of the <see cref="PropertyValue"/>.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    /// <exception cref="Remotion.Data.DomainObjects.InvalidTypeException"><paramref name="value"/> does not match the required type specified in <see cref="Definition"/>.</exception>
    /// <exception cref="Remotion.Data.DomainObjects.ValueTooLongException"><paramref name="value"/> is longer than the maximum length specified in <see cref="Definition"/>.</exception>
    public object Value
    {
      get
      {
        CheckDiscarded ();

        // Note: A ClientTransaction extension could possibly raise an exception during BeginValueGet.
        //       If another ClientTransaction extension only wants to be notified on success it should use EndValueGet.
        BeginValueGet (ValueAccess.Current);
        EndValueGet ();
      
        return _value;
      }
      set
      {
        CheckDiscarded ();
        CheckForRelationProperty ();

        SetValueInternal (value);
      }
    }

    private void SetValueInternal (object value)
    {
      if (AreValuesDifferent (_value, value))
      {
        CheckValue (value, _definition);

        BeginValueSet (value);

        object oldValue = _value;
        _value = value;

        _hasBeenTouched = true;

        EndValueSet (oldValue);
      }
      else
        _hasBeenTouched = true;
    }

    /// <summary>
    /// Gets the original <see cref="Value"/> of the <see cref="PropertyValue"/> at the point of instantiation, loading, commit or rollback.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public object OriginalValue
    {
      get 
      { 
        CheckDiscarded ();

        // Note: A ClientTransaction extension could possibly raise an exception during BeginOriginalValueGet.
        //       If another ClientTransaction extension only wants to be notified on success it should use EndOriginalValueGet.
        BeginValueGet (ValueAccess.Original);
        EndOriginalValueGet ();

        return _originalValue; 
      }
    }

    /// <summary>
    /// Indicates whether the <see cref="PropertyValue"/> may contain <see langword="null"/> as a value.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public bool IsNullable
    {
      get 
      {
        CheckDiscarded ();
        return _definition.IsNullable; 
      }
    }

    /// <summary>
    /// Gets the maximum length of the <see cref="Value"/> of the <see cref="PropertyValue"/>.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public int? MaxLength 
    {
      get 
      {
        CheckDiscarded ();
        return _definition.MaxLength; 
      }
    }

    /// <summary>
    /// Indicates if the <see cref="Value"/> of the <see cref="PropertyValue"/> has changed since instantiation, loading, commit or rollback.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public bool HasChanged
    {
      get 
      {
        CheckDiscarded ();
        return AreValuesDifferent (_value, _originalValue);
      }
    }

    /// <summary>
    /// Indicates if the <see cref="Value"/> of the <see cref="PropertyValue"/> has been assigned since instantiation, loading, commit or rollback,
    /// regardless of whether the current value differs from the <see cref="OriginalValue"/>.
    /// </summary>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public bool HasBeenTouched
    {
      get
      {
        CheckDiscarded ();
        return _hasBeenTouched;
      }
    }

    // TODO 1032: Remove
    internal void Touch ()
    {
      _hasBeenTouched = true;
    }

    /// <summary>
    /// Determines whether the specified <see cref="PropertyValue"/> is equal to the current <b>PropertyValue</b>.
    /// </summary>
    /// <param name="obj">The <see cref="PropertyValue"/> to compare with the current <b>PropertyValue</b>. </param>
    /// <returns><see langword="true"/> if the specified <see cref="PropertyValue"/> is equal to the current <b>PropertyValue</b>; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
    public override bool Equals (object obj)
    {
      CheckDiscarded ();

      PropertyValue propertyValue = obj as PropertyValue;

      if (propertyValue != null)
      {
        return this._value.Equals (propertyValue._value)
               && this._originalValue.Equals (propertyValue._originalValue)
               && this.HasChanged.Equals (propertyValue.HasChanged)
               && this.Definition.Equals (propertyValue.Definition);
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
      CheckDiscarded ();
      return EqualityUtility.GetRotatedHashCode (_definition.PropertyName, _value, _originalValue, HasChanged);
    }

    /// <summary>
    /// Gets a value indicating the discarded status of the <see cref="PropertyValue"/>.
    /// </summary>
    /// <remarks>
    /// For more information why and when a <see cref="PropertyValue"/> is discarded see <see cref="Remotion.Data.DomainObjects.DataManagement.ObjectDiscardedException"/>.
    /// </remarks>
    public bool IsDiscarded
    {
      get { return _isDiscarded; }
    }

    internal object GetFieldValue (ValueAccess valueAccess)
    {
      if (valueAccess == ValueAccess.Current)
        return _value;
      else
        return _originalValue;
    }

    internal void RegisterForAccessObservation (PropertyValueCollection propertyValueCollection)
    {
      ArgumentUtility.CheckNotNull ("propertyValueCollection", propertyValueCollection);
      _accessObservers.Add (propertyValueCollection);
    }

    internal void Commit ()
    {
      if (HasChanged)
      {
        _originalValue = _value;
        _hasBeenTouched = false;
      }
    }

    internal void Rollback ()
    {
      if (HasChanged)
      {
        _value = _originalValue;
        _hasBeenTouched = false;
      }
    }

    internal void SetRelationValue (ObjectID id)
    {
      SetValueInternal (id);
    }

    internal void Discard ()
    {
      _isDiscarded = true;
    }

    private void CheckValue (object value, PropertyDefinition definition)
    {
      if (value != null)
      {
        if (!definition.PropertyType.IsInstanceOfType (value))
          throw new InvalidTypeException (definition.PropertyName, definition.PropertyType, value.GetType ());

        if (value.GetType () == typeof (string))
          CheckStringValue ((string) value, definition);

        if (value.GetType () == typeof (byte[]))
          CheckByteArrayValue ((byte[]) value, definition);

        if (value.GetType ().IsEnum)
          CheckEnumValue (value, definition);
      }
      else
      {
        if (!definition.IsNullable)
        {
          throw new InvalidOperationException (string.Format ("Property '{0}' does not allow null values.", definition.PropertyName));
        }
      }
    }

    private void CheckStringValue (string value, PropertyDefinition definition)
    {
      if (definition.MaxLength.HasValue && value.Length > definition.MaxLength.Value)
      {
        string message = string.Format (
            "Value for property '{0}' is too long. Maximum number of characters: {1}.",
            definition.PropertyName, definition.MaxLength.Value);

        throw new ValueTooLongException (message, definition.PropertyName, definition.MaxLength.Value);
      }
    }

    private void CheckByteArrayValue (byte[] value, PropertyDefinition definition)
    {
      if (definition.MaxLength.HasValue && value.Length > definition.MaxLength.Value)
      {
        string message = string.Format (
            "Value for property '{0}' is too large. Maximum size: {1}.", 
            definition.PropertyName, definition.MaxLength.Value);

        throw new ValueTooLongException (message, definition.PropertyName, definition.MaxLength.Value);
      }
    }

    private void CheckEnumValue (object value, PropertyDefinition definition)
    {
      if (value != null)
      {
        Type underlyingType = definition.IsNullable ? NullableTypeUtility.GetBasicType (definition.PropertyType) : definition.PropertyType;
        if (!EnumUtility.IsValidEnumValue (underlyingType, value))
        {
          string message = string.Format (
              "Value '{0}' for property '{1}' is not defined by enum type '{2}'.",
              value,
              definition.PropertyName,
              underlyingType);

          throw new InvalidEnumValueException (message, definition.PropertyName, underlyingType, value);
        }
      }
    }

    private void BeginValueGet (ValueAccess valueAccess)
    {
      foreach (PropertyValueCollection accessObserver in _accessObservers)
        accessObserver.PropertyValueReading (this, valueAccess);
    }

    private void EndValueGet ()
    {
      foreach (PropertyValueCollection accessObserver in _accessObservers)
        accessObserver.PropertyValueRead (this, _value, ValueAccess.Current);
    }

    private void EndOriginalValueGet ()
    {
      foreach (PropertyValueCollection accessObserver in _accessObservers)
        accessObserver.PropertyValueRead (this, _originalValue, ValueAccess.Original);
    }

    private void BeginValueSet (object newValue)
    {
      ValueChangeEventArgs changingArgs = new ValueChangeEventArgs (_value, newValue);

      // Note: .NET 1.1 will not deserialize delegates to non-public (that means internal, protected, private) methods. 
      // Therefore notification of PropertyValueCollection when changing property values is not organized through events.
      foreach (PropertyValueCollection accessObserver in _accessObservers)
        accessObserver.PropertyValueChanging (this, changingArgs);
    }

    private void EndValueSet (object oldValue)
    {
      ValueChangeEventArgs changedArgs = new ValueChangeEventArgs (oldValue, _value);

      // Note: .NET 1.1 will not deserialize delegates to non-public (that means internal, protected, private) methods. 
      // Therefore notification of PropertyValueCollection when changing property values is not organized through events.
      foreach (PropertyValueCollection accessObserver in _accessObservers)
        accessObserver.PropertyValueChanged (this, changedArgs);
    }

    private void CheckForRelationProperty ()
    {
      if (IsRelationProperty)
        throw new InvalidOperationException (string.Format ("The relation property '{0}' cannot be set directly.", _definition.PropertyName));
    }

    public bool IsRelationProperty
    {
      get { return _definition.PropertyType == typeof (ObjectID); }
    }

    private void CheckDiscarded ()
    {
      if (_isDiscarded)
        throw new ObjectDiscardedException ();
    }

    internal void AssumeSameState (PropertyValue source)
    {
      Assertion.IsTrue (_definition == source._definition);
      _value = source._value;
      _originalValue = source._originalValue;
      _isDiscarded = source._isDiscarded;
      _hasBeenTouched = source._hasBeenTouched;
    }

    internal void TakeOverCommittedData (PropertyValue source)
    {
      Assertion.IsTrue (_definition == source._definition);
      _value = source._value;
      _isDiscarded = source._isDiscarded;
      _hasBeenTouched |= source._hasBeenTouched || HasChanged; // true if: we have been touched/source has been touched/we have changed
    }

    #region Serialization
    internal void DeserializeFromFlatStructure (FlattenedDeserializationInfo info)
    {
      _isDiscarded = info.GetBoolValue ();
      if (!_isDiscarded)
      {
        _hasBeenTouched = info.GetBoolValue ();
        _value = info.GetValue<object> ();
        if (_hasBeenTouched)
          _originalValue = info.GetValue<object> ();
        else
          _originalValue = _value;
      }
    }

    internal void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      if (_isDiscarded)
        info.AddBoolValue (true);
      else
      {
        info.AddBoolValue (_isDiscarded);
        info.AddBoolValue (_hasBeenTouched);
        info.AddValue (_value);
        if (_hasBeenTouched)
          info.AddValue (_originalValue);
      }
    }
    #endregion
  }
}