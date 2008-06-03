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
using System.Globalization;

namespace Remotion.Utilities
{
  /// <summary> Specialization of <see cref="TypeConverter"/> for conversions from and to <see cref="Enum"/> types. </summary>
  public class AdvancedEnumConverter: EnumConverter
  {
    private Type _underlyingEnumType;
    private bool _isNullable;

    public AdvancedEnumConverter (Type enumType)
        : base (Nullable.GetUnderlyingType (ArgumentUtility.CheckNotNull ("enumType", enumType)) ?? enumType)
    {
      _underlyingEnumType = Enum.GetUnderlyingType (EnumType);
      _isNullable = EnumType != enumType;
    }

    /// <summary> Test: Can convert from <paramref name="sourceType"/> to <see cref="String"/>? </summary>
    /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
    /// <param name="sourceType"> The <see cref="Type"/> of the value to be converted into an <see cref="Enum"/> type. </param>
    /// <returns> <see langword="true"/> if the conversion is supported. </returns>
    public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);

      if (sourceType == _underlyingEnumType)
        return true;
  
      if (_isNullable && Nullable.GetUnderlyingType (sourceType) == _underlyingEnumType)
        return true;

      return base.CanConvertFrom (context, sourceType);
    }

    /// <summary> Test: Can convert from <see cref="String"/> to <paramref name="destinationType"/>? </summary>
    /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
    /// <param name="destinationType"> The <see cref="Type"/> to convert an <see cref="Enum"/> value to. </param>
    /// <returns> <see langword="true"/> if the conversion is supported. </returns>
    public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      if (!_isNullable && destinationType == _underlyingEnumType)
        return true;

      if (Nullable.GetUnderlyingType (destinationType) == _underlyingEnumType)
        return true;

      return base.CanConvertFrom (context, destinationType);
    }

    /// <summary> Converts <paramref name="value"/> into an <see cref="Enum"/> value. </summary>
    /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
    /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
    /// <param name="value"> The source value. </param>
    /// <returns> An <see cref="Enum"/> value.  </returns>
    /// <exception cref="NotSupportedException"> The conversion could not be performed. </exception>
    public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (_isNullable && (value == null || (value is string) && string.IsNullOrEmpty ((string) value)))
        return null;

      if (!(value is string))
      {
        if (value != null && _underlyingEnumType == value.GetType())
        {
          if (!Enum.IsDefined (EnumType, value))
            throw new ArgumentOutOfRangeException (string.Format ("The value {0} is not supported for enumeration '{1}'.", value, EnumType.FullName), (Exception) null);

          return Enum.ToObject (EnumType, value);
        }
      }

      return base.ConvertFrom (context, culture, value);
    }

    /// <summary> Convertes an <see cref="Enum"/> value into the <paramref name="destinationType"/>. </summary>
    /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
    /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
    /// <param name="value"> The <see cref="Enum"/> value to be converted. </param>
    /// <param name="destinationType"> The destination <see cref="Type"/>. Must not be <see langword="null"/>. </param>
    /// <returns> An <see cref="Object"/> that represents the converted value. </returns>
    /// <exception cref="NotSupportedException"> The conversion could not be performed. </exception>
    public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      if (_isNullable && value == null)
        return (destinationType == typeof (string)) ? string.Empty : null;
      
      bool isMatchingDestinationType = !_isNullable && destinationType == _underlyingEnumType;
      bool isMatchingNullableDestinationType = Nullable.GetUnderlyingType (destinationType) == _underlyingEnumType;
      
      if (value is Enum && (isMatchingDestinationType || isMatchingNullableDestinationType))
        return Convert.ChangeType (value, _underlyingEnumType, culture);
      
      return base.ConvertTo (context, culture, value, destinationType);
    }
  }
}
