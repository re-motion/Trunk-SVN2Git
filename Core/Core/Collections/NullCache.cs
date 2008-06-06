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
  /// <summary>
  /// This class implements a cache that does not actually cache anything.
  /// </summary>
  /// <remarks>
  /// Use NullCache objects if some code expects an <see cref="ICache{TKey,TValue}"/> interface, but you don't actually want to use caching.
  /// </remarks>
  [Serializable]
  public class NullCache<TKey, TValue> : ICache<TKey, TValue>
  {
    public NullCache ()
    {
    }

    public void Add (TKey key, TValue value)
    {
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      value = default (TValue);
      return false;
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey,TValue> valueFactory)
    {
      ArgumentUtility.CheckNotNull ("valueFactory", valueFactory);
      return valueFactory(key);
    }

    public void Clear ()
    {
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }
  }
}
