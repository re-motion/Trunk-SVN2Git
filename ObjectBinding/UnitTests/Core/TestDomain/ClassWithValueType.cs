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
using System.Collections.Generic;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObject]
  public class ClassWithValueType<T>
      where T: struct
  {
    private T _scalar;
    private T? _nullableScalar;
    private readonly T _readOnlyScalar = default (T);
    private T _readOnlyNonPublicSetterScalar;
    private T _notVisibleScalar;
    private T _readOnlyAttributeScalar;
    private T[] _array;
    private T?[] _nullableArray;
    private List<T> _list = new List<T> ();

    public ClassWithValueType ()
    {
    }

    public T Scalar
    {
      get { return _scalar; }
      set { _scalar = value; }
    }

    public T? NullableScalar
    {
      get { return _nullableScalar; }
      set { _nullableScalar = value; }
    }

    public T ReadOnlyScalar
    {
      get { return _readOnlyScalar; }
    }

    public T ReadOnlyNonPublicSetterScalar
    {
      get { return _readOnlyNonPublicSetterScalar; }
      protected set { _readOnlyNonPublicSetterScalar = value; }
    }

    [ObjectBinding (Visible = false)]
    public T NotVisibleScalar
    {
      get { return _notVisibleScalar; }
      set { _notVisibleScalar = value; }
    }

    [ObjectBinding (ReadOnly = true)]
    public T ReadOnlyAttributeScalar
    {
      get { return _readOnlyAttributeScalar; }
      set { _readOnlyAttributeScalar = value; }
    }

    public T[] Array
    {
      get { return _array; }
      set { _array = value; }
    }

    public T?[] NullableArray
    {
      get { return _nullableArray; }
      set { _nullableArray = value; }
    }

    public List<T> List
    {
      get { return _list; }
    }
  }
}
