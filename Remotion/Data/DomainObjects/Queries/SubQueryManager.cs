// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// <see cref="RootQueryManager"/> provides methods to execute queries within a <see cref="SubClientTransaction"/>.
  /// </summary>
  /// <remarks>
  /// This query manager executes all queries indirectly, via the <see cref="SubClientTransaction.ParentTransaction"/> of this manager's transaction.
  /// </remarks>
  [Serializable]
  public class SubQueryManager : IQueryManager
  {
    private readonly SubClientTransaction _clientTransaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubQueryManager"/> class.
    /// </summary>
    /// <param name="clientTransaction">The client transaction to manage queries for.</param>
    public SubQueryManager (SubClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      _clientTransaction = clientTransaction;
    }

    /// <summary>
    /// Gets the <see cref="Remotion.Data.DomainObjects.ClientTransaction"/> that is associated with the <see cref="SubQueryManager"/>.
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
    /// <remarks>
    /// This query manager executes all queries indirectly, via the <see cref="SubClientTransaction.ParentTransaction"/> of this manager's transaction.
    /// </remarks>
    public object GetScalar (IQuery query)
    {
      return ClientTransaction.ParentTransaction.QueryManager.GetScalar (query);
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
    /// <remarks>
    /// This query manager executes all queries indirectly, via the <see cref="SubClientTransaction.ParentTransaction"/> of this manager's transaction.
    /// </remarks>
    public QueryResult<DomainObject> GetCollection (IQuery query)
    {
      using (TransactionUnlocker.MakeWriteable (ClientTransaction.ParentTransaction))
      {
        return ClientTransaction.ParentTransaction.QueryManager.GetCollection (query);
      }
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
    /// <remarks>
    /// This query manager executes all queries indirectly, via the <see cref="SubClientTransaction.ParentTransaction"/> of this manager's transaction.
    /// </remarks>
    public QueryResult<T> GetCollection<T> (IQuery query) where T: DomainObject
    {
      using (TransactionUnlocker.MakeWriteable (ClientTransaction.ParentTransaction))
      {
        return ClientTransaction.ParentTransaction.QueryManager.GetCollection<T> (query);
      }
    }
  }
}
