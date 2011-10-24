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
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Holds common code used by both <see cref="RootClientTransactionComponentFactory"/> and <see cref="SubClientTransactionComponentFactory"/>.
  /// </summary>
  public static class ClientTransactionComponentFactoryUtility
  {
    public static Dictionary<Enum, object> CreateApplicationData ()
    {
      return new Dictionary<Enum, object> ();
    }

    public static ClientTransactionExtensionCollection CreateExtensionCollectionFromServiceLocator (
        ClientTransaction clientTransaction, 
        params IClientTransactionExtension[] fixedExtensions)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      var extensionFactories = SafeServiceLocator.Current.GetAllInstances<IClientTransactionExtensionFactory> ();
      var extensions = fixedExtensions.Concat (extensionFactories.SelectMany (f => f.CreateClientTransactionExtensions (clientTransaction)));

      var extensionCollection = new ClientTransactionExtensionCollection ("root");
      foreach (var factory in extensions)
        extensionCollection.Add (factory);
      
      return extensionCollection;
    }

    public static IObjectLoader CreateObjectLoader (
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
      return new ObjectLoader (
          persistenceStrategy,
          eagerFetcher,
          new LoadedObjectDataRegistrationAgent (clientTransaction, eventSink));
    }

   public static IQueryManager CreateQueryManager (
        ClientTransaction clientTransaction,
        IPersistenceStrategy persistenceStrategy,
        IObjectLoader objectLoader,
        IDataManager dataManager,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IClientTransactionListener eventSink)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("persistenceStrategy", persistenceStrategy);
      ArgumentUtility.CheckNotNull ("objectLoader", objectLoader);
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);
     ArgumentUtility.CheckNotNull ("eventSink", eventSink);
     ArgumentUtility.CheckNotNull ("invalidDomainObjectManager", invalidDomainObjectManager);

     return new QueryManager (
         persistenceStrategy,
         objectLoader,
         clientTransaction,
         eventSink,
         dataManager,
         new LoadedObjectDataProvider (dataManager, invalidDomainObjectManager));
    }
  }
}