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
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;

namespace Remotion.TypePipe.TypeAssembly.Implementation
{
  /// <summary>
  /// A base class for <see cref="ITypeAssemblyContext"/> implementers that provides the possibility to raise
  /// the <see cref="GenerationCompleted"/> event.
  /// </summary>
  public abstract class TypeAssemblyContextBase : ITypeAssemblyContext
  {
    private readonly IMutableTypeFactory _mutableTypeFactory;
    private readonly IDictionary<string, object> _state;
    private readonly List<MutableType> _additionalTypes = new List<MutableType>();

    protected TypeAssemblyContextBase (IMutableTypeFactory mutableTypeFactory, IDictionary<string, object> state)
    {
      ArgumentUtility.CheckNotNull ("mutableTypeFactory", mutableTypeFactory);
      ArgumentUtility.CheckNotNull ("state", state);

      _mutableTypeFactory = mutableTypeFactory;
      _state = state;
    }

    public event Action<GeneratedTypesContext> GenerationCompleted;

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
      // Base type may be null (for interfaces).

      var type = _mutableTypeFactory.CreateType (name, @namespace, attributes, baseType, null);
      _additionalTypes.Add (type);

      return type;
    }

    public MutableType CreateProxy (Type baseType)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);

      var type = _mutableTypeFactory.CreateProxy (baseType).Type;
      _additionalTypes.Add (type);

      return type;
    }

    public void OnGenerationCompleted (GeneratedTypesContext generatedTypesContext)
    {
      var handler = GenerationCompleted;
      if (handler != null)
        handler (generatedTypesContext);
    }
  }
}