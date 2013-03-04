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
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection.BodyBuilding;

namespace Remotion.TypePipe.MutableReflection.Implementation.MemberFactory
{
  /// <summary>
  /// Serves as a factory for mutable members.
  /// </summary>
  /// <remarks>
  /// This interface is an implementation detail of <see cref="ProxyType"/>.
  /// </remarks>
  public interface IMutableMemberFactory
  {
    Expression CreateInitialization (ProxyType declaringType, Func<InitializationBodyContext, Expression> initializationProvider);

    MutableFieldInfo CreateField (ProxyType declaringType, string name, Type type, FieldAttributes attributes);

    MutableConstructorInfo CreateConstructor (
        ProxyType declaringType,
        MethodAttributes attributes,
        IEnumerable<ParameterDeclaration> parameters,
        Func<ConstructorBodyCreationContext, Expression> bodyProvider);

    MutableMethodInfo CreateMethod (
        ProxyType declaringType,
        string name,
        MethodAttributes attributes,
        IEnumerable<GenericParameterDeclaration> genericParameters,
        Func<GenericParameterContext, Type> returnTypeProvider,
        Func<GenericParameterContext, IEnumerable<ParameterDeclaration>> parameterProvider,
        Func<MethodBodyCreationContext, Expression> bodyProvider);

    MutableMethodInfo CreateExplicitOverride (
        ProxyType declaringType, MethodInfo overriddenMethodBaseDefinition, Func<MethodBodyCreationContext, Expression> bodyProvider);

    MutableMethodInfo GetOrCreateOverride (ProxyType declaringType, MethodInfo overriddenMethod, out bool isNewlyCreated);

    MutablePropertyInfo CreateProperty (
        ProxyType declaringType,
        string name,
        Type type,
        IEnumerable<ParameterDeclaration> indexParameters,
        MethodAttributes accessorAttributes,
        Func<MethodBodyCreationContext, Expression> getBodyProvider,
        Func<MethodBodyCreationContext, Expression> setBodyProvider);

    MutablePropertyInfo CreateProperty (
        ProxyType declaringType, string name, PropertyAttributes attributes, MutableMethodInfo getMethod, MutableMethodInfo setMethod);

    MutableEventInfo CreateEvent (
        ProxyType declaringType,
        string name,
        Type handlerType,
        MethodAttributes accessorAttributes,
        Func<MethodBodyCreationContext, Expression> addBodyProvider,
        Func<MethodBodyCreationContext, Expression> removeBodyProvider,
        Func<MethodBodyCreationContext, Expression> raiseBodyProvider);

    MutableEventInfo CreateEvent (
        ProxyType declaringType,
        string name,
        EventAttributes attributes,
        MutableMethodInfo addMethod,
        MutableMethodInfo removeMethod,
        MutableMethodInfo raiseMethod);
  }
}