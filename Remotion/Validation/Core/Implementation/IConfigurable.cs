// Decompiled with JetBrains decompiler
// Type: FluentValidation.Internal.IConfigurable`2
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;
using System.ComponentModel;

namespace Remotion.Validation.Implementation
{
  /// <summary>Represents an object that is configurable.</summary>
  /// <typeparam name="TConfiguration">Type of object being configured</typeparam>
  /// <typeparam name="TNext">Return type</typeparam>
  public interface IConfigurable<out TConfiguration, out TNext>
  {
    /// <summary>Configures the current object.</summary>
    /// <param name="configurator">Action to configure the object.</param>
    /// <returns></returns>
    [EditorBrowsable (EditorBrowsableState.Never)]
    TNext Configure (Action<TConfiguration> configurator);
  }
}