// Decompiled with JetBrains decompiler
// Type: FluentValidation.Internal.MemberNameValidatorSelector
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Validation.Rules;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Selects validators that are associated with a particular property.
  /// </summary>
  public class MemberNameValidatorSelector : IValidatorSelector
  {
    private readonly IEnumerable<string> memberNames;

    /// <summary>
    /// Creates a new instance of MemberNameValidatorSelector.
    /// </summary>
    public MemberNameValidatorSelector(IEnumerable<string> memberNames)
    {
      this.memberNames = memberNames;
    }

    /// <summary>Determines whether or not a rule should execute.</summary>
    /// <param name="rule">The rule</param>
    /// <param name="propertyPath">Property path (eg Customer.Address.Line1)</param>
    /// <param name="context">Contextual information</param>
    /// <returns>Whether or not the validator can execute.</returns>
    public bool CanExecute(IValidationRule rule, string propertyPath, ValidationContext context)
    {
      if (!context.IsChildContext)
        return this.memberNames.Any<string>((Func<string, bool>)(x =>
        {
          if (!(x == propertyPath))
            return propertyPath.StartsWith(x + ".");
          return true;
        }));
      return true;
    }

    /// <summary>
    ///  Creates a MemberNameValidatorSelector from a collection of expressions.
    /// </summary>
    public static MemberNameValidatorSelector FromExpressions<T>(
      params Expression<Func<T, object>>[] propertyExpressions)
    {
      return new MemberNameValidatorSelector((IEnumerable<string>)((IEnumerable<Expression<Func<T, object>>>)propertyExpressions).Select<Expression<Func<T, object>>, string>(new Func<Expression<Func<T, object>>, string>(MemberNameValidatorSelector.MemberFromExpression<T>)).ToList<string>());
    }

    private static string MemberFromExpression<T>(Expression<Func<T, object>> expression)
    {
      PropertyChain propertyChain = PropertyChain.FromExpression((LambdaExpression)expression);
      if (propertyChain.Count == 0)
        throw new ArgumentException(string.Format("Expression '{0}' does not specify a valid property or field.", (object)expression));
      return propertyChain.ToString();
    }
  }
}