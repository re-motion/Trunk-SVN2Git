using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  public abstract class DefinitionCollectionBase<TKey, TValue> : IEnumerable<TValue>
      where TValue : IVisitableDefinition
  {
    private List<TValue> _orderedItems = new List<TValue> ();

    public delegate TKey KeyMaker (TValue value);
    private KeyMaker _keyMaker;

    private Predicate<TValue> _guardian;

    public DefinitionCollectionBase (KeyMaker keyMaker, Predicate<TValue> guardian)
    {
      ArgumentUtility.CheckNotNull ("keyMaker", keyMaker);
      _keyMaker = keyMaker;
      _guardian = guardian;
    }

    public IEnumerator<TValue> GetEnumerator ()
    {
      return _orderedItems.GetEnumerator ();
    }

    IEnumerator System.Collections.IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }

    public TValue[] ToArray ()
    {
      return _orderedItems.ToArray();
    }

    public int Count
    {
      get { return _orderedItems.Count; }
    }

    public abstract bool ContainsKey (TKey key);

    protected internal void Add (TValue newItem)
    {
      ArgumentUtility.CheckNotNull ("newItem", newItem);
      if (_guardian != null && !_guardian (newItem))
        throw new ArgumentException (string.Format ("The item does not match the criteria to be added to the collection: {0}.", _guardian.Method),
            "newItem");

      TKey key = _keyMaker (newItem);

      CustomizedAdd (key, newItem);

      _orderedItems.Add (newItem);
    }

    protected abstract void CustomizedAdd (TKey key, TValue value);

    public void Clear ()
    {
      _orderedItems.Clear ();
      CustomizedClear ();
    }

    protected abstract void CustomizedClear ();

    public TValue this[int index]
    {
      get { return _orderedItems[index]; }
    }

    internal void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      foreach (TValue value in this)
      {
        value.Accept (visitor);
      }
    }
  }
}
