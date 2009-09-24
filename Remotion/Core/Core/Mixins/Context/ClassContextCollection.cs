// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections;
using System.Collections.Generic;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.Context
{
  public class ClassContextCollection : ICollection, ICollection<ClassContext>
  {
    private readonly Dictionary<Type, ClassContext> _values = new Dictionary<Type, ClassContext> ();
    private readonly IMixinInheritancePolicy _inheritancePolicy = DefaultMixinInheritancePolicy.Instance;

    public ClassContextCollection (IEnumerable<ClassContext> classContexts)
    {
      _values = classContexts.ToDictionary (cc => cc.Type);
    }

    public ClassContextCollection (params ClassContext[] classContexts)
        : this ((IEnumerable<ClassContext>) classContexts)
    {
    }

    public int Count
    {
      get { return _values.Count; }
    }

    public IEnumerator<ClassContext> GetEnumerator ()
    {
      return _values.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }

    public void CopyTo (ClassContext[] array, int arrayIndex)
    {
      ArgumentUtility.CheckNotNull ("array", array);
      ((ICollection) this).CopyTo (array, arrayIndex);
    }

    void ICollection.CopyTo (Array array, int index)
    {
      ArgumentUtility.CheckNotNull ("array", array);
      ((ICollection) _values.Values).CopyTo (array, index);
    }

    public ClassContext GetExact (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (_values.ContainsKey (type))
      {
        var result = _values[type];
        Assertion.IsTrue (result.Type == type);
        return result;
      }
      else
      {
        return null;
      }
    }

    public ClassContext GetWithInheritance (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var exactMatch = GetExact (type);
      if (exactMatch != null)
        return exactMatch;

      var contextsToInheritFrom = _inheritancePolicy.GetClassContextsToInheritFrom (type, GetWithInheritance); // Recursion!

      var inheritedContextCombiner = new ClassContextCombiner ();
      inheritedContextCombiner.AddRangeAllowingNulls (contextsToInheritFrom);
      
      var result = inheritedContextCombiner.GetCombinedContexts (type);
      Assertion.IsTrue (result == null || result.Type == type);
      return result;
    }

    public bool ContainsExact (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return GetExact (type) != null;
    }

    public bool ContainsWithInheritance (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return GetWithInheritance (type) != null;
    }

    public bool Contains (ClassContext item)
    {
      ArgumentUtility.CheckNotNull ("item", item);
      return item.Equals (GetExact (item.Type));
    }

    object ICollection.SyncRoot
    {
      get { return ((ICollection)_values).SyncRoot; }
    }

    bool ICollection.IsSynchronized
    {
      get { return false; }
    }

    bool ICollection<ClassContext>.IsReadOnly
    {
      get { return true; }
    }

    void ICollection<ClassContext>.Clear ()
    {
      throw new NotSupportedException ("This collection is read-only.");
    }

    void ICollection<ClassContext>.Add (ClassContext value)
    {
      throw new NotSupportedException ("This collection is read-only.");
    }

    bool ICollection<ClassContext>.Remove (ClassContext item)
    {
      throw new NotSupportedException ("This collection is read-only.");
    }
  }
}
