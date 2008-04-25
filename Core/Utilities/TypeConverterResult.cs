using System;
using System.ComponentModel;

namespace Remotion.Utilities
{
  /// <summary>
  /// The <see cref="TypeConverterResult"/> structure encapsulates the reult of the <see cref="TypeConversionProvider.GetTypeConverter(Type,Type)"/>
  /// method.
  /// </summary>
  public struct TypeConverterResult
  {
    public static readonly TypeConverterResult Empty = new TypeConverterResult();

    public readonly TypeConverterType TypeConverterType;
    public readonly TypeConverter TypeConverter;

    public TypeConverterResult (TypeConverterType typeConverterType, TypeConverter typeConverter)
    {
      TypeConverterType = typeConverterType;
      TypeConverter = typeConverter;
    }
  }
}