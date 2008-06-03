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
using Remotion.Utilities;

namespace Remotion.Web.UI.Design
{

public class StringArrayConverter : TypeConverter
{
  /// <summary> Test: Can convert from the <paramref name="sourceType"/> to a <see cref="String"/> array? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="sourceType"> 
  ///   The <see cref="Type"/> of the value to be converted into an <see cref="String"/>.
  /// </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    if (sourceType == typeof (string))
      return true;
    return false;  
  }

  /// <summary> Test: Can convert a <see cref="String"/> array to the <paramref name="destinationType"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="destinationType"> 
  ///   The <see cref="Type"/> of the value to be converted into a <see cref="String"/> Array.
  /// </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
  {
    if (destinationType == typeof (string))
      return true;
    return false;  
  }

  /// <summary> Converts <paramref name="value"/> into a <see cref="String"/> array. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value">  The source value. </param>
  /// <returns>
  ///    A <see cref="String"/> array or <see langword="null"/> if the conversion failed. 
  /// </returns>
  public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
  {
    if (value is string)
      return ((string)value).Split (new char[] {','});
    return null;
  }

  /// <summary>
  ///   Convertes a <see cref="String"/> array into the <paramref name="destinationType"/>.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value"> The <see cref="String"/> array to be converted. </param>
  /// <param name="destinationType"> The destination <see cref="Type"/>. </param>
  /// <returns> An <see cref="Object"/> that represents the converted value. </returns>
  public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
  {
    if (value is string[])
      return StringUtility.ConcatWithSeparator ((string[]) value, ",");
    return string.Empty;
  }
}
}
