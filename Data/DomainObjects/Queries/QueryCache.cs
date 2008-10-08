/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Linq;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// Provides a thread-safe way to cache LINQ queries by a unique identifier.
  /// </summary>
  /// <remarks>
  /// While LINQ queries are convenient, flexible, and maintainable, parsing them and converting them to SQL is an expensive operation. Therefore,
  /// it is usually not a good idea to re-parse the same query over and over again. To foster reuse of the parsed query data, instances of this class 
  /// provide a thread-safe caching mechanism. The cache keys are supplied by the user, because LINQ queries cannot be used as cache keys with 
  /// reasonable performance.
  /// </remarks>
  public class QueryCache
  {
    private readonly InterlockedCache<string, IQuery> _cache = new InterlockedCache<string, IQuery>();

    /// <summary>
    /// Gets a query for the given LINQ query, returning it from the cache if possible.
    /// </summary>
    /// <typeparam name="T">The <see cref="DomainObject"/> type to start the query with.</typeparam>
    /// <param name="id">The ID to associate with the LINQ queryable. This ID is used as the cache key of the parsed query.</param>
    /// <param name="queryGenerator">A delegate returning the LINQ queryable. The argument of this delegate is a <see cref="DomainObjectQueryable{T}"/>
    /// to start the LINQ query with.</param>
    /// <returns>An <see cref="IQuery"/> implementation corresponding to the LINQ query returned by <paramref name="queryGenerator"/>.</returns>
    /// <remarks>
    /// The <paramref name="queryGenerator"/> delegate is only evaluated if no query with the given <paramref name="id"/> is found in the cache.
    /// </remarks>
    public IQuery GetQuery<T> (string id, Func<DomainObjectQueryable<T>, IQueryable> queryGenerator) where T : DomainObject
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("queryGenerator", queryGenerator);

      return _cache.GetOrCreateValue (id, delegate
                                          {
                                            var querySource = QueryFactory.CreateQueryable<T> ();
                                            var query = queryGenerator (querySource);
                                            return QueryFactory.CreateQuery (id, query);
                                          });
    }

    /// <summary>
    /// Gets a query for the given LINQ query and executes it in the given <paramref name="transaction"/>. The query is taken from the cache if 
    /// possible.
    /// </summary>
    /// <typeparam name="T">The <see cref="DomainObject"/> type to start the query with.</typeparam>
    /// <param name="transaction">The transaction whose <see cref="IQueryManager"/> is used to execute the query.</param>
    /// <param name="id">The ID to associate with the LINQ queryable. This ID is used as the cache key of the parsed query.</param>
    /// <param name="queryGenerator">A delegate returning the LINQ queryable. The argument of this delegate is a <see cref="DomainObjectQueryable{T}"/>
    /// to start the LINQ query with.</param>
    /// <returns>An <see cref="ObjectList{T}"/> holding the query results.</returns>
    /// <remarks>
    /// The <paramref name="queryGenerator"/> delegate is only evaluated if no query with the given <paramref name="id"/> is found in the cache. However,
    /// the query is always executed.
    /// </remarks>
    public ObjectList<T> ExecuteCollectionQuery<T> (ClientTransaction transaction, string id, Func<DomainObjectQueryable<T>, IQueryable> queryGenerator) where T : DomainObject
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("queryGenerator", queryGenerator);

      IQuery query = GetQuery (id, queryGenerator);
      return transaction.QueryManager.GetCollection<T> (query);
    }
  }
}