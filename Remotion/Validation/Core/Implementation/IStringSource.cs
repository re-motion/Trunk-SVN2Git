// Decompiled with JetBrains decompiler
// Type: FluentValidation.Resources.IStringSource
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;

namespace Remotion.Validation.Implementation
{
  /// <summary>Provides error message templates</summary>
  public interface IStringSource
  {
    /// <summary>Construct the error message template</summary>
    /// <returns>Error message template</returns>
    string GetString ();

    /// <summary>The name of the resource if localized.</summary>
    string ResourceName { get; }

    /// <summary>The type of the resource provider if localized.</summary>
    Type ResourceType { get; }
  }
}