// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.PropertyValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Validation.Implementation;
using Remotion.Validation.Results;

namespace Remotion.Validation.Validators
{
  public abstract class PropertyValidator : IPropertyValidator
  {
    private readonly List<Func<object, object, object>> _customFormatArgs = new List<Func<object, object, object>>();
    private IStringSource _errorSource;

    public Func<object, object> CustomStateProvider { get; set; }

    public ICollection<Func<object, object, object>> CustomMessageFormatArguments
    {
      get { return _customFormatArgs; }
    }

    protected PropertyValidator (Expression<Func<string>> errorMessageResourceSelector)
    {
      _errorSource = LocalizedStringSource.CreateFromExpression (errorMessageResourceSelector, new StaticResourceAccessorBuilder());
    }

    public IStringSource ErrorMessageSource
    {
      get { return _errorSource; }
      set { _errorSource = value ?? throw new ArgumentNullException (nameof (value)); }
    }

    public virtual IEnumerable<ValidationFailure> Validate (PropertyValidatorContext context)
    {
      context.MessageFormatter.AppendPropertyName (context.PropertyName);
      context.MessageFormatter.AppendArgument ("PropertyValue", context.PropertyValue);
      if (IsValid (context))
        return Enumerable.Empty<ValidationFailure>();

      return new[]
             {
                 CreateValidationError (context)
             };
    }

    protected abstract bool IsValid (PropertyValidatorContext context);

    /// <summary>
    /// Creates an error validation result for this validator.
    /// </summary>
    /// <param name="context">The validator context</param>
    /// <returns>Returns an error validation result.</returns>
    private ValidationFailure CreateValidationError (PropertyValidatorContext context)
    {
      // TODO RM-5906
      var error = BuildErrorMessage (context);
      var validationFailure = new ValidationFailure (context.PropertyName, error);
      if (CustomStateProvider != null)
        validationFailure.CustomState = CustomStateProvider (context.Instance);

      return validationFailure;
    }

    private string BuildErrorMessage (PropertyValidatorContext context)
    {
      context.MessageFormatter.AppendAdditionalArguments (_customFormatArgs.Select (func => func (context.Instance, context.PropertyValue)).ToArray());
      return context.MessageFormatter.BuildMessage (_errorSource.GetString());
    }
  }
}