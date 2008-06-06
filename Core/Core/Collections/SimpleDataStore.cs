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
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// Implements the <see cref="IDataStore{TKey,TValue}"/> interface as a simple, not thread-safe in-memory data store based on a 
  /// <see cref="Dictionary{TKey,TValue}"/>.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys.</typeparam>
  /// <typeparam name="TValue">The type of the values.</typeparam>
  [Serializable]
  public class SimpleDataStore<TKey, TValue> : IDataStore<TKey, TValue>
  {
    private readonly Dictionary<TKey, TValue> _innerDictionary = new Dictionary<TKey, TValue>();

    bool INullObject.IsNull
    {
      get { return false; }
    }

    /// <summary>
    /// Determines whether the store contains an element with the specified <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <returns>
    /// true if the store contains the specified key; otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    public bool ContainsKey (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return _innerDictionary.ContainsKey (key);
    }

    /// <summary>
    /// Adds a new element to the store.
    /// </summary>
    /// <param name="key">The key of the new element.</param>
    /// <param name="value">The value of the new element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">An item with an equal key already exists in the store.</exception>
    public void Add (TKey key, TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      try
      {
        _innerDictionary.Add (key, value);
      }
      catch (ArgumentException ex)
      {
        string message =
            string.Format ("The store already contains an element with key '{0}'. (Old value: '{1}', new value: '{2}')", key, this[key], value);
        throw new ArgumentException (message, "key", ex);
      }
    }

    /// <summary>
    /// Removes the element with the specified key from the store, if any.
    /// </summary>
    /// <param name="key">The key of the element to be removed.</param>
    /// <returns>
    /// true if the item was found in the store; otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    public bool Remove (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return _innerDictionary.Remove (key);
    }

    /// <summary>
    /// Removes all elements from the store.
    /// </summary>
    public void Clear ()
    {
      _innerDictionary.Clear();
    }

    /// <summary>
    /// Gets or sets the <typeparamref name="TValue"/> with the specified key.
    /// </summary>
    /// <value></value>
    public TValue this[TKey key]
    {
      get
      {
        ArgumentUtility.CheckNotNull ("key", key);
        try
        {
          return _innerDictionary[key];
        }
        catch (KeyNotFoundException ex)
        {
          string message = string.Format ("There is no element with key '{0}' in the store.", key);
          throw new KeyNotFoundException (message, ex);
        }
      }
      set 
      {
        ArgumentUtility.CheckNotNull ("key", key);
        _innerDictionary[key] = value; 
      }
    }

    /// <summary>
    /// Gets the value of the element with the specified key, or <typeparamref name="TValue"/>'s default value if no such element exists.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <returns>
    /// The value of the element, or the default value if no such element exists.
    /// </returns>
    public TValue GetValueOrDefault (TKey key)
    {
      TValue value;
      TryGetValue (key, out value);
      return value;
    }

    /// <summary>
    /// Tries to get the value of the element with the specified key.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <param name="value">The value of the element with the specified key, or <typeparamref name="TValue"/>'s default value if no such element
    /// exists.</param>
    /// <returns>
    /// true if an element with the specified key was found; otherwise, false.
    /// </returns>
    public bool TryGetValue (TKey key, out TValue value)
    {
      return _innerDictionary.TryGetValue (key, out value);
    }

    /// <summary>
    /// Gets the value of the element with the specified key, creating a new one if none exists.
    /// </summary>
    /// <param name="key">The key of the element to be retrieved.</param>
    /// <param name="creator">A delegate used for creating a new element if none exists.</param>
    /// <returns>
    /// The value of the element that was found or created.
    /// </returns>
    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> creator)
    {
      TValue value;
      if (!TryGetValue (key, out value))
      {
        value = creator (key);
        Add (key, value);
      }
      return value;
    }
  }
}
