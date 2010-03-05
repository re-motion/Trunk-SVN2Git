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
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public static class ClientTransactionTestHelper
  {
    public static DataManager GetDataManager (ClientTransaction clientTransaction)
    {
      return (DataManager) PrivateInvoke.GetNonPublicProperty (clientTransaction, "DataManager");
    }

    public static DomainObject[] CallLoadObjects (ClientTransaction clientTransaction, IList<ObjectID> objectIds, bool throwOnNotFound)
    {
      return (DomainObject[]) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "LoadObjects", objectIds, throwOnNotFound);
    }

    public static DomainObject[] CallLoadRelatedObjects (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      return (DomainObject[]) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "LoadRelatedObjects", endPointID);
    }

    public static DataContainerCollection CallLoadRelatedDataContainers (ClientTransaction clientTransaction, RelationEndPointID endPointID)
    {
      return (DataContainerCollection) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "LoadRelatedDataContainers", endPointID);
    }

    public static DomainObject CallGetObject (ClientTransaction clientTransaction, ObjectID objectID, bool includeDeleted)
    {
      return (DomainObject) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "GetObject", objectID, includeDeleted);
    }

    public static DomainObject CallGetObjectReference (ClientTransaction clientTransaction, ObjectID objectID)
    {
      return (DomainObject) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "GetObjectReference", objectID);
    }

    public static void AddListener (ClientTransaction clientTransaction, IClientTransactionListener listener)
    {
      PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "AddListener", listener);
    }

    public static void EnsureTransactionThrowsOnEvents (ClientTransaction clientTransaction)
    {
      var listenerMock = MockRepository.GenerateStrictMock<IClientTransactionListener>();
      listenerMock.Stub (stub => stub.TransactionDiscarding());
      AddListener (clientTransaction, listenerMock);
      listenerMock.Replay (); // no events expected
    }

    public static void EnsureTransactionThrowsOnEvent (ClientTransaction clientTransaction, Action<IClientTransactionListener> forbiddenEventExpectation)
    {
      IClientTransactionListener listenerMock = CreateAndAddListenerMock(clientTransaction);
      listenerMock.Expect (forbiddenEventExpectation).WhenCalled (mi => { throw new InvalidOperationException ("Forbidden event raised."); });
      listenerMock.Replay ();
    }

    public static IClientTransactionListener CreateAndAddListenerMock (ClientTransaction clientTransaction)
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener>();
      AddListener (clientTransaction, listenerMock);
      return listenerMock;
    }
  }
}