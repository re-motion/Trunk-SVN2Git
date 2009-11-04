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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Collections;
using Remotion.ExtensibleEnums.Infrastructure;
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
      private static Dictionary<string, ExtensibleEnumInfo<T>> CreateValueDictionary (ExtensibleEnumInfo<T>[] valueInfoArray)
      {
        var dictionary = new Dictionary<string, ExtensibleEnumInfo<T>> ();
        foreach (var valueInfo in valueInfoArray)
        {
          try
          {
            dictionary.Add (valueInfo.Value.ID, valueInfo);
          }
          catch (ArgumentException ex)
          {
            string message = string.Format ("Extensible enum '{0}' defines two values with ID '{1}'.", typeof (T), valueInfo.Value.ID);
            throw new InvalidExtensibleEnumDefinitionException (message, ex);
          }
        }
        return dictionary;
      }

      public CacheItem (ExtensibleEnumInfo<T>[] valueInfoArray)
      {
        Collection = new ReadOnlyCollection<ExtensibleEnumInfo<T>> (valueInfoArray.OrderBy (info => info.Value.ID).ToArray ());
        Dictionary = new ReadOnlyDictionary<string, ExtensibleEnumInfo<T>> (CreateValueDictionary (valueInfoArray));
      }

      public ReadOnlyCollection<ExtensibleEnumInfo<T>> Collection { get; private set; }
      public ReadOnlyDictionary<string, ExtensibleEnumInfo<T>> Dictionary { get; private set; }
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
    /// Gets <see cref="ExtensibleEnumInfo{T}"/> objects describing the values defined by the extensible enum type.
    /// </summary>
    /// <returns>A <see cref="ReadOnlyCollection{T}"/> holding the <see cref="ExtensibleEnumInfo{T}"/> objects describing the values for the 
    /// extensible enum type.</returns>
    public ReadOnlyCollection<ExtensibleEnumInfo<T>> GetValueInfos ()
    {
      return _cache.Value.Collection;
    }

    /// <summary>
    /// Gets an <see cref="ExtensibleEnumInfo{T}"/> object describing the enum value identified by <paramref name="id"/>, throwing an exception if the 
    /// value cannot be found.
    /// </summary>
    /// <param name="id">The identifier of the enum value to return.</param>
    /// <returns>An <see cref="ExtensibleEnumInfo{T}"/> describing the enum value identified by <paramref name="id"/>.</returns>
    /// <exception cref="KeyNotFoundException">No enum value with the given <paramref name="id"/> exists.</exception>
    public ExtensibleEnumInfo<T> GetValueInfoByID (string id)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      ExtensibleEnumInfo<T> value;
      if (TryGetValueInfoByID (id, out value))
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
    /// Gets an <see cref="ExtensibleEnumInfo{T}"/> object describing the enum value identified by <paramref name="id"/>, returning a boolean value 
    /// indicating whether such a value could be found.
    /// </summary>
    /// <param name="id">The identifier of the enum value to return.</param>
    /// <param name="value">The <see cref="ExtensibleEnumInfo{T}"/> describing the enum value identified by <paramref name="id"/>, or 
    /// <see langword="null" /> if no such value exists.</param>
    /// <returns>
    /// <see langword="true" /> if a value with the given <paramref name="id"/> could be found; <see langword="false" /> otherwise.
    /// </returns>
    public bool TryGetValueInfoByID (string id, out ExtensibleEnumInfo<T> value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      return _cache.Value.Dictionary.TryGetValue (id, out value);
    }

    private CacheItem RetrieveValues ()
    {
      var valueArray = _valueDiscoveryService.GetValues (this).Select(v => new ExtensibleEnumInfo<T>(v)).ToArray();
      if (valueArray.Length == 0)
      {
        string message = string.Format ("Extensible enum '{0}' does not define any values.", typeof (T));
        throw new InvalidExtensibleEnumDefinitionException (message);
      }
      return new CacheItem (valueArray);
    }

    IExtensibleEnumInfo IExtensibleEnumDefinition.GetValueInfoByID (string id)
    {
      return GetValueInfoByID (id);
    }

    bool IExtensibleEnumDefinition.TryGetValueInfoByID (string id, out IExtensibleEnumInfo valueInfo)
    {
      ExtensibleEnumInfo<T> typedValue;
      var success = TryGetValueInfoByID (id, out typedValue);
      valueInfo = typedValue;
      return success;
    }

    ReadOnlyCollection<IExtensibleEnumInfo> IExtensibleEnumDefinition.GetValueInfos ()
    {
      return new ReadOnlyCollection<IExtensibleEnumInfo> (GetValueInfos().ToArray());
    }
  }
}
