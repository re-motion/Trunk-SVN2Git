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
using System.Linq.Expressions;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Parsing;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;
using Remotion.Utilities;

namespace Remotion.Linq.Clauses.Expressions
{
  /// <summary>
  /// Wraps an exception whose partial evaluation caused an exception.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When <see cref="PartialEvaluatingExpressionTreeVisitor"/> encounters an exception while evaluating an independent expression subtree, it
  /// will wrap the subtree within a <see cref="PartialEvaluationExceptionExpression"/>. The wrapper contains both the <see cref="Exception"/> 
  /// instance and the <see cref="EvaluatedExpression"/> that caused the exception.
  /// </para>
  /// <para>
  /// To explicitly support this expression type, implement  <see cref="IPartialEvaluationExceptionExpressionVisitor"/>.
  /// To ignore this wrapper and only handle the inner <see cref="EvaluatedExpression"/>, call the <see cref="Reduce"/> method and visit the result.
  /// </para>
  /// <para>
  /// Subclasses of <see cref="ThrowingExpressionTreeVisitor"/> that do not implement <see cref="IPartialEvaluationExceptionExpressionVisitor"/> will, 
  /// by default, automatically reduce this expression type to the <see cref="EvaluatedExpression"/> in the 
  /// <see cref="ThrowingExpressionTreeVisitor.VisitExtensionExpression"/> method.
  /// </para>
  /// <para>
  /// Subclasses of <see cref="ExpressionTreeVisitor"/> that do not implement <see cref="IPartialEvaluationExceptionExpressionVisitor"/> will, 
  /// by default, ignore this expression and visit its child expressions via the <see cref="ExpressionTreeVisitor.VisitExtensionExpression"/> and 
  /// <see cref="VisitChildren"/> methods.
  /// </para>
  /// </remarks>
  public class PartialEvaluationExceptionExpression : ExtensionExpression
  {
    public const ExpressionType ExpressionType = (ExpressionType) 100004;

    private readonly Exception _exception;
    private readonly Expression _evaluatedExpression;

    public PartialEvaluationExceptionExpression (Exception exception, Expression evaluatedExpression)
      : base (ArgumentUtility.CheckNotNull ("evaluatedExpression", evaluatedExpression).Type, ExpressionType)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);
      
      _exception = exception;
      _evaluatedExpression = evaluatedExpression;
    }

    public Exception Exception
    {
      get { return _exception; }
    }

    public Expression EvaluatedExpression
    {
      get { return _evaluatedExpression; }
    }

    public override bool CanReduce
    {
      get { return true; }
    }

    public override Expression Reduce ()
    {
      return _evaluatedExpression;
    }

    protected internal override Expression VisitChildren (ExpressionTreeVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      var newEvaluatedExpression = visitor.VisitExpression (_evaluatedExpression);
      if (newEvaluatedExpression != _evaluatedExpression)
        return new PartialEvaluationExceptionExpression (_exception, newEvaluatedExpression);
      else
        return this;
    }

    public override Expression Accept (ExpressionTreeVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      var specificVisitor = visitor as IPartialEvaluationExceptionExpressionVisitor;
      if (specificVisitor != null)
        return specificVisitor.VisitPartialEvaluationExceptionExpression (this);
      else
        return base.Accept (visitor);
    }

    public override string ToString ()
    {
      return string.Format (
          @"PartialEvalException ({0} (""{1}""), {2})",
          _exception.GetType().Name,
          _exception.Message,
          FormattingExpressionTreeVisitor.Format (_evaluatedExpression));
    }
  }
}