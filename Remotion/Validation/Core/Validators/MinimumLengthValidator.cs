// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.MinimumLengthValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation.Resources;
using System;
using System.Linq.Expressions;

namespace Remotion.Validation.Validators
{
  public class MinimumLengthValidator : LengthValidator
  {
    public MinimumLengthValidator(int min)
        : this(min, (Expression<Func<string>>) (() => Messages.length_error))
    {
    }

    public MinimumLengthValidator(
        int min,
        Expression<Func<string>> errorMessageResourceSelector)
        : base(min, -1, errorMessageResourceSelector)
    {
    }
  }
}