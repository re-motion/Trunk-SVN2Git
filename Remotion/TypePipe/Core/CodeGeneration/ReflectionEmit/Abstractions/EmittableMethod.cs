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
using System.Reflection.Emit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.LambdaCompilation;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions
{
  /// <summary>
  /// Represents a method which knows how to emit itself using an <see cref="IILGenerator"/>.
  /// </summary>
  public class EmittableMethod : IEmittableMethodOperand
  {
    private readonly MethodInfo _methodInfo;

    public EmittableMethod (MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);

      _methodInfo = methodInfo;
    }

    public MethodInfo MethodInfo
    {
      get { return _methodInfo; }
    }

    [CLSCompliant (false)]
    public void Emit (IILGenerator ilGenerator, OpCode opCode)
    {
      ArgumentUtility.CheckNotNull ("ilGenerator", ilGenerator);

      ilGenerator.Emit (opCode, _methodInfo);
    }

    [CLSCompliant (false)]
    public void EmitCall (IILGenerator ilGenerator, OpCode opCode, Type[] optionalParameterTypes)
    {
      ArgumentUtility.CheckNotNull ("ilGenerator", ilGenerator);
      // Optional parameter types may be null

      ilGenerator.EmitCall (opCode, _methodInfo, optionalParameterTypes);
    }
  }
}