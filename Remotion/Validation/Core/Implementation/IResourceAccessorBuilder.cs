// Decompiled with JetBrains decompiler
// Type: FluentValidation.Resources.IResourceAccessorBuilder
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Builds a delegate for retrieving a localised resource from a resource type and property name.
  /// </summary>
  public interface IResourceAccessorBuilder
  {
    /// <summary>
    /// Gets a function that can be used to retrieve a message from a resource type and resource name.
    /// </summary>
    Func<string> GetResourceAccessor (Type resourceType, string resourceName);
  }
}