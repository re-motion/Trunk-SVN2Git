// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.LessThanValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Validators
{
  public class LessThanValidator : AbstractComparisonValidator
  {
    public LessThanValidator (IComparable value)
        : base (value, () => Constants.LessThanError)
    {
    }

    protected override bool IsValid (IComparable value, IComparable valueToCompare)
    {
      return Comparer.GetComparisonResult (value, valueToCompare) < 0;
    }

    public override Comparison Comparison
    {
      get { return Comparison.LessThan; }
    }
  }
}