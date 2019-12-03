// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.NotEqualValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Collections;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Validators
{
  public class NotEqualValidator : PropertyValidator, IValueComparisonValidator
  {
    public object ValueToCompare { get; }
    private IEqualityComparer Comparer { get; }

    public NotEqualValidator (object comparisonValue, IEqualityComparer comparer = null, IValidationMessage validationMessage= null)
        : base (Constants.NotEqualError, validationMessage ?? new InvariantValidationMessage (Constants.NotEqualError))
    {
      ValueToCompare = comparisonValue;
      Comparer = comparer;
    }

    protected override bool IsValid (PropertyValidatorContext context)
    {
      var comparisonValue = ValueToCompare;
      if (!Compare (comparisonValue, context.PropertyValue))
        return true;

      context.MessageFormatter.AppendArgument ("ComparisonValue", comparisonValue);
      return false;
    }

    private bool Compare (object comparisonValue, object propertyValue)
    {
      if (Comparer != null)
        return Comparer.Equals (comparisonValue, propertyValue);

      if (comparisonValue is IComparable value && propertyValue is IComparable compare)
        return Remotion.Validation.Implementation.Comparer.GetEqualsResult (value, compare);

      return comparisonValue == propertyValue;
    }
  }
}