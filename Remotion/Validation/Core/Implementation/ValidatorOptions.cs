// Decompiled with JetBrains decompiler
// Type: FluentValidation.ValidatorOptions
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Remotion.Validation.Implementation
{
  public static class ValidatorOptions
  {
    public static CascadeMode CascadeMode = CascadeMode.Continue;
    private static Func<Type, MemberInfo, LambdaExpression, string> propertyNameResolver = new Func<Type, MemberInfo, LambdaExpression, string>(ValidatorOptions.DefaultPropertyNameResolver);
    private static Func<Type, MemberInfo, LambdaExpression, string> displayNameResolver = new Func<Type, MemberInfo, LambdaExpression, string>(ValidatorOptions.DefaultDisplayNameResolver);
    public static Type ResourceProviderType;

    public static Func<Type, MemberInfo, LambdaExpression, string> PropertyNameResolver
    {
      get
      {
        return ValidatorOptions.propertyNameResolver;
      }
      set
      {
        ValidatorOptions.propertyNameResolver = value ?? new Func<Type, MemberInfo, LambdaExpression, string>(ValidatorOptions.DefaultPropertyNameResolver);
      }
    }

    public static Func<Type, MemberInfo, LambdaExpression, string> DisplayNameResolver
    {
      get
      {
        return ValidatorOptions.displayNameResolver;
      }
      set
      {
        ValidatorOptions.displayNameResolver = value ?? new Func<Type, MemberInfo, LambdaExpression, string>(ValidatorOptions.DefaultDisplayNameResolver);
      }
    }

    private static string DefaultPropertyNameResolver(
      Type type,
      MemberInfo memberInfo,
      LambdaExpression expression)
    {
      if (expression != null)
      {
        PropertyChain propertyChain = PropertyChain.FromExpression(expression);
        if (propertyChain.Count > 0)
          return propertyChain.ToString();
      }
      if (memberInfo != (MemberInfo)null)
        return memberInfo.Name;
      return (string)null;
    }

    private static string DefaultDisplayNameResolver(
      Type type,
      MemberInfo memberInfo,
      LambdaExpression expression)
    {
      if (memberInfo == (MemberInfo)null)
        return (string)null;
      return ValidatorOptions.GetDisplayName(memberInfo);
    }

    private static string GetDisplayName(MemberInfo member)
    {
      var list = ((IEnumerable<object>) member.GetCustomAttributes (true)).Select (
          attr => new
      {
        attr = attr,
        type = attr.GetType()
      }).ToList();
      string str = list.Where(attr => attr.type.Name == "DisplayAttribute").Select(attr => new
      {
        attr = attr,
        method = attr.type.GetMethod("GetName", BindingFlags.Instance | BindingFlags.Public)
      }).Where(_param0 => _param0.method != (MethodInfo)null).Select(_param0 => _param0.method.Invoke(_param0.attr.attr, (object[])null) as string).FirstOrDefault<string>();
      if (string.IsNullOrEmpty(str))
        str = list.Where(attr => attr.type.Name == "DisplayNameAttribute").Select(attr => new
        {
          attr = attr,
          property = attr.type.GetProperty("DisplayName", BindingFlags.Instance | BindingFlags.Public)
        }).Where(_param0 => _param0.property != (PropertyInfo)null).Select(_param0 => _param0.property.GetValue(_param0.attr.attr, (object[])null) as string).FirstOrDefault<string>();
      return str;
    }
  }
}