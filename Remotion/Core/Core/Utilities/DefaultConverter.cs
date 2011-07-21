// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.ComponentModel;

namespace Remotion.Utilities
{
  /// <summary>
  /// The <see cref="DefaultConverter"/> provides a default implementation of <see cref="TypeConverter"/> that does not actually modify the converted 
  /// values and can only convert from and to a given <see cref="Type"/>. It is, therefore, de-facto a no-op implementation of 
  /// <see cref="TypeConverter"/>. It also supports converting from the underlying type to a nullable value type.
  /// </summary>
  public class DefaultConverter : TypeConverter
  {
    private readonly Type _type;
    private readonly bool _isNullableType;
    private readonly Type _underlyingType;

    public DefaultConverter (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      _type = type;
      _isNullableType = NullableTypeUtility.IsNullableType (type);
      _underlyingType = Nullable.GetUnderlyingType (type) ?? type;
    }

    public Type Type
    {
      get { return _type; }
    }
    
    public bool IsNullableType
    {
      get { return _isNullableType; }
    }

    public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);

      return _type == sourceType || _underlyingType == sourceType;
    }

    public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      return destinationType == _type || Nullable.GetUnderlyingType (destinationType) == _type;
    }

    public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
      // ReSharper disable ConditionIsAlwaysTrueOrFalse
      // ReSharper disable HeuristicUnreachableCode
      if (value == null)
      {
        if (!IsNullableType)
          throw new NotSupportedException (string.Format ("Null cannot be converted to type '{0}'.", _type));
        return null;
      }
          // ReSharper restore ConditionIsAlwaysTrueOrFalse
          // ReSharper restore HeuristicUnreachableCode
      else
      {
        if (!CanConvertFrom (context, value.GetType ()))
          throw new NotSupportedException (string.Format ("Value of type '{0}' cannot be connverted to type '{1}'.", value.GetType(), _type));
          
        return value;
      }
    }

    public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      if (!CanConvertTo (destinationType))
        throw new NotSupportedException (string.Format ("This TypeConverter cannot convert to type '{0}'.", destinationType));
      // TODO Review 4167: Check if IsValid (value); if not, throw NotSupportedException ("The given value cannot be converted by this TypeConverter.");

      return value;
    }

    public override bool IsValid (ITypeDescriptorContext context, object value)
    {
      // TODO Review 4167: Implement this value: if value == null return IsNullable, otherwise, return CanConvertFrom (value.GetType())
      return base.IsValid (context, value);
    }
  }
}