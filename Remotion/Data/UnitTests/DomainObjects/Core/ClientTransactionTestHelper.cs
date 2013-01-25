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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.HierarchyManagement;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Remotion.Utilities;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public static class ClientTransactionTestHelper
  {
    public static ITransactionHierarchyManager GetHierarchyManager (ClientTransaction clientTransaction)
    {
      return TransactionHierarchyManagerService.GetTransactionHierarchyManager (clientTransaction);
    }

    public static DataManager GetDataManager (ClientTransaction clientTransaction)
    {
      return (DataManager) DataManagementService.GetDataManager (clientTransaction);
    }

    public static IDataManager GetIDataManager (ClientTransaction clientTransaction)
    {
      return DataManagementService.GetDataManager (clientTransaction);
    }

    public static IObjectInitializationContext GetCurrentObjectInitializationContext (ClientTransaction clientTransaction)
    {
      return (IObjectInitializationContext) PrivateInvoke.GetNonPublicProperty (clientTransaction, "CurrentObjectInitializationContext");
    }

    public static IObjectLifetimeAgent GetObjectLifetimeAgent (ClientTransaction clientTransaction)
    {
      return (IObjectLifetimeAgent) PrivateInvoke.GetNonPublicField (clientTransaction, "_objectLifetimeAgent");
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

    public static IClientTransactionEventBroker GetEventBroker (ClientTransaction clientTransaction)
    {
      return (IClientTransactionEventBroker) PrivateInvoke.GetNonPublicField (clientTransaction, "_eventBroker");
    }

    public static ICommitRollbackAgent GetCommitRollbackAgent (ClientTransaction clientTransaction)
    {
      return (ICommitRollbackAgent) PrivateInvoke.GetNonPublicField (clientTransaction, "_commitRollbackAgent");
    }
    public static T CallGetObject<T> (ClientTransaction clientTransaction, IObjectID<T> objectID, bool includeDeleted) 
        where T : DomainObject
    {
      return (T) PrivateInvokeGeneric (clientTransaction, "GetObject", new[] { typeof (T) }, objectID, includeDeleted);
    }

    public static T CallTryGetObject<T> (ClientTransaction clientTransaction, IObjectID<T> objectID)
        where T : DomainObject
    {
      return (T) PrivateInvokeGeneric (clientTransaction, "TryGetObject", new[] { typeof (T) }, objectID);
    }

    public static T CallGetObjectReference<T> (ClientTransaction clientTransaction, IObjectID<T> objectID)
        where T : DomainObject
    {
      return (T) PrivateInvokeGeneric (clientTransaction, "GetObjectReference", new[] { typeof (T) }, objectID);
    }

    public static T CallGetInvalidObjectReference<T> (ClientTransaction clientTransaction, IObjectID<T> objectID)
        where T : DomainObject
    {
      return (T) PrivateInvokeGeneric (clientTransaction, "GetInvalidObjectReference", new[] { typeof (T) }, objectID);
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

    public static T[] CallGetObjects<T> (ClientTransaction clientTransaction, params IObjectID<T>[] objectIDs)
        where T : DomainObject
    {
      return (T[]) PrivateInvokeGeneric (clientTransaction, "GetObjects", new[] { typeof (T) }, new[] { objectIDs });
    }

    public static T[] CallTryGetObjects<T> (ClientTransaction clientTransaction, params IObjectID<T>[] objectIDs)
        where T : DomainObject
    {
      return (T[]) PrivateInvokeGeneric (clientTransaction, "TryGetObjects", new[] { typeof (T) }, new[] { objectIDs });
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
      IClientTransactionListener listenerMock = CreateAndAddListenerStrictMock (clientTransaction);
      listenerMock.Stub (stub => stub.TransactionDiscard (clientTransaction)); // allow TransactionDicarding to be called
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

    public static IClientTransactionListener CreateAndAddListenerStrictMock (ClientTransaction clientTransaction)
    {
      var listenerMock = MockRepository.GenerateStrictMock<IClientTransactionListener> ();
      AddListener (clientTransaction, listenerMock);
      return listenerMock;
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

    public static void SetIsActive (ClientTransaction transaction, bool value)
    {
      var hierarchyManager = (TransactionHierarchyManager) GetHierarchyManager (transaction);
      TransactionHierarchyManagerTestHelper.SetIsActive (hierarchyManager, value);
    }

    public static void SetActiveSubTransaction (ClientTransaction clientTransaction, ClientTransaction subTransaction)
    {
      var hierarchyManager = (TransactionHierarchyManager) GetHierarchyManager (clientTransaction);
      TransactionHierarchyManagerTestHelper.SetSubtransaction(hierarchyManager, subTransaction);
    }

    public static void ClearAllListeners (ClientTransaction clientTransaction)
    {
      var listenerManager = GetEventBroker (clientTransaction);
      foreach (var listener in listenerManager.Listeners.ToArray ().Reverse ())
        listenerManager.RemoveListener (listener);
    }

    public static IEnumerable<IClientTransactionListener> GetListeners (ClientTransaction clientTransaction)
    {
      var listenerManager = GetEventBroker (clientTransaction);
      return listenerManager.Listeners;
    }

    private static object PrivateInvokeGeneric (object obj, string methodName, Type[] typeParameters, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("typeParameters", typeParameters);
      ArgumentUtility.CheckNotNull ("args", args);

      // TODO 5118: Use PrivateInvoke when it gets support for generic.
      try
      {
        var genericMethod =
            obj.GetType()
               .GetMethods (BindingFlags.NonPublic | BindingFlags.Instance)
               .SingleOrDefault (m => m.Name == methodName && m.GetGenericArguments().Length == typeParameters.Length);
        if (genericMethod == null)
        {
          var message = string.Format ("Method '{0}.{1}`{2}' not found.", obj.GetType(), methodName, typeParameters.Length);
          throw new ArgumentException (message, "methodName");
        }
        var method = genericMethod.MakeGenericMethod (typeParameters);
        return method.Invoke (obj, args);
      }
      catch (TargetInvocationException ex)
      {
        throw ExceptionUtility.PreserveStackTrace (ex.InnerException);
      }
    }
  }
}