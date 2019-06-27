// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.EqualValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation.Resources;
using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Remotion.Validation.Validators
{
  public class EqualValidator : PropertyValidator, IComparisonValidator, IPropertyValidator
  {
    private readonly Func<object, object> func;
    private readonly IEqualityComparer comparer;

    public EqualValidator(object valueToCompare)
      : base((Expression<Func<string>>) (() => Messages.equal_error))
    {
      this.ValueToCompare = valueToCompare;
    }

    public EqualValidator(object valueToCompare, IEqualityComparer comparer)
      : base((Expression<Func<string>>) (() => Messages.equal_error))
    {
      this.ValueToCompare = valueToCompare;
      this.comparer = comparer;
    }

    public EqualValidator(Func<object, object> comparisonProperty, MemberInfo member)
      : base((Expression<Func<string>>) (() => Messages.equal_error))
    {
      this.func = comparisonProperty;
      this.MemberToCompare = member;
    }

    public EqualValidator(
      Func<object, object> comparisonProperty,
      MemberInfo member,
      IEqualityComparer comparer)
      : base((Expression<Func<string>>) (() => Messages.equal_error))
    {
      this.func = comparisonProperty;
      this.MemberToCompare = member;
      this.comparer = comparer;
    }

    protected override bool IsValid(PropertyValidatorContext context)
    {
      object comparisonValue = this.GetComparisonValue(context);
      if (this.Compare(comparisonValue, context.PropertyValue))
        return true;
      context.MessageFormatter.AppendArgument("ComparisonValue", comparisonValue);
      return false;
    }

    private object GetComparisonValue(PropertyValidatorContext context)
    {
      if (this.func != null)
        return this.func(context.Instance);
      return this.ValueToCompare;
    }

    public FluentValidation.Validators.Comparison Comparison
    {
      get
      {
        return FluentValidation.Validators.Comparison.Equal;
      }
    }

    public MemberInfo MemberToCompare { get; private set; }

    public object ValueToCompare { get; private set; }

    protected bool Compare(object comparisonValue, object propertyValue)
    {
      if (this.comparer != null)
        return this.comparer.Equals(comparisonValue, propertyValue);
      if (comparisonValue is IComparable && propertyValue is IComparable)
        return FluentValidation.Internal.Comparer.GetEqualsResult((IComparable) comparisonValue, (IComparable) propertyValue);
      return comparisonValue == propertyValue;
    }
  }
}
