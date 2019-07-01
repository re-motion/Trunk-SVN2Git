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
    public ScalePrecisionValidator(int scale, int precision)
      : this(scale, precision, (Expression<Func<string>>) (() => Constants.ScalePrecisionError))
    {
      this.Init(scale, precision);
    }

    public ScalePrecisionValidator(
      int scale,
      int precision,
      string errorMessageResourceName,
      Type errorMessageResourceType)
      : base(errorMessageResourceName, errorMessageResourceType)
    {
      this.Init(scale, precision);
    }

    public ScalePrecisionValidator(int scale, int precision, string errorMessage)
      : base(errorMessage)
    {
      this.Init(scale, precision);
    }

    public ScalePrecisionValidator(
      int scale,
      int precision,
      Expression<Func<string>> errorMessageResourceSelector)
      : base(errorMessageResourceSelector)
    {
      this.Init(scale, precision);
    }

    public int Scale { get; set; }

    public int Precision { get; set; }

    public bool? IgnoreTrailingZeros { get; set; }

    protected override bool IsValid(PropertyValidatorContext context)
    {
      Decimal? propertyValue = context.PropertyValue as Decimal?;
      if (propertyValue.HasValue)
      {
        int scale = this.GetScale(propertyValue.Value);
        int precision = this.GetPrecision(propertyValue.Value);
        if (scale > this.Scale || precision > this.Precision)
        {
          context.MessageFormatter.AppendArgument("expectedPrecision", (object) this.Precision).AppendArgument("expectedScale", (object) this.Scale).AppendArgument("digits", (object) (precision - scale)).AppendArgument("actualScale", (object) scale);
          return false;
        }
      }
      return true;
    }

    private void Init(int scale, int precision)
    {
      this.Scale = scale;
      this.Precision = precision;
      if (this.Scale < 0)
        throw new ArgumentOutOfRangeException(nameof (scale), string.Format("Scale must be a positive integer. [value:{0}].", (object) this.Scale));
      if (this.Precision < 0)
        throw new ArgumentOutOfRangeException(nameof (precision), string.Format("Precision must be a positive integer. [value:{0}].", (object) this.Precision));
      if (this.Precision < this.Scale)
        throw new ArgumentOutOfRangeException(nameof (scale), string.Format("Scale must be greater than precision. [scale:{0}, precision:{1}].", (object) this.Scale, (object) this.Precision));
    }

    private static uint[] GetBits(Decimal Decimal)
    {
      return (uint[]) (object) Decimal.GetBits(Decimal);
    }

    private static Decimal GetMantissa(Decimal Decimal)
    {
      uint[] bits = ScalePrecisionValidator.GetBits(Decimal);
      return (Decimal) bits[2] * new Decimal(4294967296L) * new Decimal(4294967296L) + (Decimal) bits[1] * new Decimal(4294967296L) + (Decimal) bits[0];
    }

    private static uint GetUnsignedScale(Decimal Decimal)
    {
      return ScalePrecisionValidator.GetBits(Decimal)[3] >> 16 & 31U;
    }

    private int GetScale(Decimal Decimal)
    {
      uint unsignedScale = ScalePrecisionValidator.GetUnsignedScale(Decimal);
      if (this.IgnoreTrailingZeros.HasValue && this.IgnoreTrailingZeros.Value)
        return (int) unsignedScale - (int) ScalePrecisionValidator.NumTrailingZeros(Decimal);
      return (int) unsignedScale;
    }

    private static uint NumTrailingZeros(Decimal Decimal)
    {
      uint num = 0;
      uint unsignedScale = ScalePrecisionValidator.GetUnsignedScale(Decimal);
      Decimal mantissa = ScalePrecisionValidator.GetMantissa(Decimal);
      while (mantissa % new Decimal(10) == new Decimal(0) && num < unsignedScale)
      {
        ++num;
        mantissa /= new Decimal(10);
      }
      return num;
    }

    private int GetPrecision(Decimal Decimal)
    {
      uint num = 0;
      if (Decimal != new Decimal(0))
      {
        Decimal mantissa = ScalePrecisionValidator.GetMantissa(Decimal);
        while (mantissa >= new Decimal(1))
        {
          ++num;
          mantissa /= new Decimal(10);
        }
        if (this.IgnoreTrailingZeros.HasValue && this.IgnoreTrailingZeros.Value)
          return (int) num - (int) ScalePrecisionValidator.NumTrailingZeros(Decimal);
      }
      else
        num = (uint) (this.GetScale(Decimal) + 1);
      return (int) num;
    }
  }
}
