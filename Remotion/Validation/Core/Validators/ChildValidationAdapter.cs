// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.ChildValidatorAdaptor
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Remotion.Validation.Results;

namespace Remotion.Validation.Validators
{
  public class ChildValidatorAdaptor : NoopPropertyValidator
  {
    private readonly IValidator validator;

    public IValidator Validator
    {
      get
      {
        return this.validator;
      }
    }

    public ChildValidatorAdaptor(IValidator validator)
    {
      this.validator = validator;
    }

    public override IEnumerable<ValidationFailure> Validate(
      PropertyValidatorContext context)
    {
      if (context.Rule.Member == (MemberInfo) null)
        throw new InvalidOperationException(string.Format("Nested validators can only be used with Member Expressions."));
      object propertyValue = context.PropertyValue;
      if (propertyValue == null)
        return Enumerable.Empty<ValidationFailure>();
      IValidator validator = this.GetValidator(context);
      if (validator == null)
        return Enumerable.Empty<ValidationFailure>();
      ValidationContext forChildValidator = this.CreateNewValidationContextForChildValidator(propertyValue, context);
      return (IEnumerable<ValidationFailure>) validator.Validate(forChildValidator).Errors;
    }

    protected virtual IValidator GetValidator(PropertyValidatorContext context)
    {
      return this.Validator;
    }

    protected ValidationContext CreateNewValidationContextForChildValidator(
      object instanceToValidate,
      PropertyValidatorContext context)
    {
      ValidationContext validationContext = context.ParentContext.CloneForChildValidator(instanceToValidate);
      validationContext.PropertyChain.Add(context.Rule.PropertyName);
      return validationContext;
    }
  }
}
