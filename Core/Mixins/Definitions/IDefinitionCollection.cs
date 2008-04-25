using System.Collections.Generic;

namespace Remotion.Mixins.Definitions
{
  public interface IDefinitionCollection<TKey, TValue> : IEnumerable<TValue>
  {
    TValue[] ToArray ();
    int Count { get; }
    bool ContainsKey (TKey key);
    TValue this[int index] { get; }
    TValue this [TKey key] { get; }
  }
}