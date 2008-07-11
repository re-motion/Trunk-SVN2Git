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
using Remotion.Data.DomainObjects;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Collections;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Indicates the kind of a <see cref="DomainObject">DomainObject's</see> property.
  /// </summary>
  public enum PropertyKind
  {
    /// <summary>
    /// The property is a simple value.
    /// </summary>
    PropertyValue,
    /// <summary>
    /// The property is a single related domain object.
    /// </summary>
    RelatedObject,
    /// <summary>
    /// The property is a collection of related domain objects.
    /// </summary>
    RelatedObjectCollection
  }

  /// <summary>
  /// Provides an encapsulation of a <see cref="DomainObject">DomainObject's</see> property for simple access as well as static methods
  /// supporting working with properties.
  /// </summary>
  public struct PropertyAccessor
  {
    /// <summary>
    /// Gets the <see cref="PropertyKind"/> for a given property identifier and class definition.
    /// </summary>
    /// <param name="classDefinition">The <see cref="ClassDefinition"/> object describing the property's declaring class.</param>
    /// <param name="propertyIdentifier">The property identifier.</param>
    /// <returns>The <see cref="PropertyKind"/> of the property.</returns>
    /// <exception cref="ArgumentNullException">One of the method's arguments is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The domain object does not have a property with the given identifier.</exception>
    public static PropertyKind GetPropertyKind (ClassDefinition classDefinition, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      Tuple<PropertyDefinition, IRelationEndPointDefinition> propertyObjects = GetPropertyDefinitionObjects (classDefinition, propertyIdentifier);
      return GetPropertyKind (propertyObjects.B);
    }

    private static PropertyKind GetPropertyKind (IRelationEndPointDefinition relationEndPointDefinition)
    {
      if (relationEndPointDefinition == null)
        return PropertyKind.PropertyValue;
      else if (relationEndPointDefinition.Cardinality == CardinalityType.One)
        return PropertyKind.RelatedObject;
      else
        return PropertyKind.RelatedObjectCollection;
    }

    private static IPropertyAccessorStrategy GetStrategy (PropertyKind kind)
    {
      switch (kind)
      {
        case PropertyKind.PropertyValue:
          return ValuePropertyAccessorStrategy.Instance;
        case PropertyKind.RelatedObject:
          return RelatedObjectPropertyAccessorStrategy.Instance;
        default:
          Assertion.IsTrue (kind == PropertyKind.RelatedObjectCollection);
          return RelatedObjectCollectionPropertyAccessorStrategy.Instance;
      }
    }

    /// <summary>
    /// Returns the value type of the given property.
    /// </summary>
    /// <param name="classDefinition">The <see cref="ClassDefinition"/> object describing the property's declaring class.</param>
    /// <param name="propertyIdentifier">The property identifier.</param>
    /// <returns>The property's value type.</returns>
    /// <remarks>For simple value properties, this returns the simple property type. For related objects, it
    /// returns the related object's type. For related object collections, it returns type <see cref="ObjectList{T}"/>, where "T" is the related
    /// objects' type.</remarks>
    /// <exception cref="ArgumentNullException">One of the method's arguments is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The class definition does not have a property with the given identifier.</exception>
    public static Type GetPropertyType (ClassDefinition classDefinition, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      Tuple<PropertyDefinition, IRelationEndPointDefinition> definitionObjects =
        PropertyAccessor.GetPropertyDefinitionObjects (classDefinition, propertyIdentifier);

      return GetStrategy (GetPropertyKind (definitionObjects.B)).GetPropertyType (definitionObjects.A, definitionObjects.B);
    }

    /// <summary>
    /// Returns mapping objects for the given property.
    /// </summary>
    /// <param name="classDefinition">The <see cref="ClassDefinition"/> object describing the property's declaring class.</param>
    /// <param name="propertyIdentifier">The property identifier.</param>
    /// <returns>The property's <see cref="PropertyDefinition"/> and <see cref="IRelationEndPointDefinition"/> objects.</returns>
    /// <exception cref="ArgumentNullException">One of the method's arguments is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The class definition does not have a property with the given identifier.</exception>
    public static Tuple<PropertyDefinition, IRelationEndPointDefinition> GetPropertyDefinitionObjects (
        ClassDefinition classDefinition,
        string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      PropertyDefinition propertyDefinition = classDefinition.GetPropertyDefinition (propertyIdentifier);
      IRelationEndPointDefinition relationEndPointDefinition = classDefinition.GetRelationEndPointDefinition (propertyIdentifier);

      if (propertyDefinition == null && relationEndPointDefinition == null)
      {
        string message = string.Format (
            "The domain object type {0} does not have a mapping property named '{1}'.",
            classDefinition.ClassType.FullName,
            propertyIdentifier);

        throw new ArgumentException (message, "propertyIdentifier");
      }
      else
        return new Tuple<PropertyDefinition, IRelationEndPointDefinition> (propertyDefinition, relationEndPointDefinition);
    }

    /// <summary>
    /// Checks whether the given property identifier denotes an existing property on the given <see cref="ClassDefinition"/>.
    /// </summary>
    /// <param name="classDefinition">The class definition to be checked.</param>
    /// <param name="propertyID">The property to be looked for.</param>
    /// <returns>True if <paramref name="classDefinition"/> contains a simple, related object, or related object collection property
    /// with the given identifier; false otherwise.</returns>
    public static bool IsValidProperty (ClassDefinition classDefinition, string propertyID)
    {
      return classDefinition.GetPropertyDefinition (propertyID) != null || classDefinition.GetRelationEndPointDefinition (propertyID) != null;
    }

    private readonly DomainObject _domainObject;
    private readonly string _propertyIdentifier;
    private readonly PropertyKind _kind;

    private readonly PropertyDefinition _propertyDefinition;
    private readonly IRelationEndPointDefinition _relationEndPointDefinition;
    private readonly ClassDefinition _classDefinition;
    private readonly Type _propertyType;

    private readonly IPropertyAccessorStrategy _strategy;

    /// <summary>
    /// Initializes the <see cref="PropertyAccessor"/> object.
    /// </summary>
    /// <param name="domainObject">The domain object whose property is to be encapsulated.</param>
    /// <param name="propertyIdentifier">The identifier of the property to be encapsulated.</param>
    /// <exception cref="ArgumentNullException">One of the parameters passed to the constructor is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The domain object does not have a property with the given identifier.</exception>
    internal PropertyAccessor (DomainObject domainObject, string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("propertyIdentifier", propertyIdentifier);

      _domainObject = domainObject;
      _propertyIdentifier = propertyIdentifier;
      _classDefinition = _domainObject.ID.ClassDefinition;

      Tuple<PropertyDefinition, IRelationEndPointDefinition> propertyObjects =
        PropertyAccessor.GetPropertyDefinitionObjects (_classDefinition, propertyIdentifier);
      _propertyDefinition = propertyObjects.A;
      _relationEndPointDefinition = propertyObjects.B;

      _kind = PropertyAccessor.GetPropertyKind (_relationEndPointDefinition);
      _strategy = PropertyAccessor.GetStrategy (_kind);

      _propertyType = _strategy.GetPropertyType (_propertyDefinition, _relationEndPointDefinition);
    }

    /// <summary>
    /// The definition object for the property's declaring class.
    /// </summary>
    public ClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    /// <summary>
    /// Indicates which kind of property is encapsulated by this structure.
    /// </summary>
    public PropertyKind Kind
    {
      get { return _kind; }
    }

    /// <summary>
    /// The identifier for the property encapsulated by this structure.
    /// </summary>
    public string PropertyIdentifier
    {
      get { return _propertyIdentifier; }
    }

    /// <summary>
    /// The property value type. For simple value properties, this is the simple property type. For related objects, this
    /// is the related object's type. For related object collections, this is <see cref="ObjectList{T}"/>, where "T" is the
    /// related objects' type.
    /// </summary>
    public Type PropertyType
    {
      get { return _propertyType; }
    }

    /// <summary>
    /// The encapsulated object's property definition object (can be <see langword="null"/>).
    /// </summary>
    public PropertyDefinition PropertyDefinition
    {
      get { return _propertyDefinition; }
    }

    /// <summary>
    /// The encapsulated object's relation end point definition object (can be <see langword="null"/>).
    /// </summary>
    public IRelationEndPointDefinition RelationEndPointDefinition
    {
      get { return _relationEndPointDefinition; }
    }

    /// <summary>
    /// Gets the domain object of this property.
    /// </summary>
    /// <value>The domain object this <see cref="PropertyAccessor"/> is associated with.</value>
    public DomainObject DomainObject
    {
      get { return _domainObject; }
    }

    private ClientTransaction DefaultTransaction
    {
      get { return DomainObject.GetNonNullClientTransaction(); }
    }

    private void CheckTransactionalStatus (ClientTransaction transaction)
    {
      _domainObject.CheckIfObjectIsDiscarded (transaction);
      _domainObject.CheckIfRightTransaction (transaction);
    }

    /// <summary>
    /// Indicates whether the property's value has been changed in its current transaction.
    /// </summary>
    /// <value>True if the property's value has changed; false otherwise.</value>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public bool HasChanged
    {
      get
      {
        return HasChangedTx (DefaultTransaction);
      }
    }

    /// <summary>
    /// Indicates whether the property's value has been changed in the given transaction.
    /// </summary>
    /// <param name="transaction">The transaction to be used when checking the property's status.</param>
    /// <value>True if the property's value has changed; false otherwise.</value>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the given <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public bool HasChangedTx (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      CheckTransactionalStatus (transaction);
      return _strategy.HasChanged (this, transaction);
    }

    /// <summary>
    /// Indicates whether  the property's value (for simple and related object properties) or one of its elements (for related object collection
    /// properties) has been assigned since instantiation, loading, commit or rollback, regardless of whether the current value differs from the
    /// original value.
    /// </summary>
    /// <remarks>This property differs from <see cref="HasChanged"/> in that for <see cref="HasChanged"/> to be true, the property's value (or its
    /// elements) actually must have changed in an assignment operation. <see cref="HasBeenTouched"/> is true also if a property gets assigned the
    /// same value it originally had. This can be useful to determine whether the property has been written once since the last load, commit, or
    /// rollback operation.
    /// </remarks>
    public bool HasBeenTouched
    {
      get
      {
        return HasBeenTouchedTx(DefaultTransaction);
      }
    }

    /// <summary>
    /// Indicates whether  the property's value (for simple and related object properties) or one of its elements (for related object collection
    /// properties) has been assigned since instantiation, loading, commit or rollback, regardless of whether the current value differs from the
    /// original value. The check is performed in the given transaction.
    /// </summary>
    /// <param name="transaction">The transaction to be used when checking the property's status.</param>
    /// <remarks>This property differs from <see cref="HasChanged"/> in that for <see cref="HasChanged"/> to be true, the property's value (or its
    /// elements) actually must have changed in an assignment operation. <see cref="HasBeenTouched"/> is true also if a property gets assigned the
    /// same value it originally had. This can be useful to determine whether the property has been written once since the last load, commit, or
    /// rollback operation.
    /// </remarks>
    public bool HasBeenTouchedTx (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      CheckTransactionalStatus (transaction);
      return _strategy.HasBeenTouched (this, transaction);
    }


    /// <summary>
    /// Gets a value indicating whether the property's value is <see langword="null"/>.
    /// </summary>
    /// <value>True if this instance is null; otherwise, false.</value>
    /// <remarks>This can be used to efficiently check whether a related object property has a value without actually loading the related
    /// object.</remarks>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public bool IsNull
    {
      get
      {
        CheckTransactionalStatus (DefaultTransaction);
        return _strategy.IsNull (this, DefaultTransaction);
      }
    }

    internal void CheckType (Type typeToCheck)
    {
      ArgumentUtility.CheckNotNull ("typeToCheck", typeToCheck);
      if (!PropertyType.Equals (typeToCheck))
        throw new InvalidTypeException (PropertyIdentifier, typeToCheck, PropertyType);
    }

    /// <summary>
    /// Gets the property's value.
    /// </summary>
    /// <typeparam name="T">
    /// The property value type. This must be the same as the type returned by <see cref="PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type.
    /// </typeparam>
    /// <returns>The value of the encapsulated property. For simple value properties,
    /// this is the property value. For related objects, this is the related object. For related object collections,
    /// this is an <see cref="ObjectList{T}"/>, where "T" is the related objects' type.</returns>
    /// <exception cref="InvalidTypeException">
    /// The type requested via <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyType"/>.
    /// </exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public T GetValue<T> ()
    {
      return GetValueTx<T> (DefaultTransaction);
    }

    /// <summary>
    /// Gets the ID of the related object for related object properties.
    /// </summary>
    /// <returns>The ID of the related object stored in the encapsulated property.</returns>
    /// <exception cref="InvalidOperationException">The property type is not <see cref="PropertyKind.RelatedObject"/> or the property is a virtual
    /// relation end point (i.e. the other side of the relation holds the foreign key).</exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public ObjectID GetRelatedObjectID ()
    {
      return GetRelatedObjectIDTx (DefaultTransaction);
    }

    /// <summary>
    /// Gets the property's value for a given <see cref="ClientTransaction"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The property value type. This must be the same as the type returned by <see cref="PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type.
    /// </typeparam>
    /// <param name="transaction">The <see cref="ClientTransaction"/> to get the value for.</param>
    /// <returns>The value of the encapsulated property. For simple value properties,
    /// this is the property value. For related objects, this is the related object. For related object collections,
    /// this is an <see cref="ObjectList{T}"/>, where "T" is the related objects' type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="transaction"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="InvalidTypeException">
    /// The type requested via <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyType"/>.
    /// </exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the given <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public T GetValueTx<T> (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      CheckType(typeof (T));

      object value = GetValueWithoutTypeCheckTx (transaction);
      Assertion.DebugAssert (
          value != null || !PropertyType.IsValueType || Nullable.GetUnderlyingType (PropertyType) != null,
          "Property '{0}' is a value type but the DataContainer returned null.",
          PropertyIdentifier);
      Assertion.DebugAssert (value == null || value is T);
      return (T) value;
    }

    /// <summary>
    /// Gets the ID of the related object for related object properties for a given <see cref="ClientTransaction"/>.
    /// </summary>
    /// <param name="transaction">The <see cref="ClientTransaction"/> to get the value for.</param>
    /// <returns>The ID of the related object stored in the encapsulated property in the given transaction.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="transaction"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The property type is not <see cref="PropertyKind.RelatedObject"/> or the property is a virtual
    /// relation end point (i.e. the other side of the relation holds the foreign key).</exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the given <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public ObjectID GetRelatedObjectIDTx (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      CheckTransactionalStatus (transaction);

      if (Kind != PropertyKind.RelatedObject)
        throw new InvalidOperationException ("This operation can only be used on related object properties.");

      if (RelationEndPointDefinition.IsVirtual)
        throw new InvalidOperationException ("ObjectIDs only exist on the real side of a relation, not on the virtual side.");

      return (ObjectID) ValuePropertyAccessorStrategy.Instance.GetValueWithoutTypeCheck (this, transaction);
    }

    /// <summary>
    /// Sets the property's value.
    /// </summary>
    /// <typeparam name="T">
    /// The property value type. This must be the same as the type returned by <see cref="PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type. The type parameter can usually be inferred and needn't be
    /// specified in such cases.
    /// </typeparam>
    /// <param name="value">The value to be set. For simple value properties,
    /// this is the value to be set. For related objects, this is the related object. For related object collections,
    /// this is an <see cref="ObjectList{T}"/>, where "T" is the related objects' type.</param>
    /// <exception cref="InvalidTypeException">
    /// The type <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyType"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">The property is a related object collection; such properties cannot be set.</exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public void SetValue<T> (T value)
    {
      SetValueTx (DefaultTransaction, value);
    }

    /// <summary>
    /// Sets the property's value for the given <see cref="ClientTransaction"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The property value type. This must be the same as the type returned by <see cref="PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type. The type parameter can usually be inferred and needn't be
    /// specified in such cases.
    /// </typeparam>
    /// <param name="transaction">The <see cref="ClientTransaction"/> to set the value for.</param>
    /// <param name="value">The value to be set. For simple value properties,
    /// this is the value to be set. For related objects, this is the related object. For related object collections,
    /// this is an <see cref="ObjectList{T}"/>, where "T" is the related objects' type.</param>
    /// <exception cref="InvalidTypeException">
    /// The type <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyType"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">The property is a related object collection; such properties cannot be set.</exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the given <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public void SetValueTx<T> (ClientTransaction transaction, T value)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      CheckType (typeof (T));

      SetValueWithoutTypeCheckTx (transaction, value);
    }

    /// <summary>
    /// Sets the property's value without performing an exact type check on the given value. The value must still be asssignable to
    /// <see cref="PropertyType"/>, though.
    /// </summary>
    /// <param name="value">The value to be set. For simple value properties,
    /// this is the value to be set. For related objects, this is the related object. For related object collections,
    /// this is an <see cref="ObjectList{T}"/>, where "T" is the related objects' type.</param>
    /// <exception cref="InvalidTypeException">
    /// The given <paramref name="value"/> is not assignable to the property because of its type.
    /// </exception>
    /// <exception cref="InvalidOperationException">The property is a related object collection; such properties cannot be set.</exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public void SetValueWithoutTypeCheck (object value)
    {
      SetValueWithoutTypeCheckTx (DefaultTransaction, value);
    }

    /// <summary>
    /// Sets the property's value without performing an exact type check on the given value for the given transaction. The value must still be
    /// asssignable to <see cref="PropertyType"/>, though.
    /// </summary>
    /// <param name="transaction">The <see cref="ClientTransaction"/> to set the value for.</param>
    /// <param name="value">The value to be set. For simple value properties,
    /// this is the value to be set. For related objects, this is the related object. For related object collections,
    /// this is an <see cref="ObjectList{T}"/>, where "T" is the related objects' type.</param>
    /// <exception cref="InvalidTypeException">
    /// The given <paramref name="value"/> is not assignable to the property because of its type.
    /// </exception>
    /// <exception cref="ArgumentNullException">The <paramref name="transaction"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The property is a related object collection; such properties cannot be set.</exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the given <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public void SetValueWithoutTypeCheckTx (ClientTransaction transaction, object value)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      CheckTransactionalStatus (transaction);
      _strategy.SetValueWithoutTypeCheck (this, transaction, value);
    }

    /// <summary>
    /// Gets the property's value without performing a type check.
    /// </summary>
    /// <returns>The value of the encapsulated property. For simple value properties,
    /// this is the property value. For related objects, this is the related object. For related object collections,
    /// this is an <see cref="ObjectList{T}"/>, where "T" is the related objects' type.</returns>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public object GetValueWithoutTypeCheck ()
    {
      return GetValueWithoutTypeCheckTx (DefaultTransaction);
    }

    /// <summary>
    /// Gets the property's value without performing a type check for the given transaction.
    /// </summary>
    /// <param name="transaction">The <see cref="ClientTransaction"/> to get the value for.</param>
    /// <returns>The value of the encapsulated property. For simple value properties,
    /// this is the property value. For related objects, this is the related object. For related object collections,
    /// this is an <see cref="ObjectList{T}"/>, where "T" is the related objects' type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="transaction"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the given <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public object GetValueWithoutTypeCheckTx (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      CheckTransactionalStatus (transaction);
      return _strategy.GetValueWithoutTypeCheck (this, transaction);
    }

    /// <summary>
    /// Gets the property's value from that moment when the property's domain object was enlisted in the current <see cref="ClientTransaction"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The property value type. This must be the same as the type returned by <see cref="PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type. The type parameter can usually be inferred and needn't be
    /// specified in such cases.
    /// </typeparam>
    /// <returns>The original value of the encapsulated property in the current transaction.</returns>
    /// <exception cref="InvalidTypeException">
    /// The type requested via <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyType"/>.
    /// </exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public T GetOriginalValue<T> ()
    {
      CheckType (typeof (T));
      return (T) GetOriginalValueWithoutTypeCheck();
    }

    /// <summary>
    /// Gets the property's value from that moment when the property's domain object was enlisted in the given <see cref="ClientTransaction"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The property value type. This must be the same as the type returned by <see cref="PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type. The type parameter can usually be inferred and needn't be
    /// specified in such cases.
    /// </typeparam>
    /// <param name="transaction">The <see cref="ClientTransaction"/> to get the value for.</param>
    /// <returns>The original value of the encapsulated property in the current transaction.</returns>
    /// <exception cref="InvalidTypeException">
    /// The type requested via <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyType"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">The <paramref name="transaction"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the given <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public T GetOriginalValueTx<T> (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      CheckType(typeof (T));

      return (T) GetOriginalValueWithoutTypeCheckTx (transaction);
    }

    /// <summary>
    /// Gets the property's original value without performing a type check.
    /// </summary>
    /// <returns>The original value of the encapsulated property in the current transaction.</returns>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public object GetOriginalValueWithoutTypeCheck ()
    {
      return GetOriginalValueWithoutTypeCheckTx (DefaultTransaction);
    }

    /// <summary>
    /// Gets the property's original value for the given <see cref="ClientTransaction"/> without performing a type check.
    /// </summary>
    /// <returns>The original value of the encapsulated property in the current transaction.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="transaction"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the given <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public object GetOriginalValueWithoutTypeCheckTx (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      CheckTransactionalStatus (transaction);
      return _strategy.GetOriginalValueWithoutTypeCheck (this, transaction);
    }

    /// <summary>
    /// Gets the original ID of the related object for related object properties.
    /// </summary>
    /// <returns>The ID of the related object originally stored in the encapsulated property.</returns>
    /// <exception cref="InvalidOperationException">The property type is not <see cref="PropertyKind.RelatedObject"/> or the property is a virtual
    /// relation end point (i.e. the other side of the relation holds the foreign key).</exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public ObjectID GetOriginalRelatedObjectID ()
    {
      return GetOriginalRelatedObjectIDTx (DefaultTransaction);
    }

    /// <summary>
    /// Gets the ID of the original related object for related object properties for a given <see cref="ClientTransaction"/>.
    /// </summary>
    /// <param name="transaction">The <see cref="ClientTransaction"/> to get the value for.</param>
    /// <returns>The ID of the related object originally stored in the encapsulated property in the given transaction.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="transaction"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The property type is not <see cref="PropertyKind.RelatedObject"/> or the property is a virtual
    /// relation end point (i.e. the other side of the relation holds the foreign key).</exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the given <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public ObjectID GetOriginalRelatedObjectIDTx (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      CheckTransactionalStatus (transaction);

      if (Kind != PropertyKind.RelatedObject)
        throw new InvalidOperationException ("This operation can only be used on related object properties.");

      if (RelationEndPointDefinition.IsVirtual)
        throw new InvalidOperationException ("ObjectIDs only exist on the real side of a relation, not on the virtual side.");

      return (ObjectID) ValuePropertyAccessorStrategy.Instance.GetOriginalValueWithoutTypeCheck (this, transaction);
    }

    public override string ToString ()
    {
      return _propertyIdentifier;
    }
  }
}
