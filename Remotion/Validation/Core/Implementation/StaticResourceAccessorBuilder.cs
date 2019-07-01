// Decompiled with JetBrains decompiler
// Type: FluentValidation.Resources.StaticResourceAccessorBuilder
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;
using System.Reflection;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Builds a delegate for retrieving a localised resource from a resource type and property name.
  /// </summary>
  public class StaticResourceAccessorBuilder : IResourceAccessorBuilder
  {
    /// <summary>Builds a function used to retrieve the resource.</summary>
    public virtual Func<string> GetResourceAccessor(Type resourceType, string resourceName)
    {
      PropertyInfo resourceProperty = this.GetResourceProperty(ref resourceType, ref resourceName);
      if (resourceProperty == (PropertyInfo)null)
        throw new InvalidOperationException(string.Format("Could not find a property named '{0}' on type '{1}'.", (object)resourceName, (object)resourceType));
      if (resourceProperty.PropertyType != typeof(string))
        throw new InvalidOperationException(string.Format("Property '{0}' on type '{1}' does not return a string", (object)resourceName, (object)resourceType));
      return (Func<string>)Delegate.CreateDelegate(typeof(Func<string>), resourceProperty.GetGetMethod());
    }

    /// <summary>
    /// Gets the PropertyInfo for a resource.
    /// ResourceType and ResourceName are ref parameters to allow derived types
    /// to replace the type/name of the resource before the delegate is constructed.
    /// </summary>
    protected virtual PropertyInfo GetResourceProperty(
      ref Type resourceType,
      ref string resourceName)
    {
      return resourceType.GetProperty(resourceName, BindingFlags.Static | BindingFlags.Public);
    }
  }
}