// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.PropertyValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Validation.Results;

namespace Remotion.Validation.Validators
{
  public abstract class PropertyValidator : IPropertyValidator
  {
    private readonly List<Func<object, object, object>> customFormatArgs = new List<Func<object, object, object>>();
    private IStringSource errorSource;

    public Func<object, object> CustomStateProvider { get; set; }

    public ICollection<Func<object, object, object>> CustomMessageFormatArguments
    {
      get
      {
        return (ICollection<Func<object, object, object>>) this.customFormatArgs;
      }
    }

    protected PropertyValidator(string errorMessageResourceName, Type errorMessageResourceType)
    {
      this.errorSource = (IStringSource) new LocalizedStringSource(errorMessageResourceType, errorMessageResourceName, (IResourceAccessorBuilder) new FallbackAwareResourceAccessorBuilder());
    }

    protected PropertyValidator(string errorMessage)
    {
      this.errorSource = (IStringSource) new StaticStringSource(errorMessage);
    }

    protected PropertyValidator(
      Expression<Func<string>> errorMessageResourceSelector)
    {
      this.errorSource = LocalizedStringSource.CreateFromExpression(errorMessageResourceSelector, (IResourceAccessorBuilder) new FallbackAwareResourceAccessorBuilder());
    }

    public IStringSource ErrorMessageSource
    {
      get
      {
        return this.errorSource;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof (value));
        this.errorSource = value;
      }
    }

    public virtual IEnumerable<ValidationFailure> Validate(
      PropertyValidatorContext context)
    {
      context.MessageFormatter.AppendPropertyName(context.PropertyDescription);
      context.MessageFormatter.AppendArgument("PropertyValue", context.PropertyValue);
      if (this.IsValid(context))
        return Enumerable.Empty<ValidationFailure>();
      return (IEnumerable<ValidationFailure>) new ValidationFailure[1]
      {
        this.CreateValidationError(context)
      };
    }

    protected abstract bool IsValid(PropertyValidatorContext context);

    /// <summary>
    /// Creates an error validation result for this validator.
    /// </summary>
    /// <param name="context">The validator context</param>
    /// <returns>Returns an error validation result.</returns>
    protected virtual ValidationFailure CreateValidationError(
      PropertyValidatorContext context)
    {
      string error = (context.Rule.MessageBuilder ?? new Func<PropertyValidatorContext, string>(this.BuildErrorMessage))(context);
      ValidationFailure validationFailure = new ValidationFailure(context.PropertyName, error, context.PropertyValue);
      if (this.CustomStateProvider != null)
        validationFailure.CustomState = this.CustomStateProvider(context.Instance);
      return validationFailure;
    }

    private string BuildErrorMessage(PropertyValidatorContext context)
    {
      context.MessageFormatter.AppendAdditionalArguments(this.customFormatArgs.Select<Func<object, object, object>, object>((Func<Func<object, object, object>, object>) (func => func(context.Instance, context.PropertyValue))).ToArray<object>());
      return context.MessageFormatter.BuildMessage(this.errorSource.GetString());
    }
  }
}
