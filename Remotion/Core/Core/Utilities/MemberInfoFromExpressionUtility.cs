// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Remotion.Utilities
{
  /// <summary>
  /// Provides typed access to the reflection objects for members referenced in <see cref="Expression"/> instances.
  /// </summary>
  public static class MemberInfoFromExpressionUtility
  {
    private const BindingFlags AllBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    public static FieldInfo GetField<TFieldType> (Expression<Func<TFieldType>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      return GetFieldInfoFromMemberExpression (expression.Body);
    }

    public static FieldInfo GetField<TSourceObject, TFieldType> (Expression<Func<TSourceObject, TFieldType>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      return GetFieldInfoFromMemberExpression (expression.Body);
    }

    public static ConstructorInfo GetConstructor<TType> (Expression<Func<TType>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      return GetConstructorInfoFromNewExpression (expression.Body);
    }

    public static MethodInfo GetMethod (Expression<Action> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      return GetMethodInfoFromMethodCallExpression (null, expression.Body);
    }

    public static MethodInfo GetMethod<TReturnType> (Expression<Func<TReturnType>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      return GetMethodInfoFromMethodCallExpression (null, expression.Body);
    }

    public static MethodInfo GetMethod<TSourceObject> (Expression<Action<TSourceObject>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      return GetMethodInfoFromMethodCallExpression (typeof (TSourceObject), expression.Body);
    }

    public static MethodInfo GetMethod<TSourceObject, TReturnType> (Expression<Func<TSourceObject, TReturnType>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      return GetMethodInfoFromMethodCallExpression (typeof (TSourceObject), expression.Body);
    }

    public static MethodInfo GetGenericMethodDefinition (Expression<Action> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      return GetGenericMethodDefinition(null, expression.Body);
    }

    public static MethodInfo GetGenericMethodDefinition<TReturnType> (Expression<Func<TReturnType>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      return GetGenericMethodDefinition (null, expression.Body);
    }

    public static MethodInfo GetGenericMethodDefinition<TSourceObject> (Expression<Action<TSourceObject>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      return GetGenericMethodDefinition (typeof (TSourceObject), expression.Body);
    }

    public static MethodInfo GetGenericMethodDefinition<TSourceObject, TReturnType> (Expression<Func<TSourceObject, TReturnType>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      return GetGenericMethodDefinition (typeof(TSourceObject), expression.Body);
    }

    public static PropertyInfo GetProperty<TPropertyType> (Expression<Func<TPropertyType>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      return GetPropertyInfoFromMemberExpression (null, expression.Body);
    }

    public static PropertyInfo GetProperty<TSourceObject, TPropertyType> (Expression<Func<TSourceObject, TPropertyType>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      return GetPropertyInfoFromMemberExpression (typeof (TSourceObject), expression.Body);
    }

    private static T GetTypedMemberInfoFromMemberExpression<T> (Expression expression, string memberType)
        where T: MemberInfo
    {
      var memberExpression = expression as MemberExpression;
      if (memberExpression == null)
        throw new ArgumentException ("Must be a MemberExpression.", "expression");

      var member = memberExpression.Member as T;
      if (member == null)
      {
        var message = string.Format ("Must hold a {0} access expression.", memberType);
        throw new ArgumentException (message, "expression");
      }

      return member;
    }  

    private static FieldInfo GetFieldInfoFromMemberExpression (Expression expression)
    {
      return GetTypedMemberInfoFromMemberExpression<FieldInfo> (expression, "field");
    }

    private static PropertyInfo GetPropertyInfoFromMemberExpression (Type sourceObjectType, Expression expression)
    {
      var property = GetTypedMemberInfoFromMemberExpression<PropertyInfo> (expression, "property");

      return property;
      // For redeclared properties (overridden in C#) the MemberExpression contains the root definition.
      //if (sourceObjectType == null || sourceObjectType == property.DeclaringType)
      //  return property;

      //// TODO 4957: Retrieve most derived overriding property.
    }  

    private static ConstructorInfo GetConstructorInfoFromNewExpression (Expression expression)
    {
      var newExpression = expression as NewExpression;
      if (newExpression == null)
        throw new ArgumentException ("Must be a NewExpression.", "expression");

      return newExpression.Constructor;
    }

    private static MethodInfo GetMethodInfoFromMethodCallExpression (Type sourceObjectType, Expression expression)
    {
      var methodCallExpression = expression as MethodCallExpression;
      if (methodCallExpression == null)
        throw new ArgumentException ("Must be a MethodCallExpression.", "expression");

      // For virtual methods the MethodCallExpression containts the root definition.
      var method = methodCallExpression.Method;

      if (sourceObjectType == null)
        return method;

      Type[] genericMethodArguments = null;
      if (method.IsGenericMethod && !method.IsGenericMethodDefinition)
      {
        genericMethodArguments = method.GetGenericArguments();
        method = method.GetGenericMethodDefinition();
      }

      // TODO 4957: Use MemberInfoEqualityComparer instead of equals
      var methodOnSourceType = sourceObjectType.GetMethods (AllBindingFlags).Single (m => m.GetBaseDefinition ().Equals (method.GetBaseDefinition ()));

      if (genericMethodArguments != null)
        return methodOnSourceType.MakeGenericMethod (genericMethodArguments);

      return methodOnSourceType;
    }

    private static MethodInfo GetGenericMethodDefinition (Type sourceObjectType, Expression expression)
    {
      var methodInfo = GetMethodInfoFromMethodCallExpression (sourceObjectType, expression);
      if (!methodInfo.IsGenericMethod)
        throw new ArgumentException ("Must hold a generic method access expression.", "expression");

      return methodInfo.GetGenericMethodDefinition ();
    }
  }
}