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
using System.Diagnostics;
using Remotion.Utilities;
using Remotion.Collections;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("Count = {Count}")]
  public class MultiDefinitionCollection<TKey, TValue> : DefinitionCollectionBase<TKey, TValue>
      where TValue : IVisitableDefinition
  {
    private MultiDictionary<TKey, TValue> _items = new MultiDictionary<TKey, TValue>();

    public MultiDefinitionCollection (KeyMaker keyMaker)
        : base (keyMaker, null)
    {
    }


    public override bool ContainsKey (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return _items.ContainsKey (key);
    }

    protected override void CustomizedAdd (TKey key, TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("value", value);

      _items.Add (key, value);
    }

    protected override void CustomizedClear ()
    {
      _items.Clear();
    }

    public IEnumerable<TValue> this[TKey key]
    {
      get
      {
        ArgumentUtility.CheckNotNull ("key", key);
        return _items[key];
      }
    }

    public int GetItemCount (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      return _items[key].Count;
    }

    public TValue GetFirstItem (TKey key)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      if (GetItemCount (key) == 0)
        throw new ArgumentException ("There is no item with the given key.", "key");
      else
        return _items[key][0];
    }

    public IEnumerable<TKey> Keys
    {
      get { return _items.Keys; }
    }
  }
}
