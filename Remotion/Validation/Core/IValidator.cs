// Decompiled with JetBrains decompiler
// Type: FluentValidation.IValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation.Results;
using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Validation.Rules;

namespace Remotion.Validation
{
  /// <summary>Defines a validator for a particular type.</summary>
  public interface IValidator : IEnumerable<IValidationRule>, IEnumerable
  {
    /// <summary>Validates the specified instance</summary>
    /// <param name="instance"></param>
    /// <returns>A ValidationResult containing any validation failures</returns>
    ValidationResult Validate(object instance);

    /// <summary>Validates the specified instance.</summary>
    /// <param name="context">A ValidationContext</param>
    /// <returns>A ValidationResult object containy any validation failures.</returns>
    ValidationResult Validate(ValidationContext context);

    /// <summary>Creates a hook to access various meta data properties</summary>
    /// <returns>A IValidatorDescriptor object which contains methods to access metadata</returns>
    IValidatorDescriptor CreateDescriptor();

    /// <summary>
    /// Checks to see whether the validator can validate objects of the specified type
    /// </summary>
    bool CanValidateInstancesOfType(Type type);
  }
}