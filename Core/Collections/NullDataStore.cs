using System;

namespace Remotion.Collections
{
  /// <summary>
  /// This class implements a data store that doesn't actually store anything. It's part of the null object pattern.
  /// </summary>
  /// <typeparam name="TKey">The type of the keys.</typeparam>
  /// <typeparam name="TValue">The type of the values.</typeparam>
  public class NullDataStore<TKey, TValue> : IDataStore<TKey, TValue>
  {
    public static readonly NullDataStore<TKey, TValue> Instance = new NullDataStore<TKey, TValue> ();

    private NullDataStore()
    {
    }

    public bool ContainsKey (TKey key)
    {
      return false;
    }

    public void Add (TKey key, TValue value)
    {
    }

    public bool Remove (TKey key)
    {
      return false;
    }

    public void Clear ()
    {
    }

    public TValue this [TKey key]
    {
      get { throw new InvalidOperationException ("No values can be retrieved from this cache."); }
      set { }
    }

    public TValue GetValueOrDefault (TKey key)
    {
      return default (TValue);
    }

    public bool TryGetValue (TKey key, out TValue value)
    {
      value = default (TValue);
      return false;
    }

    public TValue GetOrCreateValue (TKey key, Func<TKey, TValue> creator)
    {
      throw new NotImplementedException();
    }

    public bool IsNull
    {
      get { return true; }
    }
  }
}