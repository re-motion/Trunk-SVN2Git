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
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions
{
  /// <summary>
  /// Decorates an instance of <see cref="IMethodBuilder"/> to allow <see cref="CustomType"/>s to be used in signatures and 
  /// for checking strong-name compatibility.
  /// </summary>
  public class MethodBuilderDecorator : MethodBaseBuilderDecoratorBase, IMethodBuilder
  {
    private readonly IMethodBuilder _methodBuilder;

    [CLSCompliant (false)]
    public MethodBuilderDecorator (IMethodBuilder methodBuilder, IEmittableOperandProvider emittableOperandProvider)
        : base (methodBuilder, emittableOperandProvider)
    {
      _methodBuilder = methodBuilder;
    }

    [CLSCompliant (false)]
    public IMethodBuilder DecoratedMethodBuilder
    {
      get { return _methodBuilder; }
    }

    public void RegisterWith (IEmittableOperandProvider emittableOperandProvider, MutableMethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("emittableOperandProvider", emittableOperandProvider);
      ArgumentUtility.CheckNotNull ("method", method);

      _methodBuilder.RegisterWith (emittableOperandProvider, method);
    }

    public IGenericTypeParameterBuilder[] DefineGenericParameters (string[] names)
    {
      ArgumentUtility.CheckNotNull ("names", names);

      return _methodBuilder
          .DefineGenericParameters (names)
          .Select (b => new GenericTypeParameterBuilderDecorator (b, EmittableOperandProvider)).Cast<IGenericTypeParameterBuilder>().ToArray();
    }

    public void SetReturnType (Type returnType)
    {
      ArgumentUtility.CheckNotNull ("returnType", returnType);

      var emittableReturnType = EmittableOperandProvider.GetEmittableType (returnType);
      _methodBuilder.SetReturnType (emittableReturnType);
    }

    public void SetParameters (Type[] parameterTypes)
    {
      ArgumentUtility.CheckNotNull ("parameterTypes", parameterTypes);

      var emittableParameterTypes = parameterTypes.Select (EmittableOperandProvider.GetEmittableType).ToArray();
      _methodBuilder.SetParameters (emittableParameterTypes);
    }
  }
}