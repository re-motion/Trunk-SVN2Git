// Decompiled with JetBrains decompiler
// Type: FluentValidation.IValidationRule
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation.Results;
using System;
using System.Collections.Generic;
using FluentValidation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Rules
{
  /// <summary>
  /// Defines a rule associated with a property which can have multiple validators.
  /// </summary>
  public interface IValidationRule
  {
    /// <summary>The validators that are grouped under this rule.</summary>
    IEnumerable<IPropertyValidator> Validators { get; }

    /// <summary>Name of the rule-set to which this rule belongs.</summary>
    string RuleSet { get; set; }

    /// <summary>
    /// Performs validation using a validation context and returns a collection of Validation Failures.
    /// </summary>
    /// <param name="context">Validation Context</param>
    /// <returns>A collection of validation failures</returns>
    IEnumerable<ValidationFailure> Validate(ValidationContext context);

    void ApplyCondition(Func<object, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators);
  }
}