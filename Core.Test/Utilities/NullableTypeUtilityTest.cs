using System;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class NullableTypeUtilityTest
  {
    [Test]
    public void IsNullableType_ValueType ()
    {
      Assert.IsFalse (NullableTypeUtility.IsNullableType (typeof (int)));
      Assert.IsFalse (NullableTypeUtility.IsNullableType (typeof (DateTime)));
    }

    [Test]
    public void IsNullableType_NullableValueType ()
    {
      Assert.IsTrue (NullableTypeUtility.IsNullableType (typeof (int?)));
      Assert.IsTrue (NullableTypeUtility.IsNullableType (typeof (DateTime?)));
    }

    [Test]
    public void IsNullableType_ReferenceType ()
    {
      Assert.IsTrue (NullableTypeUtility.IsNullableType (typeof (object)));
      Assert.IsTrue (NullableTypeUtility.IsNullableType (typeof (string)));
    }

    [Test]
    public void GetNullableType_ValueType ()
    {
      Assert.AreEqual (typeof (int?), NullableTypeUtility.GetNullableType (typeof (int)));
    }

    [Test]
    public void GetNullableType_NullableValueType ()
    {
      Assert.AreEqual (typeof (int?), NullableTypeUtility.GetNullableType (typeof (int?)));
    }

    [Test]
    public void GetNullableType_ReferenceType ()
    {
      Assert.AreEqual (typeof (string), NullableTypeUtility.GetNullableType (typeof (string)));
    }

    [Test]
    public void GetBasicType_ValueType ()
    {
      Assert.AreEqual (typeof (int), NullableTypeUtility.GetBasicType (typeof (int)));
    }

    [Test]
    public void GetBasicType_NullableValueType ()
    {
      Assert.AreEqual (typeof (int), NullableTypeUtility.GetBasicType (typeof (int?)));
    }


    [Test]
    public void GetBasicType_ReferenceType ()
    {
      Assert.AreEqual (typeof (string), NullableTypeUtility.GetBasicType (typeof (string)));
      }
  }
}