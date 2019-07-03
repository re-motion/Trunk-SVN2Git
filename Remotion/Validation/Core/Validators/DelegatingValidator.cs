// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.DelegatingValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Validation.Implementation;
using Remotion.Validation.Results;

namespace Remotion.Validation.Validators
{
  public class DelegatingValidator : IDelegatingValidator
  {
    private readonly Func<object, bool> _condition;
    private IPropertyValidator InnerValidator { get; }

    public DelegatingValidator (Func<object, bool> condition, IPropertyValidator innerValidator)
    {
      _condition = condition;
      InnerValidator = innerValidator;
    }

    public IStringSource ErrorMessageSource
    {
      get { return InnerValidator.ErrorMessageSource; }
      set { InnerValidator.ErrorMessageSource = value; }
    }

    public IEnumerable<ValidationFailure> Validate (PropertyValidatorContext context)
    {
      if (_condition (context.Instance))
        return InnerValidator.Validate (context);

      return Enumerable.Empty<ValidationFailure>();
    }

    public ICollection<Func<object, object, object>> CustomMessageFormatArguments
    {
      get { return InnerValidator.CustomMessageFormatArguments; }
    }

    public Func<object, object> CustomStateProvider
    {
      get { return InnerValidator.CustomStateProvider; }
      set { InnerValidator.CustomStateProvider = value; }
    }

    IPropertyValidator IDelegatingValidator.InnerValidator
    {
      get { return InnerValidator; }
    }
  }
}