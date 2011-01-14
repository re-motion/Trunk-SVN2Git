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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  internal class TestComponentFactoryWithSpecificPersistenceStrategy : IClientTransactionComponentFactory
  {
    private readonly IClientTransactionComponentFactory _actualFactory = new RootClientTransactionComponentFactory();
    private readonly IPersistenceStrategy _persistenceStrategy;

    public TestComponentFactoryWithSpecificPersistenceStrategy (IPersistenceStrategy persistenceStrategy)
    {
      _persistenceStrategy = persistenceStrategy;
    }

    public Dictionary<Enum, object> CreateApplicationData ()
    {
      return _actualFactory.CreateApplicationData();
    }

    public IPersistenceStrategy CreatePersistenceStrategy (Guid id)
    {
      return _persistenceStrategy;
    }

    public IEnlistedDomainObjectManager CreateEnlistedObjectManager ()
    {
      return _actualFactory.CreateEnlistedObjectManager();
    }

    public IInvalidDomainObjectManager CreateInvalidDomainObjectManager ()
    {
      return _actualFactory.CreateInvalidDomainObjectManager ();
    }

    public IDataManager CreateDataManager (ClientTransaction clientTransaction, IInvalidDomainObjectManager invalidDomainObjectManager)
    {
      return _actualFactory.CreateDataManager (clientTransaction, invalidDomainObjectManager);
    }

    public Func<ClientTransaction, ClientTransaction> CreateCloneFactory ()
    {
      throw new NotImplementedException();
    }

    public ClientTransactionExtensionCollection CreateExtensions ()
    {
      return _actualFactory.CreateExtensions();
    }

    public IEnumerable<IClientTransactionListener> CreateListeners (ClientTransaction clientTransaction)
    {
      return _actualFactory.CreateListeners (clientTransaction);
    }

    public IObjectLoader CreateObjectLoader (
        ClientTransaction clientTransaction, IDataManager dataManager, IPersistenceStrategy persistenceStrategy, IClientTransactionListener eventSink)
    {
      return _actualFactory.CreateObjectLoader (clientTransaction, dataManager, persistenceStrategy, eventSink);
    }
  }
}