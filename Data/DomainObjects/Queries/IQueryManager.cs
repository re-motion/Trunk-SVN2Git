/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Data.DomainObjects;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// <see cref="IQueryManager"/> provides an interface for methods to execute queries within a <see cref="ClientTransaction"/>.
  /// </summary>
  public interface IQueryManager
  {
    /// <summary>
    /// Gets the <see cref="Remotion.Data.DomainObjects.ClientTransaction"/> that is associated with the <see cref="RootQueryManager"/>.
    /// </summary>
    ClientTransaction ClientTransaction { get; }

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
    object GetScalar (IQuery query);

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
    DomainObjectCollection GetCollection (IQuery query);

    /// <summary>
    /// Executes a given <see cref="IQuery"/> and returns a collection of the <see cref="DomainObject"/>s returned by the query.
    /// </summary>
    /// <param name="query">The query to execute. Must not be <see langword="null"/>.</param>
    /// <typeparam name="T">The type of <see cref="DomainObjects"/> to be returned from the query.</typeparam>
    /// <returns>A collection containing the <see cref="DomainObject"/>s returned by the query.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidTypeException">The objects returned by the <paramref name="query"/> do not match the expected type
    ///   <typeparamref name="T"/> or the configured collection type is not assignable to <see cref="ObjectList{T}"/> with the given <typeparamref name="T"/>.</exception>
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
    ObjectList<T> GetCollection<T> (IQuery query) where T : DomainObject;
  }
}
