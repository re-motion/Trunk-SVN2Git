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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.TypePipe
{
  /// <summary>
  /// Retrieves constructors and creates uses <see cref="LambdaExpression"/> to create delegates enabling their efficient invocation.
  /// </summary>
  public class ConstructorProvider : IConstructorProvider
  {
    public ConstructorInfo GetConstructor (
        Type generatedType, Type[] generatedParamterTypes, bool allowNonPublic, Type originalType, Type[] originalParameterTypes)
    {
      var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      var constructor = generatedType.GetConstructor (bindingFlags, null, generatedParamterTypes, null);

      if (constructor == null)
      {
        var message = string.Format (
            "Type {0} does not contain a constructor with the following signature: ({1}).",
            originalType.FullName,
            SeparatedStringBuilder.Build (", ", originalParameterTypes, pt => pt.Name));
        throw new MissingMethodException (message);
      }

      if (!constructor.IsPublic && !allowNonPublic)
      {
        var message = string.Format (
            "Type {0} contains a constructor with the required signature, but it is not public (and the allowNonPublic flag is not set).",
            originalType.FullName);
        throw new MissingMethodException (message);
      }

      return constructor;
    }

    public Delegate CreateConstructorCall (ConstructorInfo constructor, Type delegateType)
    {
      ArgumentUtility.CheckNotNull ("constructor", constructor);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("delegateType", delegateType, typeof (Delegate));

      var parameters = constructor.GetParameters().Select (p => Expression.Parameter (p.ParameterType, p.Name)).ToArray();
      // ReSharper disable CoVariantArrayConversion
      var constructorCall = Expression.New (constructor, parameters);
      // ReSharper restore CoVariantArrayConversion
      var lambda = Expression.Lambda (delegateType, constructorCall, parameters);

      return lambda.Compile();
    }
  }
}