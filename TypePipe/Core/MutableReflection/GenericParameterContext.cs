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
using System.Linq;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// Provides access to the generic parameters of a generic method for building its parameters and return type.
  /// </summary>
  /// <seealso cref="MutableType.AddMethod(string,System.Reflection.MethodAttributes,System.Collections.Generic.IEnumerable{Remotion.TypePipe.MutableReflection.GenericParameterDeclaration},System.Func{Remotion.TypePipe.MutableReflection.GenericParameterContext,System.Type},System.Func{Remotion.TypePipe.MutableReflection.GenericParameterContext,System.Collections.Generic.IEnumerable{Remotion.TypePipe.MutableReflection.ParameterDeclaration}},System.Func{Remotion.TypePipe.MutableReflection.BodyBuilding.MethodBodyCreationContext,Remotion.TypePipe.Dlr.Ast.Expression})"/>
  public class GenericParameterContext
  {
    private readonly IReadOnlyList<Type> _genericParameters;

    public GenericParameterContext (IEnumerable<Type> genericParameters)
    {
      _genericParameters = genericParameters.ToList().AsReadOnly();
    }

    public IReadOnlyList<Type> GenericParameters
    {
      get { return _genericParameters; }
    }
  }
}