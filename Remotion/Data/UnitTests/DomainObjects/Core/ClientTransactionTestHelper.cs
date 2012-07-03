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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public static class ClientTransactionTestHelper
  {
    public static DataManager GetDataManager (ClientTransaction clientTransaction)
    {
      return (DataManager) DataManagementService.GetDataManager (clientTransaction);
    }

    public static IDataManager GetIDataManager (ClientTransaction clientTransaction)
    {
      return DataManagementService.GetDataManager (clientTransaction);
    }

    public static IEnlistedDomainObjectManager GetEnlistedDomainObjectManager (ClientTransaction clientTransaction)
    {
      return (IEnlistedDomainObjectManager) PrivateInvoke.GetNonPublicField (clientTransaction, "_enlistedDomainObjectManager");
    }

    public static IInvalidDomainObjectManager GetInvalidDomainObjectManager (ClientTransaction clientTransaction)
    {
      return (IInvalidDomainObjectManager) PrivateInvoke.GetNonPublicField (clientTransaction, "_invalidDomainObjectManager");
    }

    public static IPersistenceStrategy GetPersistenceStrategy (ClientTransaction clientTransaction)
    {
      return (IPersistenceStrategy) PrivateInvoke.GetNonPublicField (clientTransaction, "_persistenceStrategy");
    }

    public static DomainObject CallGetObject (ClientTransaction clientTransaction, ObjectID objectID, bool includeDeleted)
    {
      return (DomainObject) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "GetObject", objectID, includeDeleted);
    }

    public static DomainObject CallGetObjectReference (ClientTransaction clientTransaction, ObjectID objectID)
    {
      return (DomainObject) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "GetObjectReference", objectID);
    }

    public static DomainObject CallGetInvalidObjectReference (ClientTransaction clientTransaction, ObjectID objectID)
    {
      return (DomainObject) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "GetInvalidObjectReference", objectID);
    }

    public static DomainObject CallGetRelatedObject (ClientTransaction clientTransaction, RelationEndPointID relationEndPointID)
    {
      return (DomainObject) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "GetRelatedObject", relationEndPointID);
    }

    public static DomainObject CallGetOriginalRelatedObject (ClientTransaction clientTransaction, RelationEndPointID relationEndPointID)
    {
      return (DomainObject) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "GetOriginalRelatedObject", relationEndPointID);
    }

    public static DomainObjectCollection CallGetRelatedObjects (ClientTransaction clientTransaction, RelationEndPointID relationEndPointID)
    {
      return (DomainObjectCollection) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "GetRelatedObjects", relationEndPointID);
    }

    public static DomainObjectCollection CallGetOriginalRelatedObjects (ClientTransaction clientTransaction, RelationEndPointID relationEndPointID)
    {
      return (DomainObjectCollection) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "GetOriginalRelatedObjects", relationEndPointID);
    }

    public static DomainObject CallNewObject (ClientTransaction clientTransaction, Type domainObjectType, ParamList constructorParameters)
    {
      return (DomainObject) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "NewObject", domainObjectType, constructorParameters);
    }

    public static void AddListener (ClientTransaction clientTransaction, IClientTransactionListener listener)
    {
      PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "AddListener", listener);
    }

    public static void RemoveListener (ClientTransaction clientTransaction, IClientTransactionListener listener)
    {
      PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "RemoveListener", listener);
    }

    public static void EnsureTransactionThrowsOnEvents (ClientTransaction clientTransaction)
    {
      var listenerMock = MockRepository.GenerateStrictMock<IClientTransactionListener>();
      listenerMock.Stub (stub => stub.TransactionDiscard (clientTransaction)); // allow TransactionDicarding to be called
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

    public static IClientTransactionEventBroker GetEventBroker (ClientTransaction clientTransaction)
    {
      return (IClientTransactionEventBroker) PrivateInvoke.GetNonPublicField (clientTransaction, "_eventBroker");
    }

    public static void RegisterDataContainer (ClientTransaction clientTransaction, DataContainer dataContainer)
    {
      if (!dataContainer.HasDomainObject)
      {
        var objectReference = DomainObjectMother.GetObjectReference<DomainObject> (clientTransaction, dataContainer.ID);
        dataContainer.SetDomainObject (objectReference);
      }

      var dataManager = GetDataManager (clientTransaction);
      dataManager.RegisterDataContainer (dataContainer);
    }

    public static void SetIsReadOnly (ClientTransaction transaction, bool value)
    {
      PrivateInvoke.SetPublicProperty (transaction, "IsReadOnly", value);
    }

    public static void SetActiveSubTransaction (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      PrivateInvoke.SetNonPublicField (clientTransaction, "_subTransaction", subTransaction);
    }

    public static void ClearAllListeners (ClientTransaction clientTransaction)
    {
      var listenerManager = GetEventBroker (clientTransaction);
      foreach (var listener in listenerManager.Listeners.ToArray ().Reverse ())
        listenerManager.RemoveListener (listener);
    }
  }
}