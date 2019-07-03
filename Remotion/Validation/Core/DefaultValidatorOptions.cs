// Decompiled with JetBrains decompiler
// Type: FluentValidation.DefaultValidatorOptions
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.RuleBuilders;

namespace Remotion.Validation
{
  /// <summary>
  /// Default options that can be used to configure a validator.
  /// </summary>
  public static class DefaultValidatorOptions
  {
    /// <summary>
    /// Specifies a custom error message to use if validation fails.
    /// </summary>
    /// <param name="rule">The current rule</param>
    /// <param name="errorMessage">The error message to use</param>
    /// <param name="funcs">Additional property values to be included when formatting the custom error message.</param>
    /// <returns></returns>
    public static IRuleBuilderOptions<T, TProperty> WithMessage<T, TProperty> (
        this IRuleBuilderOptions<T, TProperty> rule,
        string errorMessage,
        params Func<T, object>[] funcs)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("errorMessage", errorMessage);

      return rule.Configure (
          config =>
          {
            config.CurrentValidator.ErrorMessageSource = new StaticStringSource (errorMessage);
            funcs
                .Select (func => (Func<object, object, object>) ((instance, value) => func ((T) instance)))
                .ForEach (config.CurrentValidator.CustomMessageFormatArguments.Add);
          });
    }

    private static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
      foreach (var obj in source)
        action(obj);
    }
  }
}
