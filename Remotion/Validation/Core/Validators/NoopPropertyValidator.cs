// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.NoopPropertyValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation.Resources;
using System;
using System.Collections.Generic;
using Remotion.Validation.Results;

namespace Remotion.Validation.Validators
{
  public abstract class NoopPropertyValidator : IPropertyValidator
  {
    public IStringSource ErrorMessageSource
    {
      get
      {
        return (IStringSource) null;
      }
      set
      {
      }
    }

    public abstract IEnumerable<ValidationFailure> Validate(
        PropertyValidatorContext context);

    public virtual ICollection<Func<object, object, object>> CustomMessageFormatArguments
    {
      get
      {
        return (ICollection<Func<object, object, object>>) new List<Func<object, object, object>>();
      }
    }

    public virtual bool SupportsStandaloneValidation
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
        return (Func<object, object>) null;
      }
      set
      {
      }
    }
  }
}