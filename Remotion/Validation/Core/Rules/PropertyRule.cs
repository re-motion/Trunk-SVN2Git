// Decompiled with JetBrains decompiler
// Type: FluentValidation.Internal.PropertyRule
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.Results;
using Remotion.Validation.Utilities;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Rules
{
  /// <summary>Defines a rule associated with a property.</summary>
  public class PropertyRule : IValidationRule
  {
    private readonly List<IPropertyValidator> _validators = new List<IPropertyValidator>();
    private readonly Func<CascadeMode> _cascadeModeThunk = () => ValidatorOptions.CascadeMode;

    /// <summary>Property associated with this rule.</summary>
    public MemberInfo Member { get; }

    /// <summary>
    /// Function that can be invoked to retrieve the value of the property.
    /// </summary>
    public Func<object, object> PropertyFunc { get; }

    /// <summary>Expression that was used to create the rule.</summary>
    public LambdaExpression Expression { get; }

    /// <summary>
    /// String source that can be used to retrieve the display name (if null, falls back to the property name)
    /// </summary>
    public IStringSource DisplayName { get; set; }

    /// <summary>Rule set that this rule belongs to (if specified)</summary>
    public string RuleSet { get; set; }

    /// <summary>
    /// Function that will be invoked if any of the validators associated with this rule fail.
    /// </summary>
    private Action<object> OnFailure { get; }

    /// <summary>The current validator being configured by this rule.</summary>
    public IPropertyValidator CurrentValidator { get; private set; }

    /// <summary>Validators associated with this rule.</summary>
    public IEnumerable<IPropertyValidator> Validators
    {
      get { return _validators; }
    }

    /// <summary>Creates a new property rule.</summary>
    /// <param name="member">Property</param>
    /// <param name="propertyFunc">Function to get the property value</param>
    /// <param name="expression">Lambda expression used to create the rule</param>
    /// <param name="cascadeModeThunk">Function to get the cascade mode.</param>
    /// <param name="containerType">Container type that owns the property</param>
    protected PropertyRule (
        MemberInfo member,
        Func<object, object> propertyFunc,
        LambdaExpression expression,
        Func<CascadeMode> cascadeModeThunk,
        Type containerType)
    {
      Member = member;
      PropertyFunc = propertyFunc;
      Expression = expression;
      OnFailure = x => { };
      _cascadeModeThunk = cascadeModeThunk;
      PropertyName = ValidatorOptions.PropertyNameResolver (containerType, member, expression);
      DisplayName = new LazyStringSource (() => ValidatorOptions.DisplayNameResolver (containerType, member, expression));
    }

    /// <summary>Creates a new property rule from a lambda expression.</summary>
    public static PropertyRule Create<T, TProperty> (Expression<Func<T, TProperty>> expression)
    {
      return Create (expression, () => ValidatorOptions.CascadeMode);
    }

    /// <summary>Creates a new property rule from a lambda expression.</summary>
    private static PropertyRule Create<T, TProperty> (Expression<Func<T, TProperty>> expression, Func<CascadeMode> cascadeModeThunk)
    {
      return new PropertyRule (
          Extensions.GetMember (expression),
          expression.Compile().CoerceToNonGeneric (),
          expression,
          cascadeModeThunk,
          typeof (T));
    }

    /// <summary>Adds a validator to the rule.</summary>
    public void AddValidator (IPropertyValidator validator)
    {
      CurrentValidator = validator;
      _validators.Add (validator);
    }

    /// <summary>
    /// Replaces a validator in this rule. Used to wrap validators.
    /// </summary>
    private void ReplaceValidator (IPropertyValidator original, IPropertyValidator newValidator)
    {
      var index = _validators.IndexOf (original);
      if (index <= -1)
        return;

      _validators[index] = newValidator;
      if (!ReferenceEquals (CurrentValidator, original))
        return;

      CurrentValidator = newValidator;
    }

    /// <summary>Remove a validator in this rule.</summary>
    protected void RemoveValidator (IPropertyValidator original)
    {
      if (ReferenceEquals (CurrentValidator, original))
        CurrentValidator = _validators.LastOrDefault (x => x != original);
      _validators.Remove (original);
    }

    /// <summary>
    /// Returns the property name for the property being validated.
    /// Returns null if it is not a property being validated (eg a method call)
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>Allows custom creation of an error message</summary>
    public Func<PropertyValidatorContext, string> MessageBuilder { get; set; }

    /// <summary>Display name for the property.</summary>
    public string GetDisplayName ()
    {
      string str = null;
      if (DisplayName != null)
        str = DisplayName.GetString();
      return str ?? PropertyName.SplitPascalCase();
    }

    /// <summary>
    /// Performs validation using a validation context and returns a collection of Validation Failures.
    /// </summary>
    /// <param name="context">Validation Context</param>
    /// <returns>A collection of validation failures</returns>
    public virtual IEnumerable<ValidationFailure> Validate (ValidationContext context)
    {
      var displayName = GetDisplayName();
      if (PropertyName == null && displayName == null)
        throw new InvalidOperationException (
            $"Property name could not be automatically determined for expression {Expression}. Please specify either a custom property name by calling 'WithName'.");

      var propertyName = context.PropertyChain.BuildPropertyName (PropertyName ?? displayName);
      if (context.Selector.CanExecute (this))
      {
        var cascade = _cascadeModeThunk();
        var hasAnyFailure = false;
        foreach (var validator in _validators)
        {
          var results = InvokePropertyValidator (context, validator, propertyName);
          var hasFailure = false;
          foreach (var validationFailure in results)
          {
            hasAnyFailure = true;
            hasFailure = true;
            yield return validationFailure;
          }
          if (cascade == CascadeMode.StopOnFirstFailure && hasFailure)
            break;
        }
        if (hasAnyFailure)
          OnFailure (context.InstanceToValidate);
      }
    }

    /// <summary>
    /// Invokes a property validator using the specified validation context.
    /// </summary>
    private IEnumerable<ValidationFailure> InvokePropertyValidator (ValidationContext context, IPropertyValidator validator, string propertyName)
    {
      var context1 = new PropertyValidatorContext (context, this, propertyName);
      return validator.Validate (context1);
    }

    public void ApplyCondition (Func<object, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
    {
      if (applyConditionTo == ApplyConditionTo.AllValidators)
      {
        foreach (IPropertyValidator propertyValidator in Validators.ToList())
        {
          var delegatingValidator = new DelegatingValidator (predicate, propertyValidator);
          ReplaceValidator (propertyValidator, delegatingValidator);
        }
      }
      else
      {
        ReplaceValidator (CurrentValidator, new DelegatingValidator (predicate, CurrentValidator));
      }
    }
  }
}