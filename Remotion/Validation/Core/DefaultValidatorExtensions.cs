// Decompiled with JetBrains decompiler
// Type: FluentValidation.DefaultValidatorExtensions
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Results;
using Remotion.Validation.Validators;
using DefaultValidatorSelector = Remotion.Validation.Validators.DefaultValidatorSelector;
using IValidatorSelector = Remotion.Validation.Validators.IValidatorSelector;

namespace Remotion.Validation
{
  /// <summary>
  /// Extension methods that provide the default set of validators.
  /// </summary>
  public static class DefaultValidatorExtensions
  {
    /// <summary>
    /// Defines a 'not null' validator on the current rule builder.
    /// Validation will fail if the property is null.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> NotNull<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder)
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new NotNullValidator());
    }

    /// <summary>
    /// Defines a 'not empty' validator on the current rule builder.
    /// Validation will fail if the property is null, an empty or the default value for the type (for example, 0 for integers)
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> NotEmpty<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder)
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new NotEmptyValidator((object) default (TProperty)));
    }

    /// <summary>
    /// Defines a length validator on the current rule builder, but only for string properties.
    /// Validation will fail if the length of the string is outside of the specifed range. The range is inclusive.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="min">TODO</param>
    /// <param name="max">TODO</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, string> Length<T>(
      this RuleBuilders.IRuleBuilder<T, string> ruleBuilder,
      int min,
      int max)
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new LengthValidator(min, max));
    }

    /// <summary>
    /// Defines a length validator on the current rule builder, but only for string properties.
    /// Validation will fail if the length of the string is not equal to the length specified.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="exactLength">TODO</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, string> Length<T>(
      this RuleBuilders.IRuleBuilder<T, string> ruleBuilder,
      int exactLength)
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new ExactLengthValidator(exactLength));
    }

    /// <summary>
    /// Defines a regular expression validator on the current rule builder, but only for string properties.
    /// Validation will fail if the value returned by the lambda does not match the regular expression.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="expression">The regular expression to check the value against.</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, string> Matches<T>(
      this RuleBuilders.IRuleBuilder<T, string> ruleBuilder,
      string expression)
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new RegularExpressionValidator(expression));
    }

    /// <summary>
    /// Defines a regular expression validator on the current rule builder, but only for string properties.
    /// Validation will fail if the value returned by the lambda does not match the regular expression.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="regex">The regular expression to use</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, string> Matches<T>(
      this RuleBuilders.IRuleBuilder<T, string> ruleBuilder,
      Regex regex)
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new RegularExpressionValidator(regex));
    }

    /// <summary>
    /// Defines a regular expression validator on the current rule builder, but only for string properties.
    /// Validation will fail if the value returned by the lambda does not match the regular expression.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="expression">The regular expression to check the value against.</param>
    /// <param name="options">Regex options</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, string> Matches<T>(
      this RuleBuilders.IRuleBuilder<T, string> ruleBuilder,
      string expression,
      RegexOptions options)
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new RegularExpressionValidator(expression, options));
    }

    /// <summary>
    /// Defines a 'not equal' validator on the current rule builder.
    /// Validation will fail if the specified value is equal to the value of the property.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="toCompare">The value to compare</param>
    /// <param name="comparer">Equality comparer to use</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> NotEqual<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder,
      TProperty toCompare,
      IEqualityComparer comparer = null)
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new NotEqualValidator((object) toCompare, comparer));
    }

    /// <summary>
    /// Defines a 'not equal' validator on the current rule builder using a lambda to specify the value.
    /// Validation will fail if the value returned by the lambda is equal to the value of the property.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="expression">A lambda expression to provide the comparison value</param>
    /// <param name="comparer">Equality Comparer to use</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> NotEqual<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder,
      Expression<Func<T, TProperty>> expression,
      IEqualityComparer comparer = null)
    {
      Func<T, TProperty> func = expression.Compile();
      return ruleBuilder.SetValidator((IPropertyValidator) new NotEqualValidator(func.CoerceToNonGeneric<T, TProperty>(), expression.GetMember<T, TProperty>(), comparer));
    }

    /// <summary>
    /// Defines an 'equals' validator on the current rule builder.
    /// Validation will fail if the specified value is not equal to the value of the property.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="toCompare">The value to compare</param>
    /// <param name="comparer">Equality Comparer to use</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> Equal<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder,
      TProperty toCompare,
      IEqualityComparer comparer = null)
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new EqualValidator((object) toCompare, comparer));
    }

    /// <summary>
    /// Defines an 'equals' validator on the current rule builder using a lambda to specify the comparison value.
    /// Validation will fail if the value returned by the lambda is not equal to the value of the property.
    /// </summary>
    /// <typeparam name="T">The type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="expression">A lambda expression to provide the comparison value</param>
    /// <param name="comparer">Equality comparer to use</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> Equal<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder,
      Expression<Func<T, TProperty>> expression,
      IEqualityComparer comparer = null)
    {
      Func<T, TProperty> func = expression.Compile();
      return ruleBuilder.SetValidator((IPropertyValidator) new EqualValidator(func.CoerceToNonGeneric<T, TProperty>(), expression.GetMember<T, TProperty>(), comparer));
    }

    /// <summary>
    /// Defines a predicate validator on the current rule builder using a lambda expression to specify the predicate.
    /// Validation will fail if the specified lambda returns false.
    /// Validation will succeed if the specifed lambda returns true.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="predicate">A lambda expression specifying the predicate</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> Must<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder,
      Func<TProperty, bool> predicate)
    {
      ArgumentUtility.CheckNotNull ("predicate", predicate);

      return Must<T, TProperty>((RuleBuilders.IRuleBuilder<T, TProperty>) ruleBuilder, (Func<T, TProperty, bool>) ((x, val) => predicate(val)));
    }

    /// <summary>
    /// Defines a predicate validator on the current rule builder using a lambda expression to specify the predicate.
    /// Validation will fail if the specified lambda returns false.
    /// Validation will succeed if the specifed lambda returns true.
    /// This overload accepts the object being validated in addition to the property being validated.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="predicate">A lambda expression specifying the predicate</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> Must<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder,
      Func<T, TProperty, bool> predicate)
    {
      ArgumentUtility.CheckNotNull ("predicate", predicate);

      return Must<T, TProperty>((RuleBuilders.IRuleBuilder<T, TProperty>) ruleBuilder, (Func<T, TProperty, PropertyValidatorContext, bool>) ((x, val, propertyValidatorContext) => predicate(x, val)));
    }

    /// <summary>
    /// Defines a predicate validator on the current rule builder using a lambda expression to specify the predicate.
    /// Validation will fail if the specified lambda returns false.
    /// Validation will succeed if the specifed lambda returns true.
    /// This overload accepts the object being validated in addition to the property being validated.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="predicate">A lambda expression specifying the predicate</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> Must<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder,
      Func<T, TProperty, PropertyValidatorContext, bool> predicate)
    {
      ArgumentUtility.CheckNotNull ("predicate", predicate);

      throw new NotImplementedException ("TODO");
      //return ruleBuilder.SetValidator((IPropertyValidator) new PredicateValidator((PredicateValidator.Predicate) ((instance, property, propertyValidatorContext) => predicate((T) instance, (TProperty) property, propertyValidatorContext))));
    }

    /// <summary>
    /// Defines a 'less than' validator on the current rule builder.
    /// The validation will succeed if the property value is less than the specified value.
    /// The validation will fail if the property value is greater than or equal to the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="valueToCompare">The value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> LessThan<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder,
      TProperty valueToCompare)
      where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new LessThanValidator((IComparable) valueToCompare));
    }

    /// <summary>
    /// Defines a 'less than' validator on the current rule builder.
    /// The validation will succeed if the property value is less than the specified value.
    /// The validation will fail if the property value is greater than or equal to the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="valueToCompare">The value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty?> LessThan<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty?> ruleBuilder,
      TProperty valueToCompare)
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new LessThanValidator((IComparable) valueToCompare));
    }

    /// <summary>
    /// Defines a 'less than or equal' validator on the current rule builder.
    /// The validation will succeed if the property value is less than or equal to the specified value.
    /// The validation will fail if the property value is greater than the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="valueToCompare">The value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> LessThanOrEqualTo<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder,
      TProperty valueToCompare)
      where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new LessThanOrEqualValidator((IComparable) valueToCompare));
    }

    /// <summary>
    /// Defines a 'less than or equal' validator on the current rule builder.
    /// The validation will succeed if the property value is less than or equal to the specified value.
    /// The validation will fail if the property value is greater than the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="valueToCompare">The value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty?> LessThanOrEqualTo<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty?> ruleBuilder,
      TProperty valueToCompare)
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new LessThanOrEqualValidator((IComparable) valueToCompare));
    }

    /// <summary>
    /// Defines a 'greater than' validator on the current rule builder.
    /// The validation will succeed if the property value is greater than the specified value.
    /// The validation will fail if the property value is less than or equal to the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="valueToCompare">The value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> GreaterThan<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder,
      TProperty valueToCompare)
      where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new GreaterThanValidator((IComparable) valueToCompare));
    }

    /// <summary>
    /// Defines a 'greater than' validator on the current rule builder.
    /// The validation will succeed if the property value is greater than the specified value.
    /// The validation will fail if the property value is less than or equal to the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="valueToCompare">The value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty?> GreaterThan<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty?> ruleBuilder,
      TProperty valueToCompare)
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new GreaterThanValidator((IComparable) valueToCompare));
    }

    /// <summary>
    /// Defines a 'greater than or equal' validator on the current rule builder.
    /// The validation will succeed if the property value is greater than or equal the specified value.
    /// The validation will fail if the property value is less than the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="valueToCompare">The value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> GreaterThanOrEqualTo<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder,
      TProperty valueToCompare)
      where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new GreaterThanOrEqualValidator((IComparable) valueToCompare));
    }

    /// <summary>
    /// Defines a 'greater than or equal' validator on the current rule builder.
    /// The validation will succeed if the property value is greater than or equal the specified value.
    /// The validation will fail if the property value is less than the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="valueToCompare">The value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty?> GreaterThanOrEqualTo<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty?> ruleBuilder,
      TProperty valueToCompare)
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new GreaterThanOrEqualValidator((IComparable) valueToCompare));
    }

    /// <summary>
    /// Defines a 'less than' validator on the current rule builder using a lambda expression.
    /// The validation will succeed if the property value is less than the specified value.
    /// The validation will fail if the property value is greater than or equal to the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="expression">A lambda that should return the value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> LessThan<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder,
      Expression<Func<T, TProperty>> expression)
      where TProperty : IComparable<TProperty>, IComparable
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      Func<T, TProperty> func = expression.Compile();
      return ruleBuilder.SetValidator((IPropertyValidator) new LessThanValidator(func.CoerceToNonGeneric<T, TProperty>(), expression.GetMember<T, TProperty>()));
    }

    /// <summary>
    /// Defines a 'less than' validator on the current rule builder using a lambda expression.
    /// The validation will succeed if the property value is less than the specified value.
    /// The validation will fail if the property value is greater than or equal to the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="expression">A lambda that should return the value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty?> LessThan<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty?> ruleBuilder,
      Expression<Func<T, TProperty>> expression)
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      ArgumentUtility.CheckNotNull ("expression", expression);

      Func<T, TProperty> func = expression.Compile();
      return ruleBuilder.SetValidator((IPropertyValidator) new LessThanValidator(func.CoerceToNonGeneric<T, TProperty>(), expression.GetMember<T, TProperty>()));
    }

    /// <summary>
    /// Defines a 'less than or equal' validator on the current rule builder using a lambda expression.
    /// The validation will succeed if the property value is less than or equal to the specified value.
    /// The validation will fail if the property value is greater than the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="expression">The value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> LessThanOrEqualTo<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder,
      Expression<Func<T, TProperty>> expression)
      where TProperty : IComparable<TProperty>, IComparable
    {
      Func<T, TProperty> func = expression.Compile();
      return ruleBuilder.SetValidator((IPropertyValidator) new LessThanOrEqualValidator(func.CoerceToNonGeneric<T, TProperty>(), expression.GetMember<T, TProperty>()));
    }

    /// <summary>
    /// Defines a 'less than or equal' validator on the current rule builder using a lambda expression.
    /// The validation will succeed if the property value is less than or equal to the specified value.
    /// The validation will fail if the property value is greater than the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="expression">The value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty?> LessThanOrEqualTo<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty?> ruleBuilder,
      Expression<Func<T, TProperty>> expression)
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      Func<T, TProperty> func = expression.Compile();
      return ruleBuilder.SetValidator((IPropertyValidator) new LessThanOrEqualValidator(func.CoerceToNonGeneric<T, TProperty>(), expression.GetMember<T, TProperty>()));
    }

    /// <summary>
    /// Defines a 'less than or equal' validator on the current rule builder using a lambda expression.
    /// The validation will succeed if the property value is less than or equal to the specified value.
    /// The validation will fail if the property value is greater than the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="expression">The value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty?> LessThanOrEqualTo<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty?> ruleBuilder,
      Expression<Func<T, TProperty?>> expression)
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      Func<T, TProperty?> func = expression.Compile();
      return ruleBuilder.SetValidator((IPropertyValidator) new LessThanOrEqualValidator(func.CoerceToNonGeneric<T, TProperty?>(), expression.GetMember<T, TProperty?>()));
    }

    /// <summary>
    /// Defines a 'less than' validator on the current rule builder using a lambda expression.
    /// The validation will succeed if the property value is greater than the specified value.
    /// The validation will fail if the property value is less than or equal to the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="expression">The value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> GreaterThan<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder,
      Expression<Func<T, TProperty>> expression)
      where TProperty : IComparable<TProperty>, IComparable
    {
      Func<T, TProperty> func = expression.Compile();
      return ruleBuilder.SetValidator((IPropertyValidator) new GreaterThanValidator(func.CoerceToNonGeneric<T, TProperty>(), expression.GetMember<T, TProperty>()));
    }

    /// <summary>
    /// Defines a 'less than' validator on the current rule builder using a lambda expression.
    /// The validation will succeed if the property value is greater than the specified value.
    /// The validation will fail if the property value is less than or equal to the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="expression">The value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty?> GreaterThan<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty?> ruleBuilder,
      Expression<Func<T, TProperty>> expression)
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      Func<T, TProperty> func = expression.Compile();
      return ruleBuilder.SetValidator((IPropertyValidator) new GreaterThanValidator(func.CoerceToNonGeneric<T, TProperty>(), expression.GetMember<T, TProperty>()));
    }

    /// <summary>
    /// Defines a 'less than' validator on the current rule builder using a lambda expression.
    /// The validation will succeed if the property value is greater than or equal the specified value.
    /// The validation will fail if the property value is less than the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="valueToCompare">The value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> GreaterThanOrEqualTo<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder,
      Expression<Func<T, TProperty>> valueToCompare)
      where TProperty : IComparable<TProperty>, IComparable
    {
      Func<T, TProperty> func = valueToCompare.Compile();
      return ruleBuilder.SetValidator((IPropertyValidator) new GreaterThanOrEqualValidator(func.CoerceToNonGeneric<T, TProperty>(), valueToCompare.GetMember<T, TProperty>()));
    }

    /// <summary>
    /// Defines a 'greater than or equal to' validator on the current rule builder using a lambda expression.
    /// The validation will succeed if the property value is greater than or equal the specified value.
    /// The validation will fail if the property value is less than the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="valueToCompare">The value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty?> GreaterThanOrEqualTo<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty?> ruleBuilder,
      Expression<Func<T, TProperty?>> valueToCompare)
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      Func<T, TProperty?> func = valueToCompare.Compile();
      return ruleBuilder.SetValidator((IPropertyValidator) new GreaterThanOrEqualValidator(func.CoerceToNonGeneric<T, TProperty?>(), valueToCompare.GetMember<T, TProperty?>()));
    }

    /// <summary>
    /// Defines a 'greater than or equal to' validator on the current rule builder using a lambda expression.
    /// The validation will succeed if the property value is greater than or equal the specified value.
    /// The validation will fail if the property value is less than the specified value.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="valueToCompare">The value being compared</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty?> GreaterThanOrEqualTo<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty?> ruleBuilder,
      Expression<Func<T, TProperty>> valueToCompare)
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      Func<T, TProperty> func = valueToCompare.Compile();
      return ruleBuilder.SetValidator((IPropertyValidator) new GreaterThanOrEqualValidator(func.CoerceToNonGeneric<T, TProperty>(), valueToCompare.GetMember<T, TProperty>()));
    }

    /// <summary>
    /// Validates certain properties of the specified instance.
    /// </summary>
    /// <param name="validator">The current validator</param>
    /// <param name="instance">The object to validate</param>
    /// <param name="propertyExpressions">Expressions to specify the properties to validate</param>
    /// <returns>A ValidationResult object containing any validation failures</returns>
    public static ValidationResult Validate<T>(
      this IValidator<T> validator,
      T instance,
      params Expression<Func<T, object>>[] propertyExpressions)
    {
      ValidationContext<T> validationContext = new ValidationContext<T>(instance, new PropertyChain(), (IValidatorSelector) MemberNameValidatorSelector.FromExpressions<T>(propertyExpressions));
      return validator.Validate((ValidationContext) validationContext);
    }

    /// <summary>
    /// Validates certain properties of the specified instance.
    /// </summary>
    /// <param name="validator">TODO</param>
    /// <param name="instance">The object to validate</param>
    /// <param name="properties">The names of the properties to validate.</param>
    /// <returns>A ValidationResult object containing any validation failures.</returns>
    public static ValidationResult Validate<T>(
      this IValidator<T> validator,
      T instance,
      params string[] properties)
    {
      ValidationContext<T> validationContext = new ValidationContext<T>(instance, new PropertyChain(), (IValidatorSelector) new MemberNameValidatorSelector((IEnumerable<string>) properties));
      return validator.Validate((ValidationContext) validationContext);
    }

    public static ValidationResult Validate<T>(
      this IValidator<T> validator,
      T instance,
      IValidatorSelector selector = null,
      string ruleSet = null)
    {
      if (selector != null && ruleSet != null)
        throw new InvalidOperationException("Cannot specify both an IValidatorSelector and a RuleSet.");
      if (selector == null)
        selector = (IValidatorSelector) new DefaultValidatorSelector();
      if (ruleSet != null)
        selector = (IValidatorSelector) new RulesetValidatorSelector(ruleSet.Split(',', ';'));
      ValidationContext<T> validationContext = new ValidationContext<T>(instance, new PropertyChain(), selector);
      return validator.Validate((ValidationContext) validationContext);
    }

    /// <summary>
    /// Performs validation and then throws an exception if validation fails.
    /// </summary>
    public static void ValidateAndThrow<T>(this IValidator<T> validator, T instance)
    {
      ValidationResult validationResult = validator.Validate(instance);
      if (!validationResult.IsValid)
        throw new ValidationException((IEnumerable<ValidationFailure>) validationResult.Errors);
    }

    /// <summary>
    /// Defines an 'inclusive between' validator on the current rule builder, but only for properties of types that implement IComparable.
    /// Validation will fail if the value of the property is outside of the specifed range. The range is inclusive.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="from">The lowest allowed value</param>
    /// <param name="to">The highest allowed value</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> InclusiveBetween<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder,
      TProperty from,
      TProperty to)
      where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new InclusiveBetweenValidator((IComparable) from, (IComparable) to));
    }

    /// <summary>
    /// Defines an 'inclusive between' validator on the current rule builder, but only for properties of types that implement IComparable.
    /// Validation will fail if the value of the property is outside of the specifed range. The range is inclusive.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="from">The lowest allowed value</param>
    /// <param name="to">The highest allowed value</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty?> InclusiveBetween<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty?> ruleBuilder,
      TProperty from,
      TProperty to)
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new InclusiveBetweenValidator((IComparable) from, (IComparable) to));
    }

    /// <summary>
    /// Defines an 'exclusive between' validator on the current rule builder, but only for properties of types that implement IComparable.
    /// Validation will fail if the value of the property is outside of the specifed range. The range is exclusive.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="from">The lowest allowed value</param>
    /// <param name="to">The highest allowed value</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty> ExclusiveBetween<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty> ruleBuilder,
      TProperty from,
      TProperty to)
      where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new ExclusiveBetweenValidator((IComparable) from, (IComparable) to));
    }

    /// <summary>
    /// Defines an 'exclusive between' validator on the current rule builder, but only for properties of types that implement IComparable.
    /// Validation will fail if the value of the property is outside of the specifed range. The range is exclusive.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="from">The lowest allowed value</param>
    /// <param name="to">The highest allowed value</param>
    /// <returns></returns>
    public static RuleBuilders.IRuleBuilderOptions<T, TProperty?> ExclusiveBetween<T, TProperty>(
      this RuleBuilders.IRuleBuilder<T, TProperty?> ruleBuilder,
      TProperty from,
      TProperty to)
      where TProperty : struct, IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator((IPropertyValidator) new ExclusiveBetweenValidator((IComparable) from, (IComparable) to));
    }

    private static Func<object, object> CoerceToNonGeneric<T, TProperty>(
        this Func<T, TProperty> func)
    {
      //TODO: Move/replace, step 1: Utilities with internal
      return (Func<object, object>) (x => (object) func((T) x));
    }

    /// <summary>Gets a MemberInfo from a member expression.</summary>
    public static MemberInfo GetMember(this LambdaExpression expression)
    {
      //TODO: Move/replace, step 1: Utilities with internal
      return RemoveUnary(expression.Body)?.Member;
    }

    /// <summary>Gets a MemberInfo from a member expression.</summary>
    public static MemberInfo GetMember<T, TProperty>(
        this Expression<Func<T, TProperty>> expression)
    {
      //TODO: Move/replace, step 1: Utilities with internal
      return RemoveUnary(expression.Body)?.Member;
    }

    private static MemberExpression RemoveUnary(Expression toUnwrap)
    {
      //TODO: Move/replace, step 1: Utilities with internal
      if (toUnwrap is UnaryExpression)
        return ((UnaryExpression) toUnwrap).Operand as MemberExpression;
      return toUnwrap as MemberExpression;
    }
  }
}
