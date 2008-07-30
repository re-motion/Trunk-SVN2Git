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
using Remotion.Development.UnitTesting;

namespace Remotion.Development.UnitTests.Core.UnitTesting
{

  public class TypeWithFields
  {
    public int IntField;
    private string _stringField = null;

    public static int StaticIntField;
    private static string s_stringField = null;

    private void DummyReferenceFieldsToSupressWarnings ()
    {
      _stringField = s_stringField;
    }
  }

  public class DerivedTypeWithFields : TypeWithFields
  {
  }

  [TestFixture]
  public class TestFieldAccess
  {
    [Test]
    public void TestInstanceFields ()
    {
      TypeWithFields twp = new TypeWithFields ();
      DerivedTypeWithFields dtwp = new DerivedTypeWithFields ();

      PrivateInvoke.SetPublicField (twp, "IntField", 21);
      Assert.AreEqual (21, PrivateInvoke.GetPublicField (twp, "IntField"));

      PrivateInvoke.SetNonPublicField (twp, "_stringField", "test 3");
      Assert.AreEqual ("test 3", PrivateInvoke.GetNonPublicField (twp, "_stringField"));

      PrivateInvoke.SetNonPublicField (dtwp, "_stringField", "test 3");
      Assert.AreEqual ("test 3", PrivateInvoke.GetNonPublicField (dtwp, "_stringField"));
    }

    [Test]
    public void TestStaticFields ()
    {
      PrivateInvoke.SetPublicStaticField (typeof (TypeWithFields), "StaticIntField", 22);
      Assert.AreEqual (22, PrivateInvoke.GetPublicStaticField (typeof (TypeWithFields), "StaticIntField"));

      PrivateInvoke.SetNonPublicStaticField (typeof (TypeWithFields), "s_stringField", "test 4");
      Assert.AreEqual ("test 4", PrivateInvoke.GetNonPublicStaticField (typeof (TypeWithFields), "s_stringField"));
    }
  }

}
