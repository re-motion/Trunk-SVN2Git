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
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPointDataManagement.CollectionEndPointDataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  public static class RelationEndPointObjectMother
  {
    public static CollectionEndPoint CreateCollectionEndPoint (
        RelationEndPointID endPointID,
        IEnumerable<DomainObject> initialContents)
    {
      var dataManager = ClientTransactionTestHelper.GetDataManager (ClientTransaction.Current);
      var changeDetectionStrategy = new RootCollectionEndPointChangeDetectionStrategy();
      var collectionEndPoint = new CollectionEndPoint (
          ClientTransaction.Current,
          endPointID,
          dataManager,
          dataManager,
          new CollectionEndPointDataKeeperFactory (dataManager.ClientTransaction, changeDetectionStrategy));
      
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
      var lazyLoader = ClientTransactionTestHelper.GetDataManager (clientTransaction);
      var endPointProvider = ClientTransactionTestHelper.GetDataManager (clientTransaction);
      return new RealObjectEndPoint (clientTransaction, endPointID, dataContainer, lazyLoader, endPointProvider);
    }

    public static VirtualObjectEndPoint CreateVirtualObjectEndPoint (RelationEndPointID endPointID, ObjectID oppositeObjectID)
    {
      var clientTransaction = ClientTransaction.Current;
      var lazyLoader = ClientTransactionTestHelper.GetDataManager (clientTransaction);
      var endPointProvider = ClientTransactionTestHelper.GetDataManager (clientTransaction);
      return new VirtualObjectEndPoint (clientTransaction, endPointID, oppositeObjectID, lazyLoader, endPointProvider);
    }

    public static ObjectEndPoint CreateObjectEndPoint (RelationEndPointID endPointID, ObjectID oppositeObjectID)
    {
      if (endPointID.Definition.IsVirtual)
        return CreateVirtualObjectEndPoint (endPointID, oppositeObjectID);
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

    public static CollectionEndPoint CreateCollectionEndPoint_Customer1_Orders (params Order[] initialContents)
    {
      var customerEndPointID = CreateRelationEndPointID (new DomainObjectIDs (MappingConfiguration.Current).Customer1, "Orders");
      return CreateCollectionEndPoint (customerEndPointID, initialContents);
    }

  }
}