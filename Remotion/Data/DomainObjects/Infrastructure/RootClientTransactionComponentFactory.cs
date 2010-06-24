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
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Creates all parts necessary to construct a <see cref="ClientTransaction"/> with root-transaction semantics.
  /// </summary>
  [Serializable]
  public class RootClientTransactionComponentFactory : IClientTransactionComponentFactory
  {
    public Dictionary<Enum, object> CreateApplicationData ()
    {
      return new Dictionary<Enum, object> ();
    }

    public ClientTransactionExtensionCollection CreateExtensions ()
    {
      return new ClientTransactionExtensionCollection ();
    }

    public IEnumerable<IClientTransactionListener> CreateListeners (ClientTransaction clientTransaction)
    {
      var factories = SafeServiceLocator.Current.GetAllInstances<IClientTransactionListenerFactory> ();
      return factories.Select (factory => factory.CreateClientTransactionListener (clientTransaction));
    }

    public IDataManager CreateDataManager (ClientTransaction clientTransaction)
    {
      return new DataManager (clientTransaction, new RootCollectionEndPointChangeDetectionStrategy ());
    }

    public IDataSource CreateDataSourceStrategy (Guid id, IDataManager dataManager)
    {
      return ObjectFactory.Create<RootClientTransaction> (true, ParamList.Create (id, dataManager));
    }

    public IObjectLoader CreateObjectLoader (ClientTransaction clientTransaction, IDataManager dataManager, IDataSource dataSource, IClientTransactionListener eventSink)
    {
      var eagerFetcher = new EagerFetcher (dataManager);
      return new ObjectLoader (clientTransaction, dataSource, eventSink, eagerFetcher);
    }

    public IEnlistedDomainObjectManager CreateEnlistedObjectManager ()
    {
      return new DictionaryBasedEnlistedDomainObjectManager ();
    }

    public Func<ClientTransaction, ClientTransaction> CreateCloneFactory ()
    {
      return templateTransaction => (ClientTransaction) TypesafeActivator
        .CreateInstance (templateTransaction.GetType (), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        .With (this);
    }
  }
}