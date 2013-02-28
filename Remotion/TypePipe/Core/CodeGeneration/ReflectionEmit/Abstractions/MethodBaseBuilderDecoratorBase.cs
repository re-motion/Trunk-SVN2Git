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
using System.Runtime.CompilerServices;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.LambdaCompilation;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions
{
  /// <summary>
  /// A base class for decorators that decorate <see cref="IMethodBaseBuilder"/>.
  /// </summary>
  public abstract class MethodBaseBuilderDecoratorBase : BuilderDecoratorBase, IMethodBaseBuilder
  {
    private readonly IMethodBaseBuilder _methodBaseBuilder;

    [CLSCompliant (false)]
    protected MethodBaseBuilderDecoratorBase (IMethodBaseBuilder methodBaseBuilder, IEmittableOperandProvider emittableOperandProvider)
        : base (methodBaseBuilder, emittableOperandProvider)
    {
      _methodBaseBuilder = methodBaseBuilder;
    }

    public IParameterBuilder DefineParameter (int iSequence, ParameterAttributes attributes, string strParamName)
    {
      // Parameter name may be null.

      var parameterBuilder = _methodBaseBuilder.DefineParameter (iSequence, attributes, strParamName);

      return new ParameterBuilderDecorator (parameterBuilder, EmittableOperandProvider);
    }

    [CLSCompliant (false)]
    public void SetBody (LambdaExpression body, IILGeneratorFactory ilGeneratorFactory, DebugInfoGenerator debugInfoGeneratorOrNull)
    {
      ArgumentUtility.CheckNotNull ("body", body);
      ArgumentUtility.CheckNotNull ("ilGeneratorFactory", ilGeneratorFactory);

      _methodBaseBuilder.SetBody (body, ilGeneratorFactory, debugInfoGeneratorOrNull);
    }
  }
}