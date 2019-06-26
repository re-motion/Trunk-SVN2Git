// Decompiled with JetBrains decompiler
// Type: FluentValidation.Internal.PropertyRule
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation.Resources;
using FluentValidation.Results;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;
using FluentValidation.Internal;

namespace Remotion.Validation.Rules
{
  /// <summary>Defines a rule associated with a property.</summary>
  public class PropertyRule : IValidationRule
  {
    private readonly List<IPropertyValidator> validators = new List<IPropertyValidator>();
    private Func<CascadeMode> cascadeModeThunk = (Func<CascadeMode>) (() => ValidatorOptions.CascadeMode);

    //TODO: remove when done
    private readonly Type _containerType;

    /// <summary>Property associated with this rule.</summary>
    public MemberInfo Member { get; private set; }

    /// <summary>
    /// Function that can be invoked to retrieve the value of the property.
    /// </summary>
    public Func<object, object> PropertyFunc { get; private set; }

    /// <summary>Expression that was used to create the rule.</summary>
    public LambdaExpression Expression { get; private set; }

    /// <summary>
    /// String source that can be used to retrieve the display name (if null, falls back to the property name)
    /// </summary>
    public IStringSource DisplayName { get; set; }

    /// <summary>Rule set that this rule belongs to (if specified)</summary>
    public string RuleSet { get; set; }

    /// <summary>
    /// Function that will be invoked if any of the validators associated with this rule fail.
    /// </summary>
    public Action<object> OnFailure { get; set; }

    /// <summary>The current validator being configured by this rule.</summary>
    public IPropertyValidator CurrentValidator { get; private set; }

    /// <summary>Type of the property being validated</summary>
    public Type TypeToValidate { get; private set; }

    /// <summary>Cascade mode for this rule.</summary>
    public CascadeMode CascadeMode
    {
      get
      {
        return this.cascadeModeThunk();
      }
      set
      {
        this.cascadeModeThunk = (Func<CascadeMode>) (() => value);
      }
    }

    /// <summary>Validators associated with this rule.</summary>
    public IEnumerable<IPropertyValidator> Validators
    {
      get
      {
        return (IEnumerable<IPropertyValidator>) this.validators;
      }
    }

    /// <summary>Creates a new property rule.</summary>
    /// <param name="member">Property</param>
    /// <param name="propertyFunc">Function to get the property value</param>
    /// <param name="expression">Lambda expression used to create the rule</param>
    /// <param name="cascadeModeThunk">Function to get the cascade mode.</param>
    /// <param name="typeToValidate">Type to validate</param>
    /// <param name="containerType">Container type that owns the property</param>
    public PropertyRule(
      MemberInfo member,
      Func<object, object> propertyFunc,
      LambdaExpression expression,
      Func<CascadeMode> cascadeModeThunk,
      Type typeToValidate,
      Type containerType)
    {
      _containerType = containerType;
      this.Member = member;
      this.PropertyFunc = propertyFunc;
      this.Expression = expression;
      this.OnFailure = (Action<object>) (x => {});
      this.TypeToValidate = typeToValidate;
      this.cascadeModeThunk = cascadeModeThunk;
      this.PropertyName = ValidatorOptions.PropertyNameResolver(containerType, member, expression);
      this.DisplayName = (IStringSource) new LazyStringSource((Func<string>) (() => ValidatorOptions.DisplayNameResolver(containerType, member, expression)));
    }

    /// <summary>Creates a new property rule from a lambda expression.</summary>
    public static PropertyRule Create<T, TProperty>(
      System.Linq.Expressions.Expression<Func<T, TProperty>> expression)
    {
      return PropertyRule.Create<T, TProperty>(expression, (Func<CascadeMode>) (() => ValidatorOptions.CascadeMode));
    }

    /// <summary>Creates a new property rule from a lambda expression.</summary>
    public static PropertyRule Create<T, TProperty>(
      System.Linq.Expressions.Expression<Func<T, TProperty>> expression,
      Func<CascadeMode> cascadeModeThunk)
    {
      return new PropertyRule(expression.GetMember<T, TProperty>(), expression.Compile().CoerceToNonGeneric<T, TProperty>(), (LambdaExpression) expression, cascadeModeThunk, typeof (TProperty), typeof (T));
    }

    /// <summary>Adds a validator to the rule.</summary>
    public void AddValidator(IPropertyValidator validator)
    {
      this.CurrentValidator = validator;
      this.validators.Add(validator);
    }

