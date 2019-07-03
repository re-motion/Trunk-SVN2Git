// Decompiled with JetBrains decompiler
// Type: FluentValidation.ValidatorOptions
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Remotion.Validation.Implementation
{
  public static class ValidatorOptions
  {
    public static CascadeMode CascadeMode = CascadeMode.Continue;
    public static Type ResourceProviderType;
    private static readonly Func<Type, MemberInfo, LambdaExpression, string> s_propertyNameResolver = DefaultPropertyNameResolver;
    private static readonly Func<Type, MemberInfo, LambdaExpression, string> s_displayNameResolver = DefaultDisplayNameResolver;

    public static Func<Type, MemberInfo, LambdaExpression, string> PropertyNameResolver
    {
      get { return s_propertyNameResolver; }
    }

    public static Func<Type, MemberInfo, LambdaExpression, string> DisplayNameResolver
    {
      get { return s_displayNameResolver; }
    }

    private static string DefaultPropertyNameResolver (Type type, MemberInfo memberInfo, LambdaExpression expression)
    {
      if (expression != null)
      {
        var propertyChain = PropertyChain.FromExpression (expression);
        if (propertyChain.Count > 0)
          return propertyChain.ToString();
      }

      if (memberInfo != null)
        return memberInfo.Name;

      return null;
    }

    private static string DefaultDisplayNameResolver (Type type, MemberInfo memberInfo, LambdaExpression expression)
    {
      if (memberInfo == null)
        return null;

      return GetDisplayName (memberInfo);
    }

    private static string GetDisplayName (MemberInfo member)
    {
      var list = member
          .GetCustomAttributes (true)
          .Select (
              attr => new
                      {
                          attr,
                          type = attr.GetType()
                      })
          .ToList();

      var str = list
          .Where (attr => attr.type.Name == "DisplayAttribute")
          .Select (
              attr => new
                      {
                          attr,
                          method = attr.type.GetMethod ("GetName", BindingFlags.Instance | BindingFlags.Public)
                      })
          .Where (_param0 => _param0.method != null as MethodInfo)
          .Select (_param0 => _param0.method.Invoke (_param0.attr.attr, null) as string)
          .FirstOrDefault();

      if (string.IsNullOrEmpty (str))
      {
        str = list
            .Where (attr => attr.type.Name == "DisplayNameAttribute")
            .Select (
                attr => new
                        {
                            attr,
                            property = attr.type.GetProperty ("DisplayName", BindingFlags.Instance | BindingFlags.Public)
                        })
            .Where (_param0 => _param0.property != null as PropertyInfo)
            .Select (_param0 => _param0.property.GetValue (_param0.attr.attr, null) as string)
            .FirstOrDefault();
      }
      return str;
    }
  }
}