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
using Microsoft.Scripting.Ast;
using Remotion.Utilities;

namespace Remotion.TypePipe.Expressions
{
  /// <summary>
  ///   Represents the address of a virtual method.
  /// </summary>
  /// <remarks>
  ///   This is the expression equivalent to <see cref="OpCodes.Ldvirtftn"/>.
  /// </remarks>
  public class VirtualMethodAddressExpression : TypePipeExpressionBase
  {
    private readonly Expression _instance;
    private readonly MethodInfo _method;

    public VirtualMethodAddressExpression (Expression instance, MethodInfo virtualMethod)
        : base (typeof (IntPtr))
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      ArgumentUtility.CheckNotNull ("virtualMethod", virtualMethod);
      Assertion.IsNotNull (virtualMethod.DeclaringType);

      if (!virtualMethod.DeclaringType.IsAssignableFrom (instance.Type))
        throw new ArgumentException ("Method is not declared on type hierarchy of instance.", "virtualMethod");

      _instance = instance;
      _method = virtualMethod;
    }

    public Expression Instance
    {
      get { return _instance; }
    }

    public MethodInfo Method
    {
      get { return _method; }
    }

    public override Expression Accept (ITypePipeExpressionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      return visitor.VisitVirtualMethodAddress (this);
    }

    protected internal override Expression VisitChildren (ExpressionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      var newInstance = visitor.Visit (_instance);
      if (newInstance != _instance)
        return new VirtualMethodAddressExpression (newInstance, _method);
      else
        return this;
    }
  }
}