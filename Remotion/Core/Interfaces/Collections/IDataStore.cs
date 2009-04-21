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

namespace Remotion.Collections
{
  /// <summary>
  /// Provides a common interface for data structures used for storing and retrieving key/value pairs.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys.</typeparam>
  /// <typeparam name="TValue">The type of the values.</typeparam>
  /// <remarks>
  /// <para>
  /// This interface is basically a simplified version of the <see cref="IDictionary{TKey,TValue}"/> interface. In contrast to 
  /// <see cref="IDictionary{TKey,TValue}"/>, it does not require implementers to support <see cref="IEnumerable{T}"/>, <see cref="ICollection{T}"/>,
  /// etc, so it is much simpler to implement.
  /// </para>
  /// <para>
  /// Use this in place of <see cref="ICache{TKey,TValue}"/> if you need a reliable data store which guarantees to keep values once inserted until
  /// they are removed.
  /// </para>
  /// </remarks>
  public interface IDataStore<TKey, TValue> : INullObject
  {
    /// <summary>
    /// Determines whether the store contains an element with the specified <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <returns>
    /// true if the store contains the specified key; otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    bool ContainsKey (TKey key);

    /// <summary>
    /// Adds a new element to the store.
    /// </summary>
    /// <param name="key">The key of the new element.</param>
    /// <param name="value">The value of the new element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">An item with an equal key already exists in the store.</exception>
    void Add (TKey key, TValue value);
    /// <summary>
    /// Removes the element with the specified key from the store, if any.
    /// </summary>
    /// <param name="key">The key of the element to be removed.</param>
    /// <returns>true if the item was found in the store; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    bool Remove (TKey key);
    /// <summary>
    /// Removes all elements from the store.
    /// </summary>
    void Clear ();

    /// <summary>
    /// Gets or sets the value of the element with the specified key.
    /// </summary>
    /// <value>The value of the element.</value>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">The element whose value should be retrieved could not be found.</exception>
    TValue this[TKey key] { get; set;}
    /// <summary>
    /// Gets the value of the element with the specified key, or <typeparamref name="TValue"/>'s default value if no such element exists.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <returns>The value of the element, or the default value if no such element exists.</returns>
    TValue GetValueOrDefault (TKey key);
    /// <summary>
    /// Tries to get the value of the element with the specified key.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <param name="value">The value of the element with the specified key, or <typeparamref name="TValue"/>'s default value if no such element 
    /// exists.</param>
    /// <returns>true if an element with the specified key was found; otherwise, false.</returns>
    bool TryGetValue (TKey key, out TValue value);

    /// <summary>
    /// Gets the value of the element with the specified key, creating a new one if none exists.
    /// </summary>
    /// <param name="key">The key of the element to be retrieved.</param>
    /// <param name="creator">A delegate used for creating a new element if none exists.</param>
    /// <returns>The value of the element that was found or created.</returns>
    TValue GetOrCreateValue (TKey key, Func<TKey, TValue> creator);
  }
}
