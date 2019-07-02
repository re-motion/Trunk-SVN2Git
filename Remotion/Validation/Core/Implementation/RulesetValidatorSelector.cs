// Decompiled with JetBrains decompiler
// Type: FluentValidation.Internal.RulesetValidatorSelector
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Validation.Rules;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Selects validators that belong to the specified rulesets.
  /// </summary>
  public class RulesetValidatorSelector : IValidatorSelector
  {
    private readonly string[] rulesetsToExecute;

    /// <summary>
    /// Creates a new instance of the RulesetValidatorSelector.
    /// </summary>
    public RulesetValidatorSelector (params string[] rulesetsToExecute)
    {
      this.rulesetsToExecute = rulesetsToExecute;
    }

    /// <summary>Determines whether or not a rule should execute.</summary>
    /// <param name="rule">The rule</param>
    /// <param name="propertyPath">Property path (eg Customer.Address.Line1)</param>
    /// <param name="context">Contextual information</param>
    /// <returns>Whether or not the validator can execute.</returns>
    public bool CanExecute (IValidationRule rule, string propertyPath, ValidationContext context)
    {
      return string.IsNullOrEmpty (rule.RuleSet) && this.rulesetsToExecute.Length == 0
             || string.IsNullOrEmpty (rule.RuleSet) && this.rulesetsToExecute.Length > 0
             && ((IEnumerable<string>) this.rulesetsToExecute).Contains<string> (
                 "default",
                 (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
             || (!string.IsNullOrEmpty (rule.RuleSet) && this.rulesetsToExecute.Length > 0
                 && ((IEnumerable<string>) this.rulesetsToExecute).Contains<string> (rule.RuleSet)
                 || ((IEnumerable<string>) this.rulesetsToExecute).Contains<string> ("*"));
    }
  }
}