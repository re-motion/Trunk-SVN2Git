// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.Validators;

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
    /// <typeparam name="TValidatedType">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <returns></returns>
    public static IAddingComponentRuleBuilder<TValidatedType, TProperty> NotNull<TValidatedType, TProperty> (
        this IAddingComponentRuleBuilder<TValidatedType, TProperty> ruleBuilder)
    {
      return ruleBuilder.SetValidator (new NotNullValidator());
    }

    /// <summary>
    /// Defines a 'not empty' validator on the current rule builder.
    /// Validation will fail if the property is null, an empty or the default value for the type (for example, 0 for integers)
    /// </summary>
    /// <typeparam name="TValidatedType">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <returns></returns>
    public static IAddingComponentRuleBuilder<TValidatedType, TProperty> NotEmpty<TValidatedType, TProperty> (
        this IAddingComponentRuleBuilder<TValidatedType, TProperty> ruleBuilder)
    {
      return ruleBuilder.SetValidator (new NotEmptyValidator (default (TProperty)));
    }

    /// <summary>
    /// Defines a length validator on the current rule builder, but only for string properties.
    /// Validation will fail if the length of the string is outside of the specified range. The range is inclusive.
    /// </summary>
    /// <typeparam name="TValidatedType">Type of object being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="min">TODO</param>
    /// <param name="max">TODO</param>
    /// <returns></returns>
    public static IAddingComponentRuleBuilder<TValidatedType, string> Length<TValidatedType> (
        this IAddingComponentRuleBuilder<TValidatedType, string> ruleBuilder,
        int min,
        int max)
    {
      return ruleBuilder.SetValidator (new LengthValidator (min, max));
    }

    /// <summary>
    /// Defines a regular expression validator on the current rule builder, but only for string properties.
    /// Validation will fail if the value returned by the lambda does not match the regular expression.
    /// </summary>
    /// <typeparam name="TValidatedType">Type of object being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="expression">The regular expression to check the value against.</param>
    /// <returns></returns>
    public static IAddingComponentRuleBuilder<TValidatedType, string> Matches<TValidatedType> (
        this IAddingComponentRuleBuilder<TValidatedType, string> ruleBuilder,
        string expression)
    {
      return ruleBuilder.SetValidator (new RegularExpressionValidator (expression));
    }

    /// <summary>
    /// Defines a 'not equal' validator on the current rule builder.
    /// Validation will fail if the specified value is equal to the value of the property.
    /// </summary>
    /// <typeparam name="TValidatedType">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="toCompare">The value to compare</param>
    /// <param name="comparer">Equality comparer to use</param>
    /// <returns></returns>
    public static IAddingComponentRuleBuilder<TValidatedType, TProperty> NotEqual<TValidatedType, TProperty> (
        this IAddingComponentRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty toCompare,
        IEqualityComparer comparer = null)
    {
      return ruleBuilder.SetValidator (new NotEqualValidator (toCompare, comparer));
    }

    /// <summary>
    /// Defines a 'less than' validator on the current rule builder.
    /// The validation will succeed if the property value is less than the specified value.
    /// The validation will fail if the property value is greater than or equal to the specified value.
    /// </summary>
    /// <typeparam name="TValidatedType">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="valueToCompare">The value being compared</param>
    /// <returns></returns>
    public static IAddingComponentRuleBuilder<TValidatedType, TProperty> LessThan<TValidatedType, TProperty> (
        this IAddingComponentRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty valueToCompare)
        where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator (new LessThanValidator (valueToCompare));
    }

    /// <summary>
    /// Defines a 'greater than' validator on the current rule builder.
    /// The validation will succeed if the property value is greater than the specified value.
    /// The validation will fail if the property value is less than or equal to the specified value.
    /// </summary>
    /// <typeparam name="TValidatedType">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="valueToCompare">The value being compared</param>
    /// <returns></returns>
    public static IAddingComponentRuleBuilder<TValidatedType, TProperty> GreaterThan<TValidatedType, TProperty> (
        this IAddingComponentRuleBuilder<TValidatedType, TProperty> ruleBuilder,
        TProperty valueToCompare)
        where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator (new GreaterThanValidator (valueToCompare));
    }

    /// <summary>Gets a MemberInfo from a member expression.</summary>
    public static MemberInfo GetMember (this LambdaExpression expression)
    {
      //TODO: Move/replace, step 1: Utilities with internal
      return RemoveUnary (expression.Body)?.Member;
    }

    /// <summary>Gets a MemberInfo from a member expression.</summary>
    public static MemberInfo GetMember<TValidatedType, TProperty> (this Expression<Func<TValidatedType, TProperty>> expression)
    {
      //TODO: Move/replace, step 1: Utilities with internal
      return RemoveUnary (expression.Body)?.Member;
    }

    private static MemberExpression RemoveUnary (Expression toUnwrap)
    {
      //TODO: Move/replace, step 1: Utilities with internal
      if (toUnwrap is UnaryExpression expression)
        return expression.Operand as MemberExpression;

      return toUnwrap as MemberExpression;
    }
  }
}