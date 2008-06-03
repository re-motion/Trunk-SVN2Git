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
using Remotion.Mixins;

namespace Remotion.ObjectBinding.UnitTests.Web.Domain
{
  [BindableObject]
  public class TypeWithBoolean
  {
    public static TypeWithBoolean Create ()
    {
      return ObjectFactory.Create<TypeWithBoolean> (true).With ();
    }

    private bool _booleanValue;
    private bool? _nullableBooleanValue;

    protected TypeWithBoolean ()
    {
    }

    public bool BooleanValue
    {
      get { return _booleanValue; }
      set { _booleanValue = value; }
    }

    public bool? NullableBooleanValue
    {
      get { return _nullableBooleanValue; }
      set { _nullableBooleanValue = value; }
    }
  }
}
