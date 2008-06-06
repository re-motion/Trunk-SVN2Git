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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("Count = {Count}")]
  public class CovariantDefinitionCollectionWrapper<TKey, TValue, TValueBase> : IDefinitionCollection<TKey, TValueBase>
      where TValue : class, TValueBase
      where TValueBase : IVisitableDefinition
  {
    private UniqueDefinitionCollection<TKey, TValue> _items;

    public CovariantDefinitionCollectionWrapper(UniqueDefinitionCollection<TKey, TValue> items)
    {
      _items = items;
    }

    public TValueBase[] ToArray()
    {
      return _items.ToArray ();
    }

    public int Count
    {
      get { return _items.Count; }
    }

    public bool ContainsKey (TKey key)
    {
      return _items.ContainsKey (key);
    }

    public TValueBase this [int index]
    {
      get { return _items[index]; }
    }

    public TValueBase this [TKey key]
    {
      get { return _items[key]; }
    }

    public IEnumerator<TValueBase> GetEnumerator()
    {
      foreach (TValue item in _items)
        yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}
