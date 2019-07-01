// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.IPropertyValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation.Resources;
using System;
using System.Collections.Generic;
using Remotion.Validation.Results;

namespace Remotion.Validation.Validators
{
  /// <summary>
  /// A custom property validator.
  /// This interface should not be implemented directly in your code as it is subject to change.
  /// Please inherit from <see cref="T:FluentValidation.Validators.PropertyValidator">PropertyValidator</see> instead.
  /// </summary>
  public interface IPropertyValidator
  {
    IEnumerable<ValidationFailure> Validate(
        PropertyValidatorContext context);

    /// <summary>
    /// Custom message arguments.
    /// Arg 1: Instance being validated
    /// Arg 2: Property value
    /// </summary>
    ICollection<Func<object, object, object>> CustomMessageFormatArguments { get; }

    Func<object, object> CustomStateProvider { get; set; }

    IStringSource ErrorMessageSource { get; set; }
  }
}