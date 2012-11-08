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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.TypePipe
{
  /// <summary>
  /// The main entry point of the pipeline.
  /// This class is used by application developers to create instances for the types generated by the pipeline.
  /// </summary>
  public class Pipeline
  {
    private readonly ITypeCache _typeCache;

    public Pipeline (ITypeCache typeCache)
    {
      ArgumentUtility.CheckNotNull ("typeCache", typeCache);

      _typeCache = typeCache;
    }

    public T CreateInstance<T> () where T : class
    {
      return (T) CreateInstance (typeof (T));
    }

    public T CreateInstance<T> (ParamList constructorArguments) where T : class
    {
      ArgumentUtility.CheckNotNull ("constructorArguments", constructorArguments);

      return (T) CreateInstance (typeof (T), constructorArguments);
    }

    public object CreateInstance (Type requestedType)
    {
      ArgumentUtility.CheckNotNull ("requestedType", requestedType);

      return CreateInstance (requestedType, ParamList.Empty);
    }

    public object CreateInstance (Type requestedType, ParamList constructorArguments)
    {
      ArgumentUtility.CheckNotNull ("requestedType", requestedType);
      ArgumentUtility.CheckNotNull ("constructorArguments", constructorArguments);

      var generatedType = _typeCache.GetOrGenerate (requestedType);
      var constructorLookupInfo = new ConstructorLookupInfo (generatedType);

      return constructorArguments.InvokeConstructor (constructorLookupInfo);
    }
  }
}