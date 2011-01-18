// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;

namespace Remotion.Data.DomainObjects.Linq
{
  public class DomainObjectQueryable<T> : QueryableBase<T>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainObjectQueryable{T}"/> class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This constructor marks the default entry point into a LINQ query for <see cref="DomainObject"/> instances. It is normally used to define
    /// the data source on which the first <c>from</c> expression operates.
    /// </para>
    /// <para>
    /// The <see cref="QueryFactory"/> class wraps this constructor and provides some additional support, so it should usually be preferred to a
    /// direct constructor call.
    /// </para>
    /// <param name="executor">The <see cref="DomainObjectQueryExecutor"/> that is used for the queries.</param>
    /// <param name="nodeTypeRegistry">Registry that maps the <see cref="MethodInfo"/> objects used in <see cref="MethodCallExpression"/> objects 
    /// to the respective <see cref="IExpressionNode"/> types.</param>
    /// </remarks>
    public DomainObjectQueryable (
        IQueryExecutor executor,
        MethodCallExpressionNodeTypeRegistry nodeTypeRegistry)
      : base (new DefaultQueryProvider (typeof (DomainObjectQueryable<>), executor, nodeTypeRegistry))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainObjectQueryable{T}"/> class.
    /// </summary>
    /// <param name="provider">The provider to be used for querying.</param>
    /// <param name="expression">The expression encapsulated by this <see cref="DomainObjectQueryable{T}"/> instance.</param>
    /// <remarks>
    /// This constructor is used by the standard query methods defined in <see cref="Queryable"/> when a LINQ query is constructed.
    /// </remarks>
    public DomainObjectQueryable (QueryProviderBase provider, Expression expression)
        : base (provider, expression)
    {
    }

    public override string ToString ()
    {
      return "DomainObjectQueryable<" + typeof (T).Name + ">";
    }

    public DomainObjectQueryExecutor GetExecutor ()
    {
      return (DomainObjectQueryExecutor) ((QueryProviderBase) Provider).Executor;
    }
  }
}