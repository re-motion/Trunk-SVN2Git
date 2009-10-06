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

    public ConcreteMixinType (ConcreteMixinTypeIdentifier identifier, Type generatedType, Type generatedOverrideInterface)
    {
      ArgumentUtility.CheckNotNull ("identifier", identifier);
      ArgumentUtility.CheckNotNull ("generatedType", generatedType);
      ArgumentUtility.CheckNotNull ("generatedOverrideInterface", generatedOverrideInterface);

      _identifier = identifier;
      _generatedType = generatedType;
      _generatedOverrideInterface = generatedOverrideInterface;
      _methodWrappers = new Dictionary<MethodInfo, MethodInfo>();
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
