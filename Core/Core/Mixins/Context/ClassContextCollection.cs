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
using System.Collections;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Mixins.Context
{
  public class ClassContextCollection : ICollection, ICollection<ClassContext>
  {
    public event EventHandler<ClassContextEventArgs> ClassContextAdded;
    public event EventHandler<ClassContextEventArgs> ClassContextRemoved;

    private readonly Dictionary<Type, ClassContext> _values = new Dictionary<Type, ClassContext> ();
    private readonly InheritedClassContextRetrievalAlgorithm _inheritanceAlgorithm;

    public ClassContextCollection ()
    {
      _inheritanceAlgorithm = new InheritedClassContextRetrievalAlgorithm (GetExact, GetWithInheritance);
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

    public void Clear ()
    {
      List<Type> keys = new List<Type> (_values.Keys);
      foreach (Type type in keys)
        RemoveExact (type);
    }

    public void Add (ClassContext value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      if (ContainsExact (value.Type))
      {
        string message = string.Format ("A class context for type {0} was already added.", value.Type.FullName);
        throw new InvalidOperationException (message);
      }
      
      _values.Add (value.Type, value);
      if (ClassContextAdded != null)
        ClassContextAdded (this, new ClassContextEventArgs (value));
    }

    public void AddOrReplace (ClassContext value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      RemoveExact (value.Type);
      Add (value);
    }

    public bool RemoveExact (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ClassContext removed = GetExact (type);
      bool result = _values.Remove (type);
      if (result && ClassContextRemoved != null)
        ClassContextRemoved (this, new ClassContextEventArgs (removed));
      return result;
    }

    bool ICollection<ClassContext>.Remove (ClassContext item)
    {
      if (!Contains (item))
        return false;
      else
      {
        bool result = RemoveExact (item.Type);
        Assertion.IsTrue (result);
        Assertion.IsFalse (Contains (item));
        return result;
      }
    }

    public ClassContext GetExact (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (_values.ContainsKey (type))
        return _values[type];
      else
        return null;
    }

    public ClassContext GetWithInheritance (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return _inheritanceAlgorithm.GetWithInheritance (type);
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
      get { return false; }
    }
  }
}
