// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.ChildValidatorAdaptor
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Validation.Results;

namespace Remotion.Validation.Validators
{
  public class ChildValidatorAdaptor : NoopPropertyValidator
  {
    private readonly IValidator _validator;

    private IValidator Validator
    {
      get { return _validator; }
    }

    public ChildValidatorAdaptor (IValidator validator)
    {
      _validator = validator;
    }

    public override IEnumerable<ValidationFailure> Validate (PropertyValidatorContext context)
    {
      if (context.Rule.Member == null)
        throw new InvalidOperationException ("Nested validators can only be used with Member Expressions.");

      var propertyValue = context.PropertyValue;
      if (propertyValue == null)
        return Enumerable.Empty<ValidationFailure>();

      var validator = GetValidator ();
      if (validator == null)
        return Enumerable.Empty<ValidationFailure>();

      var forChildValidator = CreateNewValidationContextForChildValidator (propertyValue, context);
      return validator.Validate (forChildValidator).Errors;
    }

    protected virtual IValidator GetValidator ()
    {
      return Validator;
    }

    private ValidationContext CreateNewValidationContextForChildValidator (object instanceToValidate, PropertyValidatorContext context)
    {
      var validationContext = context.ParentContext.CloneForChildValidator (instanceToValidate);
      validationContext.PropertyChain.Add (context.Rule.PropertyName);
      return validationContext;
    }
  }
}