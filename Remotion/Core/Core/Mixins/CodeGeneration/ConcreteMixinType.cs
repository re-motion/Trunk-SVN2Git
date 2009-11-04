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
    private readonly ConcreteMixinTypeIdentifier _identifier;
    private readonly Type _generatedType;
    private readonly Type _generatedOverrideInterface;
    private readonly Dictionary<MethodInfo, MethodInfo> _methodWrappers;
    private readonly Dictionary<MethodInfo, MethodInfo> _overrideInterfaceMethodsByMixinMethod;

    public ConcreteMixinType (
        ConcreteMixinTypeIdentifier identifier, 
        Type generatedType, 
        Type generatedOverrideInterface,
        Dictionary<MethodInfo, MethodInfo> overrideInterfaceMethodsByMixinMethod,
        Dictionary<MethodInfo, MethodInfo> methodWrappers)
    {
      ArgumentUtility.CheckNotNull ("identifier", identifier);
      ArgumentUtility.CheckNotNull ("generatedType", generatedType);
      ArgumentUtility.CheckNotNull ("generatedOverrideInterface", generatedOverrideInterface);
      ArgumentUtility.CheckNotNull ("overrideInterfaceMethodsByMixinMethod", overrideInterfaceMethodsByMixinMethod);
      ArgumentUtility.CheckNotNull ("methodWrappers", methodWrappers);

      _identifier = identifier;
      _generatedType = generatedType;
      _generatedOverrideInterface = generatedOverrideInterface;
      _methodWrappers = methodWrappers;
      _overrideInterfaceMethodsByMixinMethod = overrideInterfaceMethodsByMixinMethod;
    }

    public ConcreteMixinTypeIdentifier Identifier
    {
      get { return _identifier; }
    }

    public Type GeneratedType
    {
      get { return _generatedType; }
    }

    public Type GeneratedOverrideInterface
    {
      get { return _generatedOverrideInterface; }
    }

    public MethodInfo GetOverrideInterfaceMethod (MethodInfo mixinMethod)
    {
      ArgumentUtility.CheckNotNull ("mixinMethod", mixinMethod);

      MethodInfo interfaceMethod;
      if (!_overrideInterfaceMethodsByMixinMethod.TryGetValue (mixinMethod, out interfaceMethod))
      {
        string message =
            string.Format ("No override interface method was generated for method '{0}.{1}'.", mixinMethod.DeclaringType.FullName, mixinMethod.Name);
        throw new KeyNotFoundException (message);
      }
      else
      {
        return interfaceMethod;
      }
    }

    public MethodInfo GetMethodWrapper (MethodInfo wrappedMethod)
    {
      ArgumentUtility.CheckNotNull ("wrappedMethod", wrappedMethod);

      MethodInfo wrapper;
      if (!_methodWrappers.TryGetValue (wrappedMethod, out wrapper))
      {
        string message =
            string.Format ("No public wrapper was generated for method '{0}.{1}'.", wrappedMethod.DeclaringType.FullName, wrappedMethod.Name);
        throw new KeyNotFoundException (message);
      }
      else
      {
        return wrapper;
      }
    }
  }
}
