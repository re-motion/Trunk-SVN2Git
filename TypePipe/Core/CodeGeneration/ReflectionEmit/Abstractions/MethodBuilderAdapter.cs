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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.LambdaCompilation;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.TypePipe.Dlr.Ast.Compiler;
using Remotion.TypePipe.Dlr.Runtime.CompilerServices;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions
{
  /// <summary>
  /// Adapts <see cref="MethodBuilder"/> with the <see cref="IMethodBaseBuilder"/> interface.
  /// </summary>
  public class MethodBuilderAdapter : BuilderAdapterBase, IMethodBuilder
  {
    private readonly MethodBuilder _methodBuilder;

    public MethodBuilderAdapter (MethodBuilder methodBuilder)
        : base (ArgumentUtility.CheckNotNull ("methodBuilder", methodBuilder).SetCustomAttribute)
    {
      _methodBuilder = methodBuilder;
    }

    public MethodBuilder AdaptedMethodBuilder
    {
      get { return _methodBuilder; }
    }

    public void RegisterWith (IEmittableOperandProvider emittableOperandProvider, MutableMethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("emittableOperandProvider", emittableOperandProvider);
      ArgumentUtility.CheckNotNull ("method", method);

      emittableOperandProvider.AddMapping (method, _methodBuilder);
    }

    public IGenericTypeParameterBuilder[] DefineGenericParameters (string[] names)
    {
      ArgumentUtility.CheckNotNull ("names", names);

      return _methodBuilder
          .DefineGenericParameters (names)
          .Select (b => new GenericTypeParameterBuilderAdapter (b)).Cast<IGenericTypeParameterBuilder>().ToArray();
    }

    public void SetReturnType (Type returnType)
    {
      ArgumentUtility.CheckNotNull ("returnType", returnType);

      _methodBuilder.SetReturnType (returnType);
    }

    public void SetParameters (Type[] parameterTypes)
    {
      ArgumentUtility.CheckNotNull ("parameterTypes", parameterTypes);

      _methodBuilder.SetParameters (parameterTypes);
    }

    public IParameterBuilder DefineParameter (int iSequence, ParameterAttributes attributes, string strParamName)
    {
      // Parameter name may be null.

      var parameterBuilder = _methodBuilder.DefineParameter (iSequence, attributes, strParamName);
      return new ParameterBuilderAdapter (parameterBuilder);
    }

    [CLSCompliant (false)]
    public void SetBody (LambdaExpression body, IILGeneratorFactory ilGeneratorFactory, DebugInfoGenerator debugInfoGeneratorOrNull)
    {
      ArgumentUtility.CheckNotNull ("body", body);
      ArgumentUtility.CheckNotNull ("ilGeneratorFactory", ilGeneratorFactory);

      var builderForLambdaCompiler = new MethodBuilderForLambdaCompiler (_methodBuilder, ilGeneratorFactory, true);
      LambdaCompiler.Compile (body, builderForLambdaCompiler, debugInfoGeneratorOrNull);
    }
  }
}