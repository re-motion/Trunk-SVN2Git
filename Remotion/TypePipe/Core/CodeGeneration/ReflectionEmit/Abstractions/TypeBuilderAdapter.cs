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
  /// Adapts <see cref="TypeBuilder"/> with the <see cref="ITypeBuilder"/> interface.
  /// </summary>
  public class TypeBuilderAdapter : ITypeBuilder
  {
    private readonly TypeBuilder _typeBuilder;

    public TypeBuilderAdapter (TypeBuilder typeBuilder)
    {
      ArgumentUtility.CheckNotNull ("typeBuilder", typeBuilder);

      _typeBuilder = typeBuilder;
    }

    public TypeBuilder TypeBuilder
    {
      get { return _typeBuilder; }
    }

    public void RegisterWith (IEmittableOperandProvider emittableOperandProvider, Type type)
    {
      ArgumentUtility.CheckNotNull ("emittableOperandProvider", emittableOperandProvider);
      ArgumentUtility.CheckNotNull ("type", type);

      emittableOperandProvider.AddMapping (type, _typeBuilder);
    }

    public void AddInterfaceImplementation (Type interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);
      _typeBuilder.AddInterfaceImplementation (interfaceType);
    }

    [CLSCompliant (false)]
    public IFieldBuilder DefineField (string name, Type type, FieldAttributes attributes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("type", type);

      return new FieldBuilderAdapter (_typeBuilder.DefineField (name, type, attributes));
    }

    [CLSCompliant (false)]
    public IConstructorBuilder DefineConstructor (MethodAttributes attributes, CallingConventions callingConvention, Type[] parameterTypes)
    {
      ArgumentUtility.CheckNotNull ("parameterTypes", parameterTypes);

      var constructorBuilder = _typeBuilder.DefineConstructor (attributes, callingConvention, parameterTypes);
      return new ConstructorBuilderAdapter (constructorBuilder);
    }

    [CLSCompliant (false)]
    public IMethodBuilder DefineMethod (string name, MethodAttributes attributes, Type returnType, Type[] parameterTypes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("returnType", returnType);
      ArgumentUtility.CheckNotNull ("parameterTypes", parameterTypes);

      var methodBuilder = _typeBuilder.DefineMethod (name, attributes, returnType, parameterTypes);
      return new MethodBuilderAdapter (methodBuilder);
    }

    public void DefineMethodOverride (MethodInfo methodInfoBody, MethodInfo methodInfoDeclaration)
    {
      ArgumentUtility.CheckNotNull ("methodInfoBody", methodInfoBody);
      ArgumentUtility.CheckNotNull ("methodInfoDeclaration", methodInfoDeclaration);

      _typeBuilder.DefineMethodOverride (methodInfoBody, methodInfoDeclaration);
    }

    public Type CreateType ()
    {
      return _typeBuilder.CreateType();
    }

    [CLSCompliant (false)]
    public void Emit (IILGenerator ilGenerator, OpCode opCode)
    {
      ArgumentUtility.CheckNotNull ("ilGenerator", ilGenerator);

      ilGenerator.Emit (opCode, _typeBuilder);
    }
  }
}