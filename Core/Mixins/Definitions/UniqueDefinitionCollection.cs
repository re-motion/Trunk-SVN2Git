using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("Count = {Count}")]
  public class UniqueDefinitionCollection<TKey, TValue> : DefinitionCollectionBase<TKey, TValue>, IDefinitionCollection<TKey, TValue>
      where TValue : IVisitableDefinition
  {
    private Dictionary<TKey, TValue> _items = new Dictionary<TKey, TValue> ();

    public UniqueDefinitionCollection (KeyMaker keyMaker, Predicate<TValue> guardian) : base (keyMaker, guardian)
    {
    }

    public UniqueDefinitionCollection (KeyMaker keyMaker) : base (keyMaker, null)
    {
    }

    public override bool ContainsKey (TKey key)
    {
      return _items.ContainsKey (key);
    }

    protected override void CustomizedAdd (TKey key, TValue value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("value", value);

      if (ContainsKey (key))
      {
        string message = string.Format ("Duplicate key {0} for item {1}.", key, value);
        throw new InvalidOperationException (message);
      }
      _items.Add (key, value);
    }

    protected override void CustomizedClear ()
    {
      _items.Clear();
    }

    public TValue this[TKey key]
    {
      get { return ContainsKey (ArgumentUtility.CheckNotNull("key", key)) ? _items[key] : default (TValue); }
    }
  }
}
