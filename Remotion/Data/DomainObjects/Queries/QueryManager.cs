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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// <see cref="QueryManager"/> provides methods to execute queries within a <see cref="RootPersistenceStrategy"/>.
  /// </summary>
  [Serializable]
  public class QueryManager : IQueryManager
  {
    private readonly IPersistenceStrategy _persistenceStrategy;
    private readonly IObjectLoader _objectLoader;
    private readonly IClientTransactionEventSink _transactionEventSink;

    // construction and disposing

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryManager"/> class.
    /// </summary>
    /// <param name="persistenceStrategy">The <see cref="IPersistenceStrategy"/> used to load query results not involving <see cref="DomainObject"/> instances.</param>
    /// <param name="objectLoader">An <see cref="IObjectLoader"/> implementation that can be used to load objects. This parameter determines
    ///   the <see cref="ClientTransaction"/> housing the objects loaded by queries.</param>
    /// <param name="transactionEventSink">The transaction event sink to use for raising query-related notifications.</param>
    public QueryManager (
        IPersistenceStrategy persistenceStrategy,
        IObjectLoader objectLoader,
        IClientTransactionEventSink transactionEventSink)
    {
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("objectLoader", objectLoader);
      ArgumentUtility.CheckNotNull ("transactionEventSink", transactionEventSink);
      
      _persistenceStrategy = persistenceStrategy;
      _objectLoader = objectLoader;
      _transactionEventSink = transactionEventSink;
    }

    public IPersistenceStrategy PersistenceStrategy
    {
      get { return _persistenceStrategy; }
    }

    public IObjectLoader ObjectLoader
    {
      get { return _objectLoader; }
    }

    public IClientTransactionEventSink TransactionEventSink
    {
      get { return _transactionEventSink; }
    }

    /// <summary>
    /// Executes a given <see cref="IQuery"/> and returns the scalar value.
    /// </summary>
    /// <param name="query">The query to execute. Must not be <see langword="null"/>.</param>
    /// <returns>The scalar value that is returned by the query.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="query"/> does not have a <see cref="Configuration.QueryType"/> of <see cref="Configuration.QueryType.Scalar"/>.</exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
    ///   The <see cref="IQuery.StorageProviderDefinition"/> of <paramref name="query"/> could not be found.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.PersistenceException">
    ///   The <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/> for the given <see cref="IQuery"/> could not be instantiated.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.StorageProviderException">
    ///   An error occurred while executing the query.
    /// </exception>
    public object GetScalar (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      if (query.QueryType != QueryType.Scalar)
        throw new ArgumentException ("A collection query cannot be used with GetScalar.", "query");

      return _persistenceStrategy.ExecuteScalarQuery (query);
    }

    /// <summary>
    /// Executes a given <see cref="IQuery"/> and returns a collection of the <see cref="DomainObject"/>s returned by the query.
    /// </summary>
    /// <param name="query">The query to execute. Must not be <see langword="null"/>.</param>
    /// <returns>A collection containing the <see cref="DomainObject"/>s returned by the query.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="query"/> does not have a <see cref="Configuration.QueryType"/> of <see cref="Configuration.QueryType.Collection"/>.</exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
    ///   The <see cref="IQuery.StorageProviderDefinition"/> of <paramref name="query"/> could not be found.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.PersistenceException">
    ///   The <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/> for the given <see cref="IQuery"/> could not be instantiated.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.StorageProviderException">
    ///   An error occurred while executing the query.
    /// </exception>
    public QueryResult<DomainObject> GetCollection (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return GetCollection<DomainObject> (query);
    }

    /// <summary>
    /// Executes a given <see cref="IQuery"/> and returns a collection of the <see cref="DomainObject"/>s returned by the query.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="DomainObjects"/> to be returned from the query.</typeparam>
    /// <param name="query">The query to execute. Must not be <see langword="null"/>.</param>
    /// <returns>
    /// A collection containing the <see cref="DomainObject"/>s returned by the query.
    /// </returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="UnexpectedQueryResultException">The objects returned by the <paramref name="query"/> do not match the expected type
    /// <typeparamref name="T"/> or the configured collection type is not assignable to <see cref="ObjectList{T}"/> with the given <typeparamref name="T"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="query"/> does not have a <see cref="Configuration.QueryType"/> of <see cref="Configuration.QueryType.Collection"/>.</exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
    /// The <see cref="IQuery.StorageProviderDefinition"/> of <paramref name="query"/> could not be found.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.PersistenceException">
    /// The <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/> for the given <see cref="IQuery"/> could not be instantiated.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.StorageProviderException">
    /// An error occurred while executing the query.
    /// </exception>
    public QueryResult<T> GetCollection<T> (IQuery query) where T: DomainObject
    {
      ArgumentUtility.CheckNotNull ("query", query);

      if (query.QueryType != QueryType.Collection)
        throw new ArgumentException ("A scalar query cannot be used with GetCollection.", "query");

      var resultArray = _objectLoader
          .GetOrLoadCollectionQueryResult (query)
          .Select (data => ConvertLoadedDomainObject<T> (data.GetDomainObjectReference())).ToArray();
      var queryResult = new QueryResult<T> (query, resultArray);
      _transactionEventSink.RaiseEvent ((tx, l) => queryResult = l.FilterQueryResult (tx, queryResult));
      return queryResult;
    }

    private T ConvertLoadedDomainObject<T> (DomainObject domainObject) where T : DomainObject
    {
      if (domainObject == null || domainObject is T)
        return (T) domainObject;
      else
      {
        var message = string.Format (
            "The query returned an object of type '{0}', but a query result of type '{1}' was expected.",
            domainObject.GetPublicDomainObjectType (),
            typeof (T));
        throw new UnexpectedQueryResultException (message);
      }
    }

  }
}
