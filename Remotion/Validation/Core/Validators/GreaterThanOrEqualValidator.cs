// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.GreaterThanOrEqualValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation.Internal;
using FluentValidation.Resources;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Remotion.Validation.Validators
{
  public class GreaterThanOrEqualValidator : AbstractComparisonValidator
  {
    public GreaterThanOrEqualValidator(IComparable value)
        : base(value, (Expression<Func<string>>) (() => Messages.greaterthanorequal_error))
    {
    }

    public GreaterThanOrEqualValidator(Func<object, object> valueToCompareFunc, MemberInfo member)
        : base(valueToCompareFunc, member, (Expression<Func<string>>) (() => Messages.greaterthanorequal_error))
    {
    }

    public override bool IsValid(IComparable value, IComparable valueToCompare)
    {
      return Comparer.GetComparisonResult(value, valueToCompare) >= 0;
    }

    public override FluentValidation.Validators.Comparison Comparison
    {
      get
      {
        return FluentValidation.Validators.Comparison.GreaterThanOrEqual;
      }
    }
  }
}