// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.EqualValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Collections;
using System.Reflection;
using Remotion.Validation.Implementation;
using Comparer = Remotion.Validation.Implementation.Comparer;

namespace Remotion.Validation.Validators
{
  public class EqualValidator : PropertyValidator, IComparisonValidator
  {
    private readonly Func<object, object> _func;
    private readonly IEqualityComparer _comparer;

    public EqualValidator (object valueToCompare)
        : base (() => Constants.EqualError)
    {
      ValueToCompare = valueToCompare;
    }

    public EqualValidator (Func<object, object> comparisonProperty, MemberInfo member)
        : base (() => Constants.EqualError)
    {
      _func = comparisonProperty;
      MemberToCompare = member;
    }

    public EqualValidator (
        Func<object, object> comparisonProperty,
        MemberInfo member,
        IEqualityComparer comparer)
        : base (() => Constants.EqualError)
    {
      _func = comparisonProperty;
      MemberToCompare = member;
      _comparer = comparer;
    }

    protected override bool IsValid (PropertyValidatorContext context)
    {
      var comparisonValue = GetComparisonValue (context);
      if (Compare (comparisonValue, context.PropertyValue))
        return true;

      context.MessageFormatter.AppendArgument ("ComparisonValue", comparisonValue);
      return false;
    }

    private object GetComparisonValue (PropertyValidatorContext context)
    {
      if (_func != null)
        return _func (context.Instance);

      return ValueToCompare;
    }

    public Comparison Comparison
    {
      get { return Comparison.Equal; }
    }

    public MemberInfo MemberToCompare { get; }

    public object ValueToCompare { get; }

    private bool Compare (object comparisonValue, object propertyValue)
    {
      if (_comparer != null)
        return _comparer.Equals (comparisonValue, propertyValue);

      if (comparisonValue is IComparable && propertyValue is IComparable)
        return Comparer.GetEqualsResult ((IComparable) comparisonValue, (IComparable) propertyValue);

      return comparisonValue == propertyValue;
    }
  }
}