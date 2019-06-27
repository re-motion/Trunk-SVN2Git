// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.AbstractComparisonValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Validation.Validators
{
  public abstract class AbstractComparisonValidator : PropertyValidator, IComparisonValidator, IPropertyValidator
  {
    private readonly Func<object, object> valueToCompareFunc;

    protected AbstractComparisonValidator(
      IComparable value,
      Expression<Func<string>> errorMessageSelector)
      : base(errorMessageSelector)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      this.ValueToCompare = (object) value;
    }

    protected AbstractComparisonValidator(
      Func<object, object> valueToCompareFunc,
      MemberInfo member,
      Expression<Func<string>> errorMessageSelector)
      : base(errorMessageSelector)
    {
      this.valueToCompareFunc = valueToCompareFunc;
      this.MemberToCompare = member;
    }

    protected override sealed bool IsValid(PropertyValidatorContext context)
    {
      if (context.PropertyValue == null)
        return true;
      IComparable comparisonValue = this.GetComparisonValue(context);
      if (this.IsValid((IComparable) context.PropertyValue, comparisonValue))
        return true;
      context.MessageFormatter.AppendArgument("ComparisonValue", (object) comparisonValue);
      return false;
    }

    private IComparable GetComparisonValue(PropertyValidatorContext context)
    {
      if (this.valueToCompareFunc != null)
        return (IComparable) this.valueToCompareFunc(context.Instance);
      return (IComparable) this.ValueToCompare;
    }

    public abstract bool IsValid(IComparable value, IComparable valueToCompare);

    public abstract FluentValidation.Validators.Comparison Comparison { get; }

    public MemberInfo MemberToCompare { get; private set; }

    public object ValueToCompare { get; private set; }
  }
}
