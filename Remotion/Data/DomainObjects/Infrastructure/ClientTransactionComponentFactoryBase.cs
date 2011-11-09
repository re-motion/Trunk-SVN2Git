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
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Holds common code required to implement the <see cref="IClientTransactionComponentFactory"/> interface.
  /// </summary>
  [Serializable]
  public abstract class ClientTransactionComponentFactoryBase : IClientTransactionComponentFactory
  {
    public abstract ClientTransaction GetParentTransaction (ClientTransaction constructedTransaction);
    public abstract Dictionary<Enum, object> CreateApplicationData (ClientTransaction constructedTransaction);
    public abstract IEnlistedDomainObjectManager CreateEnlistedObjectManager (ClientTransaction constructedTransaction);
    public abstract IInvalidDomainObjectManager CreateInvalidDomainObjectManager (ClientTransaction constructedTransaction);
    public abstract IPersistenceStrategy CreatePersistenceStrategy (ClientTransaction constructedTransaction);
    public abstract Func<ClientTransaction, ClientTransaction> CreateCloneFactory ();

    protected abstract IRelationEndPointManager CreateRelationEndPointManager (
        ClientTransaction constructedTransaction,
        IRelationEndPointProvider endPointProvider,
        ILazyLoader lazyLoader);

    public virtual IEnumerable<IClientTransactionListener> CreateListeners (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      yield return new LoggingClientTransactionListener ();
    }

    public virtual IDataManager CreateDataManager (
        ClientTransaction constructedTransaction,
        IClientTransactionListener eventSink,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IPersistenceStrategy persistenceStrategy)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull ("invalidDomainObjectManager", invalidDomainObjectManager);

      var delegatingDataManager = new DelegatingDataManager ();
      var objectLoader = CreateObjectLoader (constructedTransaction, eventSink, persistenceStrategy, invalidDomainObjectManager, delegatingDataManager);

      Func<DataManager, IRelationEndPointManager> endPointManagerFactory = dm => CreateRelationEndPointManager (
          constructedTransaction,
          GetEndPointProvider (dm),
          GetLazyLoader (dm));

      var dataManager = new DataManager (constructedTransaction, invalidDomainObjectManager, objectLoader, endPointManagerFactory);
      delegatingDataManager.InnerDataManager = dataManager;
      return dataManager;
    }

    public IQueryManager CreateQueryManager (
        ClientTransaction constructedTransaction,
        IClientTransactionListener eventSink,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IPersistenceStrategy persistenceStrategy,
        IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull ("eventSink", eventSink);
      ArgumentUtility.CheckNotNull ("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      var objectLoader = CreateObjectLoader (constructedTransaction, eventSink, persistenceStrategy, invalidDomainObjectManager, dataManager);
      return new QueryManager (persistenceStrategy, objectLoader, constructedTransaction, eventSink);
    }

    public virtual ClientTransactionExtensionCollection CreateExtensionCollection (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);

      var extensionFactories = SafeServiceLocator.Current.GetAllInstances<IClientTransactionExtensionFactory> ();
      var extensions = extensionFactories.SelectMany (f => f.CreateClientTransactionExtensions (constructedTransaction));

      var extensionCollection = new ClientTransactionExtensionCollection ("root");
      foreach (var factory in extensions)
        extensionCollection.Add (factory);

      return extensionCollection;
    }

    protected virtual ILazyLoader GetLazyLoader (DataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);
      return dataManager;
    }

    protected virtual IRelationEndPointProvider GetEndPointProvider (IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);
      return dataManager;
    }

    protected virtual IObjectLoader CreateObjectLoader (
        ClientTransaction constructedTransaction,
        IClientTransactionListener eventSink,
        IPersistenceStrategy persistenceStrategy,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull ("eventSink", eventSink);
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);
      
      var loadedObjectDataProvider = new LoadedObjectDataProvider (dataManager, invalidDomainObjectManager);

      IFetchedRelationDataRegistrationAgent registrationAgent =
          new DelegatingFetchedRelationDataRegistrationAgent (
              new FetchedRealObjectRelationDataRegistrationAgent(),
              new FetchedVirtualObjectRelationDataRegistrationAgent (dataManager, dataManager),
              new FetchedCollectionRelationDataRegistrationAgent (dataManager, dataManager));
      var eagerFetcher = new EagerFetcher (registrationAgent);
      return new ObjectLoader (
          persistenceStrategy,
          eagerFetcher,
          new LoadedObjectDataRegistrationAgent (constructedTransaction, eventSink),
          dataManager,
          loadedObjectDataProvider);
    }
  }
}