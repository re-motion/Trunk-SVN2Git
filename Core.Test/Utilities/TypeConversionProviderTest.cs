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
using System.ComponentModel;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{

[TestFixture]
public class TypeConversionProviderTest
{
  public enum Int32Enum : int
  {
    ValueA = 0,
    ValueB = 1
  }

  private StubTypeConversionProvider _provider;
  private readonly Type _int32 = typeof (int);
  private readonly Type _nullableInt32 = typeof (int?);
  private readonly Type _string = typeof (string);
  private readonly Type _stringArray = typeof (string[]);
  private readonly Type _object = typeof (object);
  private readonly Type _guid = typeof (Guid);
  private readonly Type _int32Enum = typeof (Int32Enum);
  private readonly Type _nullableInt32Enum = typeof (Int32Enum?);

  [SetUp]
  public void SetUp()
  {
    _provider = new StubTypeConversionProvider();

    StubTypeConversionProvider.ClearCache();
  }

  [Test]
  public void Create()
  { 
    Assert.IsNotNull (TypeConversionProvider.Create());
  }

  [Test]
  public void TestCurrent()
  { 
    Assert.IsNotNull (TypeConversionProvider.Current);
    TypeConversionProvider provider = TypeConversionProvider.Create();
    TypeConversionProvider.SetCurrent (provider);
    Assert.IsNotNull (TypeConversionProvider.Current);
    Assert.AreSame (provider, TypeConversionProvider.Current);
  }

  [Test]
  public void CanConvertFromInt32ToInt32()
  {
    Assert.IsTrue (_provider.CanConvert (_int32, _int32));
  }

  [Test]
  public void CanConvertFromNullableInt32ToNullableInt32 ()
  {
    Assert.IsTrue (_provider.CanConvert (_nullableInt32, _nullableInt32));
  }

  [Test]
  public void CanConvertFromInt32ToNullableInt32 ()
  {
    Assert.IsTrue (_provider.CanConvert (_int32, _nullableInt32));
  }

  [Test]
  public void CanConvertFromNullableInt32ToInt32 ()
  {
    Assert.IsTrue (_provider.CanConvert (_nullableInt32, _int32));
  }

  [Test]
  public void CanConvertFromInt32ToString()
  {
    Assert.IsTrue (_provider.CanConvert (_string, _int32));
  }

  [Test]
  public void CanConvertFromStringToInt32()
  {
    Assert.IsTrue (_provider.CanConvert (_int32, _string));
  }

  [Test]
  public void CanConvertFromNullableInt32ToString ()
  {
    Assert.IsTrue (_provider.CanConvert (_string, _nullableInt32));
  }

  [Test]
  public void CanConvertFromStringToNullableInt32 ()
  {
    Assert.IsTrue (_provider.CanConvert (_nullableInt32, _string));
  }

  [Test]
  public void CanConvertFromObjectToNullableInt32()
  {
    Assert.IsFalse (_provider.CanConvert (_object, _nullableInt32));
  }

  [Test]
  public void CanConvertFromGuidToString()
  {
    Assert.IsTrue (_provider.CanConvert (_guid, _string));
  }

  [Test]
  public void CanConvertFromStringToGuid()
  {
    Assert.IsTrue (_provider.CanConvert (_string, _guid));
  }

  [Test]
  public void CanConvertFromStringArrayToString ()
  {
    Assert.IsTrue (_provider.CanConvert (_stringArray, _string));
  }

  [Test]
  public void CanConvertFromStringToStringArray ()
  {
    Assert.IsTrue (_provider.CanConvert (_string, _stringArray));
  }


  [Test]
  public void CanConvertFromInt32EnumToInt32Enum()
  {
    Assert.IsTrue (_provider.CanConvert (_int32Enum, _int32Enum));
  }

  [Test]
  public void CanConvertFromInt32EnumToInt32()
  {
    Assert.IsTrue (_provider.CanConvert (_int32Enum, _int32));
  }

  [Test]
  public void CanConvertFromInt32ToInt32Enum()
  {
    Assert.IsTrue (_provider.CanConvert (_int32, _int32Enum));
  }

  [Test]
  public void CanConvertFromInt32EnumToString()
  {
    Assert.IsTrue (_provider.CanConvert (_int32Enum, _string));
  }

  [Test]
  public void CanConvertFromStringToInt32Enum()
  {
    Assert.IsTrue (_provider.CanConvert (_string, _int32Enum));
  }


  [Test]
  public void CanConvertFromNullableInt32EnumToNullableInt32Enum ()
  {
    Assert.IsTrue (_provider.CanConvert (_nullableInt32Enum, _nullableInt32Enum));
  }

  [Test]
  public void CanConvertFromNullableInt32EnumToNullableInt32 ()
  {
    Assert.IsTrue (_provider.CanConvert (_nullableInt32Enum, _nullableInt32));
  }

  [Test]
  public void CanConvertFromNullableInt32ToNullableInt32Enum ()
  {
    Assert.IsTrue (_provider.CanConvert (_nullableInt32, _nullableInt32Enum));
  }

  [Test]
  public void CanConvertFromInt32EnumToNullableInt32 ()
  {
    Assert.IsTrue (_provider.CanConvert (_int32Enum, _nullableInt32));
  }

  [Test]
  public void CanConvertFromInt32ToNullableInt32Enum ()
  {
    Assert.IsTrue (_provider.CanConvert (_int32, _nullableInt32Enum));
  }

  [Test]
  public void CanConvertFromNullableInt32EnumToString ()
  {
    Assert.IsTrue (_provider.CanConvert (_nullableInt32Enum, _string));
  }

  [Test]
  public void CanConvertFromStringToNullableInt32Enum ()
  {
    Assert.IsTrue (_provider.CanConvert (_string, _nullableInt32Enum));
  }

  [Test]
  public void CanConvertFromDBNullToNullableInt32 ()
  {
    Assert.IsTrue (_provider.CanConvert (typeof (DBNull), _nullableInt32));
  }

  [Test]
  public void CanConvertFromDBNullToInt32 ()
  {
    Assert.IsFalse (_provider.CanConvert (typeof (DBNull), _int32));
  }

  [Test]
  public void ConvertFromInt32ToInt32()
  {
    Assert.AreEqual (1, _provider.Convert (_int32, _int32, 1));
  }

  [Test]
  public void ConvertFromNullableInt32ToNullableInt32 ()
  {
    Assert.AreEqual (new int? (1), _provider.Convert (_nullableInt32, _nullableInt32, new int? (1)));
  }

  [Test]
  public void ConvertFromNullableInt32ToInt32 ()
  {
    Assert.AreEqual (1, _provider.Convert (_nullableInt32, _int32, 1));
  }

  [Test]
  public void ConvertFromInt32ToNullableInt32 ()
  {
    Assert.AreEqual (1, _provider.Convert (_int32, _nullableInt32, 1));
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromObjectToNullableInt32 ()
  {
    _provider.Convert (_object, _nullableInt32, new object ());
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromNullableInt32ToObject ()
  {
    _provider.Convert (_nullableInt32, _object, 1);
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromInt32ToObject()
  {
    _provider.Convert (_int32, _object, 1);
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromObjectToInt32()
  {
    _provider.Convert (_object, _int32, new object());
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromInt64ToInt32 ()
  {
    _provider.Convert (_object, _int32, 1L);
  }

  [Test]
  public void ConvertFromNullableInt32ToString ()
  {
    Assert.AreEqual ("1", _provider.Convert (_nullableInt32, _string, 1));
  }

  [Test]
  public void ConvertFromStringToNullableInt32 ()
  {
    Assert.AreEqual (1, _provider.Convert (_string, _nullableInt32, "1"));
  }

  [Test]
  public void ConvertFromNullableInt32ToString_WithNull ()
  {
    Assert.AreEqual ("", _provider.Convert (_nullableInt32, _string, null));
  }

  [Test]
  public void ConvertFromStringToNullableInt32_WithEmpty ()
  {
    Assert.AreEqual (null, _provider.Convert (_string, _nullableInt32, ""));
  }

  [Test]
  public void ConvertFromInt32ToNullableInt32WithNull ()
  {
    Assert.AreEqual (null, _provider.Convert (_int32, _nullableInt32, null));
  }

  [Test]
  public void ConvertFromInt32ToNullableInt32WithDBNull ()
  {
    Assert.AreEqual (null, _provider.Convert (_int32, _nullableInt32, DBNull.Value));
  }

  [Test]
  [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = "Argument value has type System.DBNull when type System.Int32 was expected." 
      + "\r\nParameter name: value")]
  public void ConvertFromInt32ToInt32WithDBNull ()
  {
    _provider.Convert (_int32, _int32, DBNull.Value);
  }

  [Test]
  [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = "Argument value has type System.String when type System.Int32 was expected." 
      + "\r\nParameter name: value")]
  public void Convert_WithInvalidValue ()
  {
    _provider.Convert (_int32, _nullableInt32, "pwned!");
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Cannot convert value 'null' to non-nullable type 'System.Int32'.")]
  public void Convert_WithInvalidNullValue ()
  {
    _provider.Convert (_int32, _int32, null);
  }

  [Test]
  public void Convert_WithValidNullValue ()
  {
    _provider.Convert (_int32, _nullableInt32, null);
  }

  [Test]
  public void ConvertFromInt32ToString()
  {
    Assert.AreEqual ("1", _provider.Convert (_int32, _string, 1));
  }

  [Test]
  public void ConvertFromStringToInt32()
  {
    Assert.AreEqual (1, _provider.Convert (_string, _int32, "1"));
  }

  [Test]
  public void ConvertFromInt32ToStringWithNull()
  {
    Assert.AreEqual (string.Empty, _provider.Convert (_int32, _string, null));
  }

  [Test]
  public void ConvertFromInt32ToStringWithDBNull()
  {
    Assert.AreEqual (string.Empty, _provider.Convert (_int32, _string, DBNull.Value));
  }

  [Test]
  [ExpectedException (typeof (ParseException))]
  public void ConvertFromStringToInt32WithEmpty()
  {
    _provider.Convert (_string, _int32, string.Empty);
  }


  [Test]
  public void ConvertFromInt32EnumToInt32Enum()
  {
    Assert.AreEqual (Int32Enum.ValueA, _provider.Convert (_int32Enum, _int32Enum, Int32Enum.ValueA));
  }

  [Test]
  public void ConvertFromInt32EnumToInt32()
  {
    Assert.AreEqual (0, _provider.Convert (_int32Enum, _int32, Int32Enum.ValueA));
    Assert.AreEqual (1, _provider.Convert (_int32Enum, _int32, Int32Enum.ValueB));
  }

  [Test]
  public void ConvertFromInt32ToInt32Enum()
  {
    Assert.AreEqual (Int32Enum.ValueA, _provider.Convert (_int32, _int32Enum, 0));
    Assert.AreEqual (Int32Enum.ValueB, _provider.Convert (_int32, _int32Enum, 1));
  }

  [Test]
  public void ConvertFromInt32EnumToString()
  {
    Assert.AreEqual ("ValueA", _provider.Convert (_int32Enum, _string, Int32Enum.ValueA));
    Assert.AreEqual ("ValueB", _provider.Convert (_int32Enum, _string, Int32Enum.ValueB));
  }

  [Test]
  public void ConvertFromStringToInt32Enum()
  {
    Assert.AreEqual (Int32Enum.ValueA, _provider.Convert (_string, _int32Enum, "ValueA"));
    Assert.AreEqual (Int32Enum.ValueB, _provider.Convert (_string, _int32Enum, "ValueB"));
  }

  [Test]
  public void ConvertFromInt32EnumToStringWithNull()
  {
    Assert.AreEqual (string.Empty, _provider.Convert (_int32Enum, _string, null));
  }

  [Test]
  public void ConvertFromInt32EnumToStringWithDBNull()
  {
    Assert.AreEqual(string.Empty, _provider.Convert (_int32Enum, _string, DBNull.Value));
  }

  [Test]
  [ExpectedException (typeof (FormatException), ExpectedMessage = " is not a valid value for Int32Enum.")]
  public void ConvertFromStringToInt32EnumWithEmpty ()
  {
    _provider.Convert (_string, _int32Enum, string.Empty);
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromInt32ToInt32EnumWithNull()
  {
    _provider.Convert (_int32, _int32Enum, null);
  }

  [Test]
  public void ConvertFromNullableInt32EnumToNullableInt32Enum ()
  {
    Assert.AreEqual (Int32Enum.ValueA, _provider.Convert (_nullableInt32Enum, _nullableInt32Enum, Int32Enum.ValueA));
  }

  [Test]
  public void ConvertFromNullableInt32EnumToNullableInt32 ()
  {
    Assert.AreEqual (0, _provider.Convert (_nullableInt32Enum, _nullableInt32, Int32Enum.ValueA));
    Assert.AreEqual (1, _provider.Convert (_nullableInt32Enum, _nullableInt32, Int32Enum.ValueB));
    Assert.IsNull (_provider.Convert (_nullableInt32Enum, _nullableInt32, null));
  }

  [Test]
  public void ConvertFromNullableInt32ToNullableInt32Enum ()
  {
    Assert.AreEqual (Int32Enum.ValueA, _provider.Convert (_nullableInt32, _nullableInt32Enum, 0));
    Assert.AreEqual (Int32Enum.ValueB, _provider.Convert (_nullableInt32, _nullableInt32Enum, 1));
    Assert.IsNull (_provider.Convert (_nullableInt32, _nullableInt32Enum, null));
  }

  [Test]
  public void ConvertFromNullableInt32EnumToString ()
  {
    Assert.AreEqual ("ValueA", _provider.Convert (_nullableInt32Enum, _string, Int32Enum.ValueA));
    Assert.AreEqual ("ValueB", _provider.Convert (_nullableInt32Enum, _string, Int32Enum.ValueB));
    Assert.AreEqual (string.Empty, _provider.Convert (_nullableInt32Enum, _string, null));
  }

  [Test]
  public void ConvertFromStringToNullableInt32Enum ()
  {
    Assert.AreEqual (Int32Enum.ValueA, _provider.Convert (_string, _nullableInt32Enum, "ValueA"));
    Assert.AreEqual (Int32Enum.ValueB, _provider.Convert (_string, _nullableInt32Enum, "ValueB"));
    Assert.IsNull (_provider.Convert (_string, _nullableInt32Enum, null));
    Assert.IsNull (_provider.Convert (_string, _nullableInt32Enum, string.Empty));
  }

  [Test]
  public void ConvertFromNullableInt32EnumToStringWithDBNull ()
  {
    Assert.AreEqual (string.Empty, _provider.Convert (_nullableInt32Enum, _string, DBNull.Value));
  }


  [Test]
  public void ConvertFromStringToString()
  {
    string value = "Hello World!";
    Assert.AreEqual (value, _provider.Convert (_string, _string, value));
  }

  [Test]
  public void ConvertFromStringArrayToString ()
  {
    string[] value = new string[] { "Hello", "World", "!" };
    Assert.AreEqual ("Hello,World,!", _provider.Convert (_stringArray, _string, value));
  }

  [Test]
  public void ConvertFromStringToStringArray ()
  {
    string value = "Hello,World,!";
    Assert.AreEqual (new string[] { "Hello", "World", "!"}, _provider.Convert (_string, _stringArray, value));
  }

  [Test]
  public void ConvertFromStringToStringWithNull()
  {
    Assert.AreEqual (string.Empty, _provider.Convert (_string, _string, null));
  }

  [Test]
  public void GetTypeConverterFromInt32ToInt32 ()
  {
    TypeConverterResult converterResult = _provider.GetTypeConverter (_int32, _int32);
    Assert.AreEqual (TypeConverterResult.Empty, converterResult, "TypeConverterResult is not empty.");
  }


  [Test]
  public void GetTypeConverterFromInt32ToNullableInt32 ()
  {
    TypeConverterResult converterResult = _provider.GetTypeConverter (_int32, _nullableInt32);
    Assert.AreEqual (TypeConverterResult.Empty, converterResult, "TypeConverterResult is not empty.");
  }

  [Test]
  public void GetTypeConverterFromNullableInt32ToInt32 ()
  {
    TypeConverterResult converterResult = _provider.GetTypeConverter (_nullableInt32, _int32);
    Assert.AreEqual (TypeConverterResult.Empty, converterResult, "TypeConverterResult is not empty.");
  }

  [Test]
  public void GetTypeConverterFromNullableInt32ToString ()
  {
    TypeConverterResult converterResult = _provider.GetTypeConverter (_nullableInt32, _string);
    Assert.AreNotEqual (TypeConverterResult.Empty, converterResult, "TypeConverterResult is not empty.");
    Assert.AreEqual (typeof (BidirectionalStringConverter), converterResult.TypeConverter.GetType());
  }

  [Test]
  public void GetTypeConverterFromStringToNullableInt32 ()
  {
    TypeConverterResult converterResult = _provider.GetTypeConverter (_string, _nullableInt32);
    Assert.AreNotEqual (TypeConverterResult.Empty, converterResult, "TypeConverterResult is not empty.");
    Assert.AreEqual (typeof (BidirectionalStringConverter), converterResult.TypeConverter.GetType ());
  }

  [Test]
  public void GetTypeConverterFromObjectToString ()
  {
    TypeConverterResult converterResult = _provider.GetTypeConverter (_object, _string);
    Assert.AreEqual (TypeConverterResult.Empty, converterResult, "TypeConverterResult is not empty.");
  }

  [Test]
  public void GetTypeConverterFromStringToObject ()
  {
    TypeConverterResult converterResult = _provider.GetTypeConverter (_string, _object);
    Assert.AreEqual (TypeConverterResult.Empty, converterResult, "TypeConverterResult is not empty.");
  }

  [Test]
  public void GetTypeConverterFromStringArrayToString ()
  {
    TypeConverterResult converterResult = _provider.GetTypeConverter (_stringArray, _string);
    Assert.AreNotEqual (TypeConverterResult.Empty, converterResult, "TypeConverterResult is empty.");
    Assert.AreEqual (TypeConverterType.DestinationTypeConverter, converterResult.TypeConverterType);
    Assert.AreEqual (typeof (BidirectionalStringConverter), converterResult.TypeConverter.GetType ());
  }

  [Test]
  public void GetTypeConverterFromStringToArray ()
  {
    TypeConverterResult converterResult = _provider.GetTypeConverter (_string, _stringArray);
    Assert.AreNotEqual (TypeConverterResult.Empty, converterResult, "TypeConverterResult is empty.");
    Assert.AreEqual (TypeConverterType.SourceTypeConverter, converterResult.TypeConverterType);
    Assert.AreEqual (typeof (BidirectionalStringConverter), converterResult.TypeConverter.GetType ());
  }


  [Test]
  public void GetTypeConverterForNaByte ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (byte?));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForByte ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (byte));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaInt16 ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (short?));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForInt16 ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (short));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForInt32 ()
  {
    TypeConverter converter = _provider.GetTypeConverter (_int32);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNullableInt32 ()
  {
    TypeConverter converter = _provider.GetTypeConverter (_nullableInt32);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaInt64 ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (long?));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForInt64 ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (long));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaSingle ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (float?));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForSingle ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (float));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaDouble ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (double?));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForDouble ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (double));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaDateTime ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (DateTime?));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForDateTime ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (DateTime));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNullableBoolean ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (bool?));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForBoolean ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (bool));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaGuid ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (Guid?));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaDecimal ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (decimal?));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForDecimal ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (decimal));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForGuid ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (Guid));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterByAttributeForInt32()
  {
    TypeConverter converter = _provider.GetTypeConverterByAttribute (_int32);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterByAttributeForNullableInt32 ()
  {
    TypeConverter converter = _provider.GetTypeConverterByAttribute (_nullableInt32);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetBasicTypeConverterForInt32()
  {
    TypeConverter converter = _provider.GetBasicTypeConverter (_int32);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetBasicTypeConverterForNullableInt32 ()
  {
    TypeConverter converter = _provider.GetBasicTypeConverter (_nullableInt32);
    Assert.IsNull (converter);
  }

  [Test]
  public void GetBasicTypeConverterForInt32Enum()
  {
    TypeConverter converterFirstRun = _provider.GetBasicTypeConverter (_int32Enum);
    TypeConverter converterSecondRun = _provider.GetBasicTypeConverter (_int32Enum);
    Assert.IsNotNull (converterFirstRun, "TypeConverter from first run is null.");
    Assert.IsNotNull (converterSecondRun, "TypeConverter from second run is null.");
    Assert.AreSame (converterFirstRun, converterSecondRun);
    Assert.AreEqual (typeof (AdvancedEnumConverter), converterFirstRun.GetType());
  }

  [Test]
  public void GetTypeConverterFromCache()
  {
    NullableConverter converter = new NullableConverter(typeof (int?));
    _provider.AddTypeConverterToCache (_nullableInt32, converter);
    Assert.AreSame (converter, _provider.GetTypeConverterFromCache (_nullableInt32));
  }

  [Test]
  public void HasTypeInCache()
  {
    NullableConverter converter = new NullableConverter (typeof (int?));
    _provider.AddTypeConverterToCache (_nullableInt32, converter);
    Assert.IsTrue (_provider.HasTypeInCache (_nullableInt32));
  }

  [Test]
  public void AddTypeConverter()
  {
    NullableConverter converter = new NullableConverter(typeof (Guid?));
    Assert.IsNull (_provider.GetTypeConverter (_guid));
    _provider.AddTypeConverter (_guid, converter);
    Assert.AreSame (converter, _provider.GetTypeConverter (_guid));
  }

  [Test]
  public void RemoveTypeConverter()
  {
    NullableConverter converter = new NullableConverter (typeof (Guid?));
    _provider.AddTypeConverter (_guid, converter);
    Assert.AreSame (converter, _provider.GetTypeConverter (_guid));
    _provider.RemoveTypeConverter (_guid);
    Assert.IsNull (_provider.GetTypeConverter (_guid));
  }
}

}
