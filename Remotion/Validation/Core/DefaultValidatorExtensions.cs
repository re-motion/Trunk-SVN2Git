// Decompiled with JetBrains decompiler
// Type: FluentValidation.DefaultValidatorExtensions
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

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
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> NotNull<T, TProperty> (this IRuleBuilder<T, TProperty> ruleBuilder)
    {
      return ruleBuilder.SetValidator (new NotNullValidator());
    }

    /// <summary>
    /// Defines a 'not empty' validator on the current rule builder.
    /// Validation will fail if the property is null, an empty or the default value for the type (for example, 0 for integers)
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> NotEmpty<T, TProperty> (this IRuleBuilder<T, TProperty> ruleBuilder)
    {
      return ruleBuilder.SetValidator (new NotEmptyValidator (default (TProperty)));
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
    public static IRuleBuilderOptions<T, string> Length<T> (this IRuleBuilder<T, string> ruleBuilder, int min, int max)
    {
      return ruleBuilder.SetValidator (new LengthValidator (min, max));
    }

    /// <summary>
    /// Defines a regular expression validator on the current rule builder, but only for string properties.
    /// Validation will fail if the value returned by the lambda does not match the regular expression.
    /// </summary>
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="expression">The regular expression to check the value against.</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, string> Matches<T> (this IRuleBuilder<T, string> ruleBuilder, string expression)
    {
      return ruleBuilder.SetValidator (new RegularExpressionValidator (expression));
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
    public static IRuleBuilderOptions<T, TProperty> NotEqual<T, TProperty> (
        this IRuleBuilder<T, TProperty> ruleBuilder,
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
    /// <typeparam name="T">Type of object being validated</typeparam>
    /// <typeparam name="TProperty">Type of property being validated</typeparam>
    /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
    /// <param name="valueToCompare">The value being compared</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> LessThan<T, TProperty> (this IRuleBuilder<T, TProperty> ruleBuilder, TProperty valueToCompare)
        where TProperty : IComparable<TProperty>, IComparable
    {
      return ruleBuilder.SetValidator (new LessThanValidator (valueToCompare));
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
    public static IRuleBuilderOptions<T, TProperty> GreaterThan<T, TProperty> (this IRuleBuilder<T, TProperty> ruleBuilder, TProperty valueToCompare)
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
    public static MemberInfo GetMember<T, TProperty> (this Expression<Func<T, TProperty>> expression)
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