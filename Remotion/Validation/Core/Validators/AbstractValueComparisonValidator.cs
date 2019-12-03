// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.AbstractComparisonValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using Remotion.Utilities;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Validators
{
  public abstract class AbstractValueComparisonValidator : PropertyValidator, IValueComparisonValidator
  {
    public object ValueToCompare { get; }

    protected AbstractValueComparisonValidator (IComparable value, string errorMessage, IValidationMessage validationMessage)
        : base (errorMessage, validationMessage)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      ValueToCompare = value;

      //TODO RM-5906: Add System.IComparer, similar to NotEqualValidator with System.IEqualsComparer
      //TODO RM-5906: Refactor to do comparison based on enum and use derived types only to indicate type etc.
    }

    public abstract Comparison Comparison { get; }

    protected abstract bool IsValid (IComparable value, IComparable valueToCompare);

    protected sealed override bool IsValid (PropertyValidatorContext context)
    {
      if (context.PropertyValue == null)
        return true;

      var comparisonValue = (IComparable) ValueToCompare;
      if (IsValid ((IComparable) context.PropertyValue, comparisonValue))
        return true;

      context.MessageFormatter.AppendArgument ("ComparisonValue", comparisonValue);
      return false;
    }
  }
}