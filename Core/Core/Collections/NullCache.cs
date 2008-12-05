// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
