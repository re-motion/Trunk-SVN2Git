// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Remotion.Collections;
using Remotion.ExtensibleEnums;
using Remotion.Reflection.TypeDiscovery;

namespace Remotion.Utilities
{
  /// <summary> 
  ///   Provides functionality to get the <see cref="TypeConverter"/> for a <see cref="Type"/> and to convert a value
  ///   from a source <see cref="Type"/> into a destination <see cref="Type"/>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Use the <see cref="Create"/> method if you need to create a new instance of the
  ///     <see cref="TypeConversionProvider"/> type.
  ///   </para><para>
  ///     Conversion is possible under the following conditions:
  ///   </para>
  ///   <list type="bullet">
  ///     <item>
  ///       A type has a <see cref="TypeConverter"/> applied through the <see cref="TypeConverterAttribute"/> that
  ///       supports the conversion. 
  ///     </item>
  ///     <item>
  ///       For <see cref="Enum"/> types into the <see cref="String"/> value or the underlying numeric 
  ///       <see cref="Type"/>.
  ///     </item>
  ///     <item>
  ///       For types without a <see cref="TypeConverter"/>, the <see cref="TypeConversionProvider"/> try to use the 
  ///       <see cref="BidirectionalStringConverter"/>. See the documentation of the string converter for details on the
  ///       supported types.
  ///     </item>
  ///   </list>
  /// </remarks>
  public class TypeConversionProvider
  {
    private static readonly LockingDataStoreDecorator<Type, TypeConverter> s_typeConverters = DataStoreFactory.CreateWithLocking<Type, TypeConverter>();

    private static readonly DoubleCheckedLockingContainer<TypeConversionProvider> s_current =
        new DoubleCheckedLockingContainer<TypeConversionProvider> (Create);

    /// <summary> Creates a new instace of the <see cref="TypeConversionProvider"/> type. </summary>
    /// <returns> An instance of the <see cref="TypeConversionProvider"/> type. </returns>
    public static TypeConversionProvider Create ()
    {
      return new TypeConversionProvider();
    }

    /// <summary> Gets the current <see cref="TypeConversionProvider"/>. </summary>
    /// <value> An instance of the <see cref="TypeConversionProvider"/> type. </value>
    public static TypeConversionProvider Current
    {
      get { return s_current.Value; }
    }

    /// <summary> Sets the current <see cref="TypeConversionProvider"/>. </summary>
    /// <param name="provider"> A <see cref="TypeConversionProvider"/>. Must not be <see langword="null"/>. </param>
    public static void SetCurrent (TypeConversionProvider provider)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      s_current.Value = provider;
    }

    private readonly Dictionary<Type, TypeConverter> _additionalTypeConverters = new Dictionary<Type, TypeConverter>();
    private readonly BidirectionalStringConverter _stringConverter = new BidirectionalStringConverter();

    protected TypeConversionProvider ()
    {
    }

    /// <summary> 
    ///   Gets the <see cref="TypeConverter"/> that is able to convert an instance of the <paramref name="sourceType"/> 
    ///   <see cref="Type"/> into an instance of the <paramref name="destinationType"/> <see cref="Type"/>.
    /// </summary>
    /// <param name="sourceType"> 
    ///   The source <see cref="Type"/> of the value. Must not be <see langword="null"/>. 
    /// </param>
    /// <param name="destinationType"> 
    ///   The destination <see cref="Type"/> of the value. Must not be <see langword="null"/>. 
    /// </param>
    /// <returns> 
    ///   A <see cref="TypeConverterResult"/> or or <see cref="TypeConverterResult.Empty"/>if no matching <see cref="TypeConverter"/> can be found.
    /// </returns>
    /// <remarks> 
    ///   You can identify whether you must use the <see cref="TypeConverter.ConvertTo(object,Type)"/> or the 
    ///   <see cref="TypeConverter.ConvertFrom(object)"/> method by testing the returned <see cref="TypeConverter"/>'s
    ///   <see cref="TypeConverter.CanConvertTo(Type)"/> and <see cref="TypeConverter.CanConvertFrom(Type)"/> methods.
    /// </remarks>
    public virtual TypeConverterResult GetTypeConverter (Type sourceType, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      TypeConverterResult additionalTypeConverterResult = GetAdditionalTypeConverter (sourceType, destinationType);
      if (!additionalTypeConverterResult.Equals (TypeConverterResult.Empty))
        return additionalTypeConverterResult;

      TypeConverterResult basicTypeConverterResult = GetBasicTypeConverter (sourceType, destinationType);
      if (!basicTypeConverterResult.Equals (TypeConverterResult.Empty))
        return basicTypeConverterResult;

      TypeConverterResult stringConverterResult = GetStringConverter (sourceType, destinationType);
      if (!stringConverterResult.Equals (TypeConverterResult.Empty))
        return stringConverterResult;

      return TypeConverterResult.Empty;
    }

