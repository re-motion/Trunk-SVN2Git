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

namespace Remotion.TypePipe.MutableReflection.Generics
{
  /// <summary>
  /// Defines methods for substituting generic type parameters with type arguments in members of a generic type definition.
  /// </summary>
  public interface ITypeInstantiator : ITypeAdjuster
  {
    IEnumerable<Type> TypeArguments { get; }

    string GetFullName (Type genericTypeDefinition);

    FieldInfo SubstituteGenericParameters (FieldInfo field);
    ConstructorInfo SubstituteGenericParameters (ConstructorInfo constructor);
    MethodInfo SubstituteGenericParameters (MethodInfo method);
    PropertyInfo SubstituteGenericParameters (PropertyInfo property);
    EventInfo SubstituteGenericParameters (EventInfo event_);
  }
}