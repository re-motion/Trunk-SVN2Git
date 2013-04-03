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
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Remotion.Reflection;
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.Implementation;
using Remotion.Utilities;

namespace Remotion.TypePipe.Caching
{
  /// <summary>
  /// Retrieves the generated type or its constructors for the requested type from the cache or delegates to the contained
  /// <see cref="ITypeAssembler"/> instance.
  /// </summary>
  public class TypeCache : ITypeCache
  {
    // Storing the delegates as static readonly fields has two advantages for performance:
    // 1) It ensures that no closure is implicilty created.
    // 2) We do not create new delegate instances every time a cache key is computed.
    private static readonly Func<ICacheKeyProvider, Type, object> s_fromRequestedType = (ckp, t) => ckp.GetCacheKey (t);
    private static readonly Func<ICacheKeyProvider, Type, object> s_fromGeneratedType = (ckp, t) => ckp.RebuildCacheKey (t);

    private readonly object _lock = new object();
    private readonly Dictionary<object[], Type> _types = new Dictionary<object[], Type> (new CompoundCacheKeyEqualityComparer());
    private readonly Dictionary<object[], Delegate> _constructorCalls = new Dictionary<object[], Delegate> (new CompoundCacheKeyEqualityComparer());
    private readonly Dictionary<string, object> _participantState = new Dictionary<string, object>();

    private readonly ITypeAssembler _typeAssembler;
    private readonly ITypeAssemblyContextCodeGenerator _typeAssemblyContextCodeGenerator;
    private readonly IConstructorFinder _constructorFinder;
    private readonly IDelegateFactory _delegateFactory;

    public TypeCache (
        ITypeAssembler typeAssembler,
        ITypeAssemblyContextCodeGenerator typeAssemblyContextCodeGenerator,
        IConstructorFinder constructorFinder,
        IDelegateFactory delegateFactory)
    {
      ArgumentUtility.CheckNotNull ("typeAssembler", typeAssembler);
      ArgumentUtility.CheckNotNull ("typeAssemblyContextCodeGenerator", typeAssemblyContextCodeGenerator);
      ArgumentUtility.CheckNotNull ("constructorFinder", constructorFinder);
      ArgumentUtility.CheckNotNull ("delegateFactory", delegateFactory);

      _typeAssembler = typeAssembler;
      _typeAssemblyContextCodeGenerator = typeAssemblyContextCodeGenerator;
      _constructorFinder = constructorFinder;
      _delegateFactory = delegateFactory;
    }

    public string ParticipantConfigurationID
    {
      get { return _typeAssembler.ParticipantConfigurationID; }
    }

    public string AssemblyDirectory
    {
      get { lock (_lock) return _typeAssemblyContextCodeGenerator.CodeGenerator.AssemblyDirectory; }
    }

    public string AssemblyName
    {
      get { lock (_lock) return _typeAssemblyContextCodeGenerator.CodeGenerator.AssemblyName; }
    }

    public void SetAssemblyDirectory (string assemblyDirectory)
    {
      lock (_lock) _typeAssemblyContextCodeGenerator.CodeGenerator.SetAssemblyDirectory (assemblyDirectory);
    }

    public void SetAssemblyName (string assemblyName)
    {
      lock (_lock) _typeAssemblyContextCodeGenerator.CodeGenerator.SetAssemblyName (assemblyName);
    }

    public string FlushCodeToDisk ()
    {
      return _typeAssemblyContextCodeGenerator.CodeGenerator.FlushCodeToDisk (_typeAssembler.ParticipantConfigurationID);
    }

    public void LoadFlushedCode (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      LoadFlushedCode ((_Assembly) assembly);
    }

