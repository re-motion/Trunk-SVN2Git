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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides a common interface for classes that can load <see cref="DataContainer"/> instances from a data source and persist them.
  /// TODO 2246: Very similar to PersistenceManager.
  /// </summary>
  public interface IDataSource
  {
    /// <summary>
    /// Loads a data container from the underlying data source.
    /// </summary>
    /// <param name="id">The id of the <see cref="DataContainer"/> to load.</param>
    /// <returns>A <see cref="DataContainer"/> with the given <paramref name="id"/>.</returns>
    /// <remarks>
    /// <para>
    /// This method should not set the <see cref="ClientTransaction"/> of the loaded data container, register the container in a 
    /// <see cref="DataContainerMap"/>, or set the  <see cref="DomainObject"/> of the container.
    /// All of these activities are performed by the caller. 
    /// </para>
    /// <para>
    /// The caller should also raise the <see cref="IClientTransactionListener.ObjectsLoading"/> and 
    /// <see cref="IClientTransactionListener.ObjectsLoaded"/> events.
    /// </para>
    /// </remarks>
    DataContainer LoadDataContainer (ObjectID id);

    /// <summary>
    /// Loads a number of data containers from the underlying data source.
    /// </summary>
    /// <param name="objectIDs">The ids of the <see cref="DataContainer"/> objects to load.</param>
    /// <returns>A <see cref="DataContainerCollection"/> with the loaded containers in the same order as in <paramref name="objectIDs"/>.</returns>
    /// <param name="throwOnNotFound">If <see langword="true" />, this method should throw a <see cref="BulkLoadException"/> if a data container 
    /// cannot be found for an <see cref="ObjectID"/>. If <see langword="false" />, the method should proceed as if the invalid ID hadn't been given.
    /// </param>
    /// <remarks>
    /// <para>
    /// This method should not set the <see cref="ClientTransaction"/> of the loaded data container, register the container in a 
    /// <see cref="DataContainerMap"/>, or set the  <see cref="DomainObject"/> of the container.
    /// All of these activities are performed by the caller. 
    /// </para>
    /// <para>
    /// The caller should also raise the <see cref="IClientTransactionListener.ObjectsLoading"/> and 
    /// <see cref="IClientTransactionListener.ObjectsLoaded"/> events.
    /// </para>
    /// </remarks>
    DataContainerCollection LoadDataContainers (ICollection<ObjectID> objectIDs, bool throwOnNotFound);

    /// <summary>
    /// Loads the related <see cref="DataContainer"/> for a given <see cref="DataManagement.RelationEndPointID"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method should not set the <see cref="ClientTransaction"/> of the loaded data container, register the container in a 
    /// <see cref="DataContainerMap"/>, or set the  <see cref="DomainObject"/> of the container.
    /// All of these activities are performed by the caller. 
    /// </para>
    /// <para>
    /// The caller should also raise the <see cref="IClientTransactionListener.ObjectsLoading"/> and 
    /// <see cref="IClientTransactionListener.ObjectsLoaded"/> events.
    /// </para>
    /// </remarks>
    /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> of the end point that should be evaluated.
    /// <paramref name="relationEndPointID"/> must refer to an <see cref="ObjectEndPoint"/>. Must not be <see langword="null"/>.</param>
    /// <returns>The related <see cref="DataContainer"/>.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.InvalidCastException"><paramref name="relationEndPointID"/> does not refer to an 
    /// <see cref="DataManagement.ObjectEndPoint"/></exception>
    /// <exception cref="Persistence.PersistenceException">
    ///   The related object could not be loaded, but is mandatory.<br /> -or- <br />
    ///   The relation refers to non-existing object.<br /> -or- <br />
    ///   <paramref name="relationEndPointID"/> does not refer to an <see cref="DataManagement.ObjectEndPoint"/>.
    /// </exception>
    /// <exception cref="Persistence.StorageProviderException">
    ///   The Mapping does not contain a class definition for the given <paramref name="relationEndPointID"/>.<br /> -or- <br />
    ///   An error occurred while accessing the datasource.
    /// </exception>
    DataContainer LoadRelatedDataContainer (RelationEndPointID relationEndPointID);

    /// <summary>
    /// Loads all related <see cref="DataContainer"/>s of a given <see cref="DataManagement.RelationEndPointID"/>.
    /// </summary>
    /// <param name="relationEndPointID">The <see cref="DataManagement.RelationEndPointID"/> of the end point that should be evaluated.
    /// <paramref name="relationEndPointID"/> must refer to a <see cref="CollectionEndPoint"/>. Must not be <see langword="null"/>.</param>
    /// <returns>
    /// A <see cref="DataContainerCollection"/> containing all related <see cref="DataContainer"/>s.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method should not set the <see cref="ClientTransaction"/> of the loaded data container, register the container in a 
    /// <see cref="DataContainerMap"/>, or set the  <see cref="DomainObject"/> of the container.
    /// All of these activities are performed by the caller. 
    /// </para>
    /// <para>
    /// The caller should also raise the <see cref="IClientTransactionListener.ObjectsLoading"/> and 
    /// <see cref="IClientTransactionListener.ObjectsLoaded"/> events.
    /// </para>
    /// </remarks>
    /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
    /// <exception cref="Persistence.PersistenceException">
    /// 	<paramref name="relationEndPointID"/> does not refer to one-to-many relation.<br/> -or- <br/>
    /// The StorageProvider for the related objects could not be initialized.
    /// </exception>
    DataContainerCollection LoadRelatedDataContainers (RelationEndPointID relationEndPointID);

    /// <summary>
    /// Executes the given <see cref="IQuery"/> and returns its results as an array of <see cref="DataContainer"/> instances.
    /// </summary>
    /// <param name="query">The <see cref="IQuery"/> to be executed.</param>
    /// <returns>
    /// An array of <see cref="DataContainer"/> representing the result of the query.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method should not set the <see cref="ClientTransaction"/> of the loaded data container, register the container in a 
    /// <see cref="DataContainerMap"/>, or set the  <see cref="DomainObject"/> of the container.
    /// All of these activities are performed by the caller. 
    /// </para>
    /// <para>
    /// The caller should also raise the <see cref="IClientTransactionListener.ObjectsLoading"/> and 
    /// <see cref="IClientTransactionListener.ObjectsLoaded"/> events.
    /// </para>
    /// </remarks>
    /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="query"/> does not have a <see cref="QueryType"/> of <see cref="QueryType.Collection"/>.</exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
    /// The <see cref="IQuery.StorageProviderID"/> of <paramref name="query"/> could not be found.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.PersistenceException">
    /// The <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/> for the given <see cref="IQuery"/> could not be instantiated.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.StorageProviderException">
    /// An error occurred while executing the query.
    /// </exception>
    DataContainer[] LoadDataContainersForQuery (IQuery query);

    /// <summary>
    /// Executes the given <see cref="IQuery"/> and returns its result as a scalar value.
    /// </summary>
    /// <param name="query">The query to be executed.</param>
    /// <returns>The scalar query result.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="query"/> does not have a <see cref="QueryType"/> of <see cref="QueryType.Scalar"/>.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.StorageProviderConfigurationException">
    /// The <see cref="IQuery.StorageProviderID"/> of <paramref name="query"/> could not be found.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.PersistenceException">
    /// The <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/> for the given <see cref="IQuery"/> could not be instantiated.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.StorageProviderException">
    /// An error occurred while executing the query.
    /// </exception>
    object LoadScalarForQuery (IQuery query);

    /// <summary>
    /// Persists the changed data stored by the given <see cref="DataContainer"/> instances.
    /// </summary>
    /// <param name="changedDataContainers">The data containers whose data should be persisted.</param>
    void PersistData (DataContainerCollection changedDataContainers);

    /// <summary>
    /// Creates a new <see cref="ObjectID"/> for the given class definition. The <see cref="ObjectID"/> must be created in a such a way that it can 
    /// later be used to identify objects when persisting or loading <see cref="DataContainer"/> instances.
    /// </summary>
    /// <param name="classDefinition">The class definition to create a new <see cref="ObjectID"/> for.</param>
    /// <returns>A new <see cref="ObjectID"/> for the given class definition.</returns>
    ObjectID CreateNewObjectID (ClassDefinition classDefinition);
  }
}