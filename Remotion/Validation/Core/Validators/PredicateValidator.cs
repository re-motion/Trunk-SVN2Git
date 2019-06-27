// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.PredicateValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation.Resources;
using System;
using System.Linq.Expressions;
using Remotion.Utilities;

namespace Remotion.Validation.Validators
{
  public class PredicateValidator : PropertyValidator, IPredicateValidator, IPropertyValidator
  {
    private readonly PredicateValidator.Predicate predicate;

    public PredicateValidator(PredicateValidator.Predicate predicate)
        : base((Expression<Func<string>>) (() => Messages.predicate_error))
    {
      ArgumentUtility.CheckNotNull ("predicate", predicate);
      
      this.predicate = predicate;
    }

    protected override bool IsValid(PropertyValidatorContext context)
    {
      return this.predicate(context.Instance, context.PropertyValue, context);
    }

    public delegate bool Predicate(
        object instanceToValidate,
        object propertyValue,
        PropertyValidatorContext propertyValidatorContext);
  }
}