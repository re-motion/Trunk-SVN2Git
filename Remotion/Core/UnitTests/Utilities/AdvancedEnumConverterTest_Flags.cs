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
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class AdvancedEnumConverterTest_Flags
  {
    private AdvancedEnumConverter _int32EnumConverter;
    private AdvancedEnumConverter _int16EnumConverter;
    private AdvancedEnumConverter _nullableInt32EnumConverter;

    [Flags]
    public enum Int32Enum: int
    {
      Value0 = 0,
      Value1 = 1,
      Value2 = 2,
      Value4 = 4,
      Value5 = Value1 | Value4,
    }

    [Flags]
    public enum Int16Enum : short
    {
      Value0 = 0,
      Value1 = 1
    }

    [SetUp]
    public void SetUp()
    {
      _int32EnumConverter = new AdvancedEnumConverter (typeof (Int32Enum));
      _int16EnumConverter = new AdvancedEnumConverter (typeof (Int16Enum));
      _nullableInt32EnumConverter = new AdvancedEnumConverter (typeof (Int32Enum?));
    }

    [Test]
    public void CanConvertFromString()
    {
      Assert.IsTrue (_int32EnumConverter.CanConvertFrom (typeof (string)));
      Assert.IsTrue (_nullableInt32EnumConverter.CanConvertFrom (typeof (string)));
    }

    [Test]
    public void CanConvertToString()
    {
      Assert.IsTrue (_int32EnumConverter.CanConvertTo (typeof (string)));
      Assert.IsTrue (_nullableInt32EnumConverter.CanConvertTo (typeof (string)));
    }

    [Test]
    public void CanConvertFromNumeric()
    {
      Assert.IsTrue (_int32EnumConverter.CanConvertFrom (typeof (Int32)));
      Assert.IsTrue (_int16EnumConverter.CanConvertFrom (typeof (Int16)));
      Assert.IsTrue (_nullableInt32EnumConverter.CanConvertFrom (typeof (Int32?)));
      Assert.IsTrue (_nullableInt32EnumConverter.CanConvertFrom (typeof (Int32)));

      Assert.IsFalse (_int32EnumConverter.CanConvertFrom (typeof (Int16)));
      Assert.IsFalse (_int16EnumConverter.CanConvertFrom (typeof (Int32)));
      Assert.IsFalse (_int32EnumConverter.CanConvertFrom (typeof (Int32?)));
      Assert.IsFalse (_nullableInt32EnumConverter.CanConvertFrom (typeof (Int16?)));
    }

    [Test]
    public void CanConvertToNumeric()
    {
      Assert.IsTrue (_int32EnumConverter.CanConvertTo (typeof (Int32)));
      Assert.IsTrue (_int16EnumConverter.CanConvertTo (typeof (Int16)));
      Assert.IsTrue (_nullableInt32EnumConverter.CanConvertTo (typeof (Int32?)));
      Assert.IsTrue (_int32EnumConverter.CanConvertTo (typeof (Int32?)));

      Assert.IsFalse (_int32EnumConverter.CanConvertTo (typeof (Int16)));
      Assert.IsFalse (_int16EnumConverter.CanConvertTo (typeof (Int32)));
      Assert.IsFalse (_nullableInt32EnumConverter.CanConvertTo (typeof (Int32)));
      Assert.IsFalse (_nullableInt32EnumConverter.CanConvertTo (typeof (Int16?)));
    }

    [Test]
    public void ConvertFromString()
    {
      Assert.AreEqual (Int32Enum.Value0, _int32EnumConverter.ConvertFrom ("Value0"));
      Assert.AreEqual (Int32Enum.Value1, _int32EnumConverter.ConvertFrom ("Value1"));
      Assert.AreEqual (Int32Enum.Value2, _int32EnumConverter.ConvertFrom ("Value2"));
      Assert.AreEqual (Int32Enum.Value5, _int32EnumConverter.ConvertFrom ("Value5"));
      Assert.AreEqual (Int32Enum.Value1 | Int32Enum.Value2, _int32EnumConverter.ConvertFrom ("Value1, Value2"));
      
      Assert.AreEqual (Int32Enum.Value1, _nullableInt32EnumConverter.ConvertFrom ("Value1"));
      Assert.AreEqual (Int32Enum.Value5, _nullableInt32EnumConverter.ConvertFrom ("Value5"));
      Assert.AreEqual (Int32Enum.Value1 | Int32Enum.Value2, _nullableInt32EnumConverter.ConvertFrom ("Value1, Value2"));
      Assert.AreEqual (null, _nullableInt32EnumConverter.ConvertFrom (null));
      Assert.AreEqual (null, _nullableInt32EnumConverter.ConvertFrom (string.Empty));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ConvertFromString_WithNullAndInt32EnumConverter ()
    {
      _int32EnumConverter.ConvertFrom (null);
    }

    [Test]
    [ExpectedException (typeof (FormatException))]
    public void ConvertFromString_WithEmptyStringAndInt32EnumConverter ()
    {
      _int32EnumConverter.ConvertFrom (string.Empty);
    }

    [Test]
    public void ConvertToString()
    {
      Type destinationType = typeof (string);

      Assert.AreEqual ("Value0", _int32EnumConverter.ConvertTo (Int32Enum.Value0, destinationType));
      Assert.AreEqual ("Value1", _int32EnumConverter.ConvertTo (Int32Enum.Value1, destinationType));
      Assert.AreEqual ("Value2", _int32EnumConverter.ConvertTo (Int32Enum.Value2, destinationType));
      Assert.AreEqual ("Value5", _int32EnumConverter.ConvertTo (Int32Enum.Value5, destinationType));
      Assert.AreEqual ("Value1, Value2", _int32EnumConverter.ConvertTo (Int32Enum.Value1 | Int32Enum.Value2, destinationType));

      Assert.AreEqual ("Value1", _nullableInt32EnumConverter.ConvertTo (Int32Enum.Value1, destinationType));
      Assert.AreEqual ("Value5", _nullableInt32EnumConverter.ConvertTo (Int32Enum.Value5, destinationType));
      Assert.AreEqual ("Value1, Value2", _nullableInt32EnumConverter.ConvertTo (Int32Enum.Value1 | Int32Enum.Value2, destinationType));
      Assert.AreEqual (string.Empty, _nullableInt32EnumConverter.ConvertTo (null, destinationType));
    }

    [Test]
    public void ConvertFromInt32()
    {
      Assert.AreEqual (Int32Enum.Value0, _int32EnumConverter.ConvertFrom (0));
      Assert.AreEqual (Int32Enum.Value1, _int32EnumConverter.ConvertFrom (1));
      Assert.AreEqual (Int32Enum.Value2, _int32EnumConverter.ConvertFrom (2));
      Assert.AreEqual (Int32Enum.Value5, _int32EnumConverter.ConvertFrom (5));
      Assert.AreEqual (Int32Enum.Value1 | Int32Enum.Value2, _int32EnumConverter.ConvertFrom (3));
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException), ExpectedMessage =
        "The value -1 is not supported for enumeration 'Remotion.UnitTests.Utilities.AdvancedEnumConverterTest_Flags+Int32Enum'.")]
    public void ConvertFromInt32_WithUndefinedValue()
    {
      _int32EnumConverter.ConvertFrom (-1);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ConvertFromInt32_WithInvalidDataType()
    {
      _int32EnumConverter.ConvertFrom ((short) -1);
    }

    [Test]
    public void ConvertToInt32()
    {
      Type destinationType = typeof (Int32);

      Assert.AreEqual (0, _int32EnumConverter.ConvertTo (Int32Enum.Value0, destinationType));
      Assert.AreEqual (1, _int32EnumConverter.ConvertTo (Int32Enum.Value1, destinationType));
      Assert.AreEqual (2, _int32EnumConverter.ConvertTo (Int32Enum.Value2, destinationType));
      Assert.AreEqual (5, _int32EnumConverter.ConvertTo (Int32Enum.Value5, destinationType));
      Assert.AreEqual (3, _int32EnumConverter.ConvertTo (Int32Enum.Value1|Int32Enum.Value2, destinationType));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ConvertToInt32_WithNullableInt32EnumConverter()
    {
      Assert.AreEqual (1, _nullableInt32EnumConverter.ConvertTo (Int32Enum.Value1, typeof (Int32)));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ConvertToInt32_WithInt16EnumConverter ()
    {
      Assert.AreEqual (1, _int16EnumConverter.ConvertTo (Int32Enum.Value1, typeof (Int32)));
    }

    [Test]
    public void ConvertFromNullableInt32()
    {
      Assert.AreEqual (Int32Enum.Value0, _nullableInt32EnumConverter.ConvertFrom (0));
      Assert.AreEqual (Int32Enum.Value1, _nullableInt32EnumConverter.ConvertFrom (1));
      Assert.AreEqual (Int32Enum.Value2, _nullableInt32EnumConverter.ConvertFrom (2));
      Assert.AreEqual (Int32Enum.Value5, _nullableInt32EnumConverter.ConvertFrom (5));
      Assert.AreEqual (Int32Enum.Value1|Int32Enum.Value2, _nullableInt32EnumConverter.ConvertFrom (3));
      Assert.IsNull (_nullableInt32EnumConverter.ConvertFrom (null));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ConvertFromNullableInt32_WithInt32EnumConverter ()
    {
      _int32EnumConverter.ConvertFrom ((int?) null);
    }

    [Test]
    public void ConvertToNullableInt32()
    {
      Type destinationType = typeof (Int32?);

      Assert.AreEqual (0, _nullableInt32EnumConverter.ConvertTo (Int32Enum.Value0, destinationType));
      Assert.AreEqual (1, _nullableInt32EnumConverter.ConvertTo (Int32Enum.Value1, destinationType));
      Assert.AreEqual (2, _nullableInt32EnumConverter.ConvertTo (Int32Enum.Value2, destinationType));
      Assert.AreEqual (5, _nullableInt32EnumConverter.ConvertTo (Int32Enum.Value5, destinationType));
      Assert.AreEqual (3, _nullableInt32EnumConverter.ConvertTo (Int32Enum.Value1 | Int32Enum.Value2, destinationType));
      Assert.IsNull (_nullableInt32EnumConverter.ConvertTo (null, destinationType));
      Assert.AreEqual (0, _int32EnumConverter.ConvertTo (Int32Enum.Value0, destinationType));
    }

    [Test]
    public void ConvertFromInt16()
    {
      Assert.AreEqual (Int16Enum.Value0, _int16EnumConverter.ConvertFrom ((Int16) 0));
      Assert.AreEqual (Int16Enum.Value1, _int16EnumConverter.ConvertFrom ((Int16) 1));
    }

    [Test]
    public void ConvertToInt16()
    {
      Type destinationType = typeof (Int16);

      Assert.AreEqual ((Int16) 0, _int16EnumConverter.ConvertTo (Int16Enum.Value0, destinationType));
      Assert.AreEqual ((Int16) 1, _int16EnumConverter.ConvertTo (Int16Enum.Value1, destinationType));
    }
  }
}
