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
  /// The <see cref="DefaultConverter"/> provides default type conversions.
  /// </summary>
  public class DefaultConverter : TypeConverter
  {
    private readonly Type _type;
    private readonly bool _isNullableType;

    public DefaultConverter (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      _type = type;
      _isNullableType = NullableTypeUtility.IsNullableType (type);
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
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);
      
      return _type.IsAssignableFrom (sourceType);
    }

    public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      return destinationType.IsAssignableFrom (_type);
    }

    public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
      // ReSharper disable ConditionIsAlwaysTrueOrFalse
      if (value == null)
      // ReSharper restore ConditionIsAlwaysTrueOrFalse
      {
        if (!IsNullableType)
          throw new NotSupportedException (string.Format ("Null cannot be converted to type '{0}'", _type.Name));
        return null;
      }
      else
      {
        if (!CanConvertFrom (context, value.GetType ()))
          throw new NotSupportedException (string.Format ("Value of type '{0}' cannot be connverted to type '{1}'.", value.GetType().Name, _type.Name));
          
        return value;
      }
    }

    public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
      // ReSharper disable ConditionIsAlwaysTrueOrFalse
      if (value == null)
      // ReSharper restore ConditionIsAlwaysTrueOrFalse
      {
        if (!IsNullableType)
          throw new NotSupportedException ("Null cannot be converted by this TypeConverter.");
        return null;
      }
      else
      {
        if (!_type.IsAssignableFrom (value.GetType ()))
          throw new NotSupportedException (string.Format ("Values of type '{0}' cannot be converted by this TypeConverter.", value.GetType().Name));

        if (!CanConvertTo (context, destinationType))
          throw new NotSupportedException (string.Format ("Cannot convert type '{0}' to type '{1}.", _type, destinationType));

        return value;
      }
    }
  }
}