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
using System.Reflection;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Creates all parts necessary to construct a <see cref="ClientTransaction"/> with root-transaction semantics.
  /// </summary>
  [Serializable]
  public class RootClientTransactionComponentFactory : ClientTransactionComponentFactoryBase
  {
    public static RootClientTransactionComponentFactory Create()
    {
      return ObjectFactory.Create<RootClientTransactionComponentFactory> (true, ParamList.Empty);
    }

    protected RootClientTransactionComponentFactory ()
    {
    }

    public override ClientTransaction GetParentTransaction (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return null;
    }

    public override Dictionary<Enum, object> CreateApplicationData (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return new Dictionary<Enum, object> ();
    }

    public override IEnlistedDomainObjectManager CreateEnlistedObjectManager (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return new DictionaryBasedEnlistedDomainObjectManager ();
    }

    // TODO 3658: Inject event sink
    public override IInvalidDomainObjectManager CreateInvalidDomainObjectManager (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return new InvalidDomainObjectManager (constructedTransaction, constructedTransaction.ListenerManager);
    }

    public override IPersistenceStrategy CreatePersistenceStrategy (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return ObjectFactory.Create<RootPersistenceStrategy> (true, ParamList.Create (constructedTransaction.ID));
    }

    public override ClientTransactionExtensionCollection CreateExtensionCollection (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);

      var extensions = base.CreateExtensionCollection (constructedTransaction);
      extensions.Insert (0, new CommitValidationClientTransactionExtension (tx => new MandatoryRelationValidator()));
      return extensions;
    }

    public override Func<ClientTransaction, ClientTransaction> CreateCloneFactory ()
    {
      return templateTransaction => (ClientTransaction) TypesafeActivator
          .CreateInstance (templateTransaction.GetType (), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
          .With (this);
    }

    protected override IRelationEndPointManager CreateRelationEndPointManager (
        ClientTransaction constructedTransaction,
        IRelationEndPointProvider endPointProvider,
        ILazyLoader lazyLoader,
        IClientTransactionEventSink eventSink)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull ("eventSink", eventSink);

      var endPointChangeDetectionStrategy = new RootCollectionEndPointChangeDetectionStrategy();
      var collectionEndPointDataManagerFactory = new CollectionEndPointDataManagerFactory (endPointChangeDetectionStrategy);
      var virtualObjectEndPointDataManagerFactory = new VirtualObjectEndPointDataManagerFactory();

      var relationEndPointFactory = CreateRelationEndPointFactory (
          constructedTransaction,
          endPointProvider,
          lazyLoader,
          eventSink,
          virtualObjectEndPointDataManagerFactory,
          collectionEndPointDataManagerFactory);
      var virtualEndPointStateUpdateListener = new VirtualEndPointStateUpdateListener (constructedTransaction);
      var stateUpdateRaisingRelationEndPointFactory = new StateUpdateRaisingRelationEndPointFactoryDecorator (
          relationEndPointFactory, 
          virtualEndPointStateUpdateListener);

      var relationEndPointRegistrationAgent = new RootRelationEndPointRegistrationAgent (endPointProvider);
      return new RelationEndPointManager (
          constructedTransaction, lazyLoader, eventSink, stateUpdateRaisingRelationEndPointFactory, relationEndPointRegistrationAgent);
    }

    protected virtual RelationEndPointFactory CreateRelationEndPointFactory (
        ClientTransaction constructedTransaction,
        IRelationEndPointProvider endPointProvider,
        ILazyLoader lazyLoader,
        IClientTransactionEventSink eventSink,
        IVirtualObjectEndPointDataManagerFactory virtualObjectEndPointDataManagerFactory,
        ICollectionEndPointDataManagerFactory collectionEndPointDataManagerFactory)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);
      ArgumentUtility.CheckNotNull ("eventSink", eventSink);
      ArgumentUtility.CheckNotNull ("virtualObjectEndPointDataManagerFactory", virtualObjectEndPointDataManagerFactory);
      ArgumentUtility.CheckNotNull ("collectionEndPointDataManagerFactory", collectionEndPointDataManagerFactory);

      var associatedCollectionDataStrategyFactory = new AssociatedCollectionDataStrategyFactory (endPointProvider);
      var collectionEndPointCollectionProvider = new CollectionEndPointCollectionProvider (associatedCollectionDataStrategyFactory);
      return new RelationEndPointFactory (
          constructedTransaction,
          endPointProvider,
          lazyLoader,
          eventSink,
          virtualObjectEndPointDataManagerFactory,
          collectionEndPointDataManagerFactory, 
          collectionEndPointCollectionProvider,
          associatedCollectionDataStrategyFactory);
    }

    protected override IObjectLoader CreateObjectLoader (
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink,
        IPersistenceStrategy persistenceStrategy,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull ("eventSink", eventSink);
      var fetchEnabledPersistenceStrategy = 
          ArgumentUtility.CheckNotNullAndType<IFetchEnabledPersistenceStrategy> ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("invalidDomainObjectManager", invalidDomainObjectManager);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      var objectLoader = CreateBasicObjectLoader (
          constructedTransaction,
          eventSink,
          fetchEnabledPersistenceStrategy,
          invalidDomainObjectManager,
          dataManager);

      IFetchedRelationDataRegistrationAgent registrationAgent =
          new DelegatingFetchedRelationDataRegistrationAgent (
              new FetchedRealObjectRelationDataRegistrationAgent(),
              new FetchedVirtualObjectRelationDataRegistrationAgent (dataManager),
              new FetchedCollectionRelationDataRegistrationAgent (dataManager));
      var eagerFetcher = new EagerFetcher (registrationAgent);
      return new EagerFetchingObjectLoaderDecorator (objectLoader, eagerFetcher);
    }

    protected virtual IFetchEnabledObjectLoader CreateBasicObjectLoader (
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink,
        IFetchEnabledPersistenceStrategy persistenceStrategy,
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
      return new FetchEnabledObjectLoader (
          persistenceStrategy,
          loadedObjectDataRegistrationAgent,
          dataManager,
          loadedObjectDataProvider);
    }
  }
}