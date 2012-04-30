// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Linq;
using Remotion.Linq.EagerFetching;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Provides an implementation of <see cref="IQueryExecutor"/> for <see cref="DomainObject"/> queries.
  /// </summary>
  public class DomainObjectQueryExecutor : IQueryExecutor
  {
    private readonly ClassDefinition _startingClassDefinition;
    private readonly IStorageTypeInformationProvider _storageTypeInformationProvider;

    private readonly IDomainObjectQueryGenerator _queryGenerator;

    public DomainObjectQueryExecutor (
        ClassDefinition startingClassDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IDomainObjectQueryGenerator queryGenerator)
    {
      ArgumentUtility.CheckNotNull ("startingClassDefinition", startingClassDefinition);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);
      ArgumentUtility.CheckNotNull ("queryGenerator", queryGenerator);

      _startingClassDefinition = startingClassDefinition;
      _storageTypeInformationProvider = storageTypeInformationProvider;

      _queryGenerator = queryGenerator;
    }

    public ClassDefinition StartingClassDefinition
    {
      get { return _startingClassDefinition; }
    }

    public IStorageTypeInformationProvider StorageTypeInformationProvider
    {
      get { return _storageTypeInformationProvider; }
    }

    public IDomainObjectQueryGenerator QueryGenerator
    {
      get { return _queryGenerator; }
    }

    /// <summary>
    /// Creates and executes a given <see cref="QueryModel"/> as an <see cref="IQuery"/> using the current <see cref="ClientTransaction"/>'s
    /// <see cref="ClientTransaction.QueryManager"/>. The query is executed as a scalar query.
    /// </summary>
    /// <param name="queryModel">The generated <see cref="QueryModel"/> of the LINQ query.</param>
    /// <returns>
    /// The result of the executed query, cast to <typeparam name="T"/>.
    /// </returns>
    public T ExecuteScalar<T> (QueryModel queryModel)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      if (ClientTransaction.Current == null)
        throw new InvalidOperationException ("No ClientTransaction has been associated with the current thread.");

      var fetchQueryModelBuilders = RemoveTrailingFetchRequests (queryModel);

      var query = _queryGenerator.CreateQuery ("<dynamic query>", _startingClassDefinition, queryModel, fetchQueryModelBuilders, QueryType.Scalar);
      object scalarValue = ClientTransaction.Current.QueryManager.GetScalar (query);

      if (scalarValue == null || scalarValue == DBNull.Value)
        return (T) (object) null;

      if (scalarValue is T)
        return (T) scalarValue;

      // If we can't cast, there's still the chance that we can convert. This is especially needed for booleans, which will be integers when coming 
      // from the database.
      return (T) Convert.ChangeType (scalarValue, typeof (T));
    }

    /// <summary>
    /// Creates and executes a given <see cref="QueryModel"/> as an <see cref="IQuery"/> using the current <see cref="ClientTransaction"/>'s
    /// <see cref="ClientTransaction.QueryManager"/>. The query is executed as a collection query, and its result set is expected in its result set.
    /// </summary>
    /// <param name="queryModel">The generated <see cref="QueryModel"/> of the LINQ query.</param>
    /// <param name="returnDefaultWhenEmpty">If <see langword="true" />, the executor returns a default value when the result set is empty; 
    /// if <see langword="false" />, it throws an <see cref="InvalidOperationException"/> when its result set is empty.</param>
    /// <returns>
    /// The result of the executed query, cast to <typeparam name="T"/>.
    /// </returns>
    public T ExecuteSingle<T> (QueryModel queryModel, bool returnDefaultWhenEmpty)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      if (ClientTransaction.Current == null)
        throw new InvalidOperationException ("No ClientTransaction has been associated with the current thread.");

      // Natively supported types can be executed as scalar queries
      if (_storageTypeInformationProvider.IsTypeSupported (typeof (T)))
        return ExecuteScalar<T> (queryModel);

      var sequence = ExecuteCollection<T> (queryModel);

      if (returnDefaultWhenEmpty)
        return sequence.SingleOrDefault();
      else
        return sequence.Single();
    }

    /// <summary>
    /// Creates and executes a given <see cref="QueryModel"/> as an <see cref="IQuery"/> using the current <see cref="ClientTransaction"/>'s
    /// <see cref="ClientTransaction.QueryManager"/>. The query is executed as a collection query.
    /// </summary>
    /// <param name="queryModel">The generated <see cref="QueryModel"/> of the LINQ query.</param>
    /// <returns>
    /// The result of the executed query as an <see cref="IEnumerable{T}"/>.
    /// </returns>
    public IEnumerable<T> ExecuteCollection<T> (QueryModel queryModel)
    {
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      if (ClientTransaction.Current == null)
        throw new InvalidOperationException ("No ClientTransaction has been associated with the current thread.");

      var fetchQueryModelBuilders = RemoveTrailingFetchRequests (queryModel);

      IQuery query = _queryGenerator.CreateQuery ("<dynamic query>", _startingClassDefinition, queryModel, fetchQueryModelBuilders, QueryType.Collection);
      return ClientTransaction.Current.QueryManager.GetCollection (query).AsEnumerable().Cast<T>();
    }

    private ICollection<FetchQueryModelBuilder> RemoveTrailingFetchRequests (QueryModel queryModel)
    {
      var result = new List<FetchQueryModelBuilder> ();
      for (int i = queryModel.ResultOperators.Count - 1; i >= 0 && queryModel.ResultOperators[i] is FetchRequestBase; --i)
      {
        result.Add (new FetchQueryModelBuilder (queryModel.ResultOperators[i] as FetchRequestBase, queryModel, i));
        queryModel.ResultOperators.RemoveAt (i);
      }
      return result;
    }
  }
}