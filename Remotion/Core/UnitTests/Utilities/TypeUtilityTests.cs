// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections;
using System.Timers;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
// ReSharper disable UnusedTypeParameter
  public class GenericType<T1, T2>
// ReSharper restore UnusedTypeParameter
  {
  }

  [TestFixture]
  public class TypeUtilityTests
  {
    [Test]
    public void TestAbbreviatedTypeName()
    {
      AssertTransformation (  
          "Remotion.UnitTests::Utilities.TypeUtilityTests",
          "Remotion.UnitTests.Utilities.TypeUtilityTests, Remotion.UnitTests");
    }

    [Test]
    public void TestAbbreviatedOpenGenericTypeName()
    {
      AssertTransformation (  
          "A.B::C.D`2",
          "A.B.C.D`2, A.B");
    }

    [Test]
    public void TestAbbreviatedClosedGenericTypeName()
    {
      AssertTransformation (  
          "A.B::C.D`2[a.b::c.d,System.String]",
          "A.B.C.D`2[[a.b.c.d, a.b],System.String], A.B");
    }

    [Test]
    public void TestAbbreviatedTypeArgumentWithOptionalBrackets()
    {
      AssertTransformation (  
          "Dictionary`2[[a.b::c.d],System.String]",
          "Dictionary`2[[a.b.c.d, a.b],System.String]");
    }

    [Test]
    public void TestClosedGenericTypeNameWithAbbreviatedTypeArgument()
    {
      AssertTransformation (  
          "A.B.C.D`2[a.b::c.d,System.String], A.B",
          "A.B.C.D`2[[a.b.c.d, a.b],System.String], A.B");
    }

    [Test]
    public void TestPartiallyAbbreviatedClosedGenericTypeName()
    {
      AssertTransformation (  
          "A.B::C.D`2[a.b::c.d,System.String], Version=1.0.0.0",
          "A.B.C.D`2[[a.b.c.d, a.b],System.String], A.B, Version=1.0.0.0");
    }

    [Test]
    public void TestAbbreviatedClosedGenericTypeWithPartiallyAbbreviatedTypeArgument()
    {
      AssertTransformation (  
          "A.B::C.D`2[[a.b::c.d, Version=1.0.0.0],System.String]",
          "A.B.C.D`2[[a.b.c.d, a.b, Version=1.0.0.0],System.String], A.B");
    }

    [Test]
    public void TestGetType()
    {
      Type t = TypeUtility.GetType ("Remotion.UnitTests::Utilities.TypeUtilityTests", true);
      Assert.AreEqual (typeof (TypeUtilityTests), t);
    }

    [Test]
    public void TestNestedUnqualified ()
    {
      AssertTransformation ("a::b[c::d,e::f,g::h]",
                            "a.b[[c.d, c],[e.f, e],[g.h, g]], a");
    }

    [Test]
    public void TestNestedOptionalBrackets ()
    {
      AssertTransformation ("a::b[[c::d],[e::f],[g::h]]",
                            "a.b[[c.d, c],[e.f, e],[g.h, g]], a");
    }

    [Test]
    public void TestNestedQualified  ()
    {
      AssertTransformation ("a::b[[c::d, ver=1],[e::f, ver=2],[g::h, ver=3]], ver=4",
                            "a.b[[c.d, c, ver=1],[e.f, e, ver=2],[g.h, g, ver=3]], a, ver=4");
    }


    [Test]
    public void TestNestedWithArgsUnqualified ()
    {
      AssertTransformation ("a::b[c::d[...],e::f[...],g::h[...]]",
                            "a.b[[c.d[...], c],[e.f[...], e],[g.h[...], g]], a");
    }

    [Test]
    public void TestNestedWithArgsOptionalBrackets ()
    {
      AssertTransformation ("a::b[[c::d[...]],[e::f[...]],[g::h[...]]]",
                            "a.b[[c.d[...], c],[e.f[...], e],[g.h[...], g]], a");
    }

    [Test]
    public void TestNestedWithArgsQualified  ()
    {
      AssertTransformation ("a::b[[c::d[...], ver=1],[e::f[...], ver=2],[g::h[...], ver=3]], ver=4",
                            "a.b[[c.d[...], c, ver=1],[e.f[...], e, ver=2],[g.h[...], g, ver=3]], a, ver=4");
    }

    [Test]
    public void TestDeepNestedWithArgsUnqualified ()
    {
      AssertTransformation ("a::b[c::d[e::f[g::h[...]]]]",
                            "a.b[[c.d[[e.f[[g.h[...], g]], e]], c]], a");
    }

    [Test]
    public void TestDeepNestedWithArgsOptionalBrackets ()
    {
      AssertTransformation ("a::b[[c::d[[e::f[[g::h[...]]]]]]]",
                            "a.b[[c.d[[e.f[[g.h[...], g]], e]], c]], a");
    }

    [Test]
    public void TestDeepNestedWithArgsQualified ()
    {
      AssertTransformation ("a::b[[c::d[[e::f[[g::h[...], ver=1]], ver=2]], ver=3]], ver=4",
                            "a.b[[c.d[[e.f[[g.h[...], g, ver=1]], e, ver=2]], c, ver=3]], a, ver=4");
    }

    [Test]
    public void TestDeepNestedWithArgsWithStrongName ()
    {
      AssertTransformation ("a::b[[c::d[[e::f[[g::h[...], ver=1, token=2]], ver=2, token=3]], ver=3, token=4]], ver=4, token=5",
                            "a.b[[c.d[[e.f[[g.h[...], g, ver=1, token=2]], e, ver=2, token=3]], c, ver=3, token=4]], a, ver=4, token=5");
    }

    [Test]
    [Ignore ("COMMONS-1596")]
    public void GetAbbreviatedTypeName_WithSubNamespaceAndWithoutVersionAndCulture ()
    {
      string name = TypeUtility.GetAbbreviatedTypeName (typeof (Timer), false);
      Assert.AreEqual ("System::Timers.Timer", name);
    }

    [Test]
    [Ignore ("COMMONS-1596")]
    public void GetAbbreviatedTypeName_WithSubNamespaceAndWithVersionAndCulture ()
    {
      string name = TypeUtility.GetAbbreviatedTypeName (typeof (Timer), true);
      Assert.AreEqual ("System::Timers.Timer" + typeof (Timer).Assembly.FullName.Replace ("System", string.Empty), name);
    }

    [Test]
    public void GetAbbreviatedTypeName_WithoutAbbreviate ()
    {
      string name = TypeUtility.GetAbbreviatedTypeName (typeof (Hashtable), false);
      Assert.AreEqual ("System.Collections.Hashtable, mscorlib", name);
    }

    [Test]
    public void GetAbbreviatedTypeName_WithoutSubNamespaceAndWithoutVersionAndCulture ()
    {
      string name = TypeUtility.GetAbbreviatedTypeName (typeof (Uri), false);
      Assert.AreEqual ("System::Uri", name);
    }

    [Test]
    public void GetAbbreviatedTypeName_WithoutSubNamespaceAndWithVersionAndCulture ()
    {
      string name = TypeUtility.GetAbbreviatedTypeName (typeof (Uri), true);
      Assert.AreEqual ("System::Uri" + typeof (Uri).Assembly.FullName.Replace ("System", string.Empty), name);
    }

    private void AssertTransformation (string abbreviatedName, string fullName)
    {
      string result = TypeUtility.ParseAbbreviatedTypeName (abbreviatedName);
      Assert.AreEqual (fullName, result);
    }
  }
}
