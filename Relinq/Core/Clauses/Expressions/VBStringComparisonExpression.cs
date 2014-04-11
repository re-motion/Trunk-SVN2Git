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
using System.Linq.Expressions;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Linq.Parsing;
using Remotion.Utilities;

namespace Remotion.Linq.Clauses.Expressions
{
  /// <summary>
  /// Represents a VB-specific comparison expression.
  /// </summary>
  /// <remarks>
  /// <para>
  /// To explicitly support this expression type, implement <see cref="IVBSpecificExpressionVisitor"/>.
  /// To treat this expression as if it were an ordinary <see cref="BinaryExpression"/>, call its <see cref="Reduce"/> method and visit the result.
  /// </para>
  /// <para>
  /// Subclasses of <see cref="ThrowingExpressionTreeVisitor"/> that do not implement <see cref="IVBSpecificExpressionVisitor"/> will, by default, 
  /// automatically reduce this expression type to <see cref="BinaryExpression"/> in the 
  /// <see cref="ThrowingExpressionTreeVisitor.VisitExtensionExpression"/> method.
  /// </para>
  /// <para>
  /// Subclasses of <see cref="ExpressionTreeVisitor"/> that do not implement <see cref="IVBSpecificExpressionVisitor"/> will, by default, 
  /// ignore this expression and visit its child expressions via the <see cref="ExpressionTreeVisitor.VisitExtensionExpression"/> and 
  /// <see cref="VisitChildren"/> methods.
  /// </para>
  /// </remarks>
  public class VBStringComparisonExpression : ExtensionExpression
  {
    public const ExpressionType ExpressionType = (ExpressionType) 100003;

    private readonly Expression _comparison;
    private readonly bool _textCompare;

    public VBStringComparisonExpression (Expression comparison, bool textCompare)
        : base(comparison.Type, ExpressionType)
    {
      ArgumentUtility.CheckNotNull ("comparison", comparison);

      _comparison = comparison;
      _textCompare = textCompare;
    }

    public Expression Comparison
    {
      get { return _comparison; }
    }

    public bool TextCompare
    {
      get { return _textCompare; }
    }

    public override bool CanReduce
    {
      get { return true; }
    }

    public override Expression Reduce ()
    {
      return _comparison;
    }

    protected internal override Expression VisitChildren (ExpressionTreeVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      var newExpression = visitor.VisitExpression (_comparison);
      if (newExpression != _comparison)
        return new VBStringComparisonExpression (newExpression, _textCompare);
      else
        return this;
    }

    public override Expression Accept (ExpressionTreeVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      var specificVisitor = visitor as IVBSpecificExpressionVisitor;
      if (specificVisitor != null)
        return specificVisitor.VisitVBStringComparisonExpression (this);
      else
        return base.Accept (visitor);
    }

    public override string ToString ()
    {
      return string.Format ("VBCompareString({0}, {1})", FormattingExpressionTreeVisitor.Format(Comparison), TextCompare);
    }
    
  }
}