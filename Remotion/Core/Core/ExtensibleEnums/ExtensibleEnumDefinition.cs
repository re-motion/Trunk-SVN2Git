// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Collections;
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums
{
  /// <summary>
  /// Provides discovery services for extensible enum values. Extensible enum implementations should
  /// hold an instance of this class in a static propery of the <see cref="ExtensibleEnum{T}"/> subclass representing
  /// the enumeration. The values of the enumeration should be defined as extension methods for <see cref="ExtensibleEnumDefinition{T}"/>,
  /// where <typeparamref name="T"/> is the <see cref="ExtensibleEnum{T}"/> subclass.
  /// </summary>
  /// <typeparam name="T">The subclass of <see cref="ExtensibleEnum{T}"/> that represents the enumeration.</typeparam>
  public class ExtensibleEnumDefinition<T> : IExtensibleEnumDefinition
      // this constraint forces the user to always write 'ExtensibleEnumDefinition<MyEnum>', never 'ExtensibleEnumDefinition<MyDerivedEnum>'
      where T : ExtensibleEnum<T>
  {
    private class CacheItem
    {
      private static Dictionary<string, T> CreateValueDictionary (T[] valueArray)
      {
        var dictionary = new Dictionary<string, T> ();
        foreach (var value in valueArray)
        {
          try
          {
            dictionary.Add (value.ID, value);
          }
          catch (ArgumentException ex)
          {
            string message = string.Format ("Extensible enum '{0}' defines two values with ID '{1}'.", typeof (T), value.ID);
            throw new DuplicateEnumValueException (message, ex);
          }
        }
        return dictionary;
      }

      public CacheItem (T[] valueArray)
      {
        Collection = new ReadOnlyCollection<T> (valueArray);
        Dictionary = new ReadOnlyDictionary<string, T> (CreateValueDictionary(valueArray));
      }

      public ReadOnlyCollection<T> Collection { get; private set; }
      public ReadOnlyDictionary<string, T> Dictionary { get; private set; }
    }

    private readonly IExtensibleEnumValueDiscoveryService _valueDiscoveryService;

    private readonly DoubleCheckedLockingContainer<CacheItem> _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtensibleEnumDefinition{T}"/> class.
    /// </summary>
    /// <param name="valueDiscoveryService">An implementation of <see cref="IExtensibleEnumValueDiscoveryService"/> used to discover the values
    /// for this <see cref="ExtensibleEnumDefinition{T}"/>.</param>
    public ExtensibleEnumDefinition (IExtensibleEnumValueDiscoveryService valueDiscoveryService)
    {
      ArgumentUtility.CheckNotNull ("valueDiscoveryService", valueDiscoveryService);

      _valueDiscoveryService = valueDiscoveryService;
      _cache = new DoubleCheckedLockingContainer<CacheItem> (RetrieveValues);
    }

    /// <summary>
    /// Gets the values defined by the extensible enum type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>A <see cref="ReadOnlyCollection{T}"/> holding the values for <typeparamref name="T"/>.</returns>
    public ReadOnlyCollection<T> GetValues ()
    {
      return _cache.Value.Collection;
    }

    /// <summary>
    /// Gets the enum value identified by <paramref name="id"/>, throwing an exception if the value cannot be found.
    /// </summary>
    /// <param name="id">The identifier of the enum value to return.</param>
    /// <returns>The enum value identified by <paramref name="id"/>.</returns>
    /// <exception cref="KeyNotFoundException">No enum value with the given <paramref name="id"/> exists.</exception>
    public T GetValueByID (string id)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      T value;
      if (TryGetValueByID (id, out value))
      {
        return value;
      }
      else
      {
        var message = string.Format ("The extensible enum type '{0}' does not define a value called '{1}'.", typeof (T), id);
        throw new KeyNotFoundException (message);
      }
    }

    /// <summary>
    /// Gets the enum value identified by <paramref name="id"/>, returning a boolean value indicating whether 
    /// such a value could be found.
    /// </summary>
    /// <param name="id">The identifier of the enum value to return.</param>
    /// <param name="value">The enum value identified by <paramref name="id"/>, or <see langword="null" /> if no such value exists.</param>
    /// <returns>
    /// <see langword="true" /> if a value with the given <paramref name="id"/> could be found; <see langword="false" /> otherwise.
    /// </returns>
    public bool TryGetValueByID (string id, out T value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      return _cache.Value.Dictionary.TryGetValue (id, out value);
    }

    private CacheItem RetrieveValues ()
    {
      var valueArray = _valueDiscoveryService.GetValues (this).ToArray();
      return new CacheItem (valueArray);
    }

    IExtensibleEnum IExtensibleEnumDefinition.GetValueByID (string id)
    {
      return GetValueByID (id);
    }

    bool IExtensibleEnumDefinition.TryGetValueByID (string id, out IExtensibleEnum value)
    {
      T typedValue;
      var success = TryGetValueByID (id, out typedValue);
      value = typedValue;
      return success;
    }

    ReadOnlyCollection<IExtensibleEnum> IExtensibleEnumDefinition.GetValues ()
    {
      return new ReadOnlyCollection<IExtensibleEnum> (GetValues().ToArray());
    }
  }
}