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
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions
{
  /// <summary>
  /// Defines an interface for <see cref="TypeBuilder"/>.
  /// </summary>
  [CLSCompliant (false)]
  public interface ITypeBuilder : ICustomAttributeTargetBuilder
  {
    void RegisterWith (IEmittableOperandProvider emittableOperandProvider, MutableType type);

    void SetParent (Type parent);
    void AddInterfaceImplementation (Type interfaceType);

    ITypeBuilder DefineNestedType (string name, TypeAttributes attributes);
    IFieldBuilder DefineField (string name, Type type, FieldAttributes attributes);
    IConstructorBuilder DefineConstructor (MethodAttributes attributes, CallingConventions callingConvention, Type[] parameterTypes);
    IMethodBuilder DefineMethod (string name, MethodAttributes attributes);
    void DefineMethodOverride (MethodInfo methodInfoBody, MethodInfo methodInfoDeclaration);
    IPropertyBuilder DefineProperty (string name, PropertyAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes);
    IEventBuilder DefineEvent (string name, EventAttributes attributes, Type eventtype);

    Type CreateType ();
  }
}