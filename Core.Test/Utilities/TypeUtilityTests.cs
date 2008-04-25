using System;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  public class GenericType<T1, T2>
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

    private void AssertTransformation (string abbreviatedName, string fullName)
    {
      string result = TypeUtility.ParseAbbreviatedTypeName (abbreviatedName);
      Assert.AreEqual (fullName, result);
    }

    [Test]
    public void AbbreviateWithoutNotFull()
    {
      string name = TypeUtility.GetAbbreviatedTypeName (typeof (Uri), false);
      Assert.AreEqual ("System::Uri", name);
    }

    [Test]
    [Ignore ("TODO: SW")]
    public void AbbreviateWithoutFull ()
    {
      string name = TypeUtility.GetAbbreviatedTypeName (typeof (Uri), true);
      Assert.AreEqual ("System::Uri, XXXXXXXXXXXXXXX", name);
    }
  }
}