    [CLSCompliant (false)]
    public void LoadFlushedCode (_Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      var assemblyAttribute = (TypePipeAssemblyAttribute) assembly.GetCustomAttributes (typeof (TypePipeAssemblyAttribute), inherit: false).SingleOrDefault();
      if (assemblyAttribute == null)
        throw new ArgumentException ("The specified assembly was not generated by the pipeline.", "assembly");

      if (assemblyAttribute.ParticipantConfigurationID != ParticipantConfigurationID)
      {
        var message = string.Format (
            "The specified assembly was generated with a different participant configuration: '{0}'.", assemblyAttribute.ParticipantConfigurationID);
        throw new ArgumentException (message, "assembly");
      }

      var proxyTypes = assembly.GetTypes().Where (t => t.IsDefined (typeof (ProxyTypeAttribute), inherit: false));
      LoadProxyTypes (proxyTypes);
    }

    public Type GetOrCreateType (Type requestedType)
    {
      ArgumentUtility.CheckNotNull ("requestedType", requestedType);

      var key = GetTypeKey (requestedType, s_fromRequestedType, requestedType);
      return GetOrCreateType (key, requestedType);
    }

    public Delegate GetOrCreateConstructorCall (Type requestedType, Type delegateType, bool allowNonPublic)
    {
      ArgumentUtility.CheckNotNull ("requestedType", requestedType);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("delegateType", delegateType, typeof (Delegate));

      var key = GetConstructorKey (requestedType, delegateType, allowNonPublic);

      Delegate constructorCall;
      lock (_lock)
      {
        if (!_constructorCalls.TryGetValue (key, out constructorCall))
        {
          var typeKey = GetTypeKeyFromConstructorKey (key);
          var generatedType = GetOrCreateType (typeKey, requestedType);
          var ctorSignature = _delegateFactory.GetSignature (delegateType);
          var constructor = _constructorFinder.GetConstructor (generatedType, ctorSignature.Item1, allowNonPublic, requestedType, ctorSignature.Item1);

          constructorCall = _delegateFactory.CreateConstructorCall (constructor, delegateType);
          _constructorCalls.Add (key, constructorCall);
        }
      }

      return constructorCall;
    }

    private void LoadProxyTypes (IEnumerable<Type> proxyTypes)
    {
      var keysAndTypes = proxyTypes.Select (t => new { Key = GetTypeKey (t.BaseType, s_fromGeneratedType, t), Type = t }).ToList();

      lock (_lock)
      {
        foreach (var pair in keysAndTypes)
        {
          if (!_types.ContainsKey (pair.Key))
            _types.Add (pair.Key, pair.Type);
        }
      }
    }

    private Type GetOrCreateType (object[] typeKey, Type requestedType)
    {
      Type generatedType;
      lock (_lock)
      {
        if (!_types.TryGetValue (typeKey, out generatedType))
        {
          generatedType = _typeAssembler.AssembleType (requestedType, _participantState, _typeAssemblyContextCodeGenerator);
          _types.Add (typeKey, generatedType);
        }
      }

      return generatedType;
    }

    private object[] GetTypeKey (Type requestedType, Func<ICacheKeyProvider, Type, object> cacheKeyProviderMethod, Type fromType)
    {
      var key = _typeAssembler.GetCompoundCacheKey (cacheKeyProviderMethod, fromType, freeSlotsAtStart: 1);
      key[0] = requestedType;

      return key;
    }

    private object[] GetConstructorKey (Type requestedType, Type delegateType, bool allowNonPublic)
    {
      var key = _typeAssembler.GetCompoundCacheKey (s_fromRequestedType, requestedType, freeSlotsAtStart: 3);
      key[0] = requestedType;
      key[1] = delegateType;
      key[2] = allowNonPublic;

      return key;
    }

    private object[] GetTypeKeyFromConstructorKey (object[] constructorKey)
    {
      var typeKey = new object[constructorKey.Length - 2];
      typeKey[0] = constructorKey[0];
      Array.Copy (constructorKey, 3, typeKey, 1, constructorKey.Length - 3);

      return typeKey;
    }
  }
}