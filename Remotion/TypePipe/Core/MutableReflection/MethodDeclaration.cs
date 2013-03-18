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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Remotion.FunctionalProgramming;
using Remotion.TypePipe.MutableReflection.Generics;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// Holds all information required to declare a method signature, that is, the generic parameters, return type and parameters.
  /// </summary>
  /// <remarks><see cref="CreateEquivalent"/> can be used to create methods that have an equal or similiar signature to an existing method.</remarks>
  /// <seealso cref="MutableType.AddMethod(string,System.Reflection.MethodAttributes,System.Type,System.Collections.Generic.IEnumerable{Remotion.TypePipe.MutableReflection.ParameterDeclaration},System.Func{Remotion.TypePipe.MutableReflection.BodyBuilding.MethodBodyCreationContext,Microsoft.Scripting.Ast.Expression})"/>
  /// <seealso cref="MutableType.AddMethod(string,System.Reflection.MethodAttributes,System.Collections.Generic.IEnumerable{Remotion.TypePipe.MutableReflection.GenericParameterDeclaration},System.Func{Remotion.TypePipe.MutableReflection.GenericParameterContext,System.Type},System.Func{Remotion.TypePipe.MutableReflection.GenericParameterContext,System.Collections.Generic.IEnumerable{Remotion.TypePipe.MutableReflection.ParameterDeclaration}},System.Func{Remotion.TypePipe.MutableReflection.BodyBuilding.MethodBodyCreationContext,Microsoft.Scripting.Ast.Expression})"/>
  public class MethodDeclaration
  {
    public static MethodDeclaration CreateEquivalent (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      if (method.IsGenericMethodInstantiation())
        throw new ArgumentException (
            "The specified method must be either a non-generic method or a generic method definition; it cannot be a method instantiation.", "method");

      var oldGenericParameters = method.GetGenericArguments();
      var instantiationContext = new TypeInstantiationContext();

      var genericParameters = oldGenericParameters.Select (g => CreateEquivalentGenericParameter (g, oldGenericParameters, instantiationContext));
      Func<GenericParameterContext, Type> returnTypeProvider =
          ctx =>
          {
            var parametersToArguments = oldGenericParameters.Zip (ctx.GenericParameters).ToDictionary (t => t.Item1, t => t.Item2);
            return instantiationContext.SubstituteGenericParameters (method.ReturnType, parametersToArguments);
          };
      Func<GenericParameterContext, IEnumerable<ParameterDeclaration>> parameterProvider =
          ctx =>
          {
            var parametersToArguments = oldGenericParameters.Zip (ctx.GenericParameters).ToDictionary (t => t.Item1, t => t.Item2);
            return method.GetParameters().Select (p => CreateEquivalentParameter (p, parametersToArguments, instantiationContext));
          };

      return new MethodDeclaration (genericParameters, returnTypeProvider, parameterProvider);
    }

    private static GenericParameterDeclaration CreateEquivalentGenericParameter (
        Type genericParameter, IEnumerable<Type> oldGenericParameters, TypeInstantiationContext instantiationContext)
    {
      Func<GenericParameterContext, IEnumerable<Type>> constraintProvider = ctx =>
      {
        var parametersToArguments = oldGenericParameters.Zip (ctx.GenericParameters).ToDictionary (t => t.Item1, t => t.Item2);
        return genericParameter.GetGenericParameterConstraints()
                               .Where (g => g != typeof (ValueType))
                               .Select (c => instantiationContext.SubstituteGenericParameters (c, parametersToArguments));
      };
      return new GenericParameterDeclaration (genericParameter.Name, genericParameter.GenericParameterAttributes, constraintProvider);
    }

    private static ParameterDeclaration CreateEquivalentParameter (
        ParameterInfo parameter, IDictionary<Type, Type> parametersToArguments, TypeInstantiationContext instantiationContext)
    {
      var type = instantiationContext.SubstituteGenericParameters (parameter.ParameterType, parametersToArguments);
      return new ParameterDeclaration (type, parameter.Name, parameter.Attributes);
    }

    private readonly ReadOnlyCollection<GenericParameterDeclaration> _genericParameters;
    private readonly Func<GenericParameterContext, Type> _returnTypeProvider;
    private readonly Func<GenericParameterContext, IEnumerable<ParameterDeclaration>> _parameterProvider;

    public MethodDeclaration (
        IEnumerable<GenericParameterDeclaration> genericParameters,
        Func<GenericParameterContext, Type> returnTypeProvider,
        Func<GenericParameterContext, IEnumerable<ParameterDeclaration>> parameterProvider)
    {
      ArgumentUtility.CheckNotNull ("genericParameters", genericParameters);
      ArgumentUtility.CheckNotNull ("returnTypeProvider", returnTypeProvider);
      ArgumentUtility.CheckNotNull ("parameterProvider", parameterProvider);

      _genericParameters = genericParameters.ToList().AsReadOnly();
      _returnTypeProvider = returnTypeProvider;
      _parameterProvider = parameterProvider;
    }

    public ReadOnlyCollection<GenericParameterDeclaration> GenericParameters
    {
      get { return _genericParameters; }
    }

    public Func<GenericParameterContext, Type> ReturnTypeProvider
    {
      get { return _returnTypeProvider; }
    }

    public Func<GenericParameterContext, IEnumerable<ParameterDeclaration>> ParameterProvider
    {
      get { return _parameterProvider; }
    }
  }
}