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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Defines an interface for factories building <see cref="ClientTransaction"/> instances.
  /// </summary>
  public interface IClientTransactionComponentFactory
  {
    ClientTransaction GetParentTransaction ();

    Dictionary<Enum, object> CreateApplicationData ();
    ClientTransactionExtensionCollection CreateExtensionCollection (ClientTransaction clientTransaction);
    IEnumerable<IClientTransactionListener> CreateListeners (ClientTransaction clientTransaction);
    IPersistenceStrategy CreatePersistenceStrategy (Guid id);
    IObjectLoader CreateObjectLoader (
        ClientTransaction clientTransaction, 
        IPersistenceStrategy persistenceStrategy, 
        IClientTransactionListener eventSink);
    
    IEnlistedDomainObjectManager CreateEnlistedObjectManager ();
    IInvalidDomainObjectManager CreateInvalidDomainObjectManager ();

    IDataManager CreateDataManager (ClientTransaction clientTransaction, IInvalidDomainObjectManager invalidDomainObjectManager, IObjectLoader objectLoader);

    IQueryManager CreateQueryManager (
        ClientTransaction clientTransaction,
        IPersistenceStrategy persistenceStrategy,
        IObjectLoader objectLoader,
        IDataManager dataManager);
    
    // This member is likely to be removed in the future
    // TODO 2968: Remove this member
    Func<ClientTransaction, ClientTransaction> CreateCloneFactory ();
  }
}