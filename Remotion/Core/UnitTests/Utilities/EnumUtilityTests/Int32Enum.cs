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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.EnumUtilityTests
{
  [TestFixture]
  public class Int32Enum
  {
    public enum TestEnum:int
    {
      Negative = -1,
      Zero = 0,
      Positive = 1
    }

    [Flags]
    public enum TestFlags:int
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
      Assert.IsFalse (EnumUtility.IsValidEnumValue ((TestEnum) (-3)));
    }

    [Test]
    public void IsValidEnumValue_WithEnum_AndValueOutOfRange_Positive ()
    {
      Assert.IsFalse (EnumUtility.IsValidEnumValue ((TestEnum) 3));
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
    public void IsValidEnumValue_WithTypeAndInt32 ()
    {
      Assert.IsTrue (EnumUtility.IsValidEnumValue (typeof (TestEnum), 1));
    }

    [Test]
    public void IsValidEnumValue_WithTypeAndEnum()
    {
      Assert.IsTrue (EnumUtility.IsValidEnumValue (typeof (TestEnum), TestEnum.Positive));
    }

    [Test]
    public void IsValidEnumValue_WithFlag_AndValueOutOfRange_Negative ()
    {
      Assert.IsFalse (EnumUtility.IsValidEnumValue ((TestFlags) (-3)));
    }

    [Test]
    public void IsValidEnumValue_WithFlag_AndValueOutOfRange_Zero ()
    {
      Assert.IsFalse (EnumUtility.IsValidEnumValue ((TestFlags) (0)));
    }

    [Test]
    public void IsValidEnumValue_WithFlag_AndValueOutOfRange_UndefinedBit ()
    {
      Assert.IsFalse (EnumUtility.IsValidEnumValue (TestFlags.Flag1 | ((TestFlags) 16)));
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
