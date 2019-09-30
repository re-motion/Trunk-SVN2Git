// Decompiled with JetBrains decompiler
// Type: FluentValidation.ValidatorDescriptor`1
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Validation.Rules;
using Remotion.Validation.Validators;

namespace Remotion.Validation
{
  /// <summary>Used for providing metadata about a validator.</summary>
  public class ValidatorDescriptor<T> : IValidatorDescriptor
  {
    private IEnumerable<IValidationRule> Rules { get; }

    public ValidatorDescriptor (IEnumerable<IValidationRule> ruleBuilders)
    {
      Rules = ruleBuilders;
    }

    public IEnumerable<IPropertyValidator> GetValidatorsForMember (string name)
    {
      // TODO RM-5906
      return Rules.Where (r=>r.Property.Name == name).SelectMany (r => r.Validators);
    }
  }
}