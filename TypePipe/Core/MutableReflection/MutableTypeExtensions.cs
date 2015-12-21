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
using Remotion.TypePipe.Dlr.Ast;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// This class contains convenience APIs for <see cref="MutableType"/>.
  /// </summary>
  public static class MutableTypeExtensions
  {
    public static void AddTypeInitialization (this MutableType declaringType, Expression typeInitialization)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNull ("typeInitialization", typeInitialization);

      if (declaringType.MutableTypeInitializer == null)
        declaringType.AddTypeInitializer (ctx => typeInitialization);
      else
        declaringType.MutableTypeInitializer.SetBody (ctx => Expression.Block (ctx.PreviousBody, typeInitialization));
    }

    public static MutableMethodInfo AddMethod (
        this MutableType declaringType,
        string name,
        MethodAttributes attributes = MethodAttributes.Public,
        Type returnType = null,
        IEnumerable<ParameterDeclaration> parameters = null,
        Func<MethodBodyCreationContext, Expression> bodyProvider = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      returnType = returnType ?? typeof (void);
      parameters = parameters ?? ParameterDeclaration.None;
      // Body provider may be null (for abstract methods).

      return declaringType.AddMethod (name, attributes, GenericParameterDeclaration.None, ctx => returnType, ctx => parameters, bodyProvider);
    }

    public static MutableMethodInfo AddMethod (
        this MutableType declaringType,
        string name,
        MethodAttributes attributes,
        MethodDeclaration methodDeclaration,
        Func<MethodBodyCreationContext, Expression> bodyProvider)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("methodDeclaration", methodDeclaration);
      // Body provider may be null (for abstract methods).

      var md = methodDeclaration;
      return declaringType.AddMethod (name, attributes, md.GenericParameters, md.ReturnTypeProvider, md.ParameterProvider, bodyProvider);
    }

    public static MutableMethodInfo AddAbstractMethod (
        this MutableType declaringType,
        string name,
        MethodAttributes attributes = MethodAttributes.Public,
        Type returnType = null,
        IEnumerable<ParameterDeclaration> parameters = null)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      returnType = returnType ?? typeof (void);
      parameters = parameters ?? ParameterDeclaration.None;

      var abstractAttributes = attributes.Set (MethodAttributes.Abstract | MethodAttributes.Virtual);
      return declaringType.AddMethod (name, abstractAttributes, returnType, parameters, bodyProvider: null);
    }
  }
}