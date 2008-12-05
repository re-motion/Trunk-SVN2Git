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

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObject]
  public class ClassWithReferenceType<T> : IInterfaceWithReferenceType<T>
      where T: class
  {
    private T _explicitInterfaceScalar;
    private readonly T _readOnlyScalar = default (T);

    public ClassWithReferenceType ()
    {
    }

    public T Scalar { get; set; }

    T IInterfaceWithReferenceType<T>.ExplicitInterfaceScalar
    {
      get { return _explicitInterfaceScalar; }
      set { _explicitInterfaceScalar = value; }
    }

    T IInterfaceWithReferenceType<T>.ExplicitInterfaceReadOnlyScalar
    {
      get { return _explicitInterfaceScalar; }
    }

    public T ImplicitInterfaceScalar { get; set; }

    public T ReadOnlyScalar
    {
      get { return _readOnlyScalar; }
    }

    public T ReadOnlyNonPublicSetterScalar { get; protected set; }

    [ObjectBinding (Visible = false)]
    public T NotVisibleAttributeScalar { get; set; }

    public T NotVisibleNonPublicGetterScalar { protected get; set; }

    [ObjectBinding (ReadOnly = true)]
    public T ReadOnlyAttributeScalar { get; set; }

    public T[] Array { get; set; }
  }
}
