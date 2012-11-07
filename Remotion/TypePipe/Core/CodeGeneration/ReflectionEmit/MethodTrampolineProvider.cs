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
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.Expressions.ReflectionAdapters;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit
{
  /// <summary>
  /// Provides method stubs for performing non virtual calls to virtual methods as it is needed for base calls.
  /// </summary>
  /// <remarks>This class is used by <see cref="UnemittableExpressionVisitor"/>.</remarks>
  public class MethodTrampolineProvider : IMethodTrampolineProvider
  {
    private readonly IMemberEmitter _memberEmitter;
    private readonly Dictionary<MethodInfo, MethodInfo> _trampolineDictionary =
        new Dictionary<MethodInfo, MethodInfo> (MemberInfoEqualityComparer<MethodInfo>.Instance);

    public MethodTrampolineProvider (IMemberEmitter memberEmitter)
    {
      ArgumentUtility.CheckNotNull ("memberEmitter", memberEmitter);

      _memberEmitter = memberEmitter;
    }

    public IMemberEmitter MemberEmitter
    {
      get { return _memberEmitter; }
    }

    public MethodInfo GetNonVirtualCallTrampoline (MemberEmitterContext context, MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("method", method);

      MethodInfo trampoline;
      if (!_trampolineDictionary.TryGetValue (method, out trampoline))
      {
        var name = string.Format ("{0}.{1}_NonVirtualCallTrampoline", method.DeclaringType.FullName, method.Name);
        trampoline = CreateNonVirtualCallTrampoline (context, method, name);
        _trampolineDictionary.Add (method, trampoline);
      }

      return trampoline;
    }

    private MethodInfo CreateNonVirtualCallTrampoline (MemberEmitterContext context, MethodInfo method, string trampolineName)
    {
      var parameterDescriptors = UnderlyingParameterInfoDescriptor.CreateFromMethodBase (method);
      var parameterExpressions = parameterDescriptors.Select (p => p.Expression).Cast<Expression>();
      var body = Expression.Call (new ThisExpression (context.MutableType), new NonVirtualCallMethodInfoAdapter (method), parameterExpressions);
      
      var descriptor = UnderlyingMethodInfoDescriptor.Create (
          trampolineName,
          MethodAttributes.Private,
          method.ReturnType,
          parameterDescriptors,
          baseMethod: null,
          isGenericMethod: false,
          isGenericMethodDefinition: false,
          containsGenericParameters: false,
          body: body);

      var trampoline = new MutableMethodInfo (context.MutableType, descriptor);

      _memberEmitter.AddMethod (context, trampoline, trampoline.Attributes);

      return trampoline;
    }
  }
}