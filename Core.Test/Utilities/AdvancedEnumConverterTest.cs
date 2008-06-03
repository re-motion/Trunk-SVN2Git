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
  public class AdvancedEnumConverterTest
  {
    private AdvancedEnumConverter _int32EnumConverter;
    private AdvancedEnumConverter _int16EnumConverter;
    private AdvancedEnumConverter _nullableInt32EnumConverter;

    public enum Int32Enum: int
    {
      ValueA = 0,
      ValueB = 1
    }

    public enum Int16Enum: short
    {
      ValueA = 0,
      ValueB = 1
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
      Assert.AreEqual (Int32Enum.ValueA, _int32EnumConverter.ConvertFrom ("ValueA"));
      Assert.AreEqual (Int32Enum.ValueB, _int32EnumConverter.ConvertFrom ("ValueB"));
      Assert.AreEqual (Int32Enum.ValueA, _nullableInt32EnumConverter.ConvertFrom ("ValueA"));
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

      Assert.AreEqual ("ValueA", _int32EnumConverter.ConvertTo (Int32Enum.ValueA, destinationType));
      Assert.AreEqual ("ValueB", _int32EnumConverter.ConvertTo (Int32Enum.ValueB, destinationType));
      Assert.AreEqual ("ValueA", _nullableInt32EnumConverter.ConvertTo (Int32Enum.ValueA, destinationType));
      Assert.AreEqual (string.Empty, _nullableInt32EnumConverter.ConvertTo (null, destinationType));
    }

    [Test]
    public void ConvertFromInt32()
    {
      Assert.AreEqual (Int32Enum.ValueA, _int32EnumConverter.ConvertFrom (0));
      Assert.AreEqual (Int32Enum.ValueB, _int32EnumConverter.ConvertFrom (1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException), ExpectedMessage =
        "The value -1 is not supported for enumeration 'Remotion.UnitTests.Utilities.AdvancedEnumConverterTest+Int32Enum'.")]
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

      Assert.AreEqual (0, _int32EnumConverter.ConvertTo (Int32Enum.ValueA, destinationType));
      Assert.AreEqual (1, _int32EnumConverter.ConvertTo (Int32Enum.ValueB, destinationType));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ConvertToInt32_WithNullableInt32EnumConverter()
    {
      Assert.AreEqual (1, _nullableInt32EnumConverter.ConvertTo (Int32Enum.ValueB, typeof (Int32)));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void ConvertToInt32_WithInt16EnumConverter ()
    {
      Assert.AreEqual (1, _int16EnumConverter.ConvertTo (Int32Enum.ValueB, typeof (Int32)));
    }

    [Test]
    public void ConvertFromNullableInt32()
    {
      Assert.AreEqual (Int32Enum.ValueA, _nullableInt32EnumConverter.ConvertFrom (0));
      Assert.AreEqual (Int32Enum.ValueB, _nullableInt32EnumConverter.ConvertFrom (1));
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

      Assert.AreEqual (0, _nullableInt32EnumConverter.ConvertTo (Int32Enum.ValueA, destinationType));
      Assert.AreEqual (1, _nullableInt32EnumConverter.ConvertTo (Int32Enum.ValueB, destinationType));
      Assert.IsNull (_nullableInt32EnumConverter.ConvertTo (null, destinationType));
      Assert.AreEqual (0, _int32EnumConverter.ConvertTo (Int32Enum.ValueA, destinationType));
    }

    [Test]
    public void ConvertFromInt16()
    {
      Assert.AreEqual (Int16Enum.ValueA, _int16EnumConverter.ConvertFrom ((Int16) 0));
      Assert.AreEqual (Int16Enum.ValueB, _int16EnumConverter.ConvertFrom ((Int16) 1));
    }

    [Test]
    public void ConvertToInt16()
    {
      Type destinationType = typeof (Int16);

      Assert.AreEqual ((Int16) 0, _int16EnumConverter.ConvertTo (Int16Enum.ValueA, destinationType));
      Assert.AreEqual ((Int16) 1, _int16EnumConverter.ConvertTo (Int16Enum.ValueB, destinationType));
    }
  }
}
