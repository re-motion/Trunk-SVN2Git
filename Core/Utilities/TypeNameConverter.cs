using System;
using System.ComponentModel;
using System.Globalization;

namespace Remotion.Utilities
{

  public class TypeNameConverter : TypeConverter
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public TypeNameConverter ()
    {
    }

    // methods and properties

    public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof (string);
    }

    public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
    {
      return destinationType == typeof (string);
    }

    public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (value is string)
      {
        string stringValue = (string) value;
        if (stringValue.Length == 0)
          return null;
        else
          return TypeUtility.GetType (stringValue, true, false);
      }
      if (value == null)
        return null;
      return null;
    }

    public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      if (value == null)
        return string.Empty;

      if (value is Type && destinationType == typeof (string))
        return TypeUtility.GetPartialAssemblyQualifiedName ((Type) value);

      return base.ConvertTo (context, culture, value, destinationType);
    }
  }
}