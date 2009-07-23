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
using System.Linq;
using System.Reflection;

namespace Remotion.Scripting.UnitTests
{
  /// <summary>
  /// Same as <see cref="Type.GetMethod(string,System.Reflection.BindingFlags)"/>, but also matches the
  /// passed parameter types.
  /// </summary>
  public static class TypeExtensions {
    public static MethodInfo[] GetMethods (this Type type, string name,
                                           BindingFlags bindingFlags, params Type[] parameterTypes)
    {
      return type.GetMethods  (bindingFlags).Where (
          mi => (mi.Name == name) && mi.GetParameters ().Select (pi => pi.ParameterType).SequenceEqual (parameterTypes)).ToArray ();
    }

    public static MethodInfo[] GetPublicInstanceMethods (this Type type, string name, params Type[] parameterTypes)
    {
      return type.GetMethods (name, BindingFlags.Instance | BindingFlags.Public, parameterTypes);
    }

    public static MethodInfo[] GetAllInstanceMethods (this Type type, string name, params Type[] parameterTypes)
    {
      return type.GetMethods (name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, parameterTypes);
    }

    public static MethodInfo[] GetPrivateInstanceMethods (this Type type, string name, params Type[] parameterTypes)
    {
      return type.GetMethods (name, BindingFlags.Instance | BindingFlags.NonPublic, parameterTypes);
    }
  }
}