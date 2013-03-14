﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// Holds the <see cref="RequestedType"/> and <see cref="ProxyType"/> and allows generation of additional types.
  /// </summary>
  /// <remarks>
  /// The <see cref="ProxyType"/> represents the proxy type to be generated for the <see cref="RequestedType"/> including the modifications
  /// applied by preceding participants.
  /// Its mutating members (e.g. <see cref="MutableReflection.ProxyType.AddMethod" />) can be used to specify the needed modifications.
  /// </remarks>
  public class TypeContext : IMutableTypeFactory
  {
    private readonly IMutableTypeFactory _mutableTypeFactory;
    private readonly Type _requestedType;
    private readonly ProxyType _proxyType;

    public TypeContext (IMutableTypeFactory mutableTypeFactory, Type requestedType)
    {
      ArgumentUtility.CheckNotNull ("mutableTypeFactory", mutableTypeFactory);
      ArgumentUtility.CheckNotNull ("requestedType", requestedType);

      _mutableTypeFactory = mutableTypeFactory;
      _requestedType = requestedType;
      _proxyType = _mutableTypeFactory.CreateProxyType (requestedType);
    }

    /// <summary>
    /// The original <see cref="Type"/> that was requested by the user through an instance of <see cref="IObjectFactory"/>.
    /// </summary>
    public Type RequestedType
    {
      get { return _requestedType; }
    }

    /// <summary>
    /// The mutable proxy type that was created by the pipeline for the <see cref="RequestedType"/>.
    /// </summary>
    public ProxyType ProxyType
    {
      get { return _proxyType; }
    }

    /// <summary>
    /// Creates an additional type that should be generated.
    /// </summary>
    /// <param name="name">The type name.</param>
    /// <param name="namespace">The namespace of the type.</param>
    /// <param name="attributes">The type attributes.</param>
    /// <param name="baseType">The base type of the new type.</param>
    /// <returns>A new mutable type.</returns>
    public ProxyType CreateType (string name, string @namespace, TypeAttributes attributes, Type baseType)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      // Namespace may be null.
      ArgumentUtility.CheckNotNull ("baseType", baseType);

      return null;
    }

    /// <summary>
    /// Creates a <see cref="ProxyType"/> that represents a proxy type for the specified base type.
    /// This method copies all accessible constructors of the base type.
    /// </summary>
    /// <param name="baseType">The proxied type.</param>
    /// <returns>A new mutable proxy type.</returns>
    public ProxyType CreateProxyType (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);

      return null;
    }
  }
}