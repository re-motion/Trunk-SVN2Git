// Decompiled with JetBrains decompiler
// Type: FluentValidation.Resources.LazyStringSource
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;

namespace Remotion.Validation.Implementation
{
  public class LazyStringSource : IStringSource
  {
    private readonly Func<string> _stringProvider;

    public LazyStringSource (Func<string> stringProvider)
    {
      _stringProvider = stringProvider;
    }

    public string GetString ()
    {
      return _stringProvider();
    }

    public string ResourceName
    {
      get { return null; }
    }

    public Type ResourceType
    {
      get { return null; }
    }
  }
}