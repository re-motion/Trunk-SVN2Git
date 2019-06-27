// Decompiled with JetBrains decompiler
// Type: FluentValidation.IValidatorDescriptor
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System.Collections.Generic;
using System.Linq;
using Remotion.Validation.Rules;
using Remotion.Validation.Validators;

namespace Remotion.Validation
{
  /// <summary>Provides metadata about a validator.</summary>
  public interface IValidatorDescriptor
  {
    /// <summary>Gets the name display name for a property.</summary>
    string GetName(string property);

    /// <summary>Gets a collection of validators grouped by property.</summary>
    ILookup<string, IPropertyValidator> GetMembersWithValidators();

    /// <summary>Gets validators for a particular property.</summary>
    IEnumerable<IPropertyValidator> GetValidatorsForMember(string name);

    /// <summary>Gets rules for a property.</summary>
    IEnumerable<IValidationRule> GetRulesForMember(string name);
  }
}