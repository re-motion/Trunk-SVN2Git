// Decompiled with JetBrains decompiler
// Type: FluentValidation.ApplyConditionTo
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll
using System;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Specifies where a When/Unless condition should be applied
  /// </summary>
  public enum ApplyConditionTo
  {
    /// <summary>
    /// Applies the condition to all validators declared so far in the chain.
    /// </summary>
    AllValidators,
  }
}