/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
