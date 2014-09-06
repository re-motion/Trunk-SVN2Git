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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides an indexer to access a specific property of a domain object. Instances of this value type are returned by
  /// <see cref="DomainObjects.DomainObject.Properties"/>.
  /// </summary>
  public class PropertyIndexer
  {
    private readonly IReflectableDomainObject _domainObject;
    private readonly PropertyAccessorDataCache _propertyAccessorDataCache;

    /// <summary>
    /// Initializes a new <see cref="PropertyIndexer"/> instance. This is usually not called from the outside; instead, <see cref="PropertyIndexer"/>
    /// instances are returned by <see cref="IReflectableDomainObject.Properties"/>.
    /// </summary>
    /// <param name="domainObject">The domain object whose properties should be accessed with this <see cref="PropertyIndexer"/>.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="domainObject"/> parameter is <see langword="null"/>.</exception>
    public PropertyIndexer (IReflectableDomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      _domainObject = domainObject;
      _propertyAccessorDataCache = _domainObject.ID.ClassDefinition.PropertyAccessorDataCache;
    }

    /// <summary>
    /// Gets the <see cref="IReflectableDomainObject"/> associated with this <see cref="PropertyIndexer"/>.
    /// </summary>
    /// <value>The domain object associated with this <see cref="PropertyIndexer"/>.</value>
    public IReflectableDomainObject DomainObject
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
        return this[propertyName, _domainObject.GetDefaultTransactionContext().ClientTransaction];
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

        return this[domainObjectType, shortPropertyName, _domainObject.GetDefaultTransactionContext().ClientTransaction];
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

        var data = _propertyAccessorDataCache.GetMandatoryPropertyAccessorData (propertyName);
        return GetPropertyAccessor (transaction, data);
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

        var data = _propertyAccessorDataCache.GetMandatoryPropertyAccessorData (domainObjectType, shortPropertyName);
        return GetPropertyAccessor (transaction, data);
      }
    }

    /// <summary>
    /// Gets the number of properties defined by the domain object. This corresponds to the number of <see cref="PropertyAccessor"/> objects
    /// indexable by this structure and enumerated by <see cref="AsEnumerable()"/>.
    /// </summary>
    /// <returns>The number of properties defined by the domain object.</returns>
    public int GetPropertyCount ()
    {
      ClassDefinition classDefinition = _domainObject.ID.ClassDefinition;
      var endPointDefinitions = classDefinition.GetRelationEndPointDefinitions();
      return classDefinition.GetPropertyDefinitions().Count + endPointDefinitions.Count (endPointDefinition => endPointDefinition.IsVirtual);
    }

    /// <summary>
    /// Returns an implementation of <see cref="IEnumerable{T}"/> that enumerates over all the properties indexed by this <see cref="PropertyIndexer"/>
    /// in the <see cref="DomainObject"/>'s <see cref="DomainObjects.DomainObject.DefaultTransactionContext"/>.
    /// </summary>
    /// <returns>A sequence containing <see cref="PropertyAccessor"/> objects for each property of this <see cref="PropertyIndexer"/>'s 
    /// <see cref="DomainObject"/>.</returns>
    public IEnumerable<PropertyAccessor> AsEnumerable ()
    {
      return AsEnumerable (_domainObject.GetDefaultTransactionContext().ClientTransaction);
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
      DomainObjectCheckUtility.CheckIfRightTransaction (_domainObject, transaction);
      
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
      return _propertyAccessorDataCache.GetPropertyAccessorData (propertyIdentifier) != null;
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
      var propertyAccessorData = _propertyAccessorDataCache.GetPropertyAccessorData (domainObjectType, shortPropertyName);
      return propertyAccessorData != null;
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

      var propertyAccessorData = _propertyAccessorDataCache.FindPropertyAccessorData (typeToStartSearch, shortPropertyName);

      if (propertyAccessorData != null)
      {
        return GetPropertyAccessor (_domainObject.GetDefaultTransactionContext().ClientTransaction, propertyAccessorData);
      }
      else
      {
        var message = string.Format (
            "The domain object type '{0}' does not have or inherit a mapping property with the short name '{1}'.",
            typeToStartSearch.FullName,
            shortPropertyName);
        throw new ArgumentException (message, "shortPropertyName");
      }
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
      return Find (_domainObject.ID.ClassDefinition.ClassType, shortPropertyName);
    }

    /// <summary>
    /// Gets all related objects of the associated <see cref="DomainObject"/>.
    /// </summary>
    /// <returns>An enumeration of all <see cref="DomainObject"/> directly referenced by the associated <see cref="DomainObject"/> in the form of
    /// <see cref="PropertyKind.RelatedObject"/> and <see cref="PropertyKind.RelatedObjectCollection"/> properties.</returns>
    public IEnumerable<DomainObject> GetAllRelatedObjects ()
    {
      foreach (var property in AsEnumerable())
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
            foreach (var relatedObject in values)
              yield return relatedObject;
            break;
        }
      }
    }

    private PropertyAccessor GetPropertyAccessor (ClientTransaction transaction, PropertyAccessorData data)
    {
      return new PropertyAccessor (_domainObject, data, transaction);
    }
  }
}
