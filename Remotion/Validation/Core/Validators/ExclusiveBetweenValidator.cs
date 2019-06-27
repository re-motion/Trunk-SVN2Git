// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.ExclusiveBetweenValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using FluentValidation.Internal;
using FluentValidation.Resources;
using System;
using System.Linq.Expressions;

namespace Remotion.Validation.Validators
{
  public class ExclusiveBetweenValidator : PropertyValidator, IBetweenValidator, IPropertyValidator
  {
    public ExclusiveBetweenValidator(IComparable from, IComparable to)
        : base((Expression<Func<string>>) (() => Messages.exclusivebetween_error))
    {
      this.To = to;
      this.From = from;
      if (Comparer.GetComparisonResult(to, from) == -1)
        throw new ArgumentOutOfRangeException(nameof (to), "To should be larger than from.");
    }

    public IComparable From { get; private set; }

    public IComparable To { get; private set; }

    protected override bool IsValid(PropertyValidatorContext context)
    {
      IComparable propertyValue = (IComparable) context.PropertyValue;
      if (propertyValue == null || Comparer.GetComparisonResult(propertyValue, this.From) > 0 && Comparer.GetComparisonResult(propertyValue, this.To) < 0)
        return true;
      context.MessageFormatter.AppendArgument("From", (object) this.From).AppendArgument("To", (object) this.To).AppendArgument("Value", context.PropertyValue);
      return false;
    }
  }
}