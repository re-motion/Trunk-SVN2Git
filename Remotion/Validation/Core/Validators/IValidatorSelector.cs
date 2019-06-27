// Decompiled with JetBrains decompiler
// Type: FluentValidation.Internal.IValidatorSelector
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using Remotion.Validation.Rules;

namespace Remotion.Validation.Validators
{
  //TODO: move to Implementation?
  /// <summary>Determines whether or not a rule should execute.</summary>
  public interface IValidatorSelector
  {
    /// <summary>Determines whether or not a rule should execute.</summary>
    /// <param name="rule">The rule</param>
    /// <param name="propertyPath">Property path (eg Customer.Address.Line1)</param>
    /// <param name="context">Contextual information</param>
    /// <returns>Whether or not the validator can execute.</returns>
    bool CanExecute(IValidationRule rule, string propertyPath, ValidationContext context);
  }
}