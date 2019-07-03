// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.NotEqualValidator
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
  public class NotEqualValidator : PropertyValidator, IComparisonValidator
  {
    private readonly IEqualityComparer _comparer;
    private readonly Func<object, object> _func;

    public NotEqualValidator(Func<object, object> func, MemberInfo memberToCompare)
        : base(() => Constants.NotEqualError)
    {
      _func = func;
      MemberToCompare = memberToCompare;
    }

    public NotEqualValidator(Func<object, object> func, MemberInfo memberToCompare, IEqualityComparer equalityComparer)
        : base(() => Constants.NotEqualError)
    {
      _func = func;
      _comparer = equalityComparer;
      MemberToCompare = memberToCompare;
    }

    public NotEqualValidator (object comparisonValue)
        : base (() => Constants.NotEqualError)
    {
      ValueToCompare = comparisonValue;
    }

    public NotEqualValidator (object comparisonValue, IEqualityComparer equalityComparer)
        : base (() => Constants.NotEqualError)
    {
      ValueToCompare = comparisonValue;
      _comparer = equalityComparer;
    }

    protected override bool IsValid (PropertyValidatorContext context)
    {
      var comparisonValue = GetComparisonValue (context);
      if (!Compare (comparisonValue, context.PropertyValue))
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

    public MemberInfo MemberToCompare { get; }

    public object ValueToCompare { get; }

    private bool Compare (object comparisonValue, object propertyValue)
    {
      if (_comparer != null)
        return _comparer.Equals (comparisonValue, propertyValue);

      if (comparisonValue is IComparable value && propertyValue is IComparable compare)
        return Comparer.GetEqualsResult (value, compare);

      return comparisonValue == propertyValue;
    }
  }
}