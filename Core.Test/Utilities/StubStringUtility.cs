/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections;
using System.Reflection;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{

/// <summary> Exposes non-public members of the <see cref="StringUtility"/> type. </summary>
public class StubStringUtility
{
  public static void AddParseMethodToCache (Type key, MethodInfo parseMethod)
  {
    PrivateInvoke.InvokeNonPublicStaticMethod (
        typeof (StringUtility), "AddParseMethodToCache", new object[]{key, parseMethod});
  }

  public static MethodInfo GetParseMethodFromCache (Type key)
  {
    return (MethodInfo) PrivateInvoke.InvokeNonPublicStaticMethod (
        typeof (StringUtility), "GetParseMethodFromCache", new object[]{key});
  }

  public static bool HasTypeInCache (Type type)
  {
    return (bool) PrivateInvoke.InvokeNonPublicStaticMethod (
        typeof (StringUtility), "HasTypeInCache", new object[]{type});
  }

  public static MethodInfo GetParseMethod  (Type type, bool throwIfNotFound)
  {
    return (MethodInfo) PrivateInvoke.InvokeNonPublicStaticMethod (
        typeof (StringUtility), "GetParseMethod", new object[]{type, throwIfNotFound});
  }

  public static MethodInfo GetParseMethodWithFormatProviderFromType (Type type)
  {
    return (MethodInfo) PrivateInvoke.InvokeNonPublicStaticMethod (
        typeof (StringUtility), "GetParseMethodWithFormatProviderFromType", new object[]{type});
  }

  public static MethodInfo GetParseMethodFromType (Type type)
  {
    return (MethodInfo) PrivateInvoke.InvokeNonPublicStaticMethod (
        typeof (StringUtility), "GetParseMethodFromType", new object[]{type});
  }

  public static void ClearCache()
  {
    Hashtable cache = (Hashtable) PrivateInvoke.GetNonPublicStaticField (typeof (StringUtility), "s_parseMethods");
    cache.Clear();
  }
}

}
