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
      
      // If we have a generic type, ResolveMethod sometimes returns the method on the generic type definition, not the closed generic type.
      // To retrieve the method on the closed generic type, we use GetMethodFromHandle.
      // (We could also iterate over the methods to search the one with the right token, but going via RuntimeMethodHandle seems more elegant.)
      if (_genericTypeArguments.Length > 0 && method.DeclaringType.IsGenericTypeDefinition)
      {
        var surroundingType = method.DeclaringType.MakeGenericType (_genericTypeArguments).TypeHandle;
        return (MethodInfo) MethodBase.GetMethodFromHandle (method.MethodHandle, surroundingType);
      }
      else
        return (MethodInfo) method;
    }
  }
}
