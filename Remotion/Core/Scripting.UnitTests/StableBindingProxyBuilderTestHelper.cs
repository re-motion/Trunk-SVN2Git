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
using Remotion.Development.UnitTesting;

namespace Remotion.Scripting.UnitTests
{
  public static class StableBindingProxyBuilderTestHelper
  {
    public static ITypeFilter GetTypeFilter (this StableBindingProxyBuilder proxyBuilder)
    {
      return (ITypeFilter) PrivateInvoke.GetNonPublicField (proxyBuilder, "_typeFilter");
    }

    public static Dictionary<MethodInfo, HashSet<MethodInfo>> GetClassMethodToInterfaceMethodsMap (this StableBindingProxyBuilder proxyBuilder)
    {
      return (Dictionary<MethodInfo, HashSet<MethodInfo>>) PrivateInvoke.GetNonPublicField (proxyBuilder, "_classMethodToInterfaceMethodsMap");
    }

    public static IEnumerable<MethodInfo> GetInterfaceMethodsToClassMethod (this StableBindingProxyBuilder proxyBuilder, MethodInfo classMethod)
    {
      return (IEnumerable<MethodInfo>) PrivateInvoke.InvokeNonPublicMethod (proxyBuilder, "GetInterfaceMethodsToClassMethod", classMethod);
    }

    public static Type GetFirstKnownBaseType (this StableBindingProxyBuilder proxyBuilder)
    {
      return (Type) PrivateInvoke.InvokeNonPublicMethod (proxyBuilder, "GetFirstKnownBaseType");
    }     
  }
}