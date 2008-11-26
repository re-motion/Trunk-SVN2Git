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
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.EnumUtilityTests
{
  [TestFixture]
  public class Int16Enum
  {
    public enum TestEnum:short
    {
      Negative = -1,
      Zero = 0,
      Positive = 1
    }

    [Flags]
    public enum TestFlags : short
    {
      Flag1 = 1,
      Flag2 = 2,
      Flag3 = 4,
      Flag4 = 8,
      Flag1and2 = Flag1 | Flag2,
      AllFlags = Flag1and2 | Flag3 | Flag4
    }

    [Test]
    public void IsValidEnumValue_WithEnum_AndValueOutOfRange_Negative ()
    {
      Assert.IsFalse (EnumUtility.IsValidEnumValue ((TestEnum) (short)(-3)));
    }

    [Test]
    public void IsValidEnumValue_WithEnum_AndValueOutOfRange_Positive ()
    {
      Assert.IsFalse (EnumUtility.IsValidEnumValue ((TestEnum) (short)3));
    }

    [Test]
    public void IsValidEnumValue_WithEnum_AndValueDefinedAsNegative ()
    {
      Assert.IsTrue (EnumUtility.IsValidEnumValue (TestEnum.Negative));
    }

    [Test]
    public void IsValidEnumValue_WithEnum_AndValueDefinedAsZero ()
    {
      Assert.IsTrue (EnumUtility.IsValidEnumValue (TestEnum.Zero));
    }

    [Test]
    public void IsValidEnumValue_WithEnum_AndValueDefinedAsPositive ()
    {
      Assert.IsTrue (EnumUtility.IsValidEnumValue (TestEnum.Positive));
    }

    [Test]
    public void IsValidEnumValue_WithTypeAndInt16 ()
    {
      Assert.IsTrue (EnumUtility.IsValidEnumValue (typeof (TestEnum), (Int16)1));
    }

    [Test]
    public void IsValidEnumValue_WithTypeAndEnum ()
    {
      Assert.IsTrue (EnumUtility.IsValidEnumValue (typeof (TestEnum), TestEnum.Positive));
    }

    [Test]
    public void IsValidEnumValue_WithFlag_AndValueOutOfRange_Negative ()
    {
      Assert.IsFalse (EnumUtility.IsValidEnumValue ((TestFlags) (short)(-3)));
    }

    [Test]
    public void IsValidEnumValue_WithFlag_AndValueOutOfRange_UndefinedBit ()
    {
      Assert.IsFalse (EnumUtility.IsValidEnumValue (TestFlags.Flag1 | ((TestFlags) (short)16)));
    }

    [Test]
    public void IsValidEnumValue_WithFlag_AndFlagCombination ()
    {
      Assert.IsTrue (EnumUtility.IsValidEnumValue (TestFlags.Flag1 | TestFlags.Flag3));
    }

    [Test]
    public void IsValidEnumValue_WithFlag_AndFlagCombination2 ()
    {
      Assert.IsTrue (EnumUtility.IsValidEnumValue (TestFlags.Flag1 | TestFlags.Flag2 | TestFlags.Flag3));
    }

    [Test]
    public void IsFlagsEnumValue_NotFlag ()
    {
      Assert.That (EnumUtility.IsFlagsEnumValue (TestEnum.Zero), Is.False);
    }

    [Test]
    public void IsFlagsEnumValue_Flag ()
    {
      Assert.That (EnumUtility.IsFlagsEnumValue (TestFlags.Flag1), Is.True);
    }

    [Test]
    public void IsFlagsEnumType_NotFlag ()
    {
      Assert.That (EnumUtility.IsFlagsEnumType (typeof (TestEnum)), Is.False);
    }

    [Test]
    public void IsFlagsEnumType_Flag ()
    {
      Assert.That (EnumUtility.IsFlagsEnumType (typeof (TestFlags)), Is.True);
    }
  }
}