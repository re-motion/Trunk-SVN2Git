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

using System.Diagnostics;
using System.Reflection;

namespace Remotion.Development.UnitTesting
{
  /// <summary>
  /// Provides useful extension methods on <see cref="object"/> to increase clarity in unit tests.
  /// </summary>
  public static class ObjectExtensions
  {
    [DebuggerStepThrough]
    public static T As<T> (this object obj)
    {
      return (T) obj;
    }

    [DebuggerStepThrough]
    public static T Invoke<T> (this object target, string method, params object[] args)
    {
      return (T) PrivateInvoke.InvokeNonPublicMethod (target, method, args);
    }

    [DebuggerStepThrough]
    public static object Invoke (this object target, string method, params object[] args)
    {
      return PrivateInvoke.InvokeNonPublicMethod (target, method, args);
    }

    [DebuggerStepThrough]
    public static object Invoke (this object target, MethodInfo method, params object[] args)
    {
      return method.Invoke (target, args);
    }

    [DebuggerStepThrough]
    public static T Invoke<T> (this object target, MethodInfo method, params object[] args)
    {
      return (T) method.Invoke (target, args);
    }
  }
}