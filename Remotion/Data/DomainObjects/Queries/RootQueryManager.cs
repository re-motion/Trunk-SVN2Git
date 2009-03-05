// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// <see cref="RootQueryManager"/> provides methods to execute queries within a <see cref="RootClientTransaction"/>.
  /// </summary>
  [Serializable]
  public class RootQueryManager : IQueryManager
  {
    // types

    // static members and constants

    // member fields

    private readonly RootClientTransaction _clientTransaction;

    // construction and disposing

    /// <summary>
    /// Initializes a new instance of the <see cref="RootQueryManager"/> class. 
    /// </summary>
    /// <remarks>
    /// All <see cref="DomainObject"/>s that are loaded by the <b>RootQueryManager</b> will exist within the given <see cref="Remotion.Data.DomainObjects.ClientTransaction"/>.
    /// </remarks>
    /// <param name="clientTransaction">The <see cref="RootClientTransaction"/> to be used in the <b>RootQueryManager</b>. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="clientTransaction"/> is <see langword="null"/>.</exception>
    public RootQueryManager (RootClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      _clientTransaction = clientTransaction;
    }

    // methods and properties

    /// <summary>
    /// Gets the <see cref="Remotion.Data.DomainObjects.ClientTransaction"/> that is associated with the <see cref="RootQueryManager"/>.
    /// </summary>
    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    /// <summary>
    /// Executes a given <see cref="IQuery"/> and returns the scalar value.
    /// </summary>
    /// <param name="query">The query to execute. Must not be <see langword="null"/>.</param>
    /// <returns>The scalar value that is returned by the query.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="query"/> does not have a <see cref="Configuration.QueryType"/> of <see cref="Configuration.QueryType.Scalar"/>.</exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
    ///   The <see cref="IQuery.StorageProviderID"/> of <paramref name="query"/> could not be found.
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

      if (query.QueryType == QueryType.Collection)
        throw new ArgumentException ("A collection query cannot be used with GetScalar.", "query");

      using (StorageProviderManager storageProviderManager = new StorageProviderManager())
      {
        StorageProvider provider = storageProviderManager.GetMandatory (query.StorageProviderID);
        return provider.ExecuteScalarQuery (query);
      }
    }

    /// <summary>
    /// Executes a given <see cref="IQuery"/> and returns a collection of the <see cref="DomainObject"/>s returned by the query.
    /// </summary>
    /// <param name="query">The query to execute. Must not be <see langword="null"/>.</param>
    /// <returns>A collection containing the <see cref="DomainObject"/>s returned by the query.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="query"/> does not have a <see cref="Configuration.QueryType"/> of <see cref="Configuration.QueryType.Collection"/>.</exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
    ///   The <see cref="IQuery.StorageProviderID"/> of <paramref name="query"/> could not be found.
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
    /// <exception cref="InvalidTypeException">The objects returned by the <paramref name="query"/> do not match the expected type
    /// <typeparamref name="T"/> or the configured collection type is not assignable to <see cref="ObjectList{T}"/> with the given <typeparamref name="T"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="query"/> does not have a <see cref="Configuration.QueryType"/> of <see cref="Configuration.QueryType.Collection"/>.</exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
    /// The <see cref="IQuery.StorageProviderID"/> of <paramref name="query"/> could not be found.
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

      if (query.QueryType == QueryType.Scalar)
        throw new ArgumentException ("A scalar query cannot be used with GetCollection.", "query");

      using (_clientTransaction.EnterNonDiscardingScope())
      {
        using (var storageProviderManager = new StorageProviderManager())
        {
          StorageProvider provider = storageProviderManager.GetMandatory (query.StorageProviderID);
          var dataContainers = new DataContainerCollection (provider.ExecuteCollectionQuery (query), false);

          var resultArray = GetMergedResultArray<T> (dataContainers);
          var queryResult = new QueryResult<T> (query, resultArray);
          return _clientTransaction.TransactionEventSink.FilterQueryResult (queryResult);
        }
      }
    }

    private T[] GetMergedResultArray<T> (DataContainerCollection dataContainers)
    {
      DomainObjectCollection resultCollection;
      try
      {
        resultCollection = _clientTransaction.MergeLoadedDomainObjects (dataContainers, typeof (DomainObjectCollection), typeof (T), null);
      }
      catch (ArgumentItemTypeException ex)
      {
        string message = string.Format (
            "The query returned an object of type {0}, but a query result of type '{1}' was expected.",
            ex.ActualType.FullName,
            typeof (T).FullName);
        throw new UnexpectedQueryResultException (message, ex);
      }

      var resultArray = new T[resultCollection.Count];
      resultCollection.CopyTo (resultArray, 0);

      return resultArray;
    }
  }
}