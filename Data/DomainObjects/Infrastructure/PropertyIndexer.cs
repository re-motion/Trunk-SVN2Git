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
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides an indexer to access a specific property of a domain object. Instances of this value type are returned by
  /// <see cref="DomainObjects.DomainObject.Properties"/>.
  /// </summary>
  public class PropertyIndexer
  {
    private readonly DomainObject _domainObject;
    private readonly Cache<string, PropertyAccessorData> _dataCache = new Cache<string, PropertyAccessorData> ();

    /// <summary>
    /// Initializes a new <see cref="PropertyIndexer"/> instance. This is usually not called from the outside; instead, <see cref="PropertyIndexer"/>
    /// instances are returned by <see cref="DomainObjects.DomainObject.Properties"/>.
    /// </summary>
    /// <param name="domainObject">The domain object whose properties should be accessed with this <see cref="PropertyIndexer"/>.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="domainObject"/> parameter is <see langword="null"/>.</exception>
    public PropertyIndexer (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      _domainObject = domainObject;
    }

    /// <summary>
    /// Gets the <see cref="DomainObject"/> associated with this <see cref="PropertyIndexer"/>.
    /// </summary>
    /// <value>The domain object associated with this <see cref="PropertyIndexer"/>.</value>
    public DomainObject DomainObject
    {
      get { return _domainObject; }
    }

    /// <summary>
    /// Selects the property of the domain object with the given name.
    /// </summary>
    /// <param name="propertyName">The name of the property to be accessed.</param>
    /// <returns>A <see cref="PropertyAccessor"/> instance encapsulating the requested property.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="propertyName"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// The <paramref name="propertyName"/> parameter does not denote a valid mapping property of the domain object.
    /// </exception>
    public PropertyAccessor this[string propertyName]
    {
      get
      {
        ArgumentUtility.CheckNotNull ("propertyName", propertyName);
        PropertyAccessorData data = _dataCache.GetOrCreateValue (propertyName, CreatePropertyAccessorData);
        return new PropertyAccessor (_domainObject, data, GetDefaultTransaction());
      }
    }

    /// <summary>
    /// Selects the property of the domain object with the given short name and declaring type.
    /// </summary>
    /// <param name="shortPropertyName">The short name of the property to be accessed.</param>
    /// <param name="domainObjectType">The type declaring the property.</param>
    /// <returns>A <see cref="PropertyAccessor"/> instance encapsulating the requested property.</returns>
    /// <exception cref="ArgumentNullException">One or more of the parameters passed to this indexer are null.</exception>
    /// <exception cref="ArgumentException">
    /// The <paramref name="shortPropertyName"/> parameter does not denote a valid mapping property declared on the <paramref name="domainObjectType"/>.
    /// </exception>
    public PropertyAccessor this[Type domainObjectType, string shortPropertyName]
    {
      get
      {
        ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);
        ArgumentUtility.CheckNotNull ("shortPropertyName", shortPropertyName);

        return this[GetIdentifierFromTypeAndShortName(domainObjectType, shortPropertyName)];
      }
    }

    /// <summary>
    /// Selects the property of the domain object with the given name.
    /// </summary>
    /// <param name="propertyName">The name of the property to be accessed.</param>
    /// <param name="transaction">The transaction to use for accessing the property.</param>
    /// <returns>A <see cref="PropertyAccessor"/> instance encapsulating the requested property.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="propertyName"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// The <paramref name="propertyName"/> parameter does not denote a valid mapping property of the domain object.
    /// </exception>
    public PropertyAccessor this[string propertyName, ClientTransaction transaction]
    {
      get
      {
        ArgumentUtility.CheckNotNull ("propertyName", propertyName);
        ArgumentUtility.CheckNotNull ("transaction", transaction);
        PropertyAccessorData data = _dataCache.GetOrCreateValue (propertyName, CreatePropertyAccessorData);
        return new PropertyAccessor (_domainObject, data, transaction);
      }
    }

    /// <summary>
    /// Selects the property of the domain object with the given short name and declaring type.
    /// </summary>
    /// <param name="shortPropertyName">The short name of the property to be accessed.</param>
    /// <param name="domainObjectType">The type declaring the property.</param>
    /// <param name="transaction">The transaction to use for accessing the property.</param>
    /// <returns>A <see cref="PropertyAccessor"/> instance encapsulating the requested property.</returns>
    /// <exception cref="ArgumentNullException">One or more of the parameters passed to this indexer are null.</exception>
    /// <exception cref="ArgumentException">
    /// The <paramref name="shortPropertyName"/> parameter does not denote a valid mapping property declared on the <paramref name="domainObjectType"/>.
    /// </exception>
    public PropertyAccessor this[Type domainObjectType, string shortPropertyName, ClientTransaction transaction]
    {
      get
      {
        ArgumentUtility.CheckNotNull ("domainObjectType", domainObjectType);
        ArgumentUtility.CheckNotNull ("shortPropertyName", shortPropertyName);
        ArgumentUtility.CheckNotNull ("transaction", transaction);

        return this[GetIdentifierFromTypeAndShortName (domainObjectType, shortPropertyName), transaction];
      }
    }

    private ClientTransaction GetDefaultTransaction ()
    {
      return DomainObjectUtility.GetNonNullClientTransaction (_domainObject);
    }

    private PropertyAccessorData CreatePropertyAccessorData (string propertyName)
    {
      try
      {
        return new PropertyAccessorData (_domainObject.ID.ClassDefinition,
                                         propertyName);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException (
            string.Format (
                "The domain object type {0} does not have a mapping property named '{1}'.",
                _domainObject.ID.ClassDefinition.ClassType.FullName, propertyName), "propertyName", ex);
      }
    }

    private string GetIdentifierFromTypeAndShortName (Type domainObjectType, string shortPropertyName)
    {
      return domainObjectType.FullName + "." + shortPropertyName;
    }

    /// <summary>
    /// Gets the number of properties defined by the domain object. This corresponds to the number of <see cref="PropertyAccessor"/> objects
    /// indexable by this structure and enumerated by <see cref="AsEnumerable()"/>.
    /// </summary>
    /// <returns>The number of properties defined by the domain object.</returns>
    public int GetPropertyCount ()
    {
      ClassDefinition classDefinition = _domainObject.ID.ClassDefinition;
      IRelationEndPointDefinition[] endPointDefinitions = classDefinition.GetRelationEndPointDefinitions();
      int count = classDefinition.GetPropertyDefinitions().Count;
      foreach (IRelationEndPointDefinition endPointDefinition in endPointDefinitions)
      {
        if (endPointDefinition.IsVirtual)
          ++count;
      }
      return count;
    }

    /// <summary>
    /// Returns an implementation of <see cref="IEnumerable{T}"/> that enumerates over all the properties indexed by this <see cref="PropertyIndexer"/>
    /// in the <see cref="DomainObject"/>'s transaction. That is either the <see cref="ClientTransaction.Current"/> transaction or the object's
    /// <see cref="BindingClientTransaction"/> (if any).
    /// </summary>
    /// <returns>A sequence containing <see cref="PropertyAccessor"/> objects for each property of this <see cref="PropertyIndexer"/>'s 
    /// <see cref="DomainObject"/>.</returns>
    public IEnumerable<PropertyAccessor> AsEnumerable ()
    {
      return AsEnumerable (DomainObjectUtility.GetNonNullClientTransaction(_domainObject));
    }

    /// <summary>
    /// Returns an implementation of <see cref="IEnumerable{T}"/> that enumerates over all the properties indexed by this <see cref="PropertyIndexer"/>
    /// in the given <see cref="ClientTransaction"/>.
    /// </summary>
    /// <param name="transaction">The transaction to be used to enumerate the properties.</param>
    /// <returns>A sequence containing <see cref="PropertyAccessor"/> objects for each property of this <see cref="PropertyIndexer"/>'s 
    /// <see cref="DomainObject"/>.</returns>
    public IEnumerable<PropertyAccessor> AsEnumerable (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      DomainObjectUtility.CheckIfRightTransaction (_domainObject, transaction);
      
      ClassDefinition classDefinition = _domainObject.ID.ClassDefinition;

      foreach (PropertyDefinition propertyDefinition in classDefinition.GetPropertyDefinitions ())
        yield return this[propertyDefinition.PropertyName, transaction];

      foreach (IRelationEndPointDefinition endPointDefinition in classDefinition.GetRelationEndPointDefinitions ())
      {
        if (endPointDefinition.IsVirtual)
          yield return this[endPointDefinition.PropertyName, transaction];
      }
    }

    /// <summary>
    /// Determines whether the domain object contains a property with the specified identifier.
    /// </summary>
    /// <param name="propertyIdentifier">The long property identifier to check for.</param>
    /// <returns>
    /// True if the domain object contains a property named as specified by <paramref name="propertyIdentifier"/>; otherwise, false.
    /// </returns>
    public bool Contains (string propertyIdentifier)
    {
      ClassDefinition classDefinition = _domainObject.ID.ClassDefinition;
      return PropertyAccessorData.IsValidProperty (classDefinition, propertyIdentifier);
    }


    /// <summary>
    /// Determines whether the domain object contains a property with the specified short name and declaring type.
    /// </summary>
    /// <param name="domainObjectType">The type declaring the property with the given <paramref name="shortPropertyName"/>.</param>
    /// <param name="shortPropertyName">The short property name to check for.</param>
    /// <returns>
    /// True if the domain object contains a property named as specified by <paramref name="shortPropertyName"/> declared on
    /// <paramref name="domainObjectType"/>; otherwise, false.
    /// </returns>
    public bool Contains (Type domainObjectType, string shortPropertyName)
    {
      return Contains (GetIdentifierFromTypeAndShortName (domainObjectType, shortPropertyName));
    }

    /// <summary>
    /// Finds a property with the specified short name, starting its search at a given declaring type upwards the inheritance hierarchy.
    /// </summary>
    /// <param name="typeToStartSearch">The type to start searching from.</param>
    /// <param name="shortPropertyName">The short name of the property to find.</param>
    /// <returns>A <see cref="PropertyAccessor"/> encapsulating the first property with the given <paramref name="shortPropertyName"/>
    /// found when traversing upwards through the inheritance hierarchy, starting from <paramref name="typeToStartSearch"/>.</returns>
    /// <exception cref="ArgumentNullException">One or more of the arguments passed to this method are <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">No matching property could be found.</exception>
    public PropertyAccessor Find (Type typeToStartSearch, string shortPropertyName)
    {
      ArgumentUtility.CheckNotNull ("typeToStartSearch", typeToStartSearch);
      ArgumentUtility.CheckNotNull ("shortPropertyName", shortPropertyName);

      Type currentType = typeToStartSearch;
      while (currentType != null && !Contains (currentType, shortPropertyName))
      {
        if (currentType.IsGenericType && !currentType.IsGenericTypeDefinition)
          currentType = currentType.GetGenericTypeDefinition ();
        else
          currentType = currentType.BaseType;
      }

      if (currentType != null)
        return this[currentType, shortPropertyName];
      else
        throw new ArgumentException (string.Format ("The domain object type {0} does not have or inherit a mapping property with the short name '{1}'.",
            typeToStartSearch.FullName, shortPropertyName), "shortPropertyName");
    }

    /// <summary>
    /// Finds a property with the specified short name, starting its search at the type of the given <see cref="DomainObject"/> argument.
    /// </summary>
    /// <typeparam name="TDomainObject">The type to start searching from.</typeparam>
    /// <param name="thisDomainObject">The domain object parameter used for inference of type <typeparamref name="TDomainObject"/>.</param>
    /// <param name="shortPropertyName">The short name of the property to find.</param>
    /// <returns>A <see cref="PropertyAccessor"/> encapsulating the first property with the given <paramref name="shortPropertyName"/>
    /// found when traversing upwards through the inheritance hierarchy, starting from <typeparamref name="TDomainObject"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="shortPropertyName"/>parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">No matching property could be found.</exception>
    /// <remarks>
    /// This method exists as a convenience overload of <see cref="Find(Type, string)"/>. Instead of needing to specify a lengthy <c>typeof(...)</c>
    /// expression, this method can usually infer the type to search from the <c>this</c> parameter passed as the first argument.
    /// </remarks>
    public PropertyAccessor Find<TDomainObject> (TDomainObject thisDomainObject, string shortPropertyName)
        where TDomainObject : DomainObject
    {
      ArgumentUtility.CheckNotNull ("shortPropertyName", shortPropertyName);
      return Find (typeof (TDomainObject), shortPropertyName);
    }

    /// <summary>
    /// Finds a property with the specified short name, starting its search at the type of the <see cref="DomainObject"/> whose properties
    /// are represented by this indexer.
    /// </summary>
    /// <param name="shortPropertyName">The short name of the property to find.</param>
    /// <returns>A <see cref="PropertyAccessor"/> encapsulating the first property with the given <paramref name="shortPropertyName"/>
    /// found when traversing upwards through the inheritance hierarchy</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="shortPropertyName"/>parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">No matching property could be found.</exception>
    /// <remarks>
    /// This method exists as a convenience overload of <see cref="Find(Type, string)"/>. Instead of needing to specify a starting type for the search, 
    /// this method assumes that it should start at the actual type of the current <see cref="DomainObject"/>.
    /// </remarks>
    public PropertyAccessor Find (string shortPropertyName)
    {
      ArgumentUtility.CheckNotNull ("shortPropertyName", shortPropertyName);
      return Find (_domainObject.GetPublicDomainObjectType(), shortPropertyName);
    }

    /// <summary>
    /// Gets all related objects of the associated <see cref="DomainObject"/>.
    /// </summary>
    /// <returns>An enumeration of all <see cref="DomainObject"/> directly referenced by the associated <see cref="DomainObject"/> in the form of
    /// <see cref="PropertyKind.RelatedObject"/> and <see cref="PropertyKind.RelatedObjectCollection"/> properties.</returns>
    public IEnumerable<DomainObject> GetAllRelatedObjects ()
    {
      foreach (PropertyAccessor property in _domainObject.Properties.AsEnumerable())
      {
        switch (property.PropertyData.Kind)
        {
          case PropertyKind.RelatedObject:
            var value = (DomainObject) property.GetValueWithoutTypeCheck ();
            if (value != null)
              yield return value;
            break;
          case PropertyKind.RelatedObjectCollection:
            var values = (DomainObjectCollection) property.GetValueWithoutTypeCheck ();
            foreach (DomainObject relatedObject in values)
              yield return relatedObject;
            break;
        }
      }
    }
  }
}
