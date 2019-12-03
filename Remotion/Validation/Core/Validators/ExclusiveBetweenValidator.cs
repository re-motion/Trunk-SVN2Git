// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.ExclusiveBetweenValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Validators
{
  public class ExclusiveBetweenValidator : PropertyValidator, IBetweenValidator
  {
    public ExclusiveBetweenValidator (IComparable from, IComparable to, IValidationMessage validationMessage = null)
        : base (Constants.ExclusiveBetweenError, validationMessage ?? new InvariantValidationMessage (Constants.ExclusiveBetweenError))
    {
      To = to;
      From = from;
      if (Comparer.GetComparisonResult (to, from) == -1)
        throw new ArgumentOutOfRangeException (nameof (to), "To should be larger than from.");
    }

    public IComparable From { get; }

    public IComparable To { get; }

    protected override bool IsValid (PropertyValidatorContext context)
    {
      var propertyValue = (IComparable) context.PropertyValue;
      if (propertyValue == null || Comparer.GetComparisonResult (propertyValue, From) > 0 && Comparer.GetComparisonResult (propertyValue, To) < 0)
        return true;

      context.MessageFormatter.AppendArgument ("From", From).AppendArgument ("To", To).AppendArgument ("Value", context.PropertyValue);
      return false;
    }
  }
}