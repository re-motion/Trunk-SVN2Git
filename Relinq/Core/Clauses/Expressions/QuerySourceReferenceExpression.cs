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
using Remotion.Utilities;

namespace Remotion.Linq.Clauses.Expressions
{
  /// <summary>
  /// Represents an expression tree node that points to a query source represented by a <see cref="FromClauseBase"/>. These expressions should always
  /// point back, to a clause defined prior to the clause holding a <see cref="QuerySourceReferenceExpression"/>. Otherwise, exceptions might be 
  /// thrown at runtime.
  /// </summary>
  /// <remarks>
  /// This particular expression overrides <see cref="Equals"/>, i.e. it can be compared to another <see cref="QuerySourceReferenceExpression"/> based
  /// on the <see cref="ReferencedQuerySource"/>.
  /// </remarks>
  public class QuerySourceReferenceExpression : Expression
  {
    private readonly Type _type;
    public const ExpressionType ExpressionType = (ExpressionType) 100001;

    public QuerySourceReferenceExpression (IQuerySource querySource)
    {
      ArgumentUtility.CheckNotNull ("querySource", querySource);

      _type = querySource.ItemType;
      ReferencedQuerySource = querySource;
    }

    public override ExpressionType NodeType
    {
      get { return ExpressionType; }
    }

    public override Type Type
    {
      get { return _type; }
    }
    
    /// <summary>
    /// Gets the query source referenced by this expression.
    /// </summary>
    /// <value>The referenced query source.</value>
    public IQuerySource ReferencedQuerySource { get; private set; }

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="QuerySourceReferenceExpression"/> by 
    /// comparing the <see cref="ReferencedQuerySource"/> properties for reference equality.
    /// </summary>
    /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="QuerySourceReferenceExpression"/>.</param>
    /// <returns>
    /// <see langword="true" /> if the specified <see cref="T:System.Object"/> is a <see cref="QuerySourceReferenceExpression"/> that points to the 
    /// same <see cref="ReferencedQuerySource"/>; otherwise, false.
    /// </returns>
    public override bool Equals (object obj)
    {
      var other = obj as QuerySourceReferenceExpression;
      return other != null && ReferencedQuerySource == other.ReferencedQuerySource;
    }

    public override int GetHashCode ()
    {
      return ReferencedQuerySource.GetHashCode ();
    }
  }
}
