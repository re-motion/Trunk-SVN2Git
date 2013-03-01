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
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Generics;
using Remotion.TypePipe.StrongNaming;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit
{
  /// <summary>
  /// A decorator that checks operands for strong-name compatibility.
  /// </summary>
  /// <remarks>
  /// Uses an instance <see cref="ITypeAnalyzer"/> retrieved via the <see cref="SafeServiceLocator"/>.
  /// </remarks>
  public class StrongNameCheckingEmittableOperandProviderDecorator : IEmittableOperandProvider
  {
    private readonly ITypeAnalyzer _typeAnalyzer = SafeServiceLocator.Current.GetInstance<ITypeAnalyzer>();

    private readonly IEmittableOperandProvider _emittableOperandProvider;

    public StrongNameCheckingEmittableOperandProviderDecorator (IEmittableOperandProvider emittableOperandProvider)
    {
      ArgumentUtility.CheckNotNull ("emittableOperandProvider", emittableOperandProvider);

      _emittableOperandProvider = emittableOperandProvider;
    }

    public IEmittableOperandProvider InnerEmittableOperandProvider
    {
      get { return _emittableOperandProvider; }
    }

    public void AddMapping (ProxyType mappedType, Type emittableType)
    {
      _emittableOperandProvider.AddMapping (mappedType, emittableType);
    }

    public void AddMapping (MutableGenericParameter mappedGenericParameter, Type emittableGenericParameter)
    {
      _emittableOperandProvider.AddMapping (mappedGenericParameter, emittableGenericParameter);
    }

    public void AddMapping (MutableFieldInfo mappedField, FieldInfo emittableField)
    {
      _emittableOperandProvider.AddMapping (mappedField, emittableField);
    }

    public void AddMapping (MutableConstructorInfo mappedConstructor, ConstructorInfo emittableConstructor)
    {
      _emittableOperandProvider.AddMapping (mappedConstructor, emittableConstructor);
    }

    public void AddMapping (MutableMethodInfo mappedMethod, MethodInfo emittableMethod)
    {
      _emittableOperandProvider.AddMapping (mappedMethod, emittableMethod);
    }

    public Type GetEmittableType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var emittableType = _emittableOperandProvider.GetEmittableType (type);
      CheckStrongNameCompatibility (emittableType);

      return emittableType;
    }

    public FieldInfo GetEmittableField (FieldInfo field)
    {
      ArgumentUtility.CheckNotNull ("field", field);

      var emittableField = _emittableOperandProvider.GetEmittableField (field);
      CheckStrongNameCompatibility (emittableField.DeclaringType);

      return emittableField;
    }

    public ConstructorInfo GetEmittableConstructor (ConstructorInfo constructor)
    {
      ArgumentUtility.CheckNotNull ("constructor", constructor);

      var emittableConstructor = _emittableOperandProvider.GetEmittableConstructor (constructor);
      CheckStrongNameCompatibility (emittableConstructor.DeclaringType);

      return emittableConstructor;
    }

    public MethodInfo GetEmittableMethod (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      var emittableMethod = _emittableOperandProvider.GetEmittableMethod (method);

      CheckStrongNameCompatibility (emittableMethod.DeclaringType);
      if (emittableMethod.IsGenericMethod)
      {
        foreach (var genericArgument in emittableMethod.GetGenericArguments())
          CheckStrongNameCompatibility (genericArgument);
      }

      return emittableMethod;
    }

    private void CheckStrongNameCompatibility (Type type)
    {
      if (!_typeAnalyzer.IsStrongNamed (type))
      {
        var message = string.Format (
            "Strong-naming is enabled but a participant used the type '{0}' which comes from the unsigned assembly '{1}'.",
            type.FullName,
            type.Assembly.GetName().Name);
        throw new InvalidOperationException (message);
      }
    }
  }
}