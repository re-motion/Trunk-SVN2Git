/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.Linq.QueryProviderImplementation;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Mixins;


namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// The implementation of <see cref="IQueryable{T}"/> for querying <see cref="DomainObject"/> instances.
  /// </summary>
  /// <typeparam name="T">The <see cref="DomainObject"/> type to be queried.</typeparam>
  public class DomainObjectQueryable<T> : QueryableBase<T> 
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainObjectQueryable&lt;T&gt;"/> class.
    /// </summary>
    /// <param name="provider">The provider to be used for querying.</param>
    /// <param name="expression">The expression encapsulated by this <see cref="DomainObjectQueryable{T}"/> instance.</param>
    /// <remarks>
    /// This constructor is used by the standard query methods defined in <see cref="Queryable"/> when a LINQ query is constructed.
    /// </remarks>
    public DomainObjectQueryable (QueryProvider provider, Expression expression)
      : base (provider, expression)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainObjectQueryable&lt;T&gt;"/> class.
    /// </summary>
    /// <param name="sqlGenerator">The <see cref="ISqlGenerator"/> to be used when this query is translated into SQL.</param>
    /// <remarks>
    /// <para>
    /// This constructor marks the default entry point into a LINQ query for <see cref="DomainObject"/> instances. It is normally used to define
    /// the data source on which the first <c>from</c> expression operates.
    /// </para>
    /// <para>
    /// The <see cref="QueryFactory"/> class wraps this constructor and provides some additional support, so it should usually be preferred to a
    /// direct constructor call.
    /// </para>
    /// </remarks>
    public DomainObjectQueryable (ISqlGenerator sqlGenerator)
      : base (new QueryProvider(ObjectFactory.Create<QueryExecutor<T>>().With(sqlGenerator)))
    {
    }

    public override string ToString ()
    {
      return "DomainObjectQueryable<" + typeof (T).Name + ">";
    }

    public new QueryProvider Provider
    {
      get { return (QueryProvider) base.Provider; }
    }

    public QueryExecutor<T> GetExecutor ()
    {
      return (QueryExecutor<T>) Provider.Executor;
    }
  }
}
