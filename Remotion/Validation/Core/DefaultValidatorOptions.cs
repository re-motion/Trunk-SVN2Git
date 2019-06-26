// Decompiled with JetBrains decompiler
// Type: FluentValidation.DefaultValidatorOptions
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Utilities;
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.Rules;
using ApplyConditionTo = FluentValidation.ApplyConditionTo;
using CascadeMode = FluentValidation.CascadeMode;
using Extensions = FluentValidation.Internal.Extensions;

namespace Remotion.Validation
{
  /// <summary>
  /// Default options that can be used to configure a validator.
  /// </summary>
  public static class DefaultValidatorOptions
  {
    /// <summary>
    /// Specifies the cascade mode for failures.
    /// If set to 'Stop' then execution of the rule will stop once the first validator in the chain fails.
    /// If set to 'Continue' then all validators in the chain will execute regardless of failures.
    /// </summary>
    public static IRuleBuilderInitial<T, TProperty> Cascade<T, TProperty>(
      this IRuleBuilderInitial<T, TProperty> ruleBuilder,
      CascadeMode cascadeMode)
    {
      return ruleBuilder.Configure((Action<PropertyRule>) (cfg => cfg.CascadeMode = cascadeMode));
    }

    /// <summary>
    /// Specifies a custom action to be invoked when the validator fails.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="rule"></param>
    /// <param name="onFailure"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> OnAnyFailure<T, TProperty>(
      this IRuleBuilderOptions<T, TProperty> rule,
      Action<T> onFailure)
    {
      return rule.Configure((Action<PropertyRule>) (config => config.OnFailure = Extensions.CoerceToNonGeneric (onFailure)));
    }

