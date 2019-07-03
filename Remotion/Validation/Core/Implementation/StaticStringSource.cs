// Decompiled with JetBrains decompiler
// Type: FluentValidation.Resources.StaticStringSource
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;

namespace Remotion.Validation.Implementation
{
  /// <summary>Represents a static string.</summary>
  public class StaticStringSource : IStringSource
  {
    private readonly string _message;

    /// <summary>
    /// Creates a new StringErrorMessageSource using the specified error message as the error template.
    /// </summary>
    /// <param name="message">The error message template.</param>
    public StaticStringSource (string message)
    {
      _message = message;
    }

    /// <summary>Construct the error message template</summary>
    /// <returns>Error message template</returns>
    public string GetString ()
    {
      return _message;
    }

    /// <summary>The name of the resource if localized.</summary>
    public string ResourceName
    {
      get { return null; }
    }

    /// <summary>The type of the resource provider if localized.</summary>
    public Type ResourceType
    {
      get { return null; }
    }
  }
}