// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.ScalePrecisionValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Linq.Expressions;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Validators
{
  /// <summary>
  /// Allows a decimal to be validated for scale and precision.
  /// Scale would be the number of digits to the right of the decimal point.
  /// Precision would be the number of digits.
  /// 
  /// It can be configured to use the effective scale and precision
  /// (i.e. ignore trailing zeros) if required.
  /// 
  /// 123.4500 has an scale of 4 and a precision of 7, but an effective scale
  /// and precision of 2 and 5 respectively.
  /// </summary>
  public class ScalePrecisionValidator : PropertyValidator
  {
    public ScalePrecisionValidator (int scale, int precision)
        : this (scale, precision, () => Constants.ScalePrecisionError)
    {
      Init (scale, precision);
    }

    private ScalePrecisionValidator (int scale, int precision, Expression<Func<string>> errorMessageResourceSelector)
        : base (errorMessageResourceSelector)
    {
      Init (scale, precision);
    }

    public int Scale { get; private set; }

    public int Precision { get; private set; }

    public bool? IgnoreTrailingZeros { get; set; }

    protected override bool IsValid (PropertyValidatorContext context)
    {
      if (!(context.PropertyValue is decimal propertyValue))
        return true;

      var scale = GetScale (propertyValue);
      var precision = GetPrecision (propertyValue);
      if (scale <= Scale && precision <= Precision)
        return true;

      context.MessageFormatter
          .AppendArgument ("expectedPrecision", Precision)
          .AppendArgument ("expectedScale", Scale)
          .AppendArgument ("digits", precision - scale)
          .AppendArgument ("actualScale", scale);

      return false;
    }

    private void Init (int scale, int precision)
    {
      Scale = scale;
      Precision = precision;

      if (Scale < 0)
        throw new ArgumentOutOfRangeException (nameof (scale), $"Scale must be a positive integer. [value:{Scale}].");

      if (Precision < 0)
        throw new ArgumentOutOfRangeException (nameof (precision), $"Precision must be a positive integer. [value:{Precision}].");

      if (Precision < Scale)
        throw new ArgumentOutOfRangeException (nameof (scale), $"Scale must be greater than precision. [scale:{Scale}, precision:{Precision}].");
    }

    private static uint[] GetBits (decimal Decimal)
    {
      return (uint[]) (object) decimal.GetBits (Decimal);
    }

    private static decimal GetMantissa (decimal Decimal)
    {
      var bits = GetBits (Decimal);
      return bits[2] * new decimal (4294967296L) * new decimal (4294967296L) + bits[1] * new decimal (4294967296L) + bits[0];
    }

    private static uint GetUnsignedScale (decimal Decimal)
    {
      return GetBits (Decimal)[3] >> 16 & 31U;
    }

    private int GetScale (decimal Decimal)
    {
      var unsignedScale = GetUnsignedScale (Decimal);
      if (IgnoreTrailingZeros.HasValue && IgnoreTrailingZeros.Value)
        return (int) unsignedScale - (int) NumTrailingZeros (Decimal);

      return (int) unsignedScale;
    }

    private static uint NumTrailingZeros (decimal Decimal)
    {
      uint num = 0;
      var unsignedScale = GetUnsignedScale (Decimal);
      var mantissa = GetMantissa (Decimal);
      while (mantissa % new decimal (10) == new decimal (0) && num < unsignedScale)
      {
        ++num;
        mantissa /= new decimal (10);
      }
      return num;
    }

    private int GetPrecision (decimal Decimal)
    {
      uint num = 0;
      if (Decimal != new decimal (0))
      {
        var mantissa = GetMantissa (Decimal);
        while (mantissa >= new decimal (1))
        {
          ++num;
          mantissa /= new decimal (10);
        }
        if (IgnoreTrailingZeros.HasValue && IgnoreTrailingZeros.Value)
          return (int) num - (int) NumTrailingZeros (Decimal);
      }
      else
        num = (uint) (GetScale (Decimal) + 1);
      return (int) num;
    }
  }
}