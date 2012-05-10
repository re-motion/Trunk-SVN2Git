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
using System.Runtime.CompilerServices;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit
{
  /// <summary>
  /// This class holds information needed by <see cref="IMemberEmitter"/> instances to emit members.
  /// </summary>
  public class MemberEmitterContext
  {
    private readonly ITypeBuilder _typeBuilder;
    private readonly DebugInfoGenerator _debugInfoGenerator;
    private readonly IEmittableOperandProvider _emittableOperandProvider;
    private readonly DeferredActionManager _postDeclarationsActionManager;

    [CLSCompliant (false)]
    public MemberEmitterContext (
        ITypeBuilder typeBuilder,
        DebugInfoGenerator debugInfoGeneratorOrNull,
        IEmittableOperandProvider emittableOperandProvider,
        DeferredActionManager postDeclarationsActionManager)
    {
      ArgumentUtility.CheckNotNull ("typeBuilder", typeBuilder);
      ArgumentUtility.CheckNotNull ("emittableOperandProvider", emittableOperandProvider);
      ArgumentUtility.CheckNotNull ("postDeclarationsActionManager", postDeclarationsActionManager);

      _typeBuilder = typeBuilder;
      _debugInfoGenerator = debugInfoGeneratorOrNull;
      _emittableOperandProvider = emittableOperandProvider;
      _postDeclarationsActionManager = postDeclarationsActionManager;
    }

    [CLSCompliant (false)]
    public ITypeBuilder TypeBuilder
    {
      get { return _typeBuilder; }
    }

    public DebugInfoGenerator DebugInfoGenerator
    {
      get { return _debugInfoGenerator; }
    }

    public IEmittableOperandProvider EmittableOperandProvider
    {
      get { return _emittableOperandProvider; }
    }

    public DeferredActionManager PostDeclarationsActionManager
    {
      get { return _postDeclarationsActionManager; }
    }
  }
}