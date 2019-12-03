// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.EqualValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using System.Collections;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Validators
{
  public class EqualValidator : PropertyValidator, IValueComparisonValidator
  {
    public object ValueToCompare { get; }
    private IEqualityComparer Comparer { get; }


    public EqualValidator (object comparisonValue, IEqualityComparer comparer = null, IValidationMessage validationMessage = null)
        : base (Constants.EqualError, validationMessage ?? new InvariantValidationMessage (Constants.EqualError))
    {
      ValueToCompare = comparisonValue;
      Comparer = comparer;

      //TODO RM-5906: Add System.IEqualityComparer, similar to NotNullValidator
      //TODO RM-5906: Refactor remove duplication with NotEqualsValidator.
    }

    protected override bool IsValid (PropertyValidatorContext context)
    {
      var comparisonValue = ValueToCompare;
      if (Compare (comparisonValue, context.PropertyValue))
        return true;

      context.MessageFormatter.AppendArgument ("ComparisonValue", comparisonValue);
      return false;
    }

    public Comparison Comparison
    {
      get { return Comparison.Equal; }
    }

    private bool Compare (object comparisonValue, object propertyValue)
    {
      if (comparisonValue is IComparable && propertyValue is IComparable)
        return Remotion.Validation.Implementation.Comparer.GetEqualsResult ((IComparable) comparisonValue, (IComparable) propertyValue);

      return comparisonValue == propertyValue;
    }
  }
}