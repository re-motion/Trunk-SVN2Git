// Decompiled with JetBrains decompiler
// Type: FluentValidation.IValidator`1
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation.Results;
using System.Collections;
using System.Collections.Generic;
using Remotion.Validation.Rules;

namespace Remotion.Validation
{
  /// <summary>Defines a validator for a particular type.</summary>
  /// <typeparam name="T"></typeparam>
  public interface IValidator<in T> : IValidator, IEnumerable<IValidationRule>, IEnumerable
  {
    /// <summary>Validates the specified instance.</summary>
    /// <param name="instance">The instance to validate</param>
    /// <returns>A ValidationResult object containing any validation failures.</returns>
    ValidationResult Validate(T instance);

    /// <summary>
    /// Sets the cascade mode for all rules within this validator.
    /// </summary>
    FluentValidation.CascadeMode CascadeMode { get; set; }
  }
}