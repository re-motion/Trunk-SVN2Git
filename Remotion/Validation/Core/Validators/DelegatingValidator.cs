// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.DelegatingValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation.Resources;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Remotion.Validation.Validators
{
  public class DelegatingValidator : IDelegatingValidator, IPropertyValidator
  {
    private readonly Func<object, bool> condition;

    public IPropertyValidator InnerValidator { get; private set; }

    public DelegatingValidator(Func<object, bool> condition, IPropertyValidator innerValidator)
    {
      this.condition = condition;
      this.InnerValidator = innerValidator;
    }

    public IStringSource ErrorMessageSource
    {
      get
      {
        return this.InnerValidator.ErrorMessageSource;
      }
      set
      {
        this.InnerValidator.ErrorMessageSource = value;
      }
    }

    public IEnumerable<ValidationFailure> Validate(
      PropertyValidatorContext context)
    {
      if (this.condition(context.Instance))
        return this.InnerValidator.Validate(context);
      return Enumerable.Empty<ValidationFailure>();
    }

    public ICollection<Func<object, object, object>> CustomMessageFormatArguments
    {
      get
      {
        return this.InnerValidator.CustomMessageFormatArguments;
      }
    }

    public bool SupportsStandaloneValidation
    {
      get
      {
        return false;
      }
    }

    public Func<object, object> CustomStateProvider
    {
      get
      {
        return this.InnerValidator.CustomStateProvider;
      }
      set
      {
        this.InnerValidator.CustomStateProvider = value;
      }
    }

    IPropertyValidator IDelegatingValidator.InnerValidator
    {
      get
      {
        return this.InnerValidator;
      }
    }
  }
}
