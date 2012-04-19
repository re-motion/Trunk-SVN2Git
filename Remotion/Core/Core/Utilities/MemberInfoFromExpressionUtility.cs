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
using System.Linq.Expressions;
using System.Reflection;

namespace Remotion.Utilities
{
  /// <summary>
  /// Provides typed access to the reflection objects for members referenced in <see cref="Expression"/> instances.
  /// </summary>
  public static class MemberInfoFromExpressionUtility
  {
    public static ConstructorInfo GetConstructor<T> (Expression<Func<T>> newExpression)
    {
      Assertion.IsTrue (newExpression.Body is NewExpression, "Parameter newExpression must be a NewExpression.");
      var constructor = ((NewExpression) newExpression.Body).Constructor;
      return constructor;
    }

    public static PropertyInfo GetProperty<T> (Expression<Func<T>> memberExpression)
    {
      Assertion.IsTrue (memberExpression.Body is MemberExpression, "Parameter memberExpression must be a MemberExpression.");
      var member = ((MemberExpression) memberExpression.Body).Member;
      Assertion.IsTrue (member is PropertyInfo, "Parameter memberExpression must hold a property access expression.");
      return (PropertyInfo) member;
    }

    public static PropertyInfo GetProperty<TSourceObject, TMemberType> (Expression<Func<TSourceObject, TMemberType>> memberExpression)
    {
      Assertion.IsTrue (memberExpression.Body is MemberExpression, "Parameter memberExpression must be a MemberExpression.");
      var member = ((MemberExpression) memberExpression.Body).Member;
      Assertion.IsTrue (member is PropertyInfo, "Parameter memberExpression must hold a property access expression.");
      return (PropertyInfo) member;
    }

    public static FieldInfo GetField<T> (Expression<Func<T>> memberExpression)
    {
      Assertion.IsTrue (memberExpression.Body is MemberExpression, "Parameter memberExpression must be a MemberExpression.");
      var member = ((MemberExpression) memberExpression.Body).Member;
      Assertion.IsTrue (member is FieldInfo, "Parameter memberExpression must hold a field access expression.");
      return (FieldInfo) member;
    }

    public static FieldInfo GetField<TSourceObject, TMemberType> (Expression<Func<TSourceObject, TMemberType>> memberExpression)
    {
      Assertion.IsTrue (memberExpression.Body is MemberExpression, "Parameter memberExpression must be a MemberExpression.");
      var member = ((MemberExpression) memberExpression.Body).Member;
      Assertion.IsTrue (member is FieldInfo, "Parameter memberExpression must hold a field access expression.");
      return (FieldInfo) member;
    }

    public static MethodInfo GetMethod<T> (Expression<Func<T>> methodCallExpression)
    {
      Assertion.IsTrue (methodCallExpression.Body is MethodCallExpression, "Parameter methodCallExpression must be a MethodCallExpression.");
      return ((MethodCallExpression) methodCallExpression.Body).Method;
    }

    public static MethodInfo GetMethod<TSourceObject, TMemberType> (Expression<Func<TSourceObject, TMemberType>> methodCallExpression)
    {
      Assertion.IsTrue (methodCallExpression.Body is MethodCallExpression, "Parameter methodCallExpression must be a MethodCallExpression.");
      return ((MethodCallExpression) methodCallExpression.Body).Method;
    }

    public static MethodInfo GetMethod<T> (Expression<Action<T>> methodCallExpression)
    {
      Assertion.IsTrue (methodCallExpression.Body is MethodCallExpression, "Parameter methodCallExpression must be a MethodCallExpression.");
      return ((MethodCallExpression) methodCallExpression.Body).Method;
    }

    public static MethodInfo GetMethod<TSourceObject, TMemberType> (Expression<Action<TSourceObject, TMemberType>> methodCallExpression)
    {
      Assertion.IsTrue (methodCallExpression.Body is MethodCallExpression, "Parameter methodCallExpression must be a MethodCallExpression.");
      return ((MethodCallExpression) methodCallExpression.Body).Method;
    }
  }
}