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
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class MixinIntroducingInterfacesImplementingEachOther<[BindToTargetType]T> : IList<T>, IEnumerable<T>, IList, IEnumerable
  {
    public IEnumerator<T> GetEnumerator ()
    {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public void Add (T item)
    {
      throw new NotImplementedException();
    }

    public int Add (object value)
    {
      throw new NotImplementedException();
    }

    public bool Contains (object value)
    {
      throw new NotImplementedException();
    }

    void IList.Clear ()
    {
      throw new NotImplementedException ();
    }

    public int IndexOf (object value)
    {
      throw new NotImplementedException();
    }

    public void Insert (int index, object value)
    {
      throw new NotImplementedException();
    }

    public void Remove (object value)
    {
      throw new NotImplementedException();
    }

    void IList.RemoveAt (int index)
    {
      throw new NotImplementedException ();
    }

    object IList.this [int index]
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    bool IList.IsReadOnly
    {
      get { throw new NotImplementedException (); }
    }

    public bool IsFixedSize
    {
      get { throw new NotImplementedException(); }
    }

    void ICollection<T>.Clear ()
    {
      throw new NotImplementedException ();
    }

    public bool Contains (T item)
    {
      throw new NotImplementedException();
    }

    public void CopyTo (T[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    public bool Remove (T item)
    {
      throw new NotImplementedException();
    }

    public void CopyTo (Array array, int index)
    {
      throw new NotImplementedException();
    }

    int ICollection.Count
    {
      get { throw new NotImplementedException (); }
    }

    public object SyncRoot
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsSynchronized
    {
      get { throw new NotImplementedException(); }
    }

    int ICollection<T>.Count
    {
      get { throw new NotImplementedException(); }
    }

    bool ICollection<T>.IsReadOnly
    {
      get { throw new NotImplementedException(); }
    }

    public int IndexOf (T item)
    {
      throw new NotImplementedException();
    }

    public void Insert (int index, T item)
    {
      throw new NotImplementedException();
    }

    void IList<T>.RemoveAt (int index)
    {
      throw new NotImplementedException ();
    }

    public T this [int index]
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }
  }
}