    /// <summary> 
    ///   Gets the <see cref="TypeConverter"/> that is associated with the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type"> 
    ///   The <see cref="Type"/> to get the <see cref="TypeConverter"/> for. Must not be <see langword="null"/>.
    /// </param>
    /// <returns>
    ///   A <see cref="TypeConverter"/> or <see langword="null"/> of no <see cref="TypeConverter"/> can be found.
    /// </returns>
    public virtual TypeConverter GetTypeConverter (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      TypeConverter converter = GetAdditionalTypeConverter (type);
      if (converter != null)
        return converter;

      converter = GetBasicTypeConverter (type);
      if (converter != null)
        return converter;

      if (type == typeof (string))
        return _stringConverter;

      return null;
    }

    /// <summary> 
    ///   Registers the <paramref name="converter"/> for the <paramref name="type"/>, overriding the default settings. 
    /// </summary>
    /// <param name="type"> 
    ///   The <see cref="Type"/> for which the <paramref name="converter"/> should be used. 
    ///   Must not be <see langword="null"/>.
    /// </param>
    /// <param name="converter"> The <see cref="TypeConverter"/> to register. Must not be <see langword="null"/>. </param>
    public void AddTypeConverter (Type type, TypeConverter converter)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("converter", converter);
      _additionalTypeConverters[type] = converter;
    }

    /// <summary>
    ///   Unregisters a special <see cref="TypeConverter"/> previously registered by using <see cref="AddTypeConverter"/>.
    /// </summary>
    /// <param name="type">
    ///   The <see cref="Type"/> whose special <see cref="TypeConverter"/> should be removed. 
    ///   Must not be <see langword="null"/>.
    /// </param>
    /// <remarks> If no <see cref="TypeConverter"/> has been registered, the method has no effect. </remarks>
    public void RemoveTypeConverter (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      _additionalTypeConverters.Remove (type);
    }

    /// <summary> 
    ///   Test whether the <see cref="TypeConversionProvider"/> object can convert an object of <see cref="Type"/> 
    ///   <paramref name="sourceType"/> into an object of <see cref="Type"/> <paramref name="destinationType"/>
    ///   by using the <see cref="Convert(Type,Type,object)"/> method.
    /// </summary>
    /// <param name="sourceType"> 
    ///   The source <see cref="Type"/> of the value. Must not be <see langword="null"/>. 
    /// </param>
    /// <param name="destinationType"> 
    ///   The destination <see cref="Type"/> of the value. Must not be <see langword="null"/>. 
    /// </param>
    /// <returns> <see langword="true"/> if a conversion is possible. </returns>
    public virtual bool CanConvert (Type sourceType, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      if (sourceType == typeof (DBNull))
        return NullableTypeUtility.IsNullableType (destinationType);
      
      if (AreUnderlyingTypesEqual (destinationType, sourceType))
        return true;

      TypeConverterResult typeConverterResult = GetTypeConverter (sourceType, destinationType);
      return !typeConverterResult.Equals (TypeConverterResult.Empty);
    }

    /// <summary> Convertes the <paramref name="value"/> into the <paramref name="destinationType"/>. </summary>
    /// <param name="sourceType"> 
    ///   The source <see cref="Type"/> of the <paramref name="value"/>. Must not be <see langword="null"/>. 
    /// </param>
    /// <param name="destinationType"> 
    ///   The destination <see cref="Type"/> of the <paramref name="value"/>. Must not be <see langword="null"/>. 
    /// </param>
    /// <param name="value"> The value to be converted. Must not be <see langword="null"/>. </param>
    /// <returns> An <see cref="Object"/> that represents the converted <paramref name="value"/>. </returns>
    public object Convert (Type sourceType, Type destinationType, object value)
    {
      return Convert (null, null, sourceType, destinationType, value);
    }

    /// <summary> Convertes the <paramref name="value"/> into the <paramref name="destinationType"/>. </summary>
    /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
    /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
    /// <param name="sourceType"> 
    ///   The source <see cref="Type"/> of the <paramref name="value"/>. Must not be <see langword="null"/>. 
    /// </param>
    /// <param name="destinationType"> 
    ///   The destination <see cref="Type"/> of the <paramref name="value"/>. Must not be <see langword="null"/>. 
    /// </param>
    /// <param name="value"> The <see cref="Object"/> to be converted.</param>
    /// <returns> An <see cref="Object"/> that represents the converted <paramref name="value"/>. </returns>
    public virtual object Convert (ITypeDescriptorContext context, CultureInfo culture, Type sourceType, Type destinationType, object value)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      bool isNullableDestinationType = NullableTypeUtility.IsNullableType (destinationType);
      if (value == DBNull.Value && isNullableDestinationType)
        return GetValueOrEmptyString (destinationType, null);

      if (value == null && !isNullableDestinationType)
        throw new NotSupportedException (string.Format ("Cannot convert value 'null' to non-nullable type '{0}'.", destinationType));
      else if (value != null && !sourceType.IsInstanceOfType (value))
        throw ArgumentUtility.CreateArgumentTypeException ("value", value.GetType(), sourceType);
      
      if (AreUnderlyingTypesEqual (sourceType, destinationType))
        return GetValueOrEmptyString (destinationType, value);

      TypeConverterResult typeConverterResult = GetTypeConverter (sourceType, destinationType);
      if (!typeConverterResult.Equals (TypeConverterResult.Empty))
      {
        switch (typeConverterResult.TypeConverterType)
        {
          case TypeConverterType.SourceTypeConverter:
            return typeConverterResult.TypeConverter.ConvertTo (context, culture, value, destinationType);
          default:
            Assertion.IsTrue (typeConverterResult.TypeConverterType == TypeConverterType.DestinationTypeConverter);
            return typeConverterResult.TypeConverter.ConvertFrom (context, culture, value);
        }
      }

      throw new NotSupportedException (string.Format ("Cannot convert value '{0}' to type '{1}'.", value, destinationType));
    }

    private object GetValueOrEmptyString (Type destinationType, object value)
    {
      if (destinationType == typeof (string) && value == null)
        return string.Empty;
      return value;
    }

    protected TypeConverterResult GetAdditionalTypeConverter (Type sourceType, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      TypeConverter sourceTypeConverter = GetAdditionalTypeConverter (sourceType);
      if (sourceTypeConverter != null && sourceTypeConverter.CanConvertTo (destinationType))
        return new TypeConverterResult (TypeConverterType.SourceTypeConverter, sourceTypeConverter);

      TypeConverter destinationTypeConverter = GetAdditionalTypeConverter (destinationType);
      if (destinationTypeConverter != null && destinationTypeConverter.CanConvertFrom (sourceType))
        return new TypeConverterResult (TypeConverterType.DestinationTypeConverter, destinationTypeConverter);

      return TypeConverterResult.Empty;
    }

    protected TypeConverter GetAdditionalTypeConverter (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      TypeConverter typeConverter;
      if (_additionalTypeConverters.TryGetValue (type, out typeConverter))
        return typeConverter;

      return null;
    }

    protected TypeConverterResult GetBasicTypeConverter (Type sourceType, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      TypeConverter sourceTypeConverter = GetBasicTypeConverter (sourceType);
      if (sourceTypeConverter != null && sourceTypeConverter.CanConvertTo (destinationType))
        return new TypeConverterResult (TypeConverterType.SourceTypeConverter, sourceTypeConverter);

      TypeConverter destinationTypeConverter = GetBasicTypeConverter (destinationType);
      if (destinationTypeConverter != null && destinationTypeConverter.CanConvertFrom (sourceType))
        return new TypeConverterResult (TypeConverterType.DestinationTypeConverter, destinationTypeConverter);

      return TypeConverterResult.Empty;
    }

    protected TypeConverter GetBasicTypeConverter (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      TypeConverter converter = GetTypeConverterFromCache (type);
      if (converter == null && !HasTypeInCache (type))
      {
        converter = GetTypeConverterByAttribute (type);

        if (converter == null && (Nullable.GetUnderlyingType (type) ?? type).IsEnum)
          converter = new AdvancedEnumConverter (type);

        if (converter == null && ExtensibleEnumUtility.IsExtensibleEnumType (type))
          converter = new ExtensibleEnumConverter (type);

        AddTypeConverterToCache (type, converter);
      }
      return converter;
    }

    protected TypeConverterResult GetStringConverter (Type sourceType, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);
      
      if (sourceType == typeof (string) && _stringConverter.CanConvertTo (destinationType))
        return new TypeConverterResult (TypeConverterType.SourceTypeConverter, _stringConverter);

      if (destinationType == typeof (string) && _stringConverter.CanConvertFrom (sourceType))
        return new TypeConverterResult (TypeConverterType.DestinationTypeConverter, _stringConverter);

      return TypeConverterResult.Empty;
    }

    protected TypeConverter GetTypeConverterByAttribute (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      TypeConverterAttribute typeConverter = AttributeUtility.GetCustomAttribute<TypeConverterAttribute> (type, true);
      if (typeConverter != null)
      {
        Type typeConverterType = ContextAwareTypeDiscoveryUtility.GetType (typeConverter.ConverterTypeName, true);
        return (TypeConverter) Activator.CreateInstance (typeConverterType);
      }
      return null;
    }

    protected void AddTypeConverterToCache (Type type, TypeConverter converter)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      s_typeConverters[type] = converter;
    }

    protected TypeConverter GetTypeConverterFromCache (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
     
      TypeConverter typeConverter;
      if (s_typeConverters.TryGetValue (type, out typeConverter))
        return typeConverter;

      return null;
    }

    protected bool HasTypeInCache (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return s_typeConverters.ContainsKey (type);
    }

    private bool AreUnderlyingTypesEqual (Type destinationType, Type sourceType)
    {
      if (sourceType == destinationType)
        return true;

      if ((Nullable.GetUnderlyingType (sourceType) ?? sourceType) == (Nullable.GetUnderlyingType (destinationType) ?? destinationType))
        return true;

      return false;
    }
  }
}
