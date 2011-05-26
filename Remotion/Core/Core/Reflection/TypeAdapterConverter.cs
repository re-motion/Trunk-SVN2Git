﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Provides support for converting instances of type <see cref="TypeAdapter"/> to and from instances of type <see cref="Type"/>.
  /// </summary>
  public class TypeAdapterConverter : TypeConverter
  {
    /// <summary>
    /// Returns whether this converter can convert an object of the given type to a <see cref="TypeAdapter"/>, using the specified context. 
    /// This method returns <see langword="true" /> if <paramref name="sourceType"/> is <see cref="Type"/>; <see langword="false" /> otherwise.
    /// </summary>
    /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. Ignored by this 
    /// implementation.</param>
    /// <param name="sourceType">A <see cref="System.Type"/> that represents the type you want to convert from.</param>
    /// <returns>
    /// <see langword="true" /> if this converter can perform the conversion; otherwise, <see langword="false" />.
    /// </returns>
    public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);
      return sourceType == typeof (Type);
    }

    /// <summary>
    /// Returns whether this converter can convert a <see cref="TypeAdapter"/> to an object of the given type, using the specified context. This
    /// method returns <see langword="true" /> if <paramref name="destinationType"/> is <see cref="Type"/>; <see langword="false" /> otherwise.
    /// </summary>
    /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. Ignored by this 
    /// implementation.</param>
    /// <param name="destinationType">A <see cref="System.Type"/> that represents the type you want to convert to.</param>
    /// <returns>
    /// <see langword="true" /> if this converter can perform the conversion; otherwise, <see langword="false" />.
    /// </returns>
    public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);
      return destinationType == typeof (Type);
    }

    /// <summary>
    /// Converts the given object to a <see cref="TypeAdapter"/>, using the specified context and culture information.
    /// </summary>
    /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. Ignored by this
    /// implementation</param>
    /// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to use as the current culture. Ignored by this implementation</param>
    /// <param name="value">The <see cref="System.Object"/> to convert. Must be a <see cref="Type"/> value.</param>
    /// <returns>
    /// An <see cref="System.Object"/> that represents the converted value.
    /// </returns>
    /// <exception cref="System.NotSupportedException">
    /// The conversion cannot be performed.
    /// </exception>
    public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (value == null)
        return null;

      var type = value as Type;
      if (type == null)
      {
        var message = string.Format ("Cannot convert value from type '{0}' to type '{1}'.", value.GetType (), typeof (TypeAdapter));
        throw new NotSupportedException (message);
      }

      return TypeAdapter.Create (type);
    }

    /// <summary>
    /// Converts the given value object to a <see cref="Type"/>, using the specified context and culture information.
    /// </summary>
    /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. Ignored
    /// by this implementation.</param>
    /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed. Ignored
    /// by this implementation.</param>
    /// <param name="value">The <see cref="T:System.Object"/> to convert. Must be an instance of <see cref="TypeAdapter"/>.</param>
    /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to. Must be
    /// <see cref="Type"/>.</param>
    /// <returns>
    /// An <see cref="T:System.Object"/> that represents the converted value.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="destinationType"/> parameter is null.
    /// </exception>
    /// <exception cref="T:System.NotSupportedException">
    /// The conversion cannot be performed.
    /// </exception>
    public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      if (destinationType != typeof (Type))
      {
        var message = string.Format (
            "Cannot convert values to type '{0}'. This converter only supports converting to type '{1}'.",
            destinationType,
            typeof (Type));
        throw new NotSupportedException (message);
      }

      if (value == null)
        return null;

      var typeAdapter = value as TypeAdapter;
      if (typeAdapter == null)
      {
        var message = string.Format (
            "Cannot convert values of type '{0}' to type '{1}'. This converter only supports values of type '{2}'.",
            value.GetType (),
            destinationType,
            typeof (TypeAdapter));
        throw new NotSupportedException (message);
      }

      return typeAdapter.Type;
    }
  }
}