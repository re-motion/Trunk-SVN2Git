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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Defines an interface for factories building <see cref="ClientTransaction"/> instances.
  /// </summary>
  /// <remarks>
  /// The methods defined by this interface are executed while a <see cref="ClientTransaction"/> is constructed; they provide the components
  /// making up the <see cref="ClientTransaction"/>. When accessing the constructed <see cref="ClientTransaction"/> passed to the methods as an 
  /// argument, keep in mind that the transaction is not yet complete and may not be used. Once a method has returned a component, it is safe for
  /// subsequent methods to access that component of the constructed <see cref="ClientTransaction"/>. It is safe for all methods to access the 
  /// <see cref="ClientTransaction.ID"/> property.
  /// </remarks>
  public interface IClientTransactionComponentFactory
  {
    ClientTransaction GetParentTransaction (ClientTransaction constructedTransaction);
    Dictionary<Enum, object> CreateApplicationData (ClientTransaction constructedTransaction);
    IClientTransactionEventBroker CreateEventBroker (ClientTransaction constructedTransaction);
    IEnlistedDomainObjectManager CreateEnlistedObjectManager (ClientTransaction constructedTransaction);
    IInvalidDomainObjectManager CreateInvalidDomainObjectManager (ClientTransaction constructedTransaction, IClientTransactionEventSink eventSink);
    IPersistenceStrategy CreatePersistenceStrategy (ClientTransaction constructedTransaction);

    IDataManager CreateDataManager (
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IPersistenceStrategy persistenceStrategy);

    IQueryManager CreateQueryManager (
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink,
        IInvalidDomainObjectManager invalidDomainObjectManager,
        IPersistenceStrategy persistenceStrategy,
        IDataManager dataManager);

    ICommitRollbackAgent CreateCommitRollbackAgent (
        ClientTransaction constructedTransaction,
        IClientTransactionEventSink eventSink,
        IPersistenceStrategy persistenceStrategy,
        IDataManager dataManager);

    ClientTransactionExtensionCollection CreateExtensionCollection (ClientTransaction constructedTransaction);

    // This member is likely to be removed in the future
    // TODO 2968: Remove this member
    Func<ClientTransaction, ClientTransaction> CreateCloneFactory ();
  }
}