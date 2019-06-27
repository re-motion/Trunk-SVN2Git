// Decompiled with JetBrains decompiler
// Type: FluentValidation.IRuleBuilder`2
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using Remotion.Validation.Validators;

namespace Remotion.Validation.RuleBuilders
{
  /// <summary>Rule builder</summary>
  /// <typeparam name="T"></typeparam>
  /// <typeparam name="TProperty"></typeparam>
  public interface IRuleBuilder<T, out TProperty> : FluentValidation.Internal.IFluentInterface
  {
    /// <summary>
    /// Associates a validator with this the property for this rule builder.
    /// </summary>
    /// <param name="validator">The validator to set</param>
    /// <returns></returns>
    IRuleBuilderOptions<T, TProperty> SetValidator(IPropertyValidator validator);

    /// <summary>
    /// Associates an instance of IValidator with the current property rule.
    /// </summary>
    /// <param name="validator">The validator to use</param>
    IRuleBuilderOptions<T, TProperty> SetValidator(IValidator<TProperty> validator);
  }
}