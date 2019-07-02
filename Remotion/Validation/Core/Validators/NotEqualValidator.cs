// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.NotEqualValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Validation.Implementation;
using Comparer = Remotion.Validation.Implementation.Comparer;

namespace Remotion.Validation.Validators
{
  public class NotEqualValidator : PropertyValidator, IComparisonValidator, IPropertyValidator
  {
    private readonly IEqualityComparer comparer;
    private readonly Func<object, object> func;

    public NotEqualValidator(Func<object, object> func, MemberInfo memberToCompare)
      : base((Expression<Func<string>>) (() => Constants.NotEqualError))
    {
      this.func = func;
      this.MemberToCompare = memberToCompare;
    }

    public NotEqualValidator(
      Func<object, object> func,
      MemberInfo memberToCompare,
      IEqualityComparer equalityComparer)
      : base((Expression<Func<string>>) (() => Constants.NotEqualError))
    {
      this.func = func;
      this.comparer = equalityComparer;
      this.MemberToCompare = memberToCompare;
    }

    public NotEqualValidator(object comparisonValue)
      : base((Expression<Func<string>>) (() => Constants.NotEqualError))
    {
      this.ValueToCompare = comparisonValue;
    }

    public NotEqualValidator(object comparisonValue, IEqualityComparer equalityComparer)
      : base((Expression<Func<string>>) (() => Constants.NotEqualError))
    {
      this.ValueToCompare = comparisonValue;
      this.comparer = equalityComparer;
    }

    protected override bool IsValid(PropertyValidatorContext context)
    {
      object comparisonValue = this.GetComparisonValue(context);
      if (!this.Compare(comparisonValue, context.PropertyValue))
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

    public Comparison Comparison
    {
      get
      {
        return Comparison.NotEqual;
      }
    }

    public MemberInfo MemberToCompare { get; private set; }

    public object ValueToCompare { get; private set; }

    protected bool Compare(object comparisonValue, object propertyValue)
    {
      if (this.comparer != null)
        return this.comparer.Equals(comparisonValue, propertyValue);
      if (comparisonValue is IComparable && propertyValue is IComparable)
        return Comparer.GetEqualsResult((IComparable) comparisonValue, (IComparable) propertyValue);
      return comparisonValue == propertyValue;
    }
  }
}
