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
