// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.LengthValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation.Resources;
using System;
using System.Linq.Expressions;

namespace Remotion.Validation.Validators
{
  public class LengthValidator : PropertyValidator, ILengthValidator, IPropertyValidator
  {
    public int Min { get; private set; }

    public int Max { get; private set; }

    public LengthValidator(int min, int max)
        : this(min, max, (Expression<Func<string>>) (() => Messages.length_error))
    {
    }

    public LengthValidator(
        int min,
        int max,
        Expression<Func<string>> errorMessageResourceSelector)
        : base(errorMessageResourceSelector)
    {
      this.Max = max;
      this.Min = min;
      if (max != -1 && max < min)
        throw new ArgumentOutOfRangeException(nameof (max), "Max should be larger than min.");
    }

    protected override bool IsValid(PropertyValidatorContext context)
    {
      if (context.PropertyValue == null)
        return true;
      int length = context.PropertyValue.ToString().Length;
      if (length >= this.Min && (length <= this.Max || this.Max == -1))
        return true;
      context.MessageFormatter.AppendArgument("MinLength", (object) this.Min).AppendArgument("MaxLength", (object) this.Max).AppendArgument("TotalLength", (object) length);
      return false;
    }
  }
}