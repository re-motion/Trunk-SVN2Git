// Decompiled with JetBrains decompiler
// Type: FluentValidation.Validators.IComparisonValidator
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\Remotion\trunk-svn2git\packages\FluentValidation-Signed.5.0.0.1\lib\Net40\FluentValidation.dll

using System.Reflection;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Validators
{
  public interface IComparisonValidator : IPropertyValidator
  {
    Comparison Comparison { get; }

    MemberInfo MemberToCompare { get; }

    object ValueToCompare { get; }
  }
}