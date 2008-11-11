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
using Remotion.Utilities;
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides an encapsulation of a <see cref="DomainObject">DomainObject's</see> property for simple access as well as static methods
  /// supporting working with properties.
  /// </summary>
  public struct PropertyAccessor
  {
    private readonly DomainObject _domainObject;
    private readonly PropertyAccessorData _propertyData;

    /// <summary>
    /// Initializes the <see cref="PropertyAccessor"/> object.
    /// </summary>
    /// <param name="domainObject">The domain object whose property is to be encapsulated.</param>
    /// <param name="propertyData">a <see cref="PropertyAccessorData"/> object describing the property to be accessed.</param>
    /// <exception cref="ArgumentNullException">One of the parameters passed to the constructor is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The domain object does not have a property with the given identifier.</exception>
    public PropertyAccessor (DomainObject domainObject, PropertyAccessorData propertyData)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("propertyData", propertyData);

      _domainObject = domainObject;
      _propertyData = propertyData;
    }

    /// <summary>
    /// Gets the domain object of this property.
    /// </summary>
    /// <value>The domain object this <see cref="PropertyAccessor"/> is associated with.</value>
    public DomainObject DomainObject
    {
      get { return _domainObject; }
    }

    /// <summary>
    /// Gets the <see cref="PropertyAccessorData"/> object describing the property to be accessed.
    /// </summary>
    /// <value>The property data to be accessed.</value>
    public PropertyAccessorData PropertyData
    {
      get { return _propertyData; }
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
      return PropertyData.GetStrategy().HasChanged (this, transaction);
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
      return PropertyData.GetStrategy().HasBeenTouched (this, transaction);
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
        return PropertyData.GetStrategy().IsNull (this, DefaultTransaction);
      }
    }

    internal void CheckType (Type typeToCheck)
    {
      ArgumentUtility.CheckNotNull ("typeToCheck", typeToCheck);
      if (!PropertyData.PropertyType.Equals (typeToCheck))
        throw new InvalidTypeException (PropertyData.PropertyIdentifier, typeToCheck, PropertyData.PropertyType);
    }

    /// <summary>
    /// Gets the property's value.
    /// </summary>
    /// <typeparam name="T">
    /// The property value type. This must be the same as the type returned by <see cref="PropertyAccessorData.PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type.
    /// </typeparam>
    /// <returns>The value of the encapsulated property. For simple value properties,
    /// this is the property value. For related objects, this is the related object. For related object collections,
    /// this is an <see cref="ObjectList{T}"/>, where "T" is the related objects' type.</returns>
    /// <exception cref="InvalidTypeException">
    /// The type requested via <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyAccessorData.PropertyType"/>.
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
    /// The property value type. This must be the same as the type returned by <see cref="PropertyAccessorData.PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type.
    /// </typeparam>
    /// <param name="transaction">The <see cref="ClientTransaction"/> to get the value for.</param>
    /// <returns>The value of the encapsulated property. For simple value properties,
    /// this is the property value. For related objects, this is the related object. For related object collections,
    /// this is an <see cref="ObjectList{T}"/>, where "T" is the related objects' type.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="transaction"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="InvalidTypeException">
    /// The type requested via <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyAccessorData.PropertyType"/>.
    /// </exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the given <see cref="ClientTransaction"/>.</exception>
    /// <exception cref="ObjectDiscardedException">The domain object was discarded.</exception>
    public T GetValueTx<T> (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      CheckType(typeof (T));

      object value = GetValueWithoutTypeCheckTx (transaction);
      Assertion.DebugAssert (
          value != null || NullableTypeUtility.IsNullableType (PropertyData.PropertyType),
          "Property '{0}' is a value type but the DataContainer returned null.",
          PropertyData.PropertyIdentifier);
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

      if (PropertyData.Kind != PropertyKind.RelatedObject)
        throw new InvalidOperationException ("This operation can only be used on related object properties.");

      if (PropertyData.RelationEndPointDefinition.IsVirtual)
        throw new InvalidOperationException ("ObjectIDs only exist on the real side of a relation, not on the virtual side.");

      return (ObjectID) ValuePropertyAccessorStrategy.Instance.GetValueWithoutTypeCheck (this, transaction);
    }

    /// <summary>
    /// Sets the property's value.
    /// </summary>
    /// <typeparam name="T">
    /// The property value type. This must be the same as the type returned by <see cref="PropertyAccessorData.PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type. The type parameter can usually be inferred and needn't be
    /// specified in such cases.
    /// </typeparam>
    /// <param name="value">The value to be set. For simple value properties,
    /// this is the value to be set. For related objects, this is the related object. For related object collections,
    /// this is an <see cref="ObjectList{T}"/>, where "T" is the related objects' type.</param>
    /// <exception cref="InvalidTypeException">
    /// The type <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyAccessorData.PropertyType"/>.
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
    /// The property value type. This must be the same as the type returned by <see cref="PropertyAccessorData.PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type. The type parameter can usually be inferred and needn't be
    /// specified in such cases.
    /// </typeparam>
    /// <param name="transaction">The <see cref="ClientTransaction"/> to set the value for.</param>
    /// <param name="value">The value to be set. For simple value properties,
    /// this is the value to be set. For related objects, this is the related object. For related object collections,
    /// this is an <see cref="ObjectList{T}"/>, where "T" is the related objects' type.</param>
    /// <exception cref="InvalidTypeException">
    /// The type <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyAccessorData.PropertyType"/>.
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
    /// <see cref="PropertyAccessorData.PropertyType"/>, though.
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
    /// asssignable to <see cref="PropertyAccessorData.PropertyType"/>, though.
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
      PropertyData.GetStrategy().SetValueWithoutTypeCheck (this, transaction, value);
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
      return PropertyData.GetStrategy().GetValueWithoutTypeCheck (this, transaction);
    }

    /// <summary>
    /// Gets the property's value from that moment when the property's domain object was enlisted in the current <see cref="ClientTransaction"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The property value type. This must be the same as the type returned by <see cref="PropertyAccessorData.PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type. The type parameter can usually be inferred and needn't be
    /// specified in such cases.
    /// </typeparam>
    /// <returns>The original value of the encapsulated property in the current transaction.</returns>
    /// <exception cref="InvalidTypeException">
    /// The type requested via <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyAccessorData.PropertyType"/>.
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
    /// The property value type. This must be the same as the type returned by <see cref="PropertyAccessorData.PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type. The type parameter can usually be inferred and needn't be
    /// specified in such cases.
    /// </typeparam>
    /// <param name="transaction">The <see cref="ClientTransaction"/> to get the value for.</param>
    /// <returns>The original value of the encapsulated property in the current transaction.</returns>
    /// <exception cref="InvalidTypeException">
    /// The type requested via <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyAccessorData.PropertyType"/>.
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
      return PropertyData.GetStrategy().GetOriginalValueWithoutTypeCheck (this, transaction);
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

      if (PropertyData.Kind != PropertyKind.RelatedObject)
        throw new InvalidOperationException ("This operation can only be used on related object properties.");

      if (PropertyData.RelationEndPointDefinition.IsVirtual)
        throw new InvalidOperationException ("ObjectIDs only exist on the real side of a relation, not on the virtual side.");

      return (ObjectID) ValuePropertyAccessorStrategy.Instance.GetOriginalValueWithoutTypeCheck (this, transaction);
    }

    public override string ToString ()
    {
      return PropertyData.PropertyIdentifier;
    }
  }
}
