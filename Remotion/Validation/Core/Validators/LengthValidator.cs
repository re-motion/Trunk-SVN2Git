// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.LengthValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Validators
{
  public class LengthValidator : PropertyValidator, ILengthValidator
  {
    public int Min { get; }

    public int Max { get; }

    public LengthValidator (int min, int max, IValidationMessage validationMessage = null)
        : this (min, max, Constants.LengthError, validationMessage ?? new InvariantValidationMessage (Constants.LengthError))
    {
    }

    protected LengthValidator (int min, int max, string errorMessage, IValidationMessage validationMessage)
        : base (errorMessage, validationMessage)
    {
      Max = max;
      Min = min;
      if (max != -1 && max < min)
        throw new ArgumentOutOfRangeException (nameof (max), "Max should be larger than min.");
    }

    protected override bool IsValid (PropertyValidatorContext context)
    {
      if (context.PropertyValue == null)
        return true;

      var length = context.PropertyValue.ToString().Length;
      if (length >= Min && (length <= Max || Max == -1))
        return true;

      context.MessageFormatter.AppendArgument ("MinLength", Min).AppendArgument ("MaxLength", Max).AppendArgument ("TotalLength", length);
      return false;
    }
  }
}