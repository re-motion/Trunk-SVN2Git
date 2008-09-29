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
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  /// <summary>
  /// Holds the results of mixin code generation when a concrete mixin type was generated.
  /// </summary>
  /// <remarks>
  /// A concrete mixin type is a type derived from a mixin type that implements <see cref="OverrideMixinAttribute">mixin overrides</see> and holds
  /// public wrappers for protected methods needed to be accessed from the outside.
  /// </remarks>
  public class ConcreteMixinType
  {
    private readonly Type _generatedType;
    private readonly Dictionary<MethodInfo, MethodInfo> _methodWrappers;

    public ConcreteMixinType (Type generatedType)
    {
      ArgumentUtility.CheckNotNull ("generatedType", generatedType);
      _generatedType = generatedType;
      _methodWrappers = new Dictionary<MethodInfo, MethodInfo>();
    }

    public Type GeneratedType
    {
      get { return _generatedType; }
    }

    public void AddMethodWrapper (MethodInfo protectedMethod, MethodInfo publicWrapper)
    {
      ArgumentUtility.CheckNotNull ("protectedMethod", protectedMethod);
      ArgumentUtility.CheckNotNull ("publicWrapper", publicWrapper);

      if (_methodWrappers.ContainsKey (protectedMethod))
      {
        string message = 
            string.Format ("A public wrapper for method '{0}.{1}' was already added.", protectedMethod.DeclaringType.FullName, protectedMethod.Name);
        throw new InvalidOperationException (message);
      }
      _methodWrappers.Add (protectedMethod, publicWrapper);
    }

    public MethodInfo GetMethodWrapper (MethodInfo protectedMethod)
    {
      if (!_methodWrappers.ContainsKey (protectedMethod))
      {
        string message = 
            string.Format ("No public wrapper was generated for method '{0}.{1}'.", protectedMethod.DeclaringType.FullName, protectedMethod.Name);
        throw new KeyNotFoundException (message);
      }
      return _methodWrappers[protectedMethod];
    }
  }
}