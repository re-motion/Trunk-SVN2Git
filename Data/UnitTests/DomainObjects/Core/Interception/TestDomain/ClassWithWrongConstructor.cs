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
using NUnit.Framework;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Interception.TestDomain
{
  [DBTable]
  public class ClassWithWrongConstructor : DomainObject
  {
    public static ClassWithWrongConstructor NewObject ()
    {
      return NewObject<ClassWithWrongConstructor> ().With();
    }

    public static ClassWithWrongConstructor NewObject (double d)
    {
      return NewObject<ClassWithWrongConstructor> ().With (d);
    }

    public ClassWithWrongConstructor (string s)
    {
      Assert.Fail ("Shouldn't be executed.");
    }
  }
}
