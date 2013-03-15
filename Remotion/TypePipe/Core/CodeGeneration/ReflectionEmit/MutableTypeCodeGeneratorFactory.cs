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
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit
{
  /// <summary>
  /// Serves as a factory for instances of <see cref="IMutableTypeCodeGenerator"/>.
  /// </summary>
  public class MutableTypeCodeGeneratorFactory : IMutableTypeCodeGeneratorFactory
  {
    private readonly IMemberEmitterFactory _memberEmitterFactory;
    private readonly IInitializationBuilder _initializationBuilder;
    private readonly IProxySerializationEnabler _proxySerializationEnabler;

    public MutableTypeCodeGeneratorFactory (
        IMemberEmitterFactory memberEmitterFactory, IInitializationBuilder initializationBuilder, IProxySerializationEnabler proxySerializationEnabler)
    {
      ArgumentUtility.CheckNotNull ("memberEmitterFactory", memberEmitterFactory);
      ArgumentUtility.CheckNotNull ("initializationBuilder", initializationBuilder);
      ArgumentUtility.CheckNotNull ("proxySerializationEnabler", proxySerializationEnabler);

      _memberEmitterFactory = memberEmitterFactory;
      _initializationBuilder = initializationBuilder;
      _proxySerializationEnabler = proxySerializationEnabler;
    }

    [CLSCompliant (false)]
    public IMutableTypeCodeGenerator Create (MutableType mutableType, IReflectionEmitCodeGenerator codeGenerator)
    {
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);
      ArgumentUtility.CheckNotNull ("codeGenerator", codeGenerator);

      var memberEmitter = _memberEmitterFactory.CreateMemberEmitter (codeGenerator.EmittableOperandProvider);
      return new MutableTypeCodeGenerator (mutableType, codeGenerator, memberEmitter, _initializationBuilder, _proxySerializationEnabler);
    }
  }
}