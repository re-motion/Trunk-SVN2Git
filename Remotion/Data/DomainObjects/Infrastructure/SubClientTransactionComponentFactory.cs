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
  /// Creates all parts necessary to construct a <see cref="ClientTransaction"/> with sub-transaction semantics.
  /// </summary>
  [Serializable]
  public class SubClientTransactionComponentFactory : IClientTransactionComponentFactory
  {
    private readonly ClientTransaction _parentTransaction;
    private readonly IInvalidDomainObjectManager _parentInvalidDomainObjectManager;

    public SubClientTransactionComponentFactory (ClientTransaction parentTransaction, IInvalidDomainObjectManager parentInvalidDomainObjectManager)
    {
      ArgumentUtility.CheckNotNull ("parentTransaction", parentTransaction);
      ArgumentUtility.CheckNotNull ("parentInvalidDomainObjectManager", parentInvalidDomainObjectManager);

      _parentTransaction = parentTransaction;
      _parentInvalidDomainObjectManager = parentInvalidDomainObjectManager;
    }

    public virtual ClientTransaction GetParentTransaction ()
    {
      return _parentTransaction;
    }

    public virtual Dictionary<Enum, object> CreateApplicationData ()
    {
      return _parentTransaction.ApplicationData;
    }

    public virtual ClientTransactionExtensionCollection CreateExtensions ()
    {
      return _parentTransaction.Extensions;
    }

    public virtual IEnumerable<IClientTransactionListener> CreateListeners (ClientTransaction clientTransaction)
    {
      var factories = SafeServiceLocator.Current.GetAllInstances<IClientTransactionListenerFactory>();
      return new[] { new SubClientTransactionListener (_parentInvalidDomainObjectManager) }
          .Concat (factories.Select (factory => factory.CreateClientTransactionListener (clientTransaction)));
    }

    public virtual IPersistenceStrategy CreatePersistenceStrategy (Guid id)
    {
      return ObjectFactory.Create<SubPersistenceStrategy> (true, ParamList.Create (_parentTransaction, _parentInvalidDomainObjectManager));
    }

    public virtual IObjectLoader CreateObjectLoader (
        ClientTransaction clientTransaction,
        IPersistenceStrategy persistenceStrategy,
        IClientTransactionListener eventSink)
    {
      IFetchedRelationDataRegistrationAgent registrationAgent =
          new DelegatingFetchedRelationDataRegistrationAgent (
              new FetchedRealObjectRelationDataRegistrationAgent (),
              new FetchedVirtualObjectRelationDataRegistrationAgent (),
              new FetchedCollectionRelationDataRegistrationAgent ());
      var eagerFetcher = new EagerFetcher (registrationAgent);
      return new ObjectLoader (clientTransaction, persistenceStrategy, eventSink, eagerFetcher);
    }

    public virtual IEnlistedDomainObjectManager CreateEnlistedObjectManager ()
    {
      return new DelegatingEnlistedDomainObjectManager (_parentTransaction);
    }

    public IInvalidDomainObjectManager CreateInvalidDomainObjectManager ()
    {
      var invalidObjects =
          _parentInvalidDomainObjectManager.InvalidObjectIDs.Select (id => _parentInvalidDomainObjectManager.GetInvalidObjectReference (id));

      var parentDataManager = _parentTransaction.DataManager;
      var deletedObjects = parentDataManager.GetLoadedData().Where (tuple => tuple.Item2.State == StateType.Deleted).Select (tuple => tuple.Item1);

      var invalidDomainObjectManager = new SubInvalidDomainObjectManager (_parentInvalidDomainObjectManager);
      foreach (var objectToBeMarkedInvalid in invalidObjects.Concat (deletedObjects))
        invalidDomainObjectManager.MarkInvalid (objectToBeMarkedInvalid);

      return invalidDomainObjectManager;
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
            new SubCollectionEndPointChangeDetectionStrategy ());
        var virtualObjectEndPointDataKeeperFactory = new VirtualObjectEndPointDataKeeperFactory (clientTransaction);

        var relationEndPointFactory = new RelationEndPointFactory (
            clientTransaction,
            dataManager,
            dataManager,
            virtualObjectEndPointDataKeeperFactory,
            collectionEndPointDataKeeperFactory);
        var relationEndPointRegistrationAgent = new RelationEndPointRegistrationAgent (dataManager);
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
      return templateTransaction => _parentTransaction.CreateSubTransaction();
    }
  }
}