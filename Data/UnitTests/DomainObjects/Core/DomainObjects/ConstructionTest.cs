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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;


namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class ConstructionTest : ClientTransactionBaseTest
  {
    [DBTable]
    public class DomainObjectWithSpecialConstructor : DomainObject
    {
      public string S;
      public object O;

      public DomainObjectWithSpecialConstructor (string s)
      {
        S = s;
      }

      public DomainObjectWithSpecialConstructor (object o)
      {
        O = o;
      }

      public static DomainObjectWithSpecialConstructor NewObject (string s)
      {
        return NewObject<DomainObjectWithSpecialConstructor>().With(s);
      }

      public static DomainObjectWithSpecialConstructor NewObject (object o)
      {
        return NewObject<DomainObjectWithSpecialConstructor> ().With (o);
      }
    }

    [Test]
    public void ConstructorSelection ()
    {
      DomainObjectWithSpecialConstructor d1 = DomainObjectWithSpecialConstructor.NewObject ("string");
      Assert.AreEqual ("string", d1.S);
      Assert.IsNull (d1.O);

      object obj = new object ();
      DomainObjectWithSpecialConstructor d2 = DomainObjectWithSpecialConstructor.NewObject (obj);
      Assert.IsNull (d2.S);
      Assert.AreSame (obj, d2.O);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "DomainObject constructors must not be called directly. Use " 
        + "DomainObject.NewObject to create DomainObject instances.")]
    public void ConstructorThrowsIfCalledDirectly ()
    {
      new DomainObjectWithSpecialConstructor ("string");
    }

    [Test]
    public void ConstructorWorksIfCalledIndirectly ()
    {
      var instance = DomainObjectWithSpecialConstructor.NewObject ("string");
      Assert.That (instance, Is.Not.Null);
    }
  }
}
