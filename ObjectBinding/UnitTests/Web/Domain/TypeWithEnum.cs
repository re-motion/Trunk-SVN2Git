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
  public class TypeWithEnum
  {
    public static TypeWithEnum Create ()
    {
      return ObjectFactory.Create<TypeWithEnum> (true).With ();
    }

    private TestEnum _enumValue;

    protected TypeWithEnum ()
    {
    }

    public TestEnum EnumValue
    {
      get { return _enumValue; }
      set { _enumValue = value; }
    }
  }

  public enum TestEnum
  {
    First,
    Second,
    Third
  }
}
