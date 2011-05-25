// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Reflection;

namespace Remotion.Utilities
{
  /// <summary>
  /// Provides logic to compare two <see cref="MemberInfo"/> for logical equality, without considering the <see cref="MemberInfo.ReflectedType"/>.
  /// </summary>
  public class MemberInfoEqualityComparer<T> : IEqualityComparer<T> where T:MemberInfo
  {
    public static readonly MemberInfoEqualityComparer<T> Instance = new MemberInfoEqualityComparer<T> ();

    protected MemberInfoEqualityComparer () { }

    /// <summary>
    /// Checks two <see cref="MemberInfo"/> instances for logical equality, without considering the <see cref="MemberInfo.ReflectedType"/>.
    /// </summary>
    /// <param name="one">The left-hand side <see cref="MemberInfo"/> to compare.</param>
    /// <param name="two">The right-hand side <see cref="MemberInfo"/> to compare.</param>
    /// <returns>
    /// <para>
    /// True if the two <see cref="MemberInfo"/> objects are logically equivalent, ie., if they represent the same <see cref="MemberInfo"/>.
    /// This is very similar to the <see cref="object.Equals(object)"/> implementation of <see cref="MemberInfo"/> objects, but it ignores the
    /// <see cref="MemberInfo.ReflectedType"/> property. In effect, two members compare equal if they are declared by the same type and
    /// denote the same member on that type. For generic <see cref="MethodInfo"/> objects, the generic arguments must be the same.
    /// </para>
    /// <para>
    /// The idea for this method, but not the code, was taken from http://blogs.msdn.com/b/kingces/archive/2005/08/17/452774.aspx.
    /// </para>
    /// </returns>
    public bool Equals (T one, T two)
    {
      // Same reference => true of course
      if (ReferenceEquals (one, two))
        return true;

      if (ReferenceEquals (one, null) || ReferenceEquals (null, two))
        return false;

      // Types are always reference equals or not equal at all.
      if (one is Type || two is Type)
        return false;

      // Methods defined by concrete arrays (int[].Set (...) etc.) will always succeed in the checks above if they are equal; it doesn't seem to be 
      // possible to get two different MethodInfo references for the same array method. Therefore, return false if an array method got through the 
      // check. (Since array methods have no metadata tokens, the checks below wouldn't detect any differences.)

      // This ReSharper warning is not correct - MemberInfos can of course have null declaring types.
      // ReSharper disable ConditionIsAlwaysTrueOrFalse
      if (one.DeclaringType != null && one.DeclaringType.IsArray)
      // ReSharper restore ConditionIsAlwaysTrueOrFalse
        return false;

      // Equal members always have the same metadata token
      if (one.MetadataToken != two.MetadataToken)
        return false;

      // Equal members always have the same declaring type - if any!
      if (one.DeclaringType != two.DeclaringType)
        return false;

      // Equal members always have the same module
      if (one.Module != two.Module)
        return false;

      var oneAsMethodInfo = one as MethodInfo;
      var twoAsMethodInfo = two as MethodInfo;
      if (oneAsMethodInfo != null && twoAsMethodInfo != null && oneAsMethodInfo.IsGenericMethod)
      {
        var genericArgumentsOne = oneAsMethodInfo.GetGenericArguments ();
        var genericArgumentsTwo = twoAsMethodInfo.GetGenericArguments ();

        // No LINQ expression for performance reasons
        // ReSharper disable LoopCanBeConvertedToQuery
        for (int i = 0; i < genericArgumentsOne.Length; ++i)
        {
          if (genericArgumentsOne[i] != genericArgumentsTwo[i])
            return false;
        }
        // ReSharper restore LoopCanBeConvertedToQuery
      }

      return true;
    }

    /// <summary>
    /// Returns the hash code for the given <see cref="MemberInfo"/>. To calculate the hash code, the hash codes of the declaring type, 
    /// metadata token and module of the <see cref="MemberInfo"/> are combined. If the declaring type is an array, the name of the 
    /// <see cref="MemberInfo"/> is used instead of the metadata token.
    /// </summary>
    /// <param name="memberInfo">The <see cref="MemberInfo"/> for which the hash code should be calculated.</param>
    /// <returns>The calculated hash code of the <see cref="MemberInfo"/>.</returns>
    public int GetHashCode (T memberInfo)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);
 
      // DeclaringType can return null, even if ReSharper thinks otherwise.
      // ReSharper disable ConditionIsAlwaysTrueOrFalse
      if (memberInfo.DeclaringType != null && memberInfo.DeclaringType.IsArray)
      // ReSharper restore ConditionIsAlwaysTrueOrFalse
        return GetHashCodeOrZero (memberInfo.DeclaringType) ^ GetHashCodeOrZero (memberInfo.Name) ^ GetHashCodeOrZero (memberInfo.Module);
      else
        return GetHashCodeOrZero (memberInfo.DeclaringType) ^ GetHashCodeOrZero (memberInfo.MetadataToken) ^ GetHashCodeOrZero (memberInfo.Module);
    }

    private int GetHashCodeOrZero (object valueOrNull)
    {
      return valueOrNull != null ? valueOrNull.GetHashCode () : 0;
    }
  }
}