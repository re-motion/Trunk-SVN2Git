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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints
{
  public static class RelationEndPointObjectMother
  {
    public static CollectionEndPoint CreateCollectionEndPoint (
        RelationEndPointID endPointID,
        IEnumerable<DomainObject> initialContents)
    {
      var clientTransaction = ClientTransaction.Current;
      var dataManager = ClientTransactionTestHelper.GetDataManager (clientTransaction);
      var changeDetectionStrategy = new RootCollectionEndPointChangeDetectionStrategy();
      var collectionEndPoint = new CollectionEndPoint (
          clientTransaction,
          endPointID,
          new CollectionEndPointCollectionManager (new AssociatedCollectionDataStrategyFactory (dataManager)),
          dataManager,
          dataManager,
          new CollectionEndPointDataKeeperFactory (changeDetectionStrategy),
          new VirtualEndPointStateUpdateListener (clientTransaction, endPointID));
      
      if (initialContents != null)
        CollectionEndPointTestHelper.FillCollectionEndPointWithInitialContents (collectionEndPoint, initialContents);

      return collectionEndPoint;
    }

    public static RealObjectEndPoint CreateRealObjectEndPoint (RelationEndPointID endPointID)
    {
      var dataManager = (DataManager) PrivateInvoke.GetNonPublicProperty (ClientTransaction.Current, "DataManager");
      var dataContainer = dataManager.GetDataContainerWithLazyLoad (endPointID.ObjectID);
      return CreateRealObjectEndPoint (endPointID, dataContainer);
    }

    public static RealObjectEndPoint CreateRealObjectEndPoint (RelationEndPointID endPointID, DataContainer dataContainer)
    {
      var clientTransaction = dataContainer.ClientTransaction;
      var endPointProvider = ClientTransactionTestHelper.GetDataManager (clientTransaction);
      return new RealObjectEndPoint (clientTransaction, endPointID, dataContainer, endPointProvider);
    }

    public static VirtualObjectEndPoint CreateVirtualObjectEndPoint (RelationEndPointID endPointID, ClientTransaction clientTransaction)
    {
      var lazyLoader = ClientTransactionTestHelper.GetDataManager (clientTransaction);
      var endPointProvider = ClientTransactionTestHelper.GetDataManager (clientTransaction);
      var dataKeeperFactory = new VirtualObjectEndPointDataKeeperFactory();
      return new VirtualObjectEndPoint (
          clientTransaction,
          endPointID,
          lazyLoader,
          endPointProvider,
          dataKeeperFactory,
          new VirtualEndPointStateUpdateListener (clientTransaction, endPointID));
    }

    public static ObjectEndPoint CreateObjectEndPoint (RelationEndPointID endPointID, ObjectID oppositeObjectID)
    {
      if (endPointID.Definition.IsVirtual)
      {
        var clientTransaction = ClientTransaction.Current;
        VirtualObjectEndPoint endPoint = CreateVirtualObjectEndPoint (endPointID, clientTransaction);
        endPoint.MarkDataComplete (LifetimeService.GetObjectReference (clientTransaction, oppositeObjectID));
        return endPoint;
      }
      else
      {
        var endPoint = CreateRealObjectEndPoint (endPointID);
        endPoint.ForeignKeyProperty.Value = oppositeObjectID;
        endPoint.Commit ();
        return endPoint;
      }
    }

    public static RelationEndPointID CreateRelationEndPointID (ObjectID objectID, string shortPropertyName)
    {
      return RelationEndPointID.Create (objectID, objectID.ClassDefinition.ClassType, shortPropertyName);
    }

    public static IRelationEndPointDefinition GetEndPointDefinition (Type declaringType, string shortPropertyName)
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (declaringType);
      var propertyAccessorData = classDefinition.PropertyAccessorDataCache.FindPropertyAccessorData (declaringType, shortPropertyName);
      Assert.That (propertyAccessorData, Is.Not.Null);
      return propertyAccessorData.RelationEndPointDefinition;
    }

    public static CollectionEndPoint CreateCollectionEndPoint_Customer1_Orders (params Order[] initialContents)
    {
      var customerEndPointID = CreateRelationEndPointID (new DomainObjectIDs (MappingConfiguration.Current).Customer1, "Orders");
      return CreateCollectionEndPoint (customerEndPointID, initialContents);
    }

  }
}