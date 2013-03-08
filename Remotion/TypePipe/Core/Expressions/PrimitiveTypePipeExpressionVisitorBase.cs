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
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Expressions;
using Remotion.Utilities;

namespace Remotion.TypePipe.Expressions
{
  /// <summary>
  /// An abstract base class for <see cref="ExpressionVisitor"/> implementations that also need to handle 
  /// <see cref="IPrimitiveTypePipeExpression"/> nodes.
  /// </summary>
  public abstract class PrimitiveTypePipeExpressionVisitorBase : ExpressionVisitor, IPrimitiveTypePipeExpressionVisitor
  {
    Expression IPrimitiveTypePipeExpressionVisitor.VisitThis (ThisExpression node)
    {
      return VisitThis (node);
    }

    Expression IPrimitiveTypePipeExpressionVisitor.VisitNewDelegate (NewDelegateExpression node)
    {
      ArgumentUtility.CheckNotNull ("node", node);

      return VisitNewDelegate (node);
    }

    Expression IPrimitiveTypePipeExpressionVisitor.VisitBox (BoxExpression node)
    {
      ArgumentUtility.CheckNotNull ("node", node);

      return VisitBox (node);
    }

    Expression IPrimitiveTypePipeExpressionVisitor.VisitUnbox (UnboxExpression node)
    {
      return VisitUnbox (node);
    }

    protected virtual Expression VisitThis (ThisExpression node)
    {
      return VisitExtension (node);
    }

    protected virtual Expression VisitNewDelegate (NewDelegateExpression node)
    {
      return VisitExtension (node);
    }

    protected virtual Expression VisitBox (BoxExpression node)
    {
      return VisitExtension (node);
    }

    protected virtual Expression VisitUnbox (UnboxExpression node)
    {
      return VisitExtension (node);
    }
  }
}