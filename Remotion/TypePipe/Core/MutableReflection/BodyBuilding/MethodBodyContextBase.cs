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
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.Expressions.ReflectionAdapters;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection.BodyBuilding
{
  /// <summary>
  /// Base class for context classes used to build new method bodies. (So-called body builders.)
  /// </summary>
  public abstract class MethodBodyContextBase
  {
    private readonly MutableType _declaringType;
    private readonly ReadOnlyCollection<ParameterExpression> _parameters;
    private readonly bool _isStatic;
    private readonly IMemberSelector _memberSelector;

    protected MethodBodyContextBase (
        MutableType declaringType, IEnumerable<ParameterExpression> parameterExpressions, bool isStatic, IMemberSelector memberSelector)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNull ("parameterExpressions", parameterExpressions);
      ArgumentUtility.CheckNotNull ("memberSelector", memberSelector);

      _declaringType = declaringType;
      _parameters = parameterExpressions.ToList().AsReadOnly();
      _isStatic = isStatic;
      _memberSelector = memberSelector;
    }

    public MutableType DeclaringType
    {
      get { return _declaringType; }
    }

    public Expression This
    {
      get
      {
        if (IsStatic)
          throw new InvalidOperationException ("Static methods cannot use 'This'.");

        return new ThisExpression (_declaringType);
      }
    }

    public ReadOnlyCollection<ParameterExpression> Parameters
    {
      get { return _parameters; }
    }

    public bool IsStatic
    {
      get { return _isStatic; }
    }

    public MethodCallExpression GetBaseCall (string methodName, params Expression[] arguments)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("arguments", arguments);

      // TODO visibility and instance/static
      // TODO add test for filtering bindingflags
      var baseTypeMethods = _declaringType.BaseType.GetMethods();
      var argumentTypes = arguments.Select (a => a.Type).ToArray();
      var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

      IMemberSelector selector = null;
      var baseMethod = selector.SelectSingleMethod (baseTypeMethods, Type.DefaultBinder, bindingFlags, methodName, _declaringType, argumentTypes, null);

      //if (baseMethod == null)
      //  return null;

      return GetBaseCall (baseMethod, arguments);
    }

    public MethodCallExpression GetBaseCall (MethodInfo method, params Expression[] arguments)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      ArgumentUtility.CheckNotNull ("arguments", arguments);

      // throw exception if mehod is static

      if (IsStatic)
        throw new InvalidOperationException ("Cannot perform base call from static method.");

      return Expression.Call (This, new BaseCallMethodInfoAdapter(method), arguments);
    }
  }
}