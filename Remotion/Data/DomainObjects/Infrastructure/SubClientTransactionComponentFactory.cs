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
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Creates all parts necessary to construct a <see cref="ClientTransaction"/> with sub-transaction semantics.
  /// </summary>
  [Serializable]
  public class SubClientTransactionComponentFactory : IClientTransactionComponentFactory
  {
    public static SubClientTransactionComponentFactory Create (ClientTransaction parentTransaction, IInvalidDomainObjectManager parentInvalidDomainObjectManager)
    {
      return ObjectFactory.Create<SubClientTransactionComponentFactory> (true, ParamList.Create (parentTransaction, parentInvalidDomainObjectManager));
    }

    private readonly ClientTransaction _parentTransaction;
    private readonly IInvalidDomainObjectManager _parentInvalidDomainObjectManager;

    protected SubClientTransactionComponentFactory (ClientTransaction parentTransaction, IInvalidDomainObjectManager parentInvalidDomainObjectManager)
    {
      ArgumentUtility.CheckNotNull ("parentTransaction", parentTransaction);
      ArgumentUtility.CheckNotNull ("parentInvalidDomainObjectManager", parentInvalidDomainObjectManager);

      _parentTransaction = parentTransaction;
      _parentInvalidDomainObjectManager = parentInvalidDomainObjectManager;
    }

    public virtual ClientTransaction GetParentTransaction (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return _parentTransaction;
    }

    public virtual Dictionary<Enum, object> CreateApplicationData (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return _parentTransaction.ApplicationData;
    }

    public virtual IEnumerable<IClientTransactionListener> CreateListeners (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return new[] { new SubClientTransactionListener (_parentInvalidDomainObjectManager) }
          .Concat (ClientTransactionComponentFactoryUtility.GetListenersFromServiceLocator (constructedTransaction));
    }

    public virtual IPersistenceStrategy CreatePersistenceStrategy (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return ObjectFactory.Create<SubPersistenceStrategy> (true, ParamList.Create (_parentTransaction, _parentInvalidDomainObjectManager));
    }

    public virtual IEnlistedDomainObjectManager CreateEnlistedObjectManager (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return new DelegatingEnlistedDomainObjectManager (_parentTransaction);
    }

    public IInvalidDomainObjectManager CreateInvalidDomainObjectManager (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);

      var invalidObjects =
          _parentInvalidDomainObjectManager.InvalidObjectIDs.Select (id => _parentInvalidDomainObjectManager.GetInvalidObjectReference (id));

      var parentDataManager = _parentTransaction.DataManager;
      var deletedObjects = parentDataManager.DataContainers.Where (dc => dc.State == StateType.Deleted).Select (dc => dc.DomainObject);

      var invalidDomainObjectManager = new SubInvalidDomainObjectManager (_parentInvalidDomainObjectManager);
      foreach (var objectToBeMarkedInvalid in invalidObjects.Concat (deletedObjects))
        invalidDomainObjectManager.MarkInvalid (objectToBeMarkedInvalid);

      return invalidDomainObjectManager;
    }

    public virtual IObjectLoader CreateObjectLoader (
        ClientTransaction constructedTransaction,
        IPersistenceStrategy persistenceStrategy,
        IClientTransactionListener eventSink)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("eventSink", eventSink);

      return ClientTransactionComponentFactoryUtility.CreateObjectLoader (constructedTransaction, persistenceStrategy, eventSink);
    }

    public virtual IDataManager CreateDataManager (
        ClientTransaction constructedTransaction,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IObjectLoader objectLoader)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull ("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull ("objectLoader", objectLoader);

      Func<DataManager, IRelationEndPointManager> endPointManagerFactory = dataManager => CreateRelationEndPointManager (
          constructedTransaction, 
          GetEndPointProvider (dataManager), 
          GetLazyLoader (dataManager));
      
      return new DataManager (constructedTransaction, invalidDomainObjectManager, objectLoader, endPointManagerFactory);
    }

    public IQueryManager CreateQueryManager (
        ClientTransaction constructedTransaction,
        IPersistenceStrategy persistenceStrategy,
        IObjectLoader objectLoader,
        IDataManager dataManager,
        IClientTransactionListener eventSink)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("objectLoader", objectLoader);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);
      ArgumentUtility.CheckNotNull ("eventSink", eventSink);

      return ClientTransactionComponentFactoryUtility.CreateQueryManager (constructedTransaction, persistenceStrategy, objectLoader, dataManager, eventSink);
    }

    public virtual ClientTransactionExtensionCollection CreateExtensionCollection (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return ClientTransactionComponentFactoryUtility.CreateExtensionCollectionFromServiceLocator (constructedTransaction);
    }

    public virtual Func<ClientTransaction, ClientTransaction> CreateCloneFactory ()
    {
      return templateTransaction => _parentTransaction.CreateSubTransaction();
    }

    protected virtual ILazyLoader GetLazyLoader (DataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);
      return dataManager;
    }

    protected virtual IRelationEndPointProvider GetEndPointProvider (DataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);
      return dataManager;
    }

    protected virtual IRelationEndPointManager CreateRelationEndPointManager (
        ClientTransaction clientTransaction,
        IRelationEndPointProvider endPointProvider,
        ILazyLoader lazyLoader)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);

      var endPointChangeDetectionStrategy = new SubCollectionEndPointChangeDetectionStrategy ();
      var collectionEndPointDataKeeperFactory = new CollectionEndPointDataKeeperFactory (clientTransaction, endPointChangeDetectionStrategy);
      var virtualObjectEndPointDataKeeperFactory = new VirtualObjectEndPointDataKeeperFactory (clientTransaction);

      var relationEndPointFactory = new RelationEndPointFactory (
          clientTransaction,
          endPointProvider,
          lazyLoader,
          virtualObjectEndPointDataKeeperFactory,
          collectionEndPointDataKeeperFactory);
      var relationEndPointRegistrationAgent = new RelationEndPointRegistrationAgent (endPointProvider);
      return new RelationEndPointManager (clientTransaction, lazyLoader, relationEndPointFactory, relationEndPointRegistrationAgent);
    }
  }
}