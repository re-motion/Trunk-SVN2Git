// Decompiled with JetBrains decompiler
// Type: FluentValidation.Internal.DefaultValidatorSelector
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using Remotion.Validation.Rules;

namespace Remotion.Validation.Validators
{
  /// <summary>
  /// Default validator selector that will execute all rules that do not belong to a RuleSet.
  /// </summary>
  public class DefaultValidatorSelector : IValidatorSelector
  {
    /// <summary>Determines whether or not a rule should execute.</summary>
    /// <param name="rule">The rule</param>
    /// <returns>Whether or not the validator can execute.</returns>
    public bool CanExecute (IValidationRule rule)
    {
      return string.IsNullOrEmpty (rule.RuleSet);
    }
  }
}