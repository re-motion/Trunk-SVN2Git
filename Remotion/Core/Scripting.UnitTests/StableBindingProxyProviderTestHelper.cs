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
  public static class StableBindingProxyProviderTestHelper
  {
    public static ITypeArbiter GetTypeArbiter (this StableBindingProxyBuilder proxyBuilder)
    {
      return (ITypeArbiter) PrivateInvoke.GetNonPublicField (proxyBuilder, "_typeArbiter");
    }

    public static Dictionary<MemberInfo, HashSet<MemberInfo>> GetClassMethodToInterfaceMethodsMap (this StableBindingProxyBuilder proxyBuilder)
    {
      return (Dictionary<MemberInfo, HashSet<MemberInfo>>) PrivateInvoke.GetNonPublicField (proxyBuilder, "_classMethodToInterfaceMethodsMap");
    }

    public static IEnumerable<MemberInfo> GetInterfaceMethodsToClassMethod (this StableBindingProxyBuilder proxyBuilder, MethodInfo classMethod)
    {
      return (IEnumerable<MemberInfo>) PrivateInvoke.InvokeNonPublicMethod (proxyBuilder, "GetInterfaceMethodsToClassMethod", classMethod);
    }

    public static Type GetFirstKnownBaseType (this StableBindingProxyBuilder proxyBuilder)
    {
      return (Type) PrivateInvoke.InvokeNonPublicMethod (proxyBuilder, "GetFirstKnownBaseType");
    }     
  }
}