    /// <summary>
    /// Specifies a custom error message to use if validation fails.
    /// </summary>
    /// <param name="rule">The current rule</param>
    /// <param name="errorMessage">The error message to use</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(
      this IRuleBuilderOptions<T, TProperty> rule,
      string errorMessage)
    {
      return rule.WithMessage<T, TProperty>(errorMessage, (object[]) null);
    }

    /// <summary>
    /// Specifies a custom error message to use if validation fails.
    /// </summary>
    /// <param name="rule">The current rule</param>
    /// <param name="errorMessage">The error message to use</param>
    /// <param name="formatArgs">Additional arguments to be specified when formatting the custom error message.</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(
      this IRuleBuilderOptions<T, TProperty> rule,
      string errorMessage,
      params object[] formatArgs)
    {
      Func<T, object>[] arrayOfDelegates = DefaultValidatorOptions.ConvertArrayOfObjectsToArrayOfDelegates<T>(formatArgs);
      return DefaultValidatorOptions.WithMessage<T, TProperty>(rule, errorMessage, arrayOfDelegates);
    }

    /// <summary>
    /// Specifies a custom error message to use if validation fails.
    /// </summary>
    /// <param name="rule">The current rule</param>
    /// <param name="errorMessage">The error message to use</param>
    /// <param name="funcs">Additional property values to be included when formatting the custom error message.</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(
      this IRuleBuilderOptions<T, TProperty> rule,
      string errorMessage,
      params Func<T, object>[] funcs)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("errorMessage", errorMessage);

      return rule.Configure((Action<PropertyRule>) (config =>
      {
        config.CurrentValidator.ErrorMessageSource = (IStringSource) new StaticStringSource(errorMessage);
        ((IEnumerable<Func<T, object>>) funcs).Select<Func<T, object>, Func<object, object, object>>((Func<Func<T, object>, Func<object, object, object>>) (func => (Func<object, object, object>) ((instance, value) => func((T) instance)))).ForEach<Func<object, object, object>>(new Action<Func<object, object, object>>(config.CurrentValidator.CustomMessageFormatArguments.Add));
      }));
    }

    /// <summary>
    /// Specifies a custom error message to use if validation fails.
    /// </summary>
    /// <param name="rule">The current rule</param>
    /// <param name="errorMessage">The error message to use</param>
    /// <param name="funcs">Additional property values to use when formatting the custom error message.</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty>(
      this IRuleBuilderOptions<T, TProperty> rule,
      string errorMessage,
      params Func<T, TProperty, object>[] funcs)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("errorMessage", errorMessage);

      return rule.Configure((Action<PropertyRule>) (config =>
      {
        config.CurrentValidator.ErrorMessageSource = (IStringSource) new StaticStringSource(errorMessage);
        ((IEnumerable<Func<T, TProperty, object>>) funcs).Select<Func<T, TProperty, object>, Func<object, object, object>>((Func<Func<T, TProperty, object>, Func<object, object, object>>) (func => (Func<object, object, object>) ((instance, value) => func((T) instance, (TProperty) value)))).ForEach<Func<object, object, object>>(new Action<Func<object, object, object>>(config.CurrentValidator.CustomMessageFormatArguments.Add));
      }));
    }

    /// <summary>
    /// Specifies a custom error message resource to use when validation fails.
    /// </summary>
    /// <param name="rule">The current rule</param>
    /// <param name="resourceSelector">The resource to use as an expression, eg () =&gt; Messages.MyResource</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> WithLocalizedMessage<T, TProperty>(
      this IRuleBuilderOptions<T, TProperty> rule,
      Expression<Func<string>> resourceSelector)
    {
      return rule.WithLocalizedMessage<T, TProperty>(resourceSelector, (IResourceAccessorBuilder) new StaticResourceAccessorBuilder());
    }

    /// <summary>
    /// Specifies a custom error message resource to use when validation fails.
    /// </summary>
    /// <param name="rule">The current rule</param>
    /// <param name="resourceSelector">The resource to use as an expression, eg () =&gt; Messages.MyResource</param>
    /// <param name="formatArgs">Custom message format args</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> WithLocalizedMessage<T, TProperty>(
      this IRuleBuilderOptions<T, TProperty> rule,
      Expression<Func<string>> resourceSelector,
      params object[] formatArgs)
    {
      Func<T, object>[] arrayOfDelegates = DefaultValidatorOptions.ConvertArrayOfObjectsToArrayOfDelegates<T>(formatArgs);
      return DefaultValidatorOptions.WithLocalizedMessage<T, TProperty>(rule, resourceSelector, arrayOfDelegates);
    }

    /// <summary>
    /// Specifies a custom error message resource to use when validation fails.
    /// </summary>
    /// <param name="rule">The current rule</param>
    /// <param name="resourceSelector">The resource to use as an expression, eg () =&gt; Messages.MyResource</param>
    /// <param name="formatArgs">Custom message format args</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> WithLocalizedMessage<T, TProperty>(
      this IRuleBuilderOptions<T, TProperty> rule,
      Expression<Func<string>> resourceSelector,
      params Func<T, object>[] formatArgs)
    {
      return rule.WithLocalizedMessage<T, TProperty>(resourceSelector, (IResourceAccessorBuilder) new StaticResourceAccessorBuilder()).Configure((Action<PropertyRule>) (cfg => ((IEnumerable<Func<T, object>>) formatArgs).Select<Func<T, object>, Func<object, object, object>>((Func<Func<T, object>, Func<object, object, object>>) (func => (Func<object, object, object>) ((instance, value) => func((T) instance)))).ForEach<Func<object, object, object>>(new Action<Func<object, object, object>>(cfg.CurrentValidator.CustomMessageFormatArguments.Add))));
    }

    /// <summary>
    /// Specifies a custom error message resource to use when validation fails.
    /// </summary>
    /// <param name="rule">The current rule</param>
    /// <param name="resourceSelector">The resource to use as an expression, eg () =&gt; Messages.MyResource</param>
    /// <param name="resourceAccessorBuilder">The resource accessor builder to use. </param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> WithLocalizedMessage<T, TProperty>(
      this IRuleBuilderOptions<T, TProperty> rule,
      Expression<Func<string>> resourceSelector,
      IResourceAccessorBuilder resourceAccessorBuilder)
    {
      ArgumentUtility.CheckNotNull ("resourceSelector", resourceSelector);

      return rule.Configure((Action<PropertyRule>) (config => config.CurrentValidator.ErrorMessageSource = LocalizedStringSource.CreateFromExpression(resourceSelector, resourceAccessorBuilder)));
    }

    /// <summary>
    /// Specifies a condition limiting when the validator should run.
    /// The validator will only be executed if the result of the lambda returns true.
    /// </summary>
    /// <param name="rule">The current rule</param>
    /// <param name="predicate">A lambda expression that specifies a condition for when the validator should run</param>
    /// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> When<T, TProperty>(
      this IRuleBuilderOptions<T, TProperty> rule,
      Func<T, bool> predicate,
      ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
    {
      ArgumentUtility.CheckNotNull ("predicate", predicate);

      return rule.Configure((Action<PropertyRule>) (config => config.ApplyCondition(Extensions.CoerceToNonGeneric (predicate), applyConditionTo)));
    }

    /// <summary>
    /// Specifies a condition limiting when the validator should not run.
    /// The validator will only be executed if the result of the lambda returns false.
    /// </summary>
    /// <param name="rule">The current rule</param>
    /// <param name="predicate">A lambda expression that specifies a condition for when the validator should not run</param>
    /// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> Unless<T, TProperty>(
      this IRuleBuilderOptions<T, TProperty> rule,
      Func<T, bool> predicate,
      ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators)
    {
      ArgumentUtility.CheckNotNull ("predicate", predicate);

      return rule.When<T, TProperty>((Func<T, bool>) (x => !predicate(x)), applyConditionTo);
    }

    /// <summary>
    /// Specifies a custom property name to use within the error message.
    /// </summary>
    /// <param name="rule">The current rule</param>
    /// <param name="overridePropertyName">The property name to use</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> WithName<T, TProperty>(
      this IRuleBuilderOptions<T, TProperty> rule,
      string overridePropertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("overridePropertyName", overridePropertyName);

      return rule.Configure((Action<PropertyRule>) (config => config.DisplayName = (IStringSource) new StaticStringSource(overridePropertyName)));
    }

    /// <summary>Specifies a localized name for the error message.</summary>
    /// <param name="rule">The current rule</param>
    /// <param name="resourceSelector">The resource to use as an expression, eg () =&gt; Messages.MyResource</param>
    /// <param name="resourceAccessorBuilder">Resource accessor builder to use</param>
    public static IRuleBuilderOptions<T, TProperty> WithLocalizedName<T, TProperty>(
      this IRuleBuilderOptions<T, TProperty> rule,
      Expression<Func<string>> resourceSelector,
      IResourceAccessorBuilder resourceAccessorBuilder = null)
    {
      ArgumentUtility.CheckNotNull ("resourceSelector", resourceSelector);

      resourceAccessorBuilder = resourceAccessorBuilder ?? (IResourceAccessorBuilder) new StaticResourceAccessorBuilder();
      return rule.Configure((Action<PropertyRule>) (config => config.DisplayName = LocalizedStringSource.CreateFromExpression(resourceSelector, resourceAccessorBuilder)));
    }

    /// <summary>
    /// Overrides the name of the property associated with this rule.
    /// NOTE: This is a considered to be an advanced feature. 99% of the time that you use this, you actually meant to use WithName.
    /// </summary>
    /// <param name="rule">The current rule</param>
    /// <param name="propertyName">The property name to use</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> OverridePropertyName<T, TProperty>(
      this IRuleBuilderOptions<T, TProperty> rule,
      string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      return rule.Configure((Action<PropertyRule>) (config => config.PropertyName = propertyName));
    }

    /// <summary>
    /// Overrides the name of the property associated with this rule.
    /// NOTE: This is a considered to be an advanced feature. 99% of the time that you use this, you actually meant to use WithName.
    /// </summary>
    /// <param name="rule">The current rule</param>
    /// <param name="propertyName">The property name to use</param>
    /// <returns></returns>
    [Obsolete("WithPropertyName has been deprecated. If you wish to set the name of the property within the error message, use 'WithName'. If you actually intended to change which property this rule was declared against, use 'OverridePropertyName' instead.")]
    public static IRuleBuilderOptions<T, TProperty> WithPropertyName<T, TProperty>(
      this IRuleBuilderOptions<T, TProperty> rule,
      string propertyName)
    {
      return rule.OverridePropertyName<T, TProperty>(propertyName);
    }

    /// <summary>
    /// Specifies custom state that should be stored alongside the validation message when validation fails for this rule.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="rule"></param>
    /// <param name="stateProvider"></param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> WithState<T, TProperty>(
      this IRuleBuilderOptions<T, TProperty> rule,
      Func<T, object> stateProvider)
    {
      ArgumentUtility.CheckNotNull ("stateProvider", stateProvider);

      return rule.Configure((Action<PropertyRule>) (config => config.CurrentValidator.CustomStateProvider = Extensions.CoerceToNonGeneric (stateProvider)));
    }

    private static Func<T, object>[] ConvertArrayOfObjectsToArrayOfDelegates<T>(object[] objects)
    {
      if (objects == null || objects.Length == 0)
        return new Func<T, object>[0];
      return ((IEnumerable<object>) objects).Select<object, Func<T, object>>((Func<object, Func<T, object>>) (obj => (Func<T, object>) (x => obj))).ToArray<Func<T, object>>();
    }

    private static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
      foreach (T obj in source)
        action(obj);
    }

  }
}