    /// <summary>
    /// Replaces a validator in this rule. Used to wrap validators.
    /// </summary>
    public void ReplaceValidator(IPropertyValidator original, IPropertyValidator newValidator)
    {
      int index = this.validators.IndexOf(original);
      if (index <= -1)
        return;
      this.validators[index] = newValidator;
      if (!object.ReferenceEquals((object) this.CurrentValidator, (object) original))
        return;
      this.CurrentValidator = newValidator;
    }

    /// <summary>Remove a validator in this rule.</summary>
    public void RemoveValidator(IPropertyValidator original)
    {
      if (object.ReferenceEquals((object) this.CurrentValidator, (object) original))
        this.CurrentValidator = this.validators.LastOrDefault<IPropertyValidator>((Func<IPropertyValidator, bool>) (x => x != original));
      this.validators.Remove(original);
    }

    /// <summary>Clear all validators from this rule.</summary>
    public void ClearValidators()
    {
      this.CurrentValidator = (IPropertyValidator) null;
      this.validators.Clear();
    }

    /// <summary>
    /// Returns the property name for the property being validated.
    /// Returns null if it is not a property being validated (eg a method call)
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>Allows custom creation of an error message</summary>
    public Func<PropertyValidatorContext, string> MessageBuilder { get; set; }

    /// <summary>Display name for the property.</summary>
    public string GetDisplayName()
    {
      string str = (string) null;
      if (this.DisplayName != null)
        str = this.DisplayName.GetString();
      if (str == null)
        str = this.PropertyName.SplitPascalCase();
      return str;
    }

    /// <summary>
    /// Performs validation using a validation context and returns a collection of Validation Failures.
    /// </summary>
    /// <param name="context">Validation Context</param>
    /// <returns>A collection of validation failures</returns>
    public virtual IEnumerable<ValidationFailure> Validate(
      ValidationContext context)
    {
      string displayName = this.GetDisplayName();
      if (this.PropertyName == null && displayName == null)
        throw new InvalidOperationException(string.Format("Property name could not be automatically determined for expression {0}. Please specify either a custom property name by calling 'WithName'.", (object) this.Expression));
      string propertyName = context.PropertyChain.BuildPropertyName(this.PropertyName ?? displayName);
      if (context.Selector.CanExecute((IValidationRule) this, propertyName, context))
      {
        CascadeMode cascade = this.cascadeModeThunk();
        bool hasAnyFailure = false;
        foreach (IPropertyValidator validator in this.validators)
        {
          IEnumerable<ValidationFailure> results = this.InvokePropertyValidator(context, validator, propertyName);
          bool hasFailure = false;
          foreach (ValidationFailure validationFailure in results)
          {
            hasAnyFailure = true;
            hasFailure = true;
            yield return validationFailure;
          }
          if (cascade == CascadeMode.StopOnFirstFailure && hasFailure)
            break;
        }
        if (hasAnyFailure)
          this.OnFailure(context.InstanceToValidate);
      }
    }

    /// <summary>
    /// Invokes a property validator using the specified validation context.
    /// </summary>
    protected virtual IEnumerable<ValidationFailure> InvokePropertyValidator(
      ValidationContext context,
      IPropertyValidator validator,
      string propertyName)
    {
      PropertyValidatorContext context1 = new PropertyValidatorContext(context, this, propertyName);
      var fluentContext = new FluentValidation.Validators.PropertyValidatorContext (
          context1.ParentContext,
          new FluentValidation.Internal.PropertyRule (
              context1.Rule.Member,
              context1.Rule.PropertyFunc,
              context1.Rule.Expression,
              context1.Rule.cascadeModeThunk,
              context1.Rule.TypeToValidate,
              context1.Rule._containerType),
          context1.PropertyName);
      return validator.Validate(fluentContext);
    }

    public void ApplyCondition(Func<object, bool> predicate, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
    {
      if (applyConditionTo == ApplyConditionTo.AllValidators)
      {
        foreach (IPropertyValidator propertyValidator in this.Validators.ToList<IPropertyValidator>())
        {
          DelegatingValidator delegatingValidator = new DelegatingValidator(predicate, propertyValidator);
          this.ReplaceValidator(propertyValidator, (IPropertyValidator) delegatingValidator);
        }
      }
      else
        this.ReplaceValidator(this.CurrentValidator, (IPropertyValidator) new DelegatingValidator(predicate, this.CurrentValidator));
    }
  }
}
