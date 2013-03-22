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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// Implements <see cref="ITypeContext"/> and provides the possibility to raise the <see cref="GenerationCompleted"/> event.
  /// </summary>
  // TODO Review: TypeAssemblyContext
  public class TypeContext : ITypeContext
  {
    private readonly List<MutableType> _additionalTypes = new List<MutableType>();
    private readonly IMutableTypeFactory _mutableTypeFactory;
    private readonly Type _requestedType;
    private readonly MutableType _proxyType;
    private readonly IDictionary<string, object> _state;

    public TypeContext (IMutableTypeFactory mutableTypeFactory, Type requestedType, IDictionary<string, object> state)
    {
      ArgumentUtility.CheckNotNull ("mutableTypeFactory", mutableTypeFactory);
      ArgumentUtility.CheckNotNull ("requestedType", requestedType);

      _mutableTypeFactory = mutableTypeFactory;
      _requestedType = requestedType;
      _proxyType = _mutableTypeFactory.CreateProxy (requestedType);
      _state = state;
    }

    public event Action<GeneratedTypeContext> GenerationCompleted;

    public Type RequestedType
    {
      get { return _requestedType; }
    }

    public MutableType ProxyType
    {
      get { return _proxyType; }
    }

    public IDictionary<string, object> State
    {
      get { return _state; }
    }

    public ReadOnlyCollection<MutableType> AdditionalTypes
    {
      get { return _additionalTypes.AsReadOnly(); }
    }

    public MutableType CreateType (string name, string @namespace, TypeAttributes attributes, Type baseType)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      // Namespace may be null.
      ArgumentUtility.CheckNotNull ("baseType", baseType);

      var type = _mutableTypeFactory.CreateType (name, @namespace, attributes, baseType);
      _additionalTypes.Add (type);

      return type;
    }

    public MutableType CreateInterface (string name, string @namespace)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      // Namespace may be null.

      var type = _mutableTypeFactory.CreateInterface (name, @namespace);
      _additionalTypes.Add (type);

      return type;
    }

    public MutableType CreateProxy (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);

      var type = _mutableTypeFactory.CreateProxy (baseType);
      _additionalTypes.Add (type);

      return type;
    }

    public void OnGenerationCompleted (GeneratedTypeContext generatedTypeContext)
    {
      var handler = GenerationCompleted;
      if (handler != null)
        handler (generatedTypeContext);
    }
  }
}