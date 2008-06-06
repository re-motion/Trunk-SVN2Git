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

namespace Remotion.Collections
{
  //TODO: Doc
  [Serializable]
  public class Cache<TKey, TValue> : ICache<TKey, TValue>
  {
    // types

    // static members

    // member fields

    private readonly SimpleDataStore<TKey, TValue> _cache = new SimpleDataStore<TKey, TValue> ();

    // construction and disposing

    public Cache ()
    {
    }

    // methods and properties

    public void Add (TKey key, TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      _cache[key] = value;
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      
      return _cache.TryGetValue (key, out value);
    }

    public void Clear ()
    {
      _cache.Clear ();
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey,TValue> valueFactory)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("valueFactory", valueFactory);

      return _cache.GetOrCreateValue (key, valueFactory);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
