// Decompiled with JetBrains decompiler
// Type: FluentValidation.Internal.Extensions
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Remotion.Validation.Utilities
{
  /// <summary>Useful extensions</summary>
  public static class Extensions
  {
    /// <summary>Gets a MemberInfo from a member expression.</summary>
    public static MemberInfo GetMember<T, TProperty> (this Expression<Func<T, TProperty>> expression)
    {
      return RemoveUnary (expression.Body)?.Member;
    }

    private static MemberExpression RemoveUnary (Expression toUnwrap)
    {
      if (toUnwrap is UnaryExpression expression)
        return expression.Operand as MemberExpression;

      return toUnwrap as MemberExpression;
    }

    /// <summary>
    /// Splits pascal case, so "FooBar" would become "Foo Bar"
    /// </summary>
    public static string SplitPascalCase (this string input)
    {
      if (string.IsNullOrEmpty (input))
        return input;

      return Regex.Replace (input, "([A-Z])", " $1").Trim();
    }

    public static Func<object, object> CoerceToNonGeneric<T, TProperty> (this Func<T, TProperty> func)
    {
      return x => func ((T) x) as object;
    }

    public static Func<object, bool> CoerceToNonGeneric<T> (this Func<T, bool> func)
    {
      return x => func ((T) x);
    }
  }
}