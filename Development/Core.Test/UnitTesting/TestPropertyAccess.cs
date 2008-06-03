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

namespace Remotion.Development.UnitTests.UnitTesting
{

public class TypeWithProperties
{
  private int _intField;
  public int IntProperty
  {
    get { return _intField; }
    set { _intField = value; }
  }

  private string _stringField;
  protected string StringProperty
  {
    get { return _stringField; }
    set { _stringField = value; }
  }

  private static int s_intField;
  public static int StaticIntProperty
  {
    get { return s_intField; }
    set { s_intField = value; }
  }

  private static string s_stringField;
  protected static string StaticStringProperty
  {
    get { return s_stringField; }
    set { s_stringField = value; }
  }
}

[TestFixture]
public class TestPropertyAccess
{
  [Test]
	public void TestInstanceProperties()
	{
    TypeWithProperties twp = new TypeWithProperties();

    PrivateInvoke.SetPublicProperty (twp, "IntProperty", 12);
    Assert.AreEqual (12, PrivateInvoke.GetPublicProperty (twp, "IntProperty"));

    PrivateInvoke.SetNonPublicProperty (twp, "StringProperty", "test 1");
    Assert.AreEqual ("test 1", PrivateInvoke.GetNonPublicProperty (twp, "StringProperty"));
	}

  [Test]
	public void TestStaticProperties()
	{
    PrivateInvoke.SetPublicStaticProperty (typeof (TypeWithProperties), "StaticIntProperty", 13);
    Assert.AreEqual (13, PrivateInvoke.GetPublicStaticProperty (typeof (TypeWithProperties), "StaticIntProperty"));

    PrivateInvoke.SetNonPublicStaticProperty (typeof (TypeWithProperties), "StaticStringProperty", "test 2");
    Assert.AreEqual ("test 2", PrivateInvoke.GetNonPublicStaticProperty (typeof (TypeWithProperties), "StaticStringProperty"));
	}
}

}
