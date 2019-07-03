// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.AbstractComparisonValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Validators
{
  public abstract class AbstractComparisonValidator : PropertyValidator, IComparisonValidator
  {
    private readonly Func<object, object> _valueToCompareFunc;

    protected AbstractComparisonValidator (IComparable value, Expression<Func<string>> errorMessageSelector)
        : base (errorMessageSelector)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      ValueToCompare = value;
    }

    protected AbstractComparisonValidator (Func<object, object> valueToCompareFunc, MemberInfo member, Expression<Func<string>> errorMessageSelector)
        : base (errorMessageSelector)
    {
      _valueToCompareFunc = valueToCompareFunc;
      MemberToCompare = member;
    }

    protected sealed override bool IsValid (PropertyValidatorContext context)
    {
      if (context.PropertyValue == null)
        return true;

      var comparisonValue = GetComparisonValue (context);
      if (IsValid ((IComparable) context.PropertyValue, comparisonValue))
        return true;

      context.MessageFormatter.AppendArgument ("ComparisonValue", comparisonValue);
      return false;
    }

    private IComparable GetComparisonValue (PropertyValidatorContext context)
    {
      if (_valueToCompareFunc != null)
        return (IComparable) _valueToCompareFunc (context.Instance);

      return (IComparable) ValueToCompare;
    }

    protected abstract bool IsValid (IComparable value, IComparable valueToCompare);

    public abstract Comparison Comparison { get; }

    public MemberInfo MemberToCompare { get; }

    public object ValueToCompare { get; }
  }
}