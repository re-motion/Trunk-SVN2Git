// Decompiled with JetBrains decompiler
// Type: FluentValidation.Resources.FallbackAwareResourceAccessorBuilder
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;
using System.Reflection;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Implemenetation of IResourceAccessorBuilder that can fall back to the default resource provider.
  /// </summary>
  public class FallbackAwareResourceAccessorBuilder : StaticResourceAccessorBuilder
  {
    /// <summary>
    /// Gets the PropertyInfo for a resource.
    /// ResourceType and ResourceName are ref parameters to allow derived types
    /// to replace the type/name of the resource before the delegate is constructed.
    /// </summary>
    protected override PropertyInfo GetResourceProperty (ref Type resourceType, ref string resourceName)
    {
      if (ValidatorOptions.ResourceProviderType == null) 
        return base.GetResourceProperty (ref resourceType, ref resourceName);

      var property = ValidatorOptions.ResourceProviderType.GetProperty (resourceName, BindingFlags.Static | BindingFlags.Public);
      if (property == null) 
        return base.GetResourceProperty (ref resourceType, ref resourceName);

      resourceType = ValidatorOptions.ResourceProviderType;
      return property;
    }
  }
}