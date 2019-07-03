// Decompiled with JetBrains decompiler
// Type: FluentValidation.Resources.LocalizedStringSource
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;
using System.Linq.Expressions;

namespace Remotion.Validation.Implementation
{
  /// <summary>Represents a localized string.</summary>
  public class LocalizedStringSource : IStringSource
  {
    private readonly Func<string> _accessor;
    private readonly Type _resourceType;
    private readonly string _resourceName;

    /// <summary>
    /// Creates a new instance of the LocalizedErrorMessageSource class using the specified resource name and resource type.
    /// </summary>
    /// <param name="resourceType">The resource type</param>
    /// <param name="resourceName">The resource name</param>
    /// <param name="resourceAccessorBuilder">Strategy used to construct the resource accessor</param>
    public LocalizedStringSource (Type resourceType, string resourceName, IResourceAccessorBuilder resourceAccessorBuilder)
    {
      _resourceType = resourceType;
      _resourceName = resourceName;
      _accessor = resourceAccessorBuilder.GetResourceAccessor (resourceType, resourceName);
    }

    /// <summary>
    /// Creates an IErrorMessageSource from an expression: () =&gt; MyResources.SomeResourceName
    /// </summary>
    /// <param name="expression">The expression </param>
    /// <param name="resourceProviderSelectionStrategy">Strategy used to construct the resource accessor</param>
    /// <returns>Error message source</returns>
    public static IStringSource CreateFromExpression (Expression<Func<string>> expression, IResourceAccessorBuilder resourceProviderSelectionStrategy)
    {
      if (expression.Body is ConstantExpression body)
        return new StaticStringSource ((string) body.Value);

      var member = expression.GetMember();
      if (member == null)
        throw new InvalidOperationException ("Only MemberExpressions an be passed to BuildResourceAccessor, eg () => Messages.MyResource");

      return new LocalizedStringSource (member.DeclaringType, member.Name, resourceProviderSelectionStrategy);
    }

    /// <summary>Construct the error message template</summary>
    /// <returns>Error message template</returns>
    public string GetString ()
    {
      return _accessor();
    }

    /// <summary>The name of the resource if localized.</summary>
    public string ResourceName
    {
      get { return _resourceName; }
    }

    /// <summary>The type of the resource provider if localized.</summary>
    public Type ResourceType
    {
      get { return _resourceType; }
    }
  }
}