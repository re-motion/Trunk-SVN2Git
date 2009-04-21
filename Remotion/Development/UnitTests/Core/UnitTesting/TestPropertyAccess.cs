// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Development.UnitTesting;

namespace Remotion.Development.UnitTests.Core.UnitTesting
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
