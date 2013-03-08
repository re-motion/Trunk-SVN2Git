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
using System.Reflection.Emit;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.Expressions;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit.Expressions
{
  /// <summary>
  /// Represents a <see cref="OpCodes.Box"/> operation followed by a <see cref="OpCodes.Castclass"/> to the correct type.
  /// </summary>
  public class BoxExpression : PrimitiveTypePipeExpressionBase
  {
    private readonly Expression _operand;

    public BoxExpression (Expression operand, Type toType)
        : base (toType)
    {
      ArgumentUtility.CheckNotNull ("operand", operand);

      _operand = operand;
    }

    public Expression Operand
    {
      get { return _operand; }
    }

    public Type FromType
    {
      get { return null; }
    }

    public Type ToType
    {
      get { return null; }
    }

    public override Expression Accept (IPrimitiveTypePipeExpressionVisitor visitor)
    {
      return this;
    }

    protected internal override Expression VisitChildren (ExpressionVisitor visitor)
    {
      return this;
    }
  }
}