using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Remotion.Collections;

namespace Remotion.Mixins.Utilities
{
  [DebuggerDisplay ("Count = {_items.Count}")]
  internal class MultiSet<T> : IEnumerable<T>
  {
    private MultiDictionary<T, T> _items;

    public MultiSet ()
    {
      _items = new MultiDictionary<T, T>();
    }

    public MultiSet (IEqualityComparer<T> comparer)
    {
      _items = new MultiDictionary<T, T> (comparer);
    }

    public void Add(T item)
    {
      _items.Add (item, item);
    }

    public void AddRange (IEnumerable<T> items)
    {
      foreach (T t in items)
        Add (t);
    }

    public IEnumerable<T> this [T item]
    {
      get { return _items[item]; }
    }

    public int GetItemCount (T item)
    {
      return _items[item].Count;
    }

    public IEnumerable<T> GetUniqueItems()
    {
      foreach (T firstItem in _items.Keys)
        yield return firstItem;
    }

    public IEnumerator<T> GetEnumerator()
    {
      foreach (T firstItem in _items.Keys)
        foreach (T item in _items[firstItem])
          yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
