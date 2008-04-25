using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Utilities;
using Remotion.Reflection;

namespace Remotion.Collections
{
  /// <summary>
  /// A dictionary that contains a <see cref="List{TValue}"/> of values for every key.
  /// </summary>
  [Serializable]
  public class MultiDictionary<TKey, TValue> : AutoInitDictionary<TKey, List<TValue>>
  {
    public MultiDictionary ()
    {
    }

    public MultiDictionary (IEqualityComparer<TKey> comparer)
      : base (comparer)
    {
    }

    public int KeyCount
    {
      get { return base.Count; }
    }

    public int CountValues()
    {
      int count = 0;
      foreach (TKey key in Keys)
        count += this[key].Count;
      return count;
    }

    /// <summary>
    /// Adds a value to the key's value list.
    /// </summary>
    public void Add (TKey key, TValue value)
    {
      this[key].Add (value);
    }
  }
}
