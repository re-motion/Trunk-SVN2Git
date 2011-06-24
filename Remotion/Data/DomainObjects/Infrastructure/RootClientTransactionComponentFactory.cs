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
using System.Reflection;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Creates all parts necessary to construct a <see cref="ClientTransaction"/> with root-transaction semantics.
  /// </summary>
  [Serializable]
  public class RootClientTransactionComponentFactory : IClientTransactionComponentFactory
  {
    public static RootClientTransactionComponentFactory Create()
    {
      return ObjectFactory.Create<RootClientTransactionComponentFactory> (true, ParamList.Empty);
    }

    protected RootClientTransactionComponentFactory ()
    {
    }

    public ClientTransaction GetParentTransaction ()
    {
      return null;
    }

    public virtual Dictionary<Enum, object> CreateApplicationData ()
    {
      return new Dictionary<Enum, object> ();
    }

    public virtual ClientTransactionExtensionCollection CreateExtensions ()
    {
      return new ClientTransactionExtensionCollection ();
    }

    public virtual IEnumerable<IClientTransactionListener> CreateListeners (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      var factories = SafeServiceLocator.Current.GetAllInstances<IClientTransactionListenerFactory> ();
      return factories.Select (factory => factory.CreateClientTransactionListener (clientTransaction));
    }

    public virtual IPersistenceStrategy CreatePersistenceStrategy (Guid id)
    {
      return ObjectFactory.Create<RootPersistenceStrategy> (true, ParamList.Create (id));
    }

    public virtual IObjectLoader CreateObjectLoader (
        ClientTransaction clientTransaction, 
        IPersistenceStrategy persistenceStrategy, 
        IClientTransactionListener eventSink)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("eventSink", eventSink);

      IFetchedRelationDataRegistrationAgent registrationAgent =
          new DelegatingFetchedRelationDataRegistrationAgent (
              new FetchedRealObjectRelationDataRegistrationAgent(),
              new FetchedVirtualObjectRelationDataRegistrationAgent(),
              new FetchedCollectionRelationDataRegistrationAgent());
      var eagerFetcher = new EagerFetcher (registrationAgent);
      return new ObjectLoader (clientTransaction, persistenceStrategy, eventSink, eagerFetcher);
    }

    public virtual IEnlistedDomainObjectManager CreateEnlistedObjectManager ()
    {
      return new DictionaryBasedEnlistedDomainObjectManager ();
    }

    public IInvalidDomainObjectManager CreateInvalidDomainObjectManager ()
    {
      return new RootInvalidDomainObjectManager ();
    }

    public virtual IDataManager CreateDataManager (
        ClientTransaction clientTransaction, 
        IInvalidDomainObjectManager invalidDomainObjectManager, 
        IObjectLoader objectLoader)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull ("objectLoader", objectLoader);
      
      Func<DataManager, IRelationEndPointManager> endPointManagerFactory = dataManager =>
      {
        var collectionEndPointDataKeeperFactory = new CollectionEndPointDataKeeperFactory (
            clientTransaction, 
            new RootCollectionEndPointChangeDetectionStrategy());
        var virtualObjectEndPointDataKeeperFactory = new VirtualObjectEndPointDataKeeperFactory (clientTransaction);

        var relationEndPointFactory = new RelationEndPointFactory (
            clientTransaction,
            dataManager,
            dataManager,
            virtualObjectEndPointDataKeeperFactory,
            collectionEndPointDataKeeperFactory);
        var relationEndPointRegistrationAgent = new RootRelationEndPointRegistrationAgent (dataManager);
        return new RelationEndPointManager (clientTransaction, dataManager, relationEndPointFactory, relationEndPointRegistrationAgent);
      };

      return new DataManager (clientTransaction, invalidDomainObjectManager, objectLoader, endPointManagerFactory);
    }

    public IQueryManager CreateQueryManager (
        ClientTransaction clientTransaction,
        IPersistenceStrategy persistenceStrategy,
        IObjectLoader objectLoader,
        IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("objectLoader", objectLoader);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      return new QueryManager (persistenceStrategy, objectLoader, clientTransaction, clientTransaction.TransactionEventSink, dataManager);
    }

    public virtual Func<ClientTransaction, ClientTransaction> CreateCloneFactory ()
    {
      return templateTransaction => (ClientTransaction) TypesafeActivator
        .CreateInstance (templateTransaction.GetType (), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        .With (this);
    }
  }
}