// Decompiled with JetBrains decompiler
// Type: FluentValidation.CascadeMode
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

namespace Remotion.Validation.Implementation
{
  /// <summary>Specifies how rules should cascade when one fails.</summary>
  public enum CascadeMode
  {
    /// <summary>
    /// When a rule fails, execution continues to the next rule.
    /// </summary>
    Continue,

    /// <summary>
    /// When a rule fails, validation is stopped and all other rules in the chain will not be executed.
    /// </summary>
    StopOnFirstFailure,
  }
}