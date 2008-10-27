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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration
{
  /// <summary>
  /// Marks a generated method as a public wrapper for another method.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public class GeneratedMethodWrapperAttribute : Attribute
  {
    private readonly int _wrappedMethodRefToken;
    private readonly Type[] _genericTypeArguments;

    public GeneratedMethodWrapperAttribute (int wrappedMethodRefToken)
      : this (wrappedMethodRefToken, Type.EmptyTypes)
    {
    }
    
    public GeneratedMethodWrapperAttribute (int wrappedMethodRefToken, Type[] genericTypeArguments)
    {
      ArgumentUtility.CheckNotNull ("genericTypeArguments", genericTypeArguments);
      _wrappedMethodRefToken = wrappedMethodRefToken;
      _genericTypeArguments = genericTypeArguments;
    }

    public int WrappedMethodRefToken
    {
      get { return _wrappedMethodRefToken; }
    }

    public Type[] GenericTypeArguments
    {
      get { return _genericTypeArguments; }
    }

    public MethodInfo ResolveWrappedMethod (Module module)
    {
      var method = module.ResolveMethod (WrappedMethodRefToken);
      
      // If we have a generic type, ResolveMethod returned the method on the generic type definition, not the closed generic type.
      // To retrieve the method on the closed generic type, we use GetMethodFromHandle.
      // (We could also iterate over the methods to search the one with the right token, but going via RuntimeMethodHandle seems more elegant.)
      if (_genericTypeArguments.Length > 0)
      {
        var surroundingType = method.DeclaringType.MakeGenericType (_genericTypeArguments).TypeHandle;
        return (MethodInfo) MethodBase.GetMethodFromHandle (method.MethodHandle, surroundingType);
      }
      else
        return (MethodInfo) method;
    }
  }
}