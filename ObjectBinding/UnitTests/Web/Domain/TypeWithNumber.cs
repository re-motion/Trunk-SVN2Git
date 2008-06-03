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
  public class TypeWithNumber
  {
    public static TypeWithNumber Create ()
    {
      return ObjectFactory.Create<TypeWithNumber> (true).With ();
    }

    private int _int32Value;
    private int? _nullableInt32Value;

    protected TypeWithNumber ()
    {
    }

    public int Int32Value
    {
      get { return _int32Value; }
      set { _int32Value = value; }
    }

    public int? NullableInt32Value
    {
      get { return _nullableInt32Value; }
      set { _nullableInt32Value = value; }
    }
  }
}
