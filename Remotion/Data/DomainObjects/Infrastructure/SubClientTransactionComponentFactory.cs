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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Creates all parts necessary to construct a <see cref="ClientTransaction"/> with sub-transaction semantics.
  /// </summary>
  [Serializable]
  public class SubClientTransactionComponentFactory : ClientTransactionComponentFactoryBase
  {
    public static SubClientTransactionComponentFactory Create (
        ClientTransaction parentTransaction, IInvalidDomainObjectManager parentInvalidDomainObjectManager)
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

    public override ClientTransaction GetParentTransaction (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return _parentTransaction;
    }

    public override Dictionary<Enum, object> CreateApplicationData (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return _parentTransaction.ApplicationData;
    }

    public override IEnumerable<IClientTransactionListener> CreateListeners (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return base.CreateListeners (constructedTransaction).Concat (new[] { new SubClientTransactionListener (_parentInvalidDomainObjectManager) });
    }

    public override IEnlistedDomainObjectManager CreateEnlistedObjectManager (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return new DelegatingEnlistedDomainObjectManager (_parentTransaction);
    }

    public override IInvalidDomainObjectManager CreateInvalidDomainObjectManager (ClientTransaction constructedTransaction)
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

    public override IPersistenceStrategy CreatePersistenceStrategy (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);

      var parentTransactionContext = new ParentTransactionContext (_parentTransaction, _parentInvalidDomainObjectManager);
      return ObjectFactory.Create<SubPersistenceStrategy> (true, ParamList.Create (parentTransactionContext));
    }

    public override Func<ClientTransaction, ClientTransaction> CreateCloneFactory ()
    {
      return templateTransaction => _parentTransaction.CreateSubTransaction();
    }

    protected override IRelationEndPointManager CreateRelationEndPointManager (
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

    protected override IObjectLoader CreateObjectLoader (
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

      return CreateBasicObjectLoader (constructedTransaction, eventSink, persistenceStrategy, invalidDomainObjectManager, dataManager);
    }

    protected virtual IObjectLoader CreateBasicObjectLoader (
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
      var loadedObjectDataRegistrationAgent = new LoadedObjectDataRegistrationAgent (constructedTransaction, eventSink);
      return new ObjectLoader (
          persistenceStrategy,
          loadedObjectDataRegistrationAgent,
          dataManager,
          loadedObjectDataProvider);
    }
  }
}