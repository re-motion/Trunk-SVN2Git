// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Runtime.CompilerServices;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit
{
  /// <summary>
  /// Defines an interface for classes emitting members for mutable reflection objects. Used by <see cref="SubclassProxyBuilder"/>.
  /// </summary>
  [CLSCompliant (false)]
  public interface IMemberEmitter
  {
    void AddField (ITypeBuilder typeBuilder, IEmittableOperandProvider emittableOperandProvider, MutableFieldInfo field);

    void AddConstructor (
        ITypeBuilder typeBuilder,
        DebugInfoGenerator debugInfoGeneratorOrNull,
        IEmittableOperandProvider emittableOperandProvider,
        DeferredActionManager postDeclarationsActionManager,
        MutableConstructorInfo constructor);

    void AddMethod (
        ITypeBuilder typeBuilder,
        DebugInfoGenerator debugInfoGeneratorOrNull,
        IEmittableOperandProvider emittableOperandProvider,
        DeferredActionManager postDeclarationsActionManager,
        MutableMethodInfo method,
        string name,
        MethodAttributes attributes);
  }
}