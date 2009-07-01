// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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

namespace Remotion.Scripting
{
  public class MethodInfoEqualityComparer2 : IEqualityComparer<MemberInfo> 
  {
    private static readonly MethodInfoEqualityComparer2 s_equalityComparer = new MethodInfoEqualityComparer2 ();

    public static MethodInfoEqualityComparer2 Get
    {
      get { return s_equalityComparer; }
    }

    public bool Equals (MemberInfo x, MemberInfo y)
    {
      if (x == y)
        return true;

      if (x.DeclaringType != y.DeclaringType)
        return false;

      // Methods on arrays do not have metadata tokens but their ReflectedType
      // always equals their DeclaringType
      if (x.DeclaringType != null && x.DeclaringType.IsArray)
        return false;

      if (x.MetadataToken != y.MetadataToken || x.Module != y.Module)
        return false;

      if (x is MethodInfo)
      {
        MethodInfo lhsMethod = x as MethodInfo;

        if (lhsMethod.IsGenericMethod)
        {
          MethodInfo rhsMethod = y as MethodInfo;

          // TODO: Check if we actually need this (RS complains otherwise)
          if (rhsMethod == null)
          {
            return false;
          }

          Type[] lhsGenArgs = lhsMethod.GetGenericArguments ();
          Type[] rhsGenArgs = rhsMethod.GetGenericArguments ();
          for (int i = 0; i < rhsGenArgs.Length; i++)
          {
            if (lhsGenArgs[i] != rhsGenArgs[i])
              return false;
          }
        }
      }
      return true;

    }

    public int GetHashCode (MemberInfo obj)
    {
      throw new NotImplementedException();
    }
  }
}