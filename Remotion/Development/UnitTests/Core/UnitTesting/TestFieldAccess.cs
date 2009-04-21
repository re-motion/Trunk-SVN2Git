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
