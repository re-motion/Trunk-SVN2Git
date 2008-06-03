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

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [DBTable ("TableWithAllDataTypes")]
  [FirstStorageGroupAttribute]
  [Instantiable]
  public abstract class ClassWithAllDataTypes : DomainObject
  {
    public enum EnumType
    {
      Value0 = 0,
      Value1 = 1,
      Value2 = 2
    }

    public static ClassWithAllDataTypes NewObject()
    {
      return NewObject<ClassWithAllDataTypes>().With();
    }

    [DBColumn ("Boolean")]
    public abstract bool BooleanProperty { get; set; }

    [DBColumn ("Byte")]
    public abstract byte ByteProperty { get; set; }

    [DBColumn ("Date")]
    public abstract DateTime DateProperty { get; set; }

    [DBColumn ("DateTime")]
    public abstract DateTime DateTimeProperty { get; set; }

    [DBColumn ("Decimal")]
    public abstract decimal DecimalProperty { get; set; }

    [DBColumn ("Double")]
    public abstract double DoubleProperty { get; set; }

    [DBColumn ("Enum")]
    public abstract EnumType EnumProperty { get; set; }

    [DBColumn ("Guid")]
    public abstract Guid GuidProperty { get; set; }

    [DBColumn ("Int16")]
    public abstract short Int16Property { get; set; }

    [DBColumn ("Int32")]
    public abstract int Int32Property { get; set; }

    [DBColumn ("Int64")]
    public abstract long Int64Property { get; set; }

    [DBColumn ("Single")]
    public abstract float SingleProperty { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    [DBColumn ("String")]
    public abstract string StringProperty { get; set; }

    [StringProperty (IsNullable = false)]
    [DBColumn ("StringWithoutMaxLength")]
    public abstract string StringPropertyWithoutMaxLength { get; set; }

    [BinaryProperty (IsNullable = false)]
    [DBColumn ("Binary")]
    public abstract byte[] BinaryProperty { get; set; }

    [DBColumn ("NaBoolean")]
    public abstract bool? NaBooleanProperty { get; set; }

    [DBColumn ("NaByte")]
    public abstract byte? NaByteProperty { get; set; }

    [DBColumn ("NaDate")]
    public abstract DateTime? NaDateProperty { get; set; }

    [DBColumn ("NaDateTime")]
    public abstract DateTime? NaDateTimeProperty { get; set; }

    [DBColumn ("NaDecimal")]
    public abstract Decimal? NaDecimalProperty { get; set; }

    [DBColumn ("NaDouble")]
    public abstract double? NaDoubleProperty { get; set; }

    [DBColumn ("NaEnum")]
    public abstract EnumType? NaEnumProperty { get; set; }

    [DBColumn ("NaGuid")]
    public abstract Guid? NaGuidProperty { get; set; }

    [DBColumn ("NaInt16")]
    public abstract short? NaInt16Property { get; set; }

    [DBColumn ("NaInt32")]
    public abstract int? NaInt32Property { get; set; }

    [DBColumn ("NaInt64")]
    public abstract long? NaInt64Property { get; set; }

    [DBColumn ("NaSingle")]
    public abstract float? NaSingleProperty { get; set; }

    [StringProperty (MaximumLength = 100)]
    [DBColumn ("StringWithNullValue")]
    public abstract string StringWithNullValueProperty { get; set; }

    [DBColumn ("NaBooleanWithNullValue")]
    public abstract bool? NaBooleanWithNullValueProperty { get; set; }

    [DBColumn ("NaByteWithNullValue")]
    public abstract byte? NaByteWithNullValueProperty { get; set; }

    [DBColumn ("NaDateWithNullValue")]
    public abstract DateTime? NaDateWithNullValueProperty { get; set; }

    [DBColumn ("NaDateTimeWithNullValue")]
    public abstract DateTime? NaDateTimeWithNullValueProperty { get; set; }

    [DBColumn ("NaDecimalWithNullValue")]
    public abstract Decimal? NaDecimalWithNullValueProperty { get; set; }

    [DBColumn ("NaDoubleWithNullValue")]
    public abstract double? NaDoubleWithNullValueProperty { get; set; }

    [DBColumn ("NaEnumWithNullValue")]
    public abstract EnumType? NaEnumWithNullValueProperty { get; set; }

    [DBColumn ("NaGuidWithNullValue")]
    public abstract Guid? NaGuidWithNullValueProperty { get; set; }

    [DBColumn ("NaInt16WithNullValue")]
    public abstract short? NaInt16WithNullValueProperty { get; set; }

    [DBColumn ("NaInt32WithNullValue")]
    public abstract int? NaInt32WithNullValueProperty { get; set; }

    [DBColumn ("NaInt64WithNullValue")]
    public abstract long? NaInt64WithNullValueProperty { get; set; }

    [DBColumn ("NaSingleWithNullValue")]
    public abstract float? NaSingleWithNullValueProperty { get; set; }

    [BinaryProperty (MaximumLength = 1000000)]
    [DBColumn ("NullableBinary")]
    public abstract byte[] NullableBinaryProperty { get; set; }
  }
